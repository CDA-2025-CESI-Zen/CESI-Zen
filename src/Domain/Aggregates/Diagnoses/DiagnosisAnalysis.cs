using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Diagnoses;
public record DiagnosisAnalysis : AggregateRoot<DiagnosisAnalysis> {

    #region PROPERTIES

        public int    ScoreThreshold { get; protected init; }
        public string Content        { get; protected init; } = null!;

        public override Func<IRepository<DiagnosisAnalysis>, Task<IResponse<DiagnosisAnalysis>>>? RepositoryInvariant => async (repository) => 
            await repository.AnyAsync((x) => x.Id != this.Id && x.ScoreThreshold == this.ScoreThreshold)
                ? Response.Failure<DiagnosisAnalysis>(new InvariantException<DiagnosisAnalysis>($"Le seuil de diagnostique `{this.ScoreThreshold}` est déjà utilisé par une analyse !"))
                : Response.Success(this);

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