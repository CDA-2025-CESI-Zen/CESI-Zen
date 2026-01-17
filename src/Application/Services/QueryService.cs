using System.Linq.Expressions;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;
public sealed class QueryService<T>(
    IRepository<T> repository
) : IQueryService<T> where T : AggregateRoot<T> {
    
    public Task<IEnumerable<T>> GetAllAsync() =>
        repository.GetAllAsync();
        
    public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate) =>
        repository.GetAllAsync(predicate);

    public Task<IResponse<T>> TryGetAsync(Id id) =>
        repository.TryGetAsync(id);
}