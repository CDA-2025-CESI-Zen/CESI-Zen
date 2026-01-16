using CesiZen.Infrastructure.Core.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace CesiZen.Infrastructure.Services;
public sealed class RegistrationValidationCacheService(
    TimeSpan registrationValidationRequestExpiry
) : CacheService<string, PIN> {

    public override TimeSpan CacheDuration { get; } = registrationValidationRequestExpiry;

    public RegistrationValidationCacheService(IConfiguration configuration) : this(
        registrationValidationRequestExpiry : TimeSpan.Parse(configuration["RegistrationValidationRequestExpiry"]!)
    ) {}
}