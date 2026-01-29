using CesiZen.Domain.Aggregates.Core;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;

/// <summary>
/// A service for handling aggregate roots' queries.
/// </summary>
/// <typeparam name="T">The aggregate root's type</typeparam>
public interface IQueryService<T> where T : AggregateRoot<T> {

    /// <summary>
    /// Queries all aggregate roots.
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Queries all aggregate roots that match the predicate.
    /// </summary>
    /// <param name="predicate">A function to filter aggregate roots.</param>
    Task<IEnumerable<T>> GetAllAsync(Func<T, bool> predicate);

    /// <summary>
    /// Tries to query an aggregate root.
    /// </summary>
    /// <returns>A response containing the aggregate root.</returns>
    /// <param name="id">The aggregate root's ID.</param>
    Task<IResponse<T>> TryGetAsync(Id id);
}