using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class ProductCheck
    {
        public long Id { get; set; }
        public string UserCode { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }
        public string Note { get; set; }
        public long UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }

        public long TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}
