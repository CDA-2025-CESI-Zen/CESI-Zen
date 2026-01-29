using CesiZen.Application.Core.ValueObjects;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;
public interface IUserDiagnosisResultService {
    Task<IResponse<DiagnosisResult>> TrySaveDiagnosisResult(Id id, IEnumerable<Id> diagnosisItemIds);
}