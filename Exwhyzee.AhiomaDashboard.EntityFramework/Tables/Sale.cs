using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class Sale
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public long PurchaseId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerUnit { get; set; }

        public int Quantity { get; set; }
         [Column(TypeName = "decimal(18, 2)")]
        public decimal Price
        {
            get
            {
                return Quantity * PricePerUnit;
            }
        }


        public Order Order { get; set; }

        public Purchase Purchase { get; set; }
    }
}