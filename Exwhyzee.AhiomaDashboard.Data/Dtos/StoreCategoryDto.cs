using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.Data.Dtos
{
    public class StoreCategoryDto
    {
        public long Id { get; set; }
        public long? CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
