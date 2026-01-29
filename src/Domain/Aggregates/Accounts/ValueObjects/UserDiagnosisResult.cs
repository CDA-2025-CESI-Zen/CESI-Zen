namespace CesiZen.Domain.Aggregates.Accounts.ValueObjects;
public readonly record struct UserDiagnosisResult(int Score, DateTime Date) {
    public UserDiagnosisResult(int score) : this(score, DateTime.UtcNow) {}
}