using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    /// <summary>
    /// 
    /// </summary>
    public class ProductCart
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public long? UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public CartStatus CartStatus { get; set; }
        public string TrackOrderId { get; set; }
        public string TrackCartId { get; set; }
        public string CartTempId { get; set; }
        public int Quantity { get; set; }
        public string ItemSize { get; set; }
        public string Itemcolor { get; set; }
        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime Date { get; set; }
    }
}
