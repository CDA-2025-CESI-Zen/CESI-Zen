using CesiZen.Domain.Aggregates.Diagnoses;
using CesiZen.Presentation.FrontOffice.Api.Controllers;

namespace CesiZen.Presentation.FrontOffice.Api.Resources;
public class DiagnosisItemResource(DiagnosisItem diagnosisItem) : IResource<DiagnosisItemResource, DiagnosisItem> {

    #region PROPERTIES

        public Id Id { get; } = diagnosisItem.Id;

        public string Label { get; } = diagnosisItem.EventLabel.Value;
        public int    Score { get; } = diagnosisItem.Score;

        public DiagnosisItemLinks Links { get; } = new(
            Self : new (method : HttpMethod.GET, DiagnosisItemController.ROUTE, diagnosisItem.Id)
        );

        public readonly record struct DiagnosisItemLinks(
            Link Self
        );

    #endregion
    #region METHODS

        public static implicit operator DiagnosisItemResource(DiagnosisItem ressource) => new (ressource);

        public static DiagnosisItemResource From(DiagnosisItem ressource) => ressource;
        public static IEnumerable<DiagnosisItemResource> From(IEnumerable<DiagnosisItem> ressources) => ressources.Select(From);

    #endregion

}