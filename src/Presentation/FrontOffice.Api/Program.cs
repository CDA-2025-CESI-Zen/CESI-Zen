using CesiZen.Application.Core;
using CesiZen.Infrastructure.Core;
using CesiZen.Presentation.FrontOffice.Api.Core;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.InitInfrastructure();
builder.InitApplication();
builder.InitPresentation();

var app = builder.Build();
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

var options = new ForwardedHeadersOptions {
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};

options.KnownIPNetworks.Clear();
options.KnownProxies.Clear();

app.UseForwardedHeaders(options);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
