using System.Collections.Generic;

namespace AhiomaMigrationWorker.Dtos
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public ApiData Data { get; set; }
    }

    public class ApiData
    {
        public List<ApiProduct> Products { get; set; }
    }

    public class ApiProduct
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Sku { get; set; }
        public string Barcode { get; set; }
        public string Price { get; set; }
        public string ComparePrice { get; set; }
        public string CostPrice { get; set; }
    }
}
