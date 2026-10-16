using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhiomaMigrationWorker.Dtos
{
    internal class BaseDto
    {
    }
    public class ProductResponseDto
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public ProductDataDto Data { get; set; }
    }

    public class ProductDataDto
    {
        [JsonProperty("product")]
        public XProductDto Product { get; set; }
    }

    public class XProductDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("images")]
        public ProductImagesDto Images { get; set; }
    }

    public class ProductImagesDto
    {
        [JsonProperty("push")]  // 👈 ensures mapping works
        public List<string> Push { get; set; }
    }



}
