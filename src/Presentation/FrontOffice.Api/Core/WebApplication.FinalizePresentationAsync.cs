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
        }

        app.UseHttpsRedirection();
        app.UseHsts();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();
        app.MapControllers();

        if (args.Any(a =>
            string.Equals(a, "new-db", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(a, "-n", StringComparison.OrdinalIgnoreCase))
        ) app.InitDb(args.Any(a =>
            string.Equals(a, "dev-db", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(a, "-d", StringComparison.OrdinalIgnoreCase))
        );
    }
}