using CesiZen.Domain.Aggregates.Core;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;

/// <summary>
/// A service for handling aggregate roots' command.
/// </summary>
/// <typeparam name="T">The aggregate root's type</typeparam>
public interface ICommandService<T> where T : AggregateRoot<T> {

    /// <summary>
    /// Tries to create an aggregate root.
    /// </summary>
    /// <returns>A response containing the created aggregate root.</returns>
    /// <param name="build">The builder method to try creating the aggregate root.</param>
    Task<IResponse<T>> TryCreateAsync(Func<IResponse<T>> build);

    /// <summary>
    /// Tries to update an aggregate root.
    /// </summary>
    /// <returns>A response containing the updated aggregate root.</returns>
    /// <param name="id">The aggregate root's ID.</param>
    /// <param name="transform">The transform method to try updating the aggregate root.</param>
    Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, IResponse<T>> transform);

    /// <summary>
    /// Tries to delete an aggregate root.
    /// </summary>
    /// <returns>A successful response if the aggregate root was deleted.</returns>
    /// <param name="id">The aggregate root's ID.</param>
    Task<IResponse> TryDeleteAsync(Id id);
    
}