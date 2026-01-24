using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Core;
public interface IRepository<T> where T : AggregateRoot<T> {

    Task<IResponse<T>> TryAddAsync(T entity);
    Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, T> changes);
    Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, IResponse<T>> changes);
    Task<IResponse> TryDeleteAsync(T entity);
    Task<IResponse> TryDeleteAsync(Id id);
    Task<IResponse> TryDeleteAsync(Func<T, bool> predicate);

    Task DeleteAllAsync();
    Task DeleteAllAsync(Func<T, bool> predicate);

    Task<IResponse<T>> TryGetAsync(Id id);
    Task<IResponse<T>> TryGetAsync(Func<T, bool> predicate);

    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(IEnumerable<Id> ids);
    Task<IEnumerable<T>> GetAllAsync(Func<T, bool> predicate);

    Task<bool> ContainsAsync(T entity);
    Task<bool> ContainsIdAsync(Id id);

    Task<bool> AnyAsync();
    Task<bool> AnyAsync(Func<T, bool> predicate);

}