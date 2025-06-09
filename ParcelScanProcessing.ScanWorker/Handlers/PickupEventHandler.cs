using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParcelScanProcessing.ScanEventApi.Handlers;
using ParcelScanProcessing.ScanWorker.Models;

namespace ParcelScanProcessing.ScanWorker.Handlers
{
    public class PickupEventHandler : IScanEventHandler
    {
        public bool CanHandle(string eventType) => eventType.Equals("PICKUP", StringComparison.OrdinalIgnoreCase);

        public Task HandleAsync(ScanEvent scanEvent, ParcelState state)
        {
            state.PickupTime = scanEvent.CreatedDateTimeUtc;
            return Task.CompletedTask;
        }
    }
}
