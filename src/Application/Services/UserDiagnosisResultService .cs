using CesiZen.Application.Core.ValueObjects;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Domain.Aggregates.Diagnoses;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;
public sealed class UserDiagnosisResultService (
    IRepository<User>          repository,
    IRepository<DiagnosisItem> diagnosisItemRepository
) : IUserDiagnosisResultService {

    public async Task<IResponse<DiagnosisResult>> TrySaveDiagnosisResult(Id id, IEnumerable<Id> diagnosisItemIds) {
        
        int  newScore      = (await diagnosisItemRepository.GetAllAsync(diagnosisItemIds)).Sum(x => x.Score);
        int? previousScore = null;

        return await repository
            .TryUpdateAsync(id, user => {

                previousScore = user.LastDiagnosisResult?.Score;
                return user.WithNewDiagnosisResult(newScore);

            }).OnSuccessAsync(_ => new DiagnosisResult(newScore, previousScore));
    }
}