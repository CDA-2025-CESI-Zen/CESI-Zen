using System.Linq.Expressions;
using CesiZen.Domain.Aggregates.Content;
using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Infrastructure.Services;
public sealed class PageRepository(
    DbContext              dbContext
    //IDomainEventDispatcher domainEventDispatcher
) : Repository<Page>(dbContext) {

    public override Task<IResponse<Page>> TryGetAsync(Id id) =>
        base.TryGetAsync(id).OnSuccessAsync(page => dbContext.Entry(page).Reference(x => x.Category).LoadAsync());

    public override Task<IResponse<Page>> TryGetAsync(Func<Page, bool> predicate) =>
        base.TryGetAsync(predicate).OnSuccessAsync(page => dbContext.Entry(page).Reference(x => x.Category).LoadAsync());

    public override async Task<IEnumerable<Page>> GetAllAsync() => await this.table.Include(x => x.Category).ToListAsync();
    public override async Task<IEnumerable<Page>> GetAllAsync(Expression<Func<Page, bool>> predicate) => await this.table.Include(x => x.Category).Where(predicate).ToListAsync();

}