using CesiZen.Application.Core.ValueObjects;
using CesiZen.Domain.Aggregates.Accounts;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;

/// <summary>
/// A service for handling user sessions.
/// </summary>
public interface IUserSessionService {

    /// <summary>
    /// Tries to register as user.
    /// </summary>
    /// <returns>A response containing the user session's token and user data.</returns>
    /// <param name="mailAddress">The user's mail address.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="pin">The registration PIN associated with the user's mail address.</param>
    Task<IResponse<UserSession>> TryRegisterAsync(string mailAddress, string password, Pin pin);

    /// <summary>
    /// Tries to auth as user.
    /// </summary>
    /// <returns>A response containing the user session's token and user data.</returns>
    /// <param name="mailAddress">The user's mail address.</param>
    /// <param name="password">The user's password.</param>
    Task<IResponse<UserSession>> TryAuthAsync(string mailAddress, string password);

    /// <summary>
    /// Tries to reset a user's password.
    /// </summary>
    /// <returns>A response containing the user session's token and user data.</returns>
    /// <param name="mailAddress">The user's mail address.</param>
    /// <param name="newPassword">The user's new password.</param>
    /// <param name="pin">The password reset PIN associated with the user's mail address.</param>
    Task<IResponse<UserSession>> TryResetPasswordAsync(string mailAddress, string newPassword, Pin pin);

    /// <summary>
    /// Tries to update a user's account.
    /// </summary>
    /// <returns>A response containing the user session's token and user data.</returns>
    /// <param name="id">The user's ID.</param>
    /// <param name="mailAddress">The user's mail address.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="transform">The transform method to try updating the user.</param>
    Task<IResponse<UserSession>> TryUpdateAsync(Id id, string password, Func<User, IResponse<User>> transform);
    
    /// <summary>
    /// Tries to anonymize a user.
    /// </summary>
    /// <returns>A successful response if the user was anonymized.</returns>
    /// <param name="id">The user's ID.</param>
    /// <param name="password">The user's password.</param>
    Task<IResponse> TryAnonymizeAsync(Id id, string password);

    /// <summary>
    /// Tries to delete a user.
    /// </summary>
    /// <returns>A successful response if the user was deleted.</returns>
    /// <param name="id">The user's ID.</param>
    /// <param name="password">The user's password.</param>
    Task<IResponse> TryDeleteAsync(Id id, string password);

    /// <summary>
    /// Tries to request a user's registration PIN generation.
    /// </summary>
    /// <returns>A successful response if the PIN was generated.</returns>
    /// <param name="mailAddress">The user's wanted mail address.</param>
    Task<IResponse> TryRequestRegistrationPINAsync(string mailAddress);

    /// <summary>
    /// Tries to request a user's password reset PIN generation.
    /// </summary>
    /// <returns>A successful response if the PIN was generated.</returns>
    /// <param name="mailAddress">The user's mail address.</param>
    Task<IResponse> TryRequestPasswordResetPINAsync(string mailAddress);
    
}