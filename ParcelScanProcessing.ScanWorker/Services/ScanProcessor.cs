using Microsoft.EntityFrameworkCore;
using ParcelScanProcessing.ScanWorker.Models;
using ParcelScanProcessing.ScanWorker.DBContext;
using ParcelScanProcessing.ScanEventApi.Handlers;

namespace ParcelScanProcessing.ScanWorker.Services;
public class ScanProcessor: IScanProcessor
{
    private readonly ScanEventDbContext _db;
    private readonly IDownstreamPublisher _publisher;
    private readonly IEnumerable<IScanEventHandler> _handlers;
    private readonly ILogger<ScanProcessor> _logger;

    public ScanProcessor(ScanEventDbContext db, IDownstreamPublisher publisher, ILogger<ScanProcessor> logger, IEnumerable<IScanEventHandler> handlers)
    {
        _db = db;
        _publisher = publisher;
        _handlers = handlers;
        _logger = logger;
    }

    public async Task ProcessEventAsync(ScanEvent ev)
    {
        var state = await _db.ParcelStates.FirstOrDefaultAsync(p => p.ParcelId == ev.ParcelId);
        if (state == null)
        {
            state = new ParcelState { ParcelId = ev.ParcelId };
            _db.ParcelStates.Add(state);
        }

        var handler = _handlers.FirstOrDefault(h => h.CanHandle(ev.Type));
        if (handler != null)
        {
            await handler.HandleAsync(ev, state);
        }
        else
        {
            _logger.LogWarning("Unknown event type '{EventType}' for ParcelId {ParcelId}", ev.Type, ev.ParcelId);
        }

        state.LastEventId = ev.EventId;
        await _publisher.PublishAsync(ev);
        await _db.SaveChangesAsync();
    }
}
