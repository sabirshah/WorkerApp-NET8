using Microsoft.EntityFrameworkCore;
using ParcelScanProcessing.ScanEventApi.Handlers;
using ParcelScanProcessing.ScanWorker;
using ParcelScanProcessing.ScanWorker.DBContext;
using ParcelScanProcessing.ScanWorker.Handlers;
using ParcelScanProcessing.ScanWorker.Models;
using ParcelScanProcessing.ScanWorker.Services;
using Refit;
using Serilog;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("loggersettings.json", optional: false, reloadOnChange: true);
    })
    .UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext();
    });


// Configure the application services
builder.ConfigureServices((hostContext, services) =>
{
    var basePath = AppContext.BaseDirectory; // points to output folder
    var dbPath = Path.Combine(basePath, "App_Data", "scanworker.db");
    var fullPath = Path.GetFullPath(dbPath);

    services.AddDbContext<ScanEventDbContext>(options =>
        options.UseSqlite($"Data Source={fullPath}"));


    services.AddRefitClient<IScanEventApi>()
        .ConfigureHttpClient(c =>
        {
            c.BaseAddress = new Uri("http://localhost:5034"); // Adjust if different port  
        });
    services.AddScoped<IScanProcessor, ScanProcessor>();
    services.AddSingleton<IDownstreamPublisher, ConsolePublisher>();
    services.AddScoped<IScanEventHandler, PickupEventHandler>();
    services.AddScoped<IScanEventHandler, DeliveryEventHandler>();
    services.AddScoped<ScanProcessor>();

    services.AddHostedService<Worker>();
});

var app = builder.Build();

// Ensure folders exist
var dataPath = Path.Combine(AppContext.BaseDirectory, "App_Data");
var logsPath = Path.Combine(AppContext.BaseDirectory, "logs");
Directory.CreateDirectory(dataPath);
Directory.CreateDirectory(logsPath);

// Ensure the database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ScanEventDbContext>();
    db.Database.EnsureCreated();

    if (!db.LastProcessedEvents.Any())
    {
        db.LastProcessedEvents.Add(new LastProcessedEvent { LastEventId = 1 });
        db.SaveChanges();
    }
}

await app.RunAsync();
