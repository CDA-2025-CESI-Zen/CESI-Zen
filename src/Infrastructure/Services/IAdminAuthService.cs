using CesiZen.Domain.Aggregates.Accounts;

namespace CesiZen.Infrastructure.Services;
public interface IAdminAuthService {
    string GenerateToken(Admin admin);
}