using CesiZen.Application.Core;
using CesiZen.Infrastructure.Core;
using CesiZen.Presentation.FrontOffice.Api.Core;

var builder = WebApplication.CreateBuilder(args);

builder.InitInfrastructure();
builder.InitApplication();
builder.InitPresentation();

var app = builder.Build();
app.FinalizePresentation();

app.Run();
