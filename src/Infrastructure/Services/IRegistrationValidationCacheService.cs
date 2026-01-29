using CesiZen.Infrastructure.Core.ValueObjects;

namespace CesiZen.Infrastructure.Services;
public interface IRegistrationValidationCacheService: ICacheService<string, Pin>;