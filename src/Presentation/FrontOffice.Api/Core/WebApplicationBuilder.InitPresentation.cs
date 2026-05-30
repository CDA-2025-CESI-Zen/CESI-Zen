using System.Text;
using System.Threading.RateLimiting;
using CesiZen.Application.Services;
using CesiZen.Presentation.FrontOffice.Api.Authorization;
using FluentResponse;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace CesiZen.Presentation.FrontOffice.Api.Core;
public static partial class Extensions {

    /// <summary>
    /// Initializes all the presentations's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitPresentation(this WebApplicationBuilder builder) {

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => {

            options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer           = true,
                ValidateAudience         = true,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer              = builder.Configuration["Jwt:Issuer"],
                ValidAudience            = builder.Configuration["Jwt:Audience"],
                ClockSkew                = TimeSpan.Zero,
                IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key:User"]!))
            };
        });
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("LimitedUserAccess", policy => policy.Requirements.Add(new UserAuthorizationRequirement()));

        builder.Services.AddCors(options => {
            options.AddDefaultPolicy(builder =>
                builder
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins("cesizen.fr", "*.cesizen.fr")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
            options.AddPolicy("AllowAll",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
        });

        builder.Services.AddRateLimiter(options => {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey : httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory      : partition => new FixedWindowRateLimiterOptions {
                        AutoReplenishment = true,
                        PermitLimit       = httpContext.User.Identity?.IsAuthenticated == true ? 32 : 16,
                        QueueLimit        = 0,
                        Window            = TimeSpan.FromMinutes(1)
                    }
                )
            );
            options.OnRejected = async (context, cancellationToken) => {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsJsonAsync(
                    value : Response.Failure("Trop de requêtes ! Réessayez plus tard.") as Failure,
                    cancellationToken : cancellationToken
                );
            };
        });

        builder.Services.AddScoped<IAuthorizationHandler, UserAuthorizationHandler>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options => {

            options.SwaggerDoc("v1", new OpenApiInfo {
                Title   = "API CESI Zen Front-Office",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                Name         = "Authorization",
                Type         = SecuritySchemeType.Http,
                Scheme       = "Bearer",
                BearerFormat = "JWT",
                In           = ParameterLocation.Header,
                Description  = "Entrez votre token JWT."
            });

            options.AddSecurityRequirement(document => new() { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] });
        });

        builder.Services.AddHostedService<UserAnonymizationService>();
        
    }
}