namespace CesiZen.Presentation.FrontOffice.Api.Core;
public static partial class Extensions {

    /// <summary>
    /// Finalizes all the presentations's services.
    /// </summary>
    /// <param name="self">The app builder.</param>
    public static void FinalizePresentation(this WebApplication app, params string[] args) {
        
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("AllowAll");
        } else app.UseCors();

        app.UseHsts();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();
        app.MapControllers();

        if (args.Any(a =>
            string.Equals(a, "--init-db", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(a, "-i", StringComparison.OrdinalIgnoreCase))
        ) app.InitDb(forceInit: args.Any(a =>
            string.Equals(a, "--force-init", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(a, "-f", StringComparison.OrdinalIgnoreCase)
        ), dev: args.Any(a =>
            string.Equals(a, "--dev", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(a, "-d", StringComparison.OrdinalIgnoreCase))
        );
    }
}