namespace CesiZen.Domain.Aggregates.Accounts.ValueObjects;

/// <summary> A generic user dated diagnosis result. </summary>
public readonly record struct UserDiagnosisResult(int Score, DateTime Date) {
    public UserDiagnosisResult(int score) : this(score, DateTime.UtcNow) {}
}