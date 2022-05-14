using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Dtos
{
    public class SoaStatisticDto
    {
        public string SoaName { get; set; }
        public string UserId { get; set; }
        public string SoaId { get; set; }
        public int ShopCount { get; set; }
        public int ProductCount { get; set; }
        public int ReferralCount { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

    }
}
