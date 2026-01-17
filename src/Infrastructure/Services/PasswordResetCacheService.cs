using CesiZen.Infrastructure.Core.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace CesiZen.Infrastructure.Services;
public sealed class PasswordResetCacheService(
    TimeSpan passwordResetRequestExpiry
) : CacheService<Id, PIN>, IPasswordResetCacheService {

    public override TimeSpan CacheDuration { get; } = passwordResetRequestExpiry;

    public PasswordResetCacheService(IConfiguration configuration) : this(
        passwordResetRequestExpiry : TimeSpan.Parse(configuration["PasswordResetRequestExpiry"]!)
    ) {}
}