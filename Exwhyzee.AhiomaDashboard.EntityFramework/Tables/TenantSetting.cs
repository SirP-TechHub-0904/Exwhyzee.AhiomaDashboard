using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class TenantSetting
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public string SiteType { get; set; }
    }
}
