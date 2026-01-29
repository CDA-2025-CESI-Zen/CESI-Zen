using CesiZen.Application.Services;
using CesiZen.Domain.Aggregates.Diagnoses;
using CesiZen.Presentation.FrontOffice.Api.Extensions;
using CesiZen.Presentation.FrontOffice.Api.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Presentation.FrontOffice.Api.Controllers;

[ApiController]
[Route(ROUTE)]
public class DiagnosisAnalysisController(
    IQueryService<DiagnosisAnalysis> queryService
) : ControllerBase {

    public const string ROUTE = "/diagnosis-analyses";

    #region ROUTES

        [HttpGet(Name = nameof(GetAllDiagnosisAnalysesAsync))]
        [EndpointDescription("Queries all diagnosis analyses.")]
        public Task<IResult> GetAllDiagnosisAnalysesAsync() =>
            queryService.GetAllAsync().ToResourceAsync<DiagnosisAnalysis, DiagnosisAnalysisResource>(Results.Ok);

        [HttpGet("{id}", Name = nameof(GetDiagnosisAnalysisAsync))]
        [EndpointDescription("Queries a diagnosis analysis using its ID.")]
        public Task<IResult> GetDiagnosisAnalysisAsync(Id id) =>
            queryService.TryGetAsync(id).ToResourceAsync<DiagnosisAnalysis, DiagnosisAnalysisResource>(Results.Ok);

    #endregion

}
