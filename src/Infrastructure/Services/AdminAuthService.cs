using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CesiZen.Domain.Aggregates.Accounts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CesiZen.Infrastructure.Services;
public sealed class AdminAuthService(
    TimeSpan authenticationTokenExpiry,
    string   tokenIssuer,
    string   tokenAudience,
    string   encodingKey
) : IAdminAuthService {

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

        public AdminAuthService(
            IConfiguration configuration
        ) : this(
            authenticationTokenExpiry : TimeSpan.Parse(configuration["Jwt:Expiry:Admin"]!),
            tokenIssuer               : configuration["Jwt:Issuer"]!,
            tokenAudience             : configuration["Jwt:Audience"]!,
            encodingKey               : configuration["Jwt:Key"]!
        ) {}

    #endregion
    #region METHODS

        public string GenerateToken(Admin admin) {
            Claim[] claims = [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
                new Claim(ClaimTypes.Email,            admin.MailAddress.Address),
            ];


            JwtSecurityToken token = new (
                issuer             : this.tokenIssuer,
                audience           : this.tokenAudience,
                claims             : claims,
                expires            : DateTime.Now.Add(this.authenticationTokenExpiry),
                signingCredentials : this.credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    #endregion

}