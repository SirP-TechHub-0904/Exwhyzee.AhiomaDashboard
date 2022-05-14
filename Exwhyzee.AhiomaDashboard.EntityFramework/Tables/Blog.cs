using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class Blog
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public bool Publish { get; set; }
        public DateTime Date { get; set; }
        public string ImageUrl { get; set; }
        public long BlogCategoryId { get; set; }
        public BlogCategory Category { get; set; }

    }
}
