using System.Drawing;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Models;

namespace TickerQDemo.Jobs;

public class MyJob
{
    private readonly ILogger<MyJob> _logger;

    public MyJob(ILogger<MyJob> logger)
    {
        _logger = logger;
    }

    [TickerFunction("CleanUpLogs")]
    public void CleanUpLogs()
    {
        _logger.LogInformation("Cleaning up logs...");
        // Logic to clean up logs would go here in a real application.
    }

    [TickerFunction("WithAnObject")]
    public void WithObject(TickerFunctionContext<Point> tickerContext)
    {         
        var point = tickerContext.Request;
        _logger.LogInformation("Point received: X={x}, Y={y}", point.X, point.Y);
    }

    [TickerFunction("ScheduledCleanUpLogs", "*/1 * * * *")] // Runs every minute
    public void ScheduledCleanUpLogs()
    {
        _logger.LogInformation("Cleaning up logs...");
        // Logic to clean up logs would go here in a real application.
    }
}
