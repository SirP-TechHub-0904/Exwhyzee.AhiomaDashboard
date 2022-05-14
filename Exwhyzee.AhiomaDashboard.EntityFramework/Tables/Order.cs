using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class Order
    {
        public long Id { get; set; }
        public string OrderId { get; set; }
        public string ReferenceId { get; set; }
        public string TrackOrderId { get; set; }
        public string TrackCode { get; set; }
        public long? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
        public long? UserProfileId { get; set; }
        public long? UserAddressId { get; set; }
        public UserAddress UserAddress { get; set; }
        public UserProfile UserProfile { get; set; }

        public DateTime DateOfOrder { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OrderAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountPaid { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AdditionalPayment { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OrderCostAfterProcessing { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Vat { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? LogisticAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountMovedToCustomer { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountMovedToAdmin { get; set; }

        public long? LogisticVehicleId { get; set; }
        public LogisticVehicle LogisticVehicle { get; set; }


        public string InvoiceNumber
        {
            get; set;
        }

        public OrderStatus Status { get; set; }

        public DateTime? DeliveredDate { get; set; }
        public DateTime? DeliveryRangeStart { get; set; }
        public DateTime? DeliveryRangeEnd { get; set; }
        public string CustomerRef {get;set;}
        public string Note { get; set; }

        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}