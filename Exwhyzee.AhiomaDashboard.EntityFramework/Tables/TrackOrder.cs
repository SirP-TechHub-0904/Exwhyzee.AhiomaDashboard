using System;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class TrackOrder
    {
        public long Id { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public long OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; }

       // public long? OrderIdx { get; set; }
    }
}
