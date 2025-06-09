using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using ParcelScanProcessing.ScanWorker.DBContext;
using ParcelScanProcessing.ScanWorker.Models;
using ParcelScanProcessing.ScanWorker.Services;

namespace ParcelScanProcessing.ScanWorker;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IScanEventApi _api;
    private readonly BlockingCollection<ScanEvent> _queue = new(100);
    private readonly IServiceProvider _serviceProvider;
    public Worker(ILogger<Worker> logger, IScanEventApi api, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _api = api;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Create a new dependency injection scope to safely resolve scoped services like DbContext and ScanProcessor.
        // This ensures that these services have a fresh lifecycle tied to this scope and avoids conflicts with the singleton Worker service.
        using var scope = _serviceProvider.CreateScope();
        // Resolve the ScanEventDbContext from the scoped service provider for database operations.
        var db = scope.ServiceProvider.GetRequiredService<ScanEventDbContext>();
        // Resolve the IScanProcessor implementation from the scoped service provider to handle scan event processing logic.
        var processor = scope.ServiceProvider.GetRequiredService<IScanProcessor>();


        var producerTask = Task.Run(() => PollScanEventsAsync(db,stoppingToken), stoppingToken);
        var consumerTask = Task.Run(() => ConsumeAndProcessEventsAsync(processor, db, stoppingToken), stoppingToken);

        await Task.WhenAll(producerTask, consumerTask);
    }

    private async Task PollScanEventsAsync(ScanEventDbContext db,CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var tracking = await db.LastProcessedEvents.FindAsync(1);
                var lastEventId = tracking?.LastEventId ?? 1;

                _logger.LogInformation("Polling API from EventId: {EventId}", lastEventId);
                var response = await _api.GetScanEventsAsync(lastEventId, 50);
                var events = response.Content?.ScanEvents ?? [];
                _logger.LogInformation("Fetched {Count} events from API", events.Count);

                foreach (var ev in events)
                {
                    _queue.Add(ev, stoppingToken);
                }

                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error polling events");
                await Task.Delay(10000, stoppingToken);
            }
        }
    }

    private async Task ConsumeAndProcessEventsAsync(IScanProcessor processor, ScanEventDbContext db, CancellationToken stoppingToken)
    {
        foreach (var ev in _queue.GetConsumingEnumerable(stoppingToken))
        {
            try
            {
                await processor.ProcessEventAsync(ev);

                var tracking = await db.LastProcessedEvents.FindAsync(1);
                if (tracking == null)
                {
                    tracking = new LastProcessedEvent { Id = 1, LastEventId = ev.EventId + 1 };
                    db.LastProcessedEvents.Add(tracking);
                }
                else
                {
                    tracking.LastEventId = ev.EventId + 1;
                }
                _logger.LogInformation("Processed event {EventId} for parcel {ParcelId}", ev.EventId, ev.ParcelId);

                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process and publish event {EventId}", ev.EventId);
            }
        }
    }

}