using CesiZen.Application.Core.ValueObjects;
using CesiZen.Domain.Aggregates.Accounts;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;
public interface IAdminSessionService {
    Task<IResponse<AdminSession>> TryAuthAsync(string mailAddress, string password);
    Task<IResponse<AdminSession>> TryUpdateAsync(Id id, string password, Func<Admin, IResponse<Admin>> transform);
}