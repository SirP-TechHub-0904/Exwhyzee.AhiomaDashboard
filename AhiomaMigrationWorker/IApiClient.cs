using AhiomaMigrationWorker.Dtos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AhiomaMigrationWorker
{
    public interface IApiClient
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
    }

    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _settings;

        public ApiClient(HttpClient httpClient, IOptions<ApiSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;

            _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.BearerToken);
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var fullUrl = new Uri(_httpClient.BaseAddress, endpoint).ToString();
            Console.WriteLine($"➡️ Requesting: {fullUrl}");
            Console.WriteLine($"➡️ Auth Header: {_httpClient.DefaultRequestHeaders.Authorization}");

            var response = await _httpClient.GetAsync(endpoint);

            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"⬅️ Status: {response.StatusCode}");
            Console.WriteLine($"⬅️ Response body: {body}");

            response.EnsureSuccessStatusCode();  // will throw if not 2xx

            return JsonConvert.DeserializeObject<T>(body);
        }


        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            // Serialize request payload
            var payload = JsonConvert.SerializeObject(data, Formatting.Indented);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            // Build full URL (HttpClient.BaseAddress is set, so endpoint may be relative)
            var fullUrl = _httpClient.BaseAddress != null
                ? new Uri(_httpClient.BaseAddress, endpoint).ToString()
                : endpoint;

            // ---- Debug Info ----
             
            //Console.WriteLine("Headers:");
            //foreach (var header in _httpClient.DefaultRequestHeaders)
            //{
            //    Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            //}
             
            // Send request
            var response = await _httpClient.PostAsync(endpoint, content);

            // Read response
            var json = await response.Content.ReadAsStringAsync();

             
            Console.WriteLine("==========================");

            //response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<TResponse>(json);
        }

    }

}
