using CesiZen.Domain.Aggregates.Core;
using CesiZen.Domain.Aggregates.Diagnoses.ValueObjects;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Diagnoses;
public record DiagnosisItem : AggregateRoot<DiagnosisItem> {

    #region PROPERTIES

        /// <summary> The label of this diagnosis item event. </summary>
        public EventLabel EventLabel { get; internal init; }

        /// <summary> The stress score associated with this diagnosis item. </summary>
        public int Score { get; internal init; }

    #endregion
    #region CONSTRUCTORS

        public static IResponse<DiagnosisItem> TryCreate(
            string eventLabel,
            int    score
        ) => EventLabel.TryCreate(eventLabel).OnSuccess(eventLabel => new DiagnosisItem {
            EventLabel = eventLabel,
            Score      = score
        });

    #endregion
    #region METHODS

        public IResponse<DiagnosisItem> TryWithEventLabel(string value) =>
            EventLabel.TryCreate(value).OnSuccess(eventLabel => this with { EventLabel = eventLabel });

        public DiagnosisItem WithScore(int value) =>
            this with { Score = value };

    #endregion

}