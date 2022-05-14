using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class TenantSocialMedia
    { 
        public long Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public long TenantId { get; set; }
        public Tenant Tenant { get; set; }
      
    }
}
