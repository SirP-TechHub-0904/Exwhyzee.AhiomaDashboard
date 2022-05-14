using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class Banner
    {
        public long Id { get; set; }
        public string UrlPath { get; set; }
        public string UrlLink { get; set; }
        public string AltText { get; set; }
        public string BarnerLink { get; set; }
        public int SortOrder { get; set; }
        public bool Disable { get; set; }
        public bool Active { get; set; }
        public DateTime DateUtc { get; set; }
        public BannerType BannerType { get; set; }
    }
}
