using InternManagement.API.Extensions;
using InternManagement.Application;
using InternManagement.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration);

var app = builder.Build();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapPresentation();

var url = app.Urls.FirstOrDefault() ?? "http://localhost:5131";
Console.WriteLine();
Console.WriteLine("  Swagger UI: " + url + "/swagger");
Console.WriteLine();

app.Run();
