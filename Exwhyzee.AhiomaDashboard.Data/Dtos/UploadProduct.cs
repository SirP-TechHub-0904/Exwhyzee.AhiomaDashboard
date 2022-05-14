using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.Data.Dtos
{
    public class UploadProduct
    {


        public long Id { get; set; }


        public string Name { get; set; }


        public EntityStatus Status { get; set; }


        public long CategoryId { get; set; }

        public long? SubCategoryId { get; set; }
        public long TenantId { get; set; }
        public string ShortDescription { get; set; }

        public string FullDescription { get; set; }


        public string MetaKeywords { get; set; }
        public decimal Price { get; set; }
        public bool Published { get; set; }

        public int Quantity { get; set; }
        public bool UseColor { get; set; }

        public string[] ProductColors { get; set; }
            public bool UseSize { get; set; }
          
            public string[] ProductSizes { get; set; }
        public string UploadedByUserId { get; set; }
           

        }
    }
