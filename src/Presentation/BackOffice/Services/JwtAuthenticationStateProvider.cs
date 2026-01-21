using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CesiZen.Presentation.BackOffice.Services;
public class JwtAuthenticationStateProvider(ILocalStorageService localStorageService) : AuthenticationStateProvider {

    private const string TOKEN_KEY = "authToken";

    public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
        try {

            var token = await localStorageService.GetItemAsync<string>(TOKEN_KEY);

            if (string.IsNullOrWhiteSpace(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var claims   = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            var user     = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);

        } catch {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyUserAuthentication(string token) {

        var claims            = ParseClaimsFromJwt(token);
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        var authState         = Task.FromResult(new AuthenticationState(authenticatedUser));

        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout() {

        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState     = Task.FromResult(new AuthenticationState(anonymousUser));

        NotifyAuthenticationStateChanged(authState);
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt) {
        var handler = new JwtSecurityTokenHandler();
        var token   = handler.ReadJwtToken(jwt);

        return token.Claims;
    }
}