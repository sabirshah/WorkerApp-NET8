using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParcelScanProcessing.ScanWorker.Models;

namespace ParcelScanProcessing.ScanWorker.Services
{
    public interface IScanProcessor
    {
        Task ProcessEventAsync(ScanEvent ev);
    }
}
