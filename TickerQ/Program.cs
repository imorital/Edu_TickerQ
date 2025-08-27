using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Drawing;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;
using TickerQ.Utilities;
using TickerQ.Utilities.Interfaces.Managers;
using TickerQ.Utilities.Models.Ticker;
using TickerQDemo;
using TickerQDemo.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Use builder.Configuration instead of Configuration
builder.Services.AddDbContext<MyDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")
                               , options => options.CommandTimeout(180)));

builder.Services.AddTickerQ(options =>
{
    options.AddOperationalStore<MyDbContext>(efOpt =>
    {
        efOpt.UseModelCustomizerForMigrations();
        efOpt.CancelMissedTickersOnAppStart();
    });
    options.AddDashboard(dbopt =>
    {
        // Mount path for the dashboard UI (default: "/tickerq-dashboard").
        dbopt.BasePath = "/tickerq-dashboard";

        // Allowed CORS origins for dashboard API (default: ["*"]).
        //dbopt.CorsOrigins = new[] { "https://arcenox.com" };

        // Backend API domain (scheme/SSL prefix supported).
        //dbopt.BackendDomain = "ssl:arcenox.com";

        // Authentication
        dbopt.EnableBuiltInAuth = true;  // Use TickerQ’s built-in auth (default).
        dbopt.UseHostAuthentication = false; // Use host auth instead (off by default).
        dbopt.RequiredRoles = new[] { "Admin", "Ops" };
        dbopt.RequiredPolicies = new[] { "TickerQDashboardAccess" };

        // Basic auth toggle (default: false).
        dbopt.EnableBasicAuth = true;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseTickerQ();

// Programatically scheduled job via an endpoint
app.MapPost("/schedule", async (Point point, ITimeTickerManager<TimeTicker> timeTickerManager) =>
{
    await timeTickerManager.AddAsync(new TimeTicker
    {
        Request = TickerHelper.CreateTickerRequest<Point>(point),
        ExecutionTime = DateTime.UtcNow.AddSeconds(10), // Schedule to run after 10 seconds
        Function = nameof(MyJob.WithObject),
        Description = "A job with an object",
        Retries = 3,
        RetryIntervals = [1, 2, 3]
    });
    return Results.Ok();
});

app.UseHttpsRedirection();

app.Run();
