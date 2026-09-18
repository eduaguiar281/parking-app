using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ParkingApp.Api.Api;
using ParkingApp.Api.Application.Cash;
using ParkingApp.Api.Application.Operations;
using ParkingApp.Api.Application.Reports;
using ParkingApp.Api.Application.Sectors;
using ParkingApp.Api.Application.Sessions;
using ParkingApp.Api.Application.Spots;
using ParkingApp.Api.Application.Stays;
using ParkingApp.Api.Application.Tariffs;
using ParkingApp.Api.Application.Users;
using ParkingApp.Api.Infrastructure;
using ParkingApp.Api.Infrastructure.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    o.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

var dbPath = builder.Configuration.GetConnectionString("Parking")
    ?? Path.Combine(builder.Environment.ContentRootPath, "..", "parking.db");
builder.Services.AddDbContext<ParkingDbContext>(o => o.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.Name = "ParkingApp.Session";
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        o.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        o.Events.OnRedirectToAccessDenied = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuditWriter>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SectorService>();
builder.Services.AddScoped<SpotService>();
builder.Services.AddScoped<TariffService>();
builder.Services.AddScoped<CashService>();
builder.Services.AddScoped<RegisterEntry>();
builder.Services.AddScoped<ExitStay>();
builder.Services.AddScoped<CancelStay>();
builder.Services.AddScoped<StayHistory>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ReportService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ParkingDbContext>();
    db.Database.EnsureCreated();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
    var password = builder.Configuration["Bootstrap:Password"]
        ?? Environment.GetEnvironmentVariable("PARKING_BOOTSTRAP_PASSWORD")
        ?? "admin123";
    await BootstrapAdmin.EnsureAsync(db, hasher, password);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/alive", () => Results.Text("estou vivo", "text/plain"));

var api = app.MapGroup("/api").RequireAuthorization();
api.MapSession();
api.MapCatalog();
api.MapOperations();

app.Run();

public partial class Program;
