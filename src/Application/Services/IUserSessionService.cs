using CesiZen.Application.Core.ValueObjects;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Infrastructure.Core.ValueObjects;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;
public interface IUserSessionService {
    Task<IResponse<UserSession>> TryRegisterAsync(string mailAddress, string password, Pin pin);
    Task<IResponse<UserSession>> TryAuthAsync(string mailAddress, string password);
    Task<IResponse<UserSession>> TryResetPasswordAsync(string mailAddress, string newPassword, Pin pin);
    Task<IResponse<UserSession>> TryUpdateAsync(Id id, string password, Func<User, IResponse<User>> transform);
    Task<IResponse> TryRequestRegistrationPINAsync(string mailAddress);
    Task<IResponse> TryRequestPasswordResetPINAsync(string mailAddress);
}