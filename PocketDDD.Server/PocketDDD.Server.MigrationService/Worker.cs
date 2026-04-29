using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PocketDDD.Server.DB;
using PocketDDD.Server.Model.DBModel;

namespace PocketDDD.Server.MigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime lifetime,
    ILogger<Worker> logger) : BackgroundService
{
    public const string ActivitySourceName = "PocketDDD.Migrations";

    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PocketDDDContext>();

            await RunMigrationAsync(db, stoppingToken);
            await SeedAsync(db, stoppingToken);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            logger.LogError(ex, "Database migration failed.");
            throw;
        }

        lifetime.StopApplication();
    }

    private static Task RunMigrationAsync(PocketDDDContext db, CancellationToken cancellationToken)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(() => db.Database.MigrateAsync(cancellationToken));
    }

    private static async Task SeedAsync(PocketDDDContext db, CancellationToken cancellationToken)
    {
        if (await db.EventDetail.AnyAsync(cancellationToken)) return;

        db.EventDetail.Add(new EventDetail { SessionizeId = "dev-event", Version = 0 });
        await db.SaveChangesAsync(cancellationToken);
    }
}
