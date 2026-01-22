namespace CesiZen.Application.Core.ValueObjects;
public readonly record struct DiagnosisResult(
    int  NewScore,
    int? PreviousScore
);