using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class Purchase
    {
        public Purchase()
        {
            ManufacturedDate = DateTime.UtcNow;
            ExpiryDate = DateTime.UtcNow;
            UnitCostPrice = 0;
        }

        public long Id { get; set; }
         [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitCostPrice { get; set; }

        public string Note { get; set; }
         [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitSellingPrice { get; set; }

        public int Quantity { get; set; }

        public long ProductId { get; set; }
        public Product Product { get; set; }

               public DateTime DateEntered { get; set; }

        public DateTime ManufacturedDate { get; set; }

        public DateTime ExpiryDate { get; set; }

       
        public decimal TotalCost
        {
            get
            {
                return Math.Round(UnitCostPrice * Quantity);
            }
        }

        public int ExpiryDayCount
        {
            get
            {
                int? days = (ExpiryDate - DateTime.Now).Days;
                return days.Value;
            }
        }

        public bool Expired
        {
            get
            {
                if (ExpiryDayCount <= 0)
                {
                    return true;
                }
                return false;
            }
        }

        

    }
}
