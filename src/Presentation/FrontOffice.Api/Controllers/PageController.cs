using CesiZen.Application.Services;
using CesiZen.Domain.Aggregates.Content;
using CesiZen.Presentation.FrontOffice.Api.Extensions;
using CesiZen.Presentation.FrontOffice.Api.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Presentation.FrontOffice.Api.Controllers;

[ApiController]
[Route(ROUTE)]
public class PageController(
    IQueryService<Page> queryService
) : ControllerBase {

    public const string ROUTE = "/pages";

    #region ROUTES

        [HttpGet(Name = nameof(GetAllPagesAsync))]
        [EndpointDescription("Queries all pages.")]
        public Task<IResult> GetAllPagesAsync() =>
            queryService.GetAllAsync().ToResourceAsync<Page, PageResource>(Results.Ok);

        [HttpGet("{id}", Name = nameof(GetPageAsync))]
        [EndpointDescription("Queries a page using its ID.")]
        public Task<IResult> GetPageAsync(Id id) =>
            queryService.TryGetAsync(id).ToResourceAsync<Page, PageResource>(Results.Ok);

    #endregion

}
