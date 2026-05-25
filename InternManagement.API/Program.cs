using InternManagement.API.Extensions;
using InternManagement.Application;
using InternManagement.Infrastructure;
using InternManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 524288000L; // 500 MB
});

var app = builder.Build();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors();

app.MapPresentation();

app.UseAuthentication();
app.UseAuthorization();

var displayUrl = app.Urls.FirstOrDefault(u => u.StartsWith("https")) ?? app.Urls.FirstOrDefault() ?? "https://localhost:5131";
Console.WriteLine();
Console.WriteLine("  Swagger UI: " + displayUrl + "/swagger");
Console.WriteLine();

app.Run();
