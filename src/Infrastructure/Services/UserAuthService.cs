using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CesiZen.Domain.Aggregates.Accounts;
using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CesiZen.Infrastructure.Services;
public sealed class UserAuthService(
    TimeSpan authenticationTokenExpiry,
    string   tokenIssuer,
    string   tokenAudience,
    string   encodingKey
) : IUserAuthService {

    #region PROPERTIES

        private readonly TimeSpan authenticationTokenExpiry = authenticationTokenExpiry;
        private readonly string   tokenIssuer               = tokenIssuer;
        private readonly string   tokenAudience             = tokenAudience;

        private readonly SigningCredentials credentials = new (
            key       : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encodingKey)),
            algorithm : SecurityAlgorithms.HmacSha256
        );

    #endregion
    #region CONSTRUCTORS

        public UserAuthService(
            IConfiguration configuration
        ) : this(
            authenticationTokenExpiry : TimeSpan.Parse(configuration["Jwt:User:Expiry"]!),
            tokenIssuer               : configuration["Jwt:User:Issuer"]!,
            tokenAudience             : configuration["Jwt:User:Audience"]!,
            encodingKey               : configuration["Jwt:User:Key"]!
        ) {}

    #endregion
    #region METHODS

        public IResponse<string> TryGenerateToken(User user) {
            if (user.MailAddress is null)
                return Response.Failure<string>(new InvalidOperationException("Impossible de générer un token pour un compte anonymisé !"));

            Claim[] claims = [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Email,            user.MailAddress.Address),
            ];


            JwtSecurityToken token = new (
                issuer             : this.tokenIssuer,
                audience           : this.tokenAudience,
                claims             : claims,
                expires            : DateTime.Now.Add(this.authenticationTokenExpiry),
                signingCredentials : this.credentials
            );

            return Response.Success(new JwtSecurityTokenHandler().WriteToken(token));
        }

    #endregion
    
}