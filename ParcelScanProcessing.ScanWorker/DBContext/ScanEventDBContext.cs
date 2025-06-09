using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ParcelScanProcessing.ScanWorker.Models;

namespace ParcelScanProcessing.ScanWorker.DBContext
{
    public class ScanEventDbContext : DbContext
    {

        public DbSet<ParcelState> ParcelStates => Set<ParcelState>();
        public DbSet<LastProcessedEvent> LastProcessedEvents => Set<LastProcessedEvent>();

        public ScanEventDbContext(DbContextOptions<ScanEventDbContext> options)
            : base(options) { }
    }
}
