using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class PromoCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Banner { get; set; }
        public bool Show { get; set; }
        public DateTime Date { get; set; }
        public ICollection<PromoProduct> PromoProducts { get; set; }
    }
}
