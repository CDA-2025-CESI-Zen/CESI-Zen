using System.Linq.Expressions;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;
public interface IQueryService<T> where T : AggregateRoot<T> {
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
    Task<IResponse<T>>   TryGetAsync(Id id);
}