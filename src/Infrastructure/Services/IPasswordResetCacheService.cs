using CesiZen.Infrastructure.Core.ValueObjects;

namespace CesiZen.Infrastructure.Services;
public interface IPasswordResetCacheService: ICacheService<Id, Pin>;