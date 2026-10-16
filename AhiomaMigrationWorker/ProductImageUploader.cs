using AhiomaMigrationWorker.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AhiomaMigrationWorker
{
    
    public class ProductImageUploader
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.ahioma.com/api/api/warehouse/products";
        private readonly ILogger<ProductImageUploader> _logger;
        private readonly ApiSettings _settings;

        public ProductImageUploader(HttpClient httpClient, IConfiguration config, ILogger<ProductImageUploader> logger)
        {
            _httpClient = httpClient;
            var bearerToken = config["ApiSettings:BearerToken"]; // appsettings.json
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);
            _logger = logger;
             
        }

        public async Task<string> UploadImageFromUrlAsync(string productId, string imageUrl)
        {
            // Step 1: Download the image from the URL
            byte[] imageBytes;
            using (var webClient = new HttpClient())
            {
                imageBytes = await webClient.GetByteArrayAsync(imageUrl);
            }

            // Step 2: Create Multipart content
            using (var content = new MultipartFormDataContent())
            {
                // Use filename from URL or fallback
                var fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);
                if (string.IsNullOrWhiteSpace(fileName))
                    fileName = "upload.png";

                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png"); // adjust if needed

                // Add the file with field name "images"
                content.Add(imageContent, "images", fileName);

                // Step 3: Send POST request
                var url = $"{_baseUrl}/{productId}/images";
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Upload failed: {response.StatusCode}, {error}");
                }
                var json = await response.Content.ReadAsStringAsync();

                //string payloadJson = JsonConvert.SerializeObject(response);
                var dto = JsonConvert.DeserializeObject<ProductResponseDto>(json);
                 
                var imageUrls = dto?.Data?.Product?.Images?.Push ?? new List<string>();
                
                _logger.LogInformation("Images found: {count}", imageUrls.Count);
                if (imageUrls.Any())
                {
                    _logger.LogInformation("First image: {url}", imageUrls[0]);
                }
                return imageUrls.FirstOrDefault();
            }
        }
    }
}
