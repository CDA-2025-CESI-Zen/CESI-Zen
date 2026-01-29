using CesiZen.Application.Core.ValueObjects;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Infrastructure.Services;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;
public sealed class AdminSessionService(
    IRepository<Admin> repository,
    IAdminAuthService  authService
) : IAdminSessionService {
    
    public async Task<IResponse<AdminSession>> TryAuthAsync(string mailAddress, string password) =>
        await repository
            .TryGetAsync(admin => admin.MailAddress.Address == mailAddress)
            .OnSuccessAsync(admin => admin
                .TryVerifyPassword(password)
                .OnSuccess(() => new AdminSession(authService.GenerateToken(admin), admin))
            );
}