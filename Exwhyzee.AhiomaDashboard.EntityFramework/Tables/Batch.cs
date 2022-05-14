using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class Batch
    {
      
        public long Id { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public long TenantAddressId { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DateCreated { get; set; }

        public EntityStatus Status { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}