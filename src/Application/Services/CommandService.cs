using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;
public sealed class CommandService<T>(
    IRepository<T> repository
) : ICommandService<T> where T : AggregateRoot<T> {
    
    public Task<IResponse<T>> TryCreateAsync(Func<IResponse<T>> build) =>
        build().OnSuccessAsync(repository.TryAddAsync);
        
    public Task<IResponse<T>> TryUpdateAsync(Id id, Func<T, IResponse<T>> transform) =>
        repository.TryUpdateAsync(id, transform);

    public Task<IResponse> TryDeleteAsync(Id id) =>
        repository.TryDeleteAsync(id);
}