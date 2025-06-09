using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelScanProcessing.ScanWorker.Models
{
    public class LastProcessedEvent
    {
        public int Id { get; set; } = 1; // Single row table
        public long LastEventId { get; set; }
    }
}
