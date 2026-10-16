using AhiomaMigrationWorker.AhiomaMigrationWorker;
using AhiomaMigrationWorker.Dtos;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AhiomaMigrationWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog first
            var logPath = Path.Combine(AppContext.BaseDirectory, "Logs", "migration-log-.txt");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(logPath, rollingInterval: Serilog.RollingInterval.Day, shared: true)
                .CreateLogger();

            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));
            
            builder.Services.AddDbContext<AhiomaDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddHttpClient<IApiClient, ApiClient>();

            builder.Services.AddScoped<ProductSyncTask>();
            builder.Services.AddScoped<ProductImageUploader>();
            builder.Services.AddHostedService<Worker>();

            // Replace default logging with Serilog
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            // Configure as Windows Service
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "Ahioma Migration Worker";
            });

            var host = builder.Build();
            host.Run();
        }
    }
}