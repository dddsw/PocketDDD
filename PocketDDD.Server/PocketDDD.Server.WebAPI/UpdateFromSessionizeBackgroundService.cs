using PocketDDD.Server.Services;

namespace PocketDDD.Server.WebAPI;

public class UpdateFromSessionizeBackgroundService(
    IServiceProvider services,
    ILogger<UpdateFromSessionizeBackgroundService> logger)
    : BackgroundService
{
    private ILogger<UpdateFromSessionizeBackgroundService> Logger { get; } = logger;
    private IServiceProvider Services { get; } = services;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Update from Sessionize background task started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            Logger.LogInformation("About to update from Sessionize.");
            try
            {
                using var scope = Services.CreateScope();

                var sessionizeService = scope.ServiceProvider.GetRequiredService<SessionizeService>();
                await sessionizeService.UpdateFromSessionize();

                Logger.LogInformation("Update from Sessionize complete.");
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Update from Sessionize failed.");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
        }
    }
}