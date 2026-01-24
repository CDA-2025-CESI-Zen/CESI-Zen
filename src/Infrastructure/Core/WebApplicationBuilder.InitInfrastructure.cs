using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CesiZen.Infrastructure.Services;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Domain.Aggregates.Content;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Diagnoses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CesiZen.Infrastructure.Core;
public static partial class Extensions {

    /// <summary>
    /// Initializes all the infrastructure's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitInfrastructure(this WebApplicationBuilder builder) {

        builder.Services.AddDbContext<DbContext, ApplicationDbContext>();

        builder.Services.AddScoped<IRepository<Admin>,             Repository<Admin>>();
        builder.Services.AddScoped<IRepository<User>,              Repository<User>>();
        builder.Services.AddScoped<IRepository<Category>,          CategoryRepository>();
        builder.Services.AddScoped<IRepository<Page>,              PageRepository>();
        builder.Services.AddScoped<IRepository<DiagnosisAnalysis>, Repository<DiagnosisAnalysis>>();
        builder.Services.AddScoped<IRepository<DiagnosisItem>,     Repository<DiagnosisItem>>();

        builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
        builder.Services.AddScoped<IUserAuthService,  UserAuthService>();

        builder.Services.AddSingleton<IPasswordResetCacheService,          PasswordResetCacheService>();
        builder.Services.AddSingleton<IRegistrationValidationCacheService, RegistrationValidationCacheService>();

        builder.Services.AddScoped<IMailService, MailService>();

        builder.Services.AddScoped<IEncryptionService, EncryptionService>();

    }
}