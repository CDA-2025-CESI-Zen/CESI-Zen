using CesiZen.Domain.Aggregates.Core;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;
public interface ICommandService<T> where T : AggregateRoot<T> {
    Task<IResponse<T>> TryCreateAsync(Func<IResponse<T>> build);
    Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, IResponse<T>> transform);
    Task<IResponse>    TryDeleteAsync(Id id);
}