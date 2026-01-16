global using Id = ulong;

using System.Data;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using FluentResponse;
using FluentResponse.Interfaces;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Infrastructure.Core.Exceptions;

namespace CesiZen.Infrastructure.Services;
public class Repository<T>(
    DbContext              dbContext,
    IDomainEventDispatcher domainEventDispatcher
) : IRepository<T> where T : AggregateRoot<T> {

    #region PROPERTIES

        protected readonly DbSet<T>  table     = dbContext.Set<T>();
        protected readonly DbContext dbContext = dbContext;

    #endregion
    #region METHODS

        protected async Task<IResponse<T>> TryValidateAsync(T entity) {
            if (entity.RepositoryInvariant is not null)
                if (await entity.RepositoryInvariant(this) is IResponse<T> failure and IFailure)
                    return failure;

            entity = entity.WithConsumedEvents(out var domainEvents);
            await domainEventDispatcher.DispatchAsync(domainEvents);

            return Response.Success(entity);
        }

        public virtual async Task<IResponse<T>> TryAddAsync(T entity) {

            // We check that there isn't already an entity with the same primary key.
            if (await this.ContainsIdAsync(entity.Id))
                return Response.Failure<T>(new EntityConflictException(typeof(T), entity.Id));
            
            return await this.TryValidateAsync(entity).OnSuccessAsync(async entity => {
                await this.table.AddAsync(entity);
                await this.dbContext.SaveChangesAsync();
            });
        }

        public virtual Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, T> changes) =>
            this.TryGetAsync(id).OnSuccessAsync(oldEntity =>
                this.TryValidateAsync(changes(oldEntity)).OnSuccessAsync(async entity => {

                    this.table.Entry(oldEntity).CurrentValues.SetValues(entity);
                    await this.dbContext.SaveChangesAsync();
                
                })
            );

        public virtual Task<IResponse<T>> TryUpdateAsync(Id id, T entity) =>
            this.TryGetAsync(id).OnSuccessAsync(oldEntity =>
                this.TryValidateAsync(entity).OnSuccessAsync(async entity => {

                    // We replace the old entity with another one.
                    this.table.Entry(oldEntity).CurrentValues.SetValues(entity);
                    await this.dbContext.SaveChangesAsync();

                })
            );


        public virtual async Task<IResponse> TryDeleteAsync(T entity) {
            this.table.Remove(entity);
            return await this.dbContext.SaveChangesAsync() is not 0
                ? Response.Success()
                : Response.Failure(new EntityNotFoundException(typeof(T), entity.Id));
        }

        public virtual Task<IResponse> TryDeleteAsync(Expression<Func<T, bool>> predicate) =>
            this.TryGetAsync(predicate).OnSuccessAsync(this.TryDeleteAsync);

        public virtual Task<IResponse> TryDeleteAsync(Id id) =>
            this.TryGetAsync(id).OnSuccessAsync(this.TryDeleteAsync);

        public virtual async Task DeleteAllAsync() {
            this.table.RemoveRange(this.table);
            await this.dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAllAsync(Expression<Func<T, bool>> predicate) {
            this.table.RemoveRange(this.table.Where(predicate));
            await this.dbContext.SaveChangesAsync();
        }
        
        public virtual async Task<IResponse<T>> TryGetAsync(Id id) =>
            await this.table.FindAsync(id) is T entity
                ? Response.Success(entity)
                : Response.Failure<T>(new EntityNotFoundException(typeof(T), id));

        public virtual async Task<IResponse<T>> TryGetAsync(Expression<Func<T, bool>> predicate) =>
            await this.table.SingleOrDefaultAsync(predicate) is T entity
                ? Response.Success(entity)
                : Response.Failure<T>(new EntityNotFoundException(typeof(T)));

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await this.table.ToListAsync();
        public virtual async Task<IEnumerable<T>> GetAllAsync(IEnumerable<Id> ids) => (await Task.WhenAll(ids.Select(async key => await this.TryGetAsync(key)))).OfType<T>();
        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate) => await this.table.Where(predicate).ToListAsync();

        public virtual async Task<bool> ContainsAsync(T entity) => await this.table.ContainsAsync(entity);
        public virtual async Task<bool> ContainsIdAsync(Id id) => await this.table.FindAsync(id) is not null;

        public virtual async Task<bool> AnyAsync() => await this.table.AnyAsync();
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) => await this.table.AnyAsync(predicate);

    #endregion

}
