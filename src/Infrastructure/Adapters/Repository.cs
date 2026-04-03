using System.Data;
using Microsoft.EntityFrameworkCore;
using FluentResponse;
using FluentResponse.Interfaces;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Application.Core.Exceptions;

namespace CesiZen.Infrastructure.Adapters;
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

            return Response.Success(entity);

        }

        protected async Task<IResponse<T>> TryDispatchEventsAsync(T entity) {

            entity = entity.WithConsumedEvents(out var domainEvents);

            return await domainEventDispatcher.DispatchAsync(domainEvents).OnSuccessAsync(() => {
                return entity;
            });
        }

        public virtual async Task<IResponse<T>> TryAddAsync(T entity) {

            if (await this.ContainsIdAsync(entity.Id))
                return Response.Failure<T>(new EntityConflictException(typeof(T), entity.Id));
            
            return await this.TryValidateAsync(entity).OnSuccessAsync(async entity => {
                entity = (await this.table.AddAsync(entity)).Entity;
                await this.dbContext.SaveChangesAsync();
            }).OnSuccessAsync(TryDispatchEventsAsync);
        }

        public virtual Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, T> changes) =>
            this.TryGetAsync(id).OnSuccessAsync(oldEntity =>
                this.TryValidateAsync(changes(oldEntity)).OnSuccessAsync(async entity => {

                    this.table.Entry(oldEntity).State = EntityState.Detached;
                    this.table.Attach(entity);
                    this.table.Entry(entity).State = EntityState.Modified;
                    await this.dbContext.SaveChangesAsync();
                    this.table.Entry(oldEntity).State = EntityState.Detached;
                
                })
            ).OnSuccessAsync(TryDispatchEventsAsync);

        public virtual Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, IResponse<T>> changes) =>
            this.TryGetAsync(id).OnSuccessAsync(oldEntity =>
                changes(oldEntity).OnSuccessAsync(this.TryValidateAsync).OnSuccessAsync(async entity => {

                    this.table.Entry(oldEntity).State = EntityState.Detached;
                    this.table.Attach(entity);
                    this.table.Entry(entity).State = EntityState.Modified;
                    await this.dbContext.SaveChangesAsync();
                    this.table.Entry(oldEntity).State = EntityState.Detached;
                
                })
            ).OnSuccessAsync(TryDispatchEventsAsync);

        public virtual async Task<IResponse> TryDeleteAsync(T entity) {
            this.table.Remove(entity);
            return await this.dbContext.SaveChangesAsync() is not 0
                ? Response.Success()
                : Response.Failure(new EntityNotFoundException(typeof(T), entity.Id));
        }

        public virtual Task<IResponse> TryDeleteAsync(Func<T, bool> predicate) =>
            this.TryGetAsync(predicate).OnSuccessAsync(this.TryDeleteAsync);

        public virtual Task<IResponse> TryDeleteAsync(Id id) =>
            this.TryGetAsync(id).OnSuccessAsync(this.TryDeleteAsync);

        public virtual async Task DeleteAllAsync() {
            this.table.RemoveRange(this.table);
            await this.dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAllAsync(Func<T, bool> predicate) {
            this.table.RemoveRange(this.table.AsEnumerable().Where(predicate));
            await this.dbContext.SaveChangesAsync();
        }
        
        public virtual async Task<IResponse<T>> TryGetAsync(Id id) =>
            await this.table.FindAsync(id) is T entity
                ? Response.Success(entity)
                : Response.Failure<T>(new EntityNotFoundException(typeof(T), id));

        public virtual async Task<IResponse<T>> TryGetAsync(Func<T, bool> predicate) =>
            await this.table.AsAsyncEnumerable().SingleOrDefaultAsync(predicate) is T entity
                ? Response.Success(entity)
                : Response.Failure<T>(new EntityNotFoundException(typeof(T)));

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await this.table.ToListAsync();
        public virtual async Task<IEnumerable<T>> GetAllAsync(IEnumerable<Id> ids) {
            var results = new List<T>(ids.Count());
            foreach (var id in ids)
                await this.TryGetAsync(id).OnSuccessAsync(results.Add);

            return results;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Func<T, bool> predicate) => await this.table.AsAsyncEnumerable().Where(predicate).ToListAsync();

        public virtual async Task<bool> ContainsAsync(T entity) => await this.table.ContainsAsync(entity);
        public virtual async Task<bool> ContainsIdAsync(Id id) => await this.table.FindAsync(id) is not null;

        public virtual async Task<bool> AnyAsync() => await this.table.AnyAsync();
        public virtual async Task<bool> AnyAsync(Func<T, bool> predicate) => await this.table.AsAsyncEnumerable().AnyAsync(predicate);

    #endregion

}
