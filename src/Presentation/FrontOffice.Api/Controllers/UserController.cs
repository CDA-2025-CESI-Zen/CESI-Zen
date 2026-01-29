using CesiZen.Application.Core.ValueObjects;
using CesiZen.Application.Services;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Presentation.FrontOffice.Api.Extensions;
using CesiZen.Presentation.FrontOffice.Api.Resources;
using FluentResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Presentation.FrontOffice.Api.Controllers;

[ApiController]
[Route(ROUTE)]
public class UserController(
    IUserSessionService         sessionService,
    IUserDiagnosisResultService diagnosisResultService,
    IQueryService<User>         queryService
) : ControllerBase {

    public const string ROUTE = "/users";

    #region DTOS

        public readonly record struct RegisterDto(
            string MailAddress,
            string Password,
            uint Pin
        );

        public readonly record struct AuthDto(
            string MailAddress,
            string Password
        );

        public readonly record struct UpdateAccountDto(
            string Password,
            string? NewMailAddress = null,
            string? NewPassword    = null
        );

        public readonly record struct SaveDiagnosisResultDto(
            List<Id> DiagnosisItemIds
        );


        public readonly record struct ResetPasswordDto(
            string MailAddress,
            string NewPassword,
            uint Pin
        );

        public readonly record struct CloseAccountDto(
            string Password
        );

        public readonly record struct RequestPinGenerationDto(
            string MailAddress
        );
    
    #endregion
    #region ROUTES

        [HttpPost("/register")]
        [EndpointDescription("Tries to register using the PIN code associated with the wanted mail address.")]
        public Task<IResult> RegisterAsync(RegisterDto dto) =>
            sessionService
                .TryRegisterAsync(dto.MailAddress, dto.Password, dto.Pin)
                .ToResourceAsync<UserSession, UserSessionResource>(x => Results.CreatedAtRoute(
                    routeName   : nameof(GetUserAsync),
                    routeValues : new { x.Value.Id },
                    value       : x
                ));

        [HttpPost("/auth")]
        [EndpointDescription("Tries to start a session.")]
        public Task<IResult> AuthAsync(AuthDto dto) =>
            sessionService
                .TryAuthAsync(dto.MailAddress, dto.Password)
                .ToResourceAsync<UserSession, UserSessionResource>(Results.Ok);

        [HttpPost("/reset-password")]
        [EndpointDescription("Tries to reset a password using the PIN code associated with the wanted mail address.")]
        public Task<IResult> ResetPasswordAsync(ResetPasswordDto dto) =>
            sessionService
                .TryResetPasswordAsync(dto.MailAddress, dto.NewPassword, dto.Pin)
                .ToResourceAsync<UserSession, UserSessionResource>(Results.Ok);

        [HttpGet("{id}", Name = nameof(GetUserAsync))]
        [Authorize(Policy = "LimitedUserAccess")]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Queries the user.")]
        public Task<IResult> GetUserAsync(Id id) =>
            queryService.TryGetAsync(id).ToResourceAsync<User, UserResource>(Results.Ok);

        [HttpPatch("{id}")]
        [Authorize(Policy = "LimitedUserAccess")]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Tries to update the user.")]
        public Task<IResult> UpdateAsync(Id id, UpdateAccountDto dto) =>
            sessionService
                .TryUpdateAsync(id, dto.Password, user => {

                    var response = FluentResponse.Response.Success(user);
                    if (dto.NewMailAddress is not null) response = response.OnSuccess(x => x.TryWithMailAddress(dto.NewMailAddress));
                    if (dto.NewPassword is not null)    response = response.OnSuccess(x => x.TryWithPassword(dto.NewPassword));

                    return response;

                }).ToResourceAsync<UserSession, UserSessionResource>(Results.Ok);

        [HttpPost("{id}/save-diagnosis-result")]
        [Authorize(Policy = "LimitedUserAccess")]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Tries to save a diagnosis result for the user.")]
        public Task<IResult> SaveDiagnosisResult(Id id, SaveDiagnosisResultDto dto) =>
            diagnosisResultService
                .TrySaveDiagnosisResult(id, dto.DiagnosisItemIds)
                .ToResultAsync(Results.Ok);

        [HttpPost("{id}/anonymize")]
        [Authorize(Policy = "LimitedUserAccess")]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Tries to anonymize the user.")]
        public Task<IResult> AnonymizeAsync(Id id, CloseAccountDto dto) =>
            sessionService
                .TryAnonymizeAsync(id, dto.Password)
                .ToResultAsync(Results.Ok);

        [HttpDelete("{id}")]
        [Authorize(Policy = "LimitedUserAccess")]
        [EndpointSummary("Only accessible for this user")]
        [EndpointDescription("Tries to delete the user.")]
        public Task<IResult> DeleteAsync(Id id, CloseAccountDto dto) =>
            sessionService
                .TryDeleteAsync(id, dto.Password)
                .ToResultAsync(Results.Ok);

        [HttpPost("/request-register")]
        [EndpointDescription("Tries to request the generation of a registration PIN that will be associated with the mail address.")]
        public Task<IResult> RequestRegisterAsync(RequestPinGenerationDto dto) =>
            sessionService
                .TryRequestRegistrationPINAsync(dto.MailAddress)
                .ToResultAsync(Results.Ok);

        [HttpPost("/request-password-reset")]
        [EndpointDescription("Tries to request the generation of a password reset PIN that will be associated with the account.")]
        public Task<IResult> RequestPasswordResetAsync(RequestPinGenerationDto dto) =>
            sessionService
                .TryRequestPasswordResetPINAsync(dto.MailAddress)
                .ToResultAsync(Results.Ok);

    #endregion

}
