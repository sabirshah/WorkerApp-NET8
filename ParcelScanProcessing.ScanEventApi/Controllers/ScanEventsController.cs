using Microsoft.AspNetCore.Mvc;
using ParcelScanProcessing.ScanEventApi.Services;

namespace ParcelScanProcessing.ScanEventApi.Controllers
{
    [ApiController]
    [Route("v1/scans/[controller]")]
    public class ScanEventsController : ControllerBase
    {
        private readonly MockScanEventStore _store;

        public ScanEventsController(MockScanEventStore store)
        {
            _store = store;
        }

        [HttpGet]
        public ActionResult Get([FromQuery] long fromEventId = 1, [FromQuery] int limit = 100)
        {
            var events = _store.GetScanEvents(fromEventId, limit);
            return Ok(new { ScanEvents = events });
        }
    }



}
