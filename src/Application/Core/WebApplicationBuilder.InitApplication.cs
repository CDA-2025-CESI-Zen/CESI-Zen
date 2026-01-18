using Microsoft.Extensions.DependencyInjection;
using CesiZen.Domain.Aggregates.Content;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Diagnoses;
using Microsoft.AspNetCore.Builder;
using CesiZen.Application.Services;

namespace CesiZen.Application.Core;
public static partial class Extensions {

    /// <summary>
    /// Initializes all the application's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void InitApplication(this WebApplicationBuilder builder) {

        builder.Services.AddScoped<ICommandService<Admin>,             CommandService<Admin>>();
        builder.Services.AddScoped<ICommandService<User>,              CommandService<User>>();
        builder.Services.AddScoped<ICommandService<Category>,          CommandService<Category>>();
        builder.Services.AddScoped<ICommandService<Page>,              CommandService<Page>>();
        builder.Services.AddScoped<ICommandService<DiagnosisAnalysis>, CommandService<DiagnosisAnalysis>>();
        builder.Services.AddScoped<ICommandService<DiagnosisItem>,     CommandService<DiagnosisItem>>();

        builder.Services.AddScoped<IQueryService<Admin>,             QueryService<Admin>>();
        builder.Services.AddScoped<IQueryService<User>,              QueryService<User>>();
        builder.Services.AddScoped<IQueryService<Category>,          QueryService<Category>>();
        builder.Services.AddScoped<IQueryService<Page>,              QueryService<Page>>();
        builder.Services.AddScoped<IQueryService<DiagnosisAnalysis>, QueryService<DiagnosisAnalysis>>();
        builder.Services.AddScoped<IQueryService<DiagnosisItem>,     QueryService<DiagnosisItem>>();

        builder.Services.AddScoped<IAdminSessionService, AdminSessionService>();

    }
}