using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace ParcelScanProcessing.ScanWorker.Services
{
    public interface IScanEventApi
    {
        [Get("/v1/scans/scanevents")]
        Task<ApiResponse<ScanEventResponse>> GetScanEventsAsync([AliasAs("fromEventId")] long fromEventId = 1, [AliasAs("limit")] int limit = 100);
    }
}
