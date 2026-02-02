using CesiZen.Application.Core.ValueObjects;
using CesiZen.Application.Ports;
using Microsoft.Extensions.Configuration;

namespace CesiZen.Infrastructure.Adapters;
public sealed class PasswordResetCacheService(
    TimeSpan passwordResetRequestExpiry
) : CacheService<Id, Pin>, IPasswordResetCacheService {

    public override TimeSpan CacheDuration { get; } = passwordResetRequestExpiry;

    public PasswordResetCacheService(IConfiguration configuration) : this(
        passwordResetRequestExpiry : TimeSpan.Parse(configuration["Pin:PasswordResetRequestExpiry"]!)
    ) {}
}