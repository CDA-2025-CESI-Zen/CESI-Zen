using CesiZen.Application.Core.ValueObjects;

namespace CesiZen.Application.Ports;
public interface IPasswordResetCacheService: ICacheService<Id, Pin>;