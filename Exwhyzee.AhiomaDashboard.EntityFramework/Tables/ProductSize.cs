using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class ProductSize
    {
        public long Id { get; set; }
        public string ItemSize { get; set; }
        public long ProductId { get; set; }
    }
}
