using ParcelScanProcessing.ScanEventApi.Models;

namespace ParcelScanProcessing.ScanEventApi.Services
{
    public class MockScanEventStore
    {
        private static readonly List<ScanEvent> _scanEvents;

        static MockScanEventStore()
        {
            _scanEvents = new List<ScanEvent>();
            var random = new Random();
            var types = new[] { "PICKUP", "STATUS", "DELIVERY" };

            for (int i = 1; i <= 200; i++)
            {
                _scanEvents.Add(new ScanEvent
                {
                    EventId = i,
                    ParcelId = random.Next(1000, 1005),
                    Type = types[random.Next(types.Length)],
                    CreatedDateTimeUtc = DateTime.UtcNow.AddMinutes(-i),
                    StatusCode = "",
                    Device = new Device
                    {
                        DeviceTransactionId = i,
                        DeviceId = random.Next(100, 110)
                    },
                    User = new User
                    {
                        UserId = "NC1001",
                        CarrierId = "NC",
                        RunId = (100 + i % 5).ToString()
                    }
                });
            }
        }

        public IEnumerable<ScanEvent> GetScanEvents(long fromEventId, int limit)
        {
            return _scanEvents
                .Where(e => e.EventId >= fromEventId)
                .OrderBy(e => e.EventId)
                .Take(limit);
        }
    }

}
