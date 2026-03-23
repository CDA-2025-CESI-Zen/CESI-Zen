using CesiZen.Domain.Aggregates.Accounts;

namespace CesiZen.Application.Ports;
public interface IAdminAuthService {
    string GenerateToken(Admin admin);
}