using CesiZen.Application.Core.ValueObjects;

namespace CesiZen.Application.Ports;
public interface IRegistrationValidationCacheService: ICacheService<string, Pin>;