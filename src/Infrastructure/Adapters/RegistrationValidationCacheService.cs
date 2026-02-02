using CesiZen.Application.Core.ValueObjects;
using CesiZen.Application.Ports;
using Microsoft.Extensions.Configuration;

namespace CesiZen.Infrastructure.Adapters;
public sealed class RegistrationValidationCacheService(
    TimeSpan registrationValidationRequestExpiry
) : CacheService<string, Pin>, IRegistrationValidationCacheService {

    public override TimeSpan CacheDuration { get; } = registrationValidationRequestExpiry;

    public RegistrationValidationCacheService(IConfiguration configuration) : this(
        registrationValidationRequestExpiry : TimeSpan.Parse(configuration["Pin:RegistrationValidationRequestExpiry"]!)
    ) {}
}