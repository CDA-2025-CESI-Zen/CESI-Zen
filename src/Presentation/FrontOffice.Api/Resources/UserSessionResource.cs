using CesiZen.Application.Core.ValueObjects;

namespace CesiZen.Presentation.FrontOffice.Api.Resources;
public class UserSessionResource(UserSession userSession) : UserResource(userSession.User), IResource<UserSessionResource, UserSession> {

    #region PROPERTIES

        public string Token { get; } = userSession.Token;

    #endregion
    #region METHODS

        public static implicit operator UserSessionResource(UserSession userSession) => new (userSession);

        public static UserSessionResource From(UserSession userSession) => userSession;
        public static IEnumerable<UserSessionResource> From(IEnumerable<UserSession> userSession) => userSession.Select(From);

    #endregion

}