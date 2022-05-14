using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class TenantAddress
    { 
        public long Id { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string State { get; set; }
        public string LocalGovernment { get; set; }
        public long? MarketId { get; set; }
        public string Longitude { get; set; }
        public string Latitiude { get; set; }
        public string Zipcode { get; set; }
       
        public long TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}
