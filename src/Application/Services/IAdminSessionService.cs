using CesiZen.Application.Core.ValueObjects;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;

/// <summary>
/// A service for handling admin sessions.
/// </summary>
public interface IAdminSessionService {

    /// <summary>
    /// Tries to auth as admin.
    /// </summary>
    /// <returns>A response containing the admin session's token and admin data.</returns>
    /// <param name="mailAddress">The admin's mail address.</param>
    /// <param name="password">The admin's password.</param>
    Task<IResponse<AdminSession>> TryAuthAsync(string mailAddress, string password);

}