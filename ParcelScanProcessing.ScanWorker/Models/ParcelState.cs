using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelScanProcessing.ScanWorker.Models
{
    public class ParcelState
    {
        public int Id { get; set; } // Primary key
        public long ParcelId { get; set; }
        public long LastEventId { get; set; }
        public string LastStatus { get; set; } = "";
        public DateTime? PickupTime { get; set; }
        public DateTime? DeliveryTime { get; set; }
    }
}
