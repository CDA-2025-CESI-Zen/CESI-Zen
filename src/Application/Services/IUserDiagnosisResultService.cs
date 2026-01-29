using CesiZen.Application.Core.ValueObjects;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;

/// <summary>
/// A service for handling user diagnoses.
/// </summary>
public interface IUserDiagnosisResultService {

    /// <summary>
    /// Tries to compute and save a user diagnosis' score.
    /// </summary>
    /// <returns>A response containing the diagnosis current score and previous if any.</returns>
    /// <param name="id">The user's ID.</param>
    /// <param name="diagnosisItemIds">The diagnosis items' IDs.</param>
    Task<IResponse<DiagnosisResult>> TrySaveDiagnosisResult(Id id, IEnumerable<Id> diagnosisItemIds);
    
}