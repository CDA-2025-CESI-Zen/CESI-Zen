using CesiZen.Application.Services;
using CesiZen.Domain.Aggregates.Content;
using CesiZen.Presentation.FrontOffice.Api.Extensions;
using CesiZen.Presentation.FrontOffice.Api.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Presentation.FrontOffice.Api.Controllers;

[ApiController]
[Route(ROUTE)]
public class CategoryController(
    IQueryService<Category> queryService
) : ControllerBase {

    public const string ROUTE = "/categories";

    #region ROUTES

        [HttpGet(Name = nameof(GetAllCategoriesAsync))]
        public Task<IResult> GetAllCategoriesAsync() =>
            queryService.GetAllAsync().ToResourceAsync<Category, CategoryResource>(Results.Ok);

        [HttpGet("{id}", Name = nameof(GetCategoryAsync))]
        public Task<IResult> GetCategoryAsync(Id id) =>
            queryService.TryGetAsync(id).ToResourceAsync<Category, CategoryResource>(Results.Ok);

    #endregion

}
