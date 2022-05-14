using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class ProductColor
    {
        public long Id { get; set; }
        public string ItemColor { get; set; }
        public long ProductId { get; set; }
    }
}
