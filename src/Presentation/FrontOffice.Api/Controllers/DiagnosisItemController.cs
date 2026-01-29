using CesiZen.Application.Services;
using CesiZen.Domain.Aggregates.Diagnoses;
using CesiZen.Presentation.FrontOffice.Api.Extensions;
using CesiZen.Presentation.FrontOffice.Api.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Presentation.FrontOffice.Api.Controllers;

[ApiController]
[Route(ROUTE)]
public class DiagnosisItemController(
    IQueryService<DiagnosisItem> queryService
) : ControllerBase {

    public const string ROUTE = "/diagnosis-items";

    #region ROUTES

        [HttpGet(Name = nameof(GetAllDiagnosisItemsAsync))]
        public Task<IResult> GetAllDiagnosisItemsAsync() =>
            queryService.GetAllAsync().ToResourceAsync<DiagnosisItem, DiagnosisItemResource>(Results.Ok);

        [HttpGet("{id}", Name = nameof(GetDiagnosisItemAsync))]
        public Task<IResult> GetDiagnosisItemAsync(Id id) =>
            queryService.TryGetAsync(id).ToResourceAsync<DiagnosisItem, DiagnosisItemResource>(Results.Ok);

    #endregion

}
