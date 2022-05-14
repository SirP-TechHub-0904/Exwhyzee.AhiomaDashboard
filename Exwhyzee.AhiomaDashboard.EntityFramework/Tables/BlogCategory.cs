using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class BlogCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public bool Publish { get; set; }
        public DateTime Date { get; set; }
        public ICollection<Blog> Blogs { get; set; }
    }
}
