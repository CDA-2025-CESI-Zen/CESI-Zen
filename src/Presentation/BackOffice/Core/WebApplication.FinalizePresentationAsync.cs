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
    public static void FinalizePresentation(this WebApplication app) {
        
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
        }

        app.UseHttpsRedirection();
        app.UseHsts();
        app.UseAntiforgery();
        app.MapStaticAssets();
        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

    }
}