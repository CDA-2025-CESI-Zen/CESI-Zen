using CesiZen.Presentation.BackOffice.Components;
using CesiZen.Infrastructure.Core;
using CesiZen.Application.Core;
using CesiZen.Presentation.BackOffice.Core;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.InitInfrastructure();
builder.InitApplication();
builder.InitPresentation();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
    
using (var scope = app.Services.CreateScope()) {
    await scope.ServiceProvider
        .GetRequiredService<ICommandService<Admin>>()
        .TryCreateAsync(() => Admin.TryCreate(builder.Configuration["ROOT_USER"]!, builder.Configuration["ROOT_PASSWORD"]!));
}

app.Run();
