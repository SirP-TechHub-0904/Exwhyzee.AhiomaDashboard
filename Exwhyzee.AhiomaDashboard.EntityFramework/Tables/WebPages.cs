using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class WebPages
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string UniqueTitle { get; set; }
        
        public bool Publish { get; set; }
        public string Content { get; set; }
        public DateTime DateUtc { get; set; }
        public PageType PageType { get; set; }
    }
}
