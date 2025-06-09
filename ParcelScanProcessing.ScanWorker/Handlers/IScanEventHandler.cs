
using ParcelScanProcessing.ScanWorker.Models;

namespace ParcelScanProcessing.ScanEventApi.Handlers
{
    public interface IScanEventHandler
    {
        bool CanHandle(string eventType);
        Task HandleAsync(ScanEvent scanEvent, ParcelState state);
    }
}
