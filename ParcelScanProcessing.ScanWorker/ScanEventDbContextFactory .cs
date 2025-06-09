using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using ParcelScanProcessing.ScanWorker.DBContext;

namespace ParcelScanProcessing.ScanWorker
{
    public class ScanEventDbContextFactory : IDesignTimeDbContextFactory<ScanEventDbContext>
    {
        public ScanEventDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ScanEventDbContext>();

            // Adjust path if needed
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "scanworker.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new ScanEventDbContext(optionsBuilder.Options);
        }
    }
}
