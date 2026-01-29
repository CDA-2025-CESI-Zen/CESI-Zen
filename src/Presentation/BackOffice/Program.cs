using CesiZen.Infrastructure.Core;
using CesiZen.Application.Core;
using CesiZen.Presentation.BackOffice.Core;

var builder = WebApplication.CreateBuilder(args);

builder.InitInfrastructure();
builder.InitApplication();
builder.InitPresentation();

var app = builder.Build();

await app.FinalizePresentationAsync();

app.Run();
