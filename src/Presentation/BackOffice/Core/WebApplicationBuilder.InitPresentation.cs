using System.Text;
using Blazored.LocalStorage;
using CesiZen.Presentation.BackOffice.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using MudBlazor.Translations;

namespace CesiZen.Presentation.BackOffice.Core;
public static partial class Extensions {

    /// <summary>
    /// Initializes all the presentations's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitPresentation(this WebApplicationBuilder builder) {
        builder.Services.AddMudServices();
        builder.Services.AddMudTranslations();
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer           = true,
                ValidateAudience         = true,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer              = builder.Configuration["Jwt:Issuer"]!,
                ValidAudience            = builder.Configuration["Jwt:Audience"]!,
                IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key:Admin"]!))
            };

            options.Events = new JwtBearerEvents {
                OnChallenge = context => {
                    context.HandleResponse();
                    context.Response.Redirect("/");
                    return Task.CompletedTask;
                }
            };
        });

        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddScoped<JwtAuthenticationStateProvider, JwtAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider>(x => x.GetRequiredService<JwtAuthenticationStateProvider>());
    }
}