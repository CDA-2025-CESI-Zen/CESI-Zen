using System.Linq.Expressions;
using CesiZen.Domain.Aggregates.Content;
using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Infrastructure.Services;
public sealed class CategoryRepository(
    DbContext              dbContext
    //IDomainEventDispatcher domainEventDispatcher
) : Repository<Category>(dbContext) {

    public override Task<IResponse<Category>> TryGetAsync(Id id) =>
        base.TryGetAsync(id).OnSuccessAsync(page => dbContext.Entry(page).Collection(x => x.Pages).LoadAsync());

    public override Task<IResponse<Category>> TryGetAsync(Func<Category, bool> predicate) =>
        base.TryGetAsync(predicate).OnSuccessAsync(page => dbContext.Entry(page).Collection(x => x.Pages).LoadAsync());

    public override async Task<IEnumerable<Category>> GetAllAsync() => await this.table.Include(x => x.Pages).ToListAsync();
    public override async Task<IEnumerable<Category>> GetAllAsync(Expression<Func<Category, bool>> predicate) => await this.table.Include(x => x.Pages).Where(predicate).ToListAsync();

}