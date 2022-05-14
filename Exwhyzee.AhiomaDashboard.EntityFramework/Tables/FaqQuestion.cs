using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class FaqQuestion
    {
        public long Id { get; set; }
        public string Title { get; set; }
       public FaqType FaqType { get; set; }
        public bool Publish { get; set; }
        public string Answer { get; set; }
        public DateTime DateUtc { get; set; }
    }
}
