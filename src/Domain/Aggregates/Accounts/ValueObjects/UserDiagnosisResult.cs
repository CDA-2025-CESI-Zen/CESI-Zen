namespace CesiZen.Domain.Aggregates.Accounts.ValueObjects;
public readonly record struct UserDiagnosisResult(int Score) {
    public DateOnly Date { get; init; } = DateOnly.FromDateTime(DateTime.Now);
}