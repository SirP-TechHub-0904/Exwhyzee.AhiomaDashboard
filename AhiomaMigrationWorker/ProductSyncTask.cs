using AhiomaMigrationWorker.Dtos;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AhiomaMigrationWorker
{
    public class ProductSyncTask
    {
        private readonly AhiomaDbContext _db;
        private readonly IApiClient _apiClient;
        private readonly ILogger<ProductSyncTask> _logger;
        private readonly ProductImageUploader _productImageUploader;
        public ProductSyncTask(AhiomaDbContext db, IApiClient apiClient, ILogger<ProductSyncTask> logger, ProductImageUploader productImageUploader)
        {
            _db = db;
            _apiClient = apiClient;
            _logger = logger;
            _productImageUploader = productImageUploader;
        }


        public async Task SyncProductAsync(long productId)
        {
            string remoteProductId = "";
            var product = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Include(p => p.ProductPictures)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == productId && p.Published);

            if (product == null)
            {
                _logger.LogWarning("Product {id} not found locally", productId);
                return;
            }
            var images = new List<string>();
            foreach (var img in product.ProductPictures)
            {
                try { 
                // Construct image URL
                string imageUrl = $"https://manageahioma.exwhyzee.ng{img.PictureUrl}";

                using var http = new HttpClient();
                var bytes = await http.GetByteArrayAsync(imageUrl);

                string base64Image = $"data:image/jpeg;base64,{Convert.ToBase64String(bytes)}";

                images.Add(base64Image);
                }
                catch (Exception x)
                {

                }
            }





            // -------------------------
            // CATEGORY
            // -------------------------
            string remoteCategoryId = "";
            var categoryMap = await _db.ApiSyncMaps
                .FirstOrDefaultAsync(x => x.EntityType == "Category" && x.LocalEntityId == product.CategoryId);

            if (categoryMap == null)
            {
                var categoryPayload = new
                {
                    name = NormalizeToSentenceCase(product.Category.Name),
                    slug = GenerateSlug(product.Category.Name),
                    description = product.Category.Description,
                    image = "",
                    isActive = product.Category.Published,
                    sortOrder = product.Category.DisplayOrder,
                    parentId = (string?)null
                };

                var categoryResponse = await _apiClient.PostAsync<object, dynamic>("categories", categoryPayload);
                if (categoryResponse.success == true)
                {
                    remoteCategoryId = categoryResponse.data.category.id;
                    await SaveMap("Category", product.CategoryId, remoteCategoryId);
                    _logger.LogInformation("✅ Category '{name}' synced. RemoteId={id}", product.Category.Name, remoteCategoryId);
                }
                else
                {

                    string error = categoryResponse.error?.ToString() ?? "";

                    if (error.Contains("Duplicate", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogWarning("⚠️ Duplicate category '{name}'. Fetching remote category by name...", product.Category.Name);

                        // Fetch all categories
                        var allCategories = await _apiClient.GetAsync<RemoteCategoryList>("categories");

                        // Match by name (case-insensitive)
                        //var match = allCategories.data.categories
                        //    .FirstOrDefault(c => c.name.Equals(product.Category.Name, StringComparison.OrdinalIgnoreCase));

                        var productCat = product.Category.Name?.Trim();

                        var match = allCategories.data.categories
                            .FirstOrDefault(c => c.name.Trim()
                            .Equals(productCat, StringComparison.OrdinalIgnoreCase));


                        if (match != null)
                        {
                            remoteCategoryId = match.id;

                            await SaveMap("Category", product.CategoryId, remoteCategoryId);

                            _logger.LogInformation(
                                "🔁 Using existing remote category for '{name}'. RemoteId={id}",
                                product.Category.Name,
                                remoteCategoryId
                            );
                        }
                        else
                        {
                            _logger.LogError("❌ Duplicate detected but remote category '{name}' not found!", product.Category.Name);
                        }
                    }
                    else
                    {
                        // Other errors
                        _logger.LogWarning("⚠️ Failed to sync category '{name}'. Using existing local mapping if any.", product.Category.Name);

                        categoryMap = await _db.ApiSyncMaps
                            .FirstOrDefaultAsync(x => x.EntityType == "Category" && x.LocalEntityId == product.CategoryId);

                        remoteCategoryId = categoryMap?.RemoteEntityId ?? "";
                    }



                    //_logger.LogWarning("⚠️ Failed to sync category '{name}'. Using existing mapping if any.", product.Category.Name);
                    //categoryMap = await _db.ApiSyncMaps
                    //    .FirstOrDefaultAsync(x => x.EntityType == "Category" && x.LocalEntityId == product.CategoryId);
                    //remoteCategoryId = categoryMap?.RemoteEntityId ?? "";
                }
            }
            else
            {
                remoteCategoryId = categoryMap.RemoteEntityId;
                _logger.LogInformation("ℹ️ Category '{name}' already mapped to {remote}", product.Category.Name, remoteCategoryId);
            }

            // -------------------------
            // SUBCATEGORIES
            // -------------------------
            var subCategories = await _db.SubCategories
                .Where(sc => sc.CategoryId == product.CategoryId)
                .ToListAsync();

            foreach (var sub in subCategories)
            {
                try
                {
                    var subMap = await _db.ApiSyncMaps
                        .FirstOrDefaultAsync(x => x.EntityType == "SubCategory" && x.LocalEntityId == sub.Id);

                    if (subMap == null)
                    {
                        var subPayload = new
                        {
                            name = NormalizeToSentenceCase(sub.Name),
                            slug = GenerateSlug(sub.Name),
                            description = sub.Description,
                            image = "",
                            isActive = sub.Published,
                            sortOrder = sub.DisplayOrder,
                            parentId = remoteCategoryId
                        };

                        var subResponse = await _apiClient.PostAsync<object, dynamic>("categories", subPayload);

                        if (subResponse.success == true)
                        {
                            string remoteSubId = subResponse.data.category.id;
                            await SaveMap("SubCategory", sub.Id, remoteSubId);
                            _logger.LogInformation("==============================================✅ SubCategory '{name}' synced. RemoteId={id}", sub.Name, remoteSubId);
                        }
                        else
                        {
                            string error = subResponse.error?.ToString() ?? "";

                            if (error.Contains("Duplicate", StringComparison.OrdinalIgnoreCase))
                            {
                                _logger.LogWarning("⚠️ Duplicate category '{name}'. Fetching remote category by name...", product.Category.Name);

                                // Fetch all categories
                                var allCategories = await _apiClient.GetAsync<RemoteCategoryList>("categories");


                                var productCat = product.Category.Name?.Trim();

                                var match = allCategories.data.categories
                                    .FirstOrDefault(c => c.name.Trim()
                                    .Equals(productCat, StringComparison.OrdinalIgnoreCase));



                                // Match by name (case-insensitive)
                              //  var match = allCategories.data.categories
                               //     .FirstOrDefault(c => c.name.Equals(product.Category.Name, StringComparison.OrdinalIgnoreCase));

                                if (match != null)
                                {
                                    remoteCategoryId = match.id;

                                    await SaveMap("Category", product.CategoryId, remoteCategoryId);

                                    _logger.LogInformation(
                                        "🔁 Using existing remote category for '{name}'. RemoteId={id}",
                                        product.Category.Name,
                                        remoteCategoryId
                                    );
                                }
                                else
                                {
                                    _logger.LogError("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx❌ Duplicate detected but remote category '{name}' not found!", product.Category.Name);
                                }
                            }
                            else
                            {
                                // Other errors
                                _logger.LogWarning("⚠️ Failed to sync category '{name}'. Using existing local mapping if any.", product.Category.Name);

                                categoryMap = await _db.ApiSyncMaps
                                    .FirstOrDefaultAsync(x => x.EntityType == "Category" && x.LocalEntityId == product.CategoryId);

                                remoteCategoryId = categoryMap?.RemoteEntityId ?? "";
                            }
                            _logger.LogWarning("❌ Failed to sync SubCategory '{name}'", sub.Name);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("ℹ️ SubCategory '{name}' already mapped to {remote}", sub.Name, subMap.RemoteEntityId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Exception syncing SubCategory '{name}'", sub.Name);
                }
            }

            // -------------------------
            // PRODUCT
            // -------------------------
            var productMap = await _db.ApiSyncMaps
                .FirstOrDefaultAsync(x => x.EntityType == "Product" && x.LocalEntityId == product.Id);

            if (productMap == null)
            {
                if (remoteCategoryId != string.Empty)
                {
                    try
                    {
                        var cleanName = NormalizeToSentenceCase(product.Name);
                        var productPayload = new
                        {
                            name = cleanName,
                            slug = GenerateSlug(cleanName),
                            categoryId = remoteCategoryId,
                            price = product.Price.ToString("F2"),
                            description = RemoveImageTags(product.FullDescription),
                            shortDescription = product.ShortDescription,
                            sku = product.ProductCode ?? $"SKU-{product.Id}",
                            barcode = "",
                            comparePrice = 0,
                            costPrice = 0,
                            weight = 0,
                            images = images,
                            status = product.Published ? "ACTIVE" : "INACTIVE",
                            featured = product.ShowOnHomePage,
                            tags = !string.IsNullOrEmpty(product.MetaKeywords)
                                ? product.MetaKeywords.Split(',').Select(t => t.Trim()).ToArray()
                                : new string[] { },
                            attributes = new
                            {
                                brand = product.Manufacturer?.Name ?? "Generic",
                                fit = product.Flag ?? "Regular"
                            }
                        };

                        var productResponse = await _apiClient.PostAsync<object, dynamic>("products", productPayload);

                        if (productResponse.success == true)
                        {
                            remoteProductId = productResponse.data.product.id;
                            await SaveMap("Product", product.Id, remoteProductId);
                            _logger.LogWarning("========================================================");
                            _logger.LogWarning("===========   PRODUCT UPLOADED SUCCESSFULLY   ==========");
                            _logger.LogInformation("✅ Product '{name}' synced. RemoteId={id}", product.Name, remoteProductId);
                        }
                        else if (productResponse.success == false &&
                                 productResponse.error.ToString().Contains("Duplicate field value"))
                        {
                            var existing = await _apiClient.GetAsync<dynamic>($"products/by-slug/{productPayload.slug}");
                            if (existing?.success == true && existing.data?.product != null)
                            {
                                remoteProductId = existing.data.product.id.ToString();
                                await SaveMap("Product", product.Id, remoteProductId);
                                _logger.LogWarning("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
                                _logger.LogWarning("xxxxxxxxxxxxxx   PRODUCT DUPLICATE   xxxxxxxxxxxxxxxx");
                                _logger.LogInformation("⚠️ Product '{name}' duplicate. Using RemoteId={id}", product.Name, remoteProductId);
                            }
                            else
                            {
                                _logger.LogWarning("❌vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv Duplicate reported but product '{name}' not found by name", product.Name);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("❌bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb Failed to sync product '{name}'", product.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌nnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn Exception syncing product '{name}'", product.Name);
                    }
                }
            }
            else
            {
                remoteProductId = productMap.RemoteEntityId;
                _logger.LogInformation("ℹ️ Product '{name}' already mapped to {remote}", product.Name, remoteProductId);
            }

            // -------------------------
            // IMAGES
            // -------------------------
            //foreach (var pic in product.ProductPictures)
            //{
            //    var picMap = await _db.ApiSyncMaps
            //        .FirstOrDefaultAsync(x => x.EntityType == "Image" && x.LocalEntityId == pic.Id);

            //    if (picMap == null && !string.IsNullOrEmpty(remoteProductId))
            //    {
            //        pic.PicturePath = $"https://manageahioma.exwhyzee.ng{pic.PictureUrl}";
            //        try
            //        {
            //            string urloutput = await _productImageUploader.UploadImageFromUrlAsync(remoteProductId, pic.PicturePath);
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, "❌ Exception syncing image {pic} for product '{name}'", pic.Id, product.Name);
            //        }
            //    }
            //    else if (picMap != null)
            //    {
            //        _logger.LogInformation("ℹ️ Image {id} already mapped to {remote}", pic.Id, picMap.RemoteEntityId);
            //    }
            //}

            // -------------------------
            // VARIANTS
            // -------------------------
            foreach (var color in product.ProductColors)
            {
                var colorMap = await _db.ApiSyncMaps
                    .FirstOrDefaultAsync(x => x.EntityType == "Variant" && x.LocalEntityId == color.Id);

                if (colorMap == null)
                {
                    var variantPayload = new { productId = remoteProductId, type = "Color", value = color.ItemColor };
                    var variantResponse = await _apiClient.PostAsync<object, dynamic>("variants", variantPayload);

                    await SaveMap("Variant", color.Id, variantResponse.id);
                    _logger.LogInformation("====================================================✅ Color '{val}' synced. VariantId={id}",
                        color.ItemColor,
                        (string)variantResponse.id);
                }
                else
                {
                    _logger.LogInformation("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxℹ️ Color '{val}' already mapped to {remote}", color.ItemColor, colorMap.RemoteEntityId);
                }
            }

            foreach (var size in product.ProductSizes)
            {
                var sizeMap = await _db.ApiSyncMaps
                    .FirstOrDefaultAsync(x => x.EntityType == "Variant" && x.LocalEntityId == size.Id);

                if (sizeMap == null)
                {
                    var variantPayload = new { productId = remoteProductId, type = "Size", value = size.ItemSize };
                    var variantResponse = await _apiClient.PostAsync<object, dynamic>("variants", variantPayload);

                    await SaveMap("Variant", size.Id, variantResponse.id);
                    _logger.LogInformation("✅ Size '{val}' synced. VariantId={id}", size.ItemSize, (string)variantResponse.id);
                }
                else
                {
                    _logger.LogInformation("ℹ️ Size '{val}' already mapped to {remote}", size.ItemSize, sizeMap.RemoteEntityId);
                }
            }

            _logger.LogInformation("🎯 Product '{name}' sync completed.", product.Name);
        }

        private async Task SaveMap(string entityType, long localId, string remoteId)
        {
            _db.ApiSyncMaps.Add(new ApiSyncMap
            {
                EntityType = entityType,
                LocalEntityId = localId,
                RemoteEntityId = remoteId,
                Success = true,
                LastSyncedOnUtc = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }
        public static string NormalizeToSentenceCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var words = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }

            return string.Join(" ", words);
        }

        public static string RemoveImageTags(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return html;

            // Removes <img ...> and <img/> variations
            return Regex.Replace(html, @"<img\b[^>]*>", "", RegexOptions.IgnoreCase);
        }
        public static string GenerateSlug(string name)
        {
            return name?.ToLower()
                .Replace(" & ", "-")
                .Replace(" ", "-")
                .Replace("--", "-")
                .Replace("(", "-")
                .Replace(")", "-")
                .Trim('-');
        }
    }

    public class RemoteCategoryList
    {
        public bool success { get; set; }
        public RemoteCategoryData data { get; set; }
    }

    public class RemoteCategoryData
    {
        public List<RemoteCategory> categories { get; set; }
    }

    public class RemoteCategory
    {
        public string id { get; set; }
        public string name { get; set; }
    }

}



