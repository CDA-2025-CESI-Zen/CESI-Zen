using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Presentation.BackOffice.Components;
using FluentResponse;

namespace CesiZen.Presentation.BackOffice.Core;
public static partial class Extensions {

    /// <summary>
    /// Finalizes all the presentations's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static async Task FinalizePresentationAsync(this WebApplication app) {
        
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
        }

        app.UseHttpsRedirection();
        app.UseHsts();
        app.UseAntiforgery();
        app.MapStaticAssets();
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        using var scope = app.Services.CreateScope();
        await scope.ServiceProvider
            .GetRequiredService<IRepository<Admin>>()
            .TryAddAsync(Admin.TryCreate(app.Configuration["Root:MailAddress"]!, app.Configuration["Root:Password"]!).Unwrap());
    }
}