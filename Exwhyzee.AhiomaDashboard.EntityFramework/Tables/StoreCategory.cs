using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class StoreCategory
    {
        public long Id { get; set; }
        public long? TenantId { get; set; }
        public long? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
