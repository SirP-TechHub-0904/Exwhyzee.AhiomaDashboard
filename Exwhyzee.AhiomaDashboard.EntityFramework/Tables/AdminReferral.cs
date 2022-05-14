using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class AdminReferral
    {
        public long Id { get; set; }
        public string MainReferalId { get; set; }
        public string SubReferalId { get; set; }
        public DateTime Date { get; set; }
    }
}
