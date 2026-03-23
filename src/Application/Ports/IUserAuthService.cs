using CesiZen.Domain.Aggregates.Accounts;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Ports;
public interface IUserAuthService {
    IResponse<string> TryGenerateToken(User user);
}