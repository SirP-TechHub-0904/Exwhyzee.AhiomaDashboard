using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class ProductUploadShop
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public string UserId { get; set; }
        public long UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public DateTime Date { get; set; }
    }
}
