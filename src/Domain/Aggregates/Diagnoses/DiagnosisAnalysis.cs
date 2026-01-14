using CesiZen.Domain.Aggregates.Core;

namespace CesiZen.Domain.Aggregates.Diagnoses;
public record DiagnosisAnalysis : AggregateRoot<DiagnosisAnalysis> {

    #region PROPERTIES

        public int    ScoreThreshold { get; protected init; }
        public string Content        { get; protected init; } = null!;

    #endregion
    #region CONSTRUCTORS

        public static DiagnosisAnalysis Create(
            int    scoreThreshold,
            string content
        ) => new () { ScoreThreshold = scoreThreshold, Content = content };

    #endregion
    #region METHODS

        public DiagnosisAnalysis WithContent(string value) =>
            this with { Content = value };

        public DiagnosisAnalysis WithScoreThreshold(int value) =>
            this with { ScoreThreshold = value };

    #endregion

}