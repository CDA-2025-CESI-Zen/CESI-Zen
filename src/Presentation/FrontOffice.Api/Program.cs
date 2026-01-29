using CesiZen.Application.Core;
using CesiZen.Infrastructure.Core;
using CesiZen.Presentation.FrontOffice.Api.Core;

var builder = WebApplication.CreateBuilder(args);

builder.InitInfrastructure();
builder.InitApplication();
builder.InitPresentation();

var app = builder.Build();
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHsts();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
