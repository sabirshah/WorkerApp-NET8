using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParcelScanProcessing.ScanWorker.Models;

namespace ParcelScanProcessing.ScanWorker.Services
{

    public class ConsolePublisher : IDownstreamPublisher
    {
        public Task PublishAsync(ScanEvent scanEvent)
        {
            Console.WriteLine($"[PUBLISH] ParcelId: {scanEvent.ParcelId}, Type: {scanEvent.Type}, Time: {scanEvent.CreatedDateTimeUtc}");
            return Task.CompletedTask;
        }
    }
}
