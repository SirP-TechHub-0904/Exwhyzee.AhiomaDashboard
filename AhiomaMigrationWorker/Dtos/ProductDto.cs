using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhiomaMigrationWorker.Dtos
{
    public class ProductDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Sku { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public decimal ComparePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Weight { get; set; }
        public Dimensions Dimensions { get; set; }
        public Images Images { get; set; }
        public string Status { get; set; }
        public bool Featured { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string CategoryId { get; set; }
    }
    public class Dimensions
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
    }
    public class Images
    {
        public List<string> Push { get; set; }
    }

}
