using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class PromoProduct
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public long PromoCategoryId { get; set; }
        public PromoCategory PromoCategory { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
