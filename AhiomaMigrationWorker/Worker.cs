using AhiomaMigrationWorker.Dtos;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.Extensions.Options;

namespace AhiomaMigrationWorker
{

    using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
    using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
    using Microsoft.Extensions.Options;
    using System.Data.Entity;

    namespace AhiomaMigrationWorker
    {
        public class Worker : BackgroundService
        {
            private readonly ILogger<Worker> _logger;
            private readonly ApiSettings _apiSettings;
            private readonly IApiClient _apiClient;
            private readonly IServiceScopeFactory _scopeFactory;

            public Worker(
                ILogger<Worker> logger,
                IOptions<ApiSettings> options,
                IApiClient apiClient,
                IServiceScopeFactory scopeFactory)
            {
                _logger = logger;
                _apiSettings = options.Value;
                _apiClient = apiClient;
                _scopeFactory = scopeFactory;
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                _logger.LogInformation("API BaseUrl: {url}", _apiSettings.BaseUrl);
                _logger.LogInformation("API Token: {token}", _apiSettings.BearerToken.Substring(0, 10) + "...");

                int batchSize = 500;

                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AhiomaDbContext>();
                    var syncTask = scope.ServiceProvider.GetRequiredService<ProductSyncTask>();
                    var syncImageTask = scope.ServiceProvider.GetRequiredService<ProductImageUploader>();

                    // Get unsynced products in small batches
                    //var products = db.Products
                    //    .Include(x=>x.ProductPictures)
                    //    .Where(x => x.TenantId == 10266) // only tenant 1
                    //                 .Where(p => p.Published &&
                    //                             !db.ApiSyncMaps.Any(m => m.EntityType == "Product" && m.LocalEntityId == p.Id))
                    //                 .Where(x=>x.ProductPictures.Any()) // only products with images
                    //                 .OrderBy(p => p.Id) // deterministic order
                    //                 .Take(batchSize)
                    //                 .ToList();

                    int productscount = db.Products
.Where(x => x.TenantId != 10266)
.Where(x => x.Published)
.Where(x => x.ProductPictures.Any())
.Select(x => x.Id)
.Count();
                    var productsmacount = db.Products
    .Where(x => x.TenantId != 10266)
    .Where(x => x.Published)
    .Where(x => x.ProductPictures.Any())
    .Where(x => !db.ApiSyncMaps
        .Where(m => m.EntityType == "Product")
        .Select(m => m.LocalEntityId)
        .Contains(x.Id))
    .OrderBy(x => x.Id)
    .Select(x => x.Id) 
    .Count();

                    var products = db.Products
    .Where(x => x.TenantId != 10266)
    .Where(x => x.Published)
    .Where(x => x.ProductPictures.Any())
    .Where(x => !db.ApiSyncMaps
        .Where(m => m.EntityType == "Product")
        .Select(m => m.LocalEntityId)
        .Contains(x.Id))
    .OrderBy(x => x.Id)
    .Select(x => x.Id)
    .Take(batchSize)
    .ToList();

                    
                    if (!products.Any())
                    {
                        _logger.LogInformation("✅ All products are synced. Sleeping...");
                        await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken); // sleep longer if nothing left
                        continue;
                    }

                    foreach (var product in products)
                    {
                        try
                        {
                            await syncTask.SyncProductAsync(product);

                            // Optional: small delay to avoid API overload
                            await Task.Delay(100, stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error syncing product {id}", product);
                        }
                    }

                    // Shorter wait before grabbing next batch
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }

            }
        }
    }



    //public class Worker : BackgroundService
    //{
    //    private readonly ILogger<Worker> _logger;
    //    private readonly ApiSettings _apiSettings;
    //    private readonly IApiClient _apiClient;
    //    private readonly ProductSyncTask _syncTask;
    //    private readonly AhiomaDbContext _db;
    //    private readonly IServiceScopeFactory _scopeFactory;


    //    public Worker(ILogger<Worker> logger, IOptions<ApiSettings> options, IApiClient apiClient, ProductSyncTask syncTask, AhiomaDbContext db)
    //    {
    //        _logger = logger;
    //        _apiSettings = options.Value;
    //        _apiClient = apiClient;
    //        _syncTask = syncTask;
    //        _db = db;
    //    }

    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        _logger.LogInformation("API BaseUrl: {url}", _apiSettings.BaseUrl);
    //        _logger.LogInformation("API Token: {token}", _apiSettings.BearerToken.Substring(0, 10) + "...");
    //        while (!stoppingToken.IsCancellationRequested)
    //        {
    //            var products = _db.Products
    //                              .Where(p => p.Published) // example filter: only published
    //                              .Take(10) // sync 10 at a time
    //                              .ToList();

    //            foreach (var product in products)
    //            {
    //                try
    //                {
    //                    await _syncTask.SyncProductAsync(product.Id);
    //                }
    //                catch (Exception ex)
    //                {
    //                    _logger.LogError(ex, "Error syncing product {id}", product.Id);
    //                }
    //            }

    //            // wait before next batch
    //            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
    //        }
    //        //while (!stoppingToken.IsCancellationRequested)
    //        //{
    //        //    try
    //        //    {
    //        //        var products = await _apiClient.GetAsync<ApiResponse>(
    //        //            "products?categoryId=cmfwk1gqv000110xdtxb8az8e");

    //        //        _logger.LogInformation("Fetched {count} products from API",
    //        //            products?.Data?.Products?.Count ?? 0);
    //        //    }
    //        //    catch (Exception ex)
    //        //    {
    //        //        _logger.LogError(ex, "Error fetching products");
    //        //    }

    //        //    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
    //        //}
    //    }
    //}
}
