using System.Text.Json.Serialization;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Accounts.ValueObjects;
using CesiZen.Presentation.FrontOffice.Api.Controllers;

namespace CesiZen.Presentation.FrontOffice.Api.Resources;
public class UserResource(User user) : IResource<UserResource, User> {

    #region PROPERTIES

        [JsonIgnore]
        public Id Id { get; } = user.Id;

        public string? MailAddress { get; } = user.MailAddress?.Address;

        public UserDiagnosisResult? FirstDiagnosisResult { get; } = user.FirstDiagnosisResult;
        public UserDiagnosisResult? LastDiagnosisResult  { get; } = user.LastDiagnosisResult;

        public UserLinks Links { get; } = new(
            Self                : new (HttpMethod.GET,  UserController.ROUTE, user.Id),
            SaveDiagnosisResult : new (HttpMethod.POST, UserController.ROUTE, user.Id, "save-diagnosis-result"),
            Anonymize           : new (HttpMethod.POST, UserController.ROUTE, user.Id, "anonymize")
        );

        public readonly record struct UserLinks(
            Link Self,
            Link SaveDiagnosisResult,
            Link Anonymize
        );

    #endregion
    #region METHODS

        public static implicit operator UserResource(User ressource) => new (ressource);

        public static UserResource From(User ressource) => ressource;
        public static IEnumerable<UserResource> From(IEnumerable<User> ressources) => ressources.Select(From);

    #endregion

}