using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class OrderItem
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public string TrackCode { get; set; }

        public string ReferenceId { get; set; }
        public long OrderId { get; set; }
        public Order Order { get; set; }
        public DateTime DateOfOrder { get; set; }
        public OrderStatus Status { get; set; }
        public ShopStatus ShopStatus { get; set; }
        public LogisticStatus LogisticStatus { get; set; }
        public CustomerStatus CustomerStatus { get; set; }
        public string Note { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ItemSize { get; set; }
        public string Itemcolor { get; set; }
        public string LogisticType { get; set; }
        public string CustomerRef { get; set; }

        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }
    }
}
