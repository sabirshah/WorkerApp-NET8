namespace ParcelScanProcessing.ScanEventApi.Models
{
    public class ScanEvent
    {
        public long EventId { get; set; }
        public int ParcelId { get; set; }
        public string Type { get; set; } // PICKUP, STATUS, DELIVERY
        public DateTime CreatedDateTimeUtc { get; set; }
        public string StatusCode { get; set; }
        public Device Device { get; set; }
        public User User { get; set; }
    }
}
