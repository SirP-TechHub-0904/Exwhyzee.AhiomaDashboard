using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.Data.Dtos
{
    [Serializable]
    public class AddToCartDto
    {
        public string itemcolor { get; set; }
            public string itemsize { get; set; }
        public string quantity { get; set; }
        public string product_id { get; set; }
    }
}
