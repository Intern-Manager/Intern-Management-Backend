using InternManagement.API.Extensions;
using InternManagement.Application;
using InternManagement.Infrastructure;
using InternManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration);

var app = builder.Build();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapPresentation();

var url = app.Urls.FirstOrDefault() ?? "http://localhost:5131";
Console.WriteLine();
Console.WriteLine("  Swagger UI: " + url + "/swagger");
Console.WriteLine();

app.Run();
