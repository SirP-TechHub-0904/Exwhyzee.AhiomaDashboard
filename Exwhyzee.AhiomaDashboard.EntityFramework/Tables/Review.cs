using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class Review
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
        public long UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
