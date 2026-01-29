using CesiZen.Domain.Aggregates.Accounts;
using FluentResponse.Interfaces;

namespace CesiZen.Infrastructure.Services;
public interface IUserAuthService {
    IResponse<string> TryGenerateToken(User user);
}