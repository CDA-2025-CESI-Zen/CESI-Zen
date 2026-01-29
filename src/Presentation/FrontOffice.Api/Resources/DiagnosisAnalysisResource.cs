using System.Text.Json.Serialization;
using CesiZen.Domain.Aggregates.Diagnoses;
using CesiZen.Presentation.FrontOffice.Api.Controllers;

namespace CesiZen.Presentation.FrontOffice.Api.Resources;
public class DiagnosisAnalysisResource(DiagnosisAnalysis diagnosisAnalysis) : IResource<DiagnosisAnalysisResource, DiagnosisAnalysis> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = diagnosisAnalysis.Id;

        public int    ScoreThreshold { get; } = diagnosisAnalysis.ScoreThreshold;
        public string Content        { get; } = diagnosisAnalysis.Content;

        public DiagnosisAnalysisLinks Links { get; } = new(
            Self : new (method : HttpMethod.GET, DiagnosisAnalysisController.ROUTE, diagnosisAnalysis.Id)
        );

        public readonly record struct DiagnosisAnalysisLinks(
            Link Self
        );

    #endregion
    #region METHODS

        public static implicit operator DiagnosisAnalysisResource(DiagnosisAnalysis ressource) => new (ressource);

        public static DiagnosisAnalysisResource From(DiagnosisAnalysis ressource) => ressource;
        public static IEnumerable<DiagnosisAnalysisResource> From(IEnumerable<DiagnosisAnalysis> ressources) => ressources.Select(From);

    #endregion

}