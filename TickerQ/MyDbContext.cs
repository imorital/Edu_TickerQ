using Microsoft.EntityFrameworkCore;
using TickerQ.EntityFrameworkCore.Configurations;

namespace TickerQDemo;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new TimeTickerConfigurations());  // Allow to run at a certain time
        modelBuilder.ApplyConfiguration(new CronTickerConfigurations());  // Allow to run as a cron configuration
        modelBuilder.ApplyConfiguration(new CronTickerOccurrenceConfigurations());

        // Alternate to the 3 lines above we can just set up configurations by scanning the assembly
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(TimeTickerConfigurations).Assembly);
    }
}
