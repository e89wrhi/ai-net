using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.Common.PersistMessageProcessor;

/// <summary>
/// A worker that runs in the background and periodically checks the "Outbox" for messages that need to be sent.
/// It ensures that system events are eventually delivered, even if there's a temporary failure.
/// </summary>
public class PersistMessageBackgroundService(
    ILogger<PersistMessageBackgroundService> logger,
    IServiceProvider serviceProvider,
    IOptions<PersistMessageOptions> options
)
    : BackgroundService
{
    private PersistMessageOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("PersistMessage Background Service Start");

        await ProcessAsync(stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("PersistMessage Background Service Stop");

        return base.StopAsync(cancellationToken);
    }

    private async Task ProcessAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await using (var scope = serviceProvider.CreateAsyncScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IPersistMessageProcessor>();
                await service.ProcessAllAsync(stoppingToken);
            }

            var delay = _options.Interval is { }
                            ? TimeSpan.FromSeconds((int)_options.Interval)
                            : TimeSpan.FromSeconds(30);

            await Task.Delay(delay, stoppingToken);
        }
    }
}