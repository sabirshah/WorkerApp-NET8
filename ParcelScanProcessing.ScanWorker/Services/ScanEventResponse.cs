using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParcelScanProcessing.ScanWorker.Models;

namespace ParcelScanProcessing.ScanWorker.Services
{
    public class ScanEventResponse
    {
        public List<ScanEvent> ScanEvents { get; set; } = new();
    }
}
