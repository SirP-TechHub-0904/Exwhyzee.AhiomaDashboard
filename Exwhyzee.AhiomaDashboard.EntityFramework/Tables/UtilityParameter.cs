using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class UtilityParameter
    {
        public long Id { get; set; }
        public UtilityParam ParamType { get; set; }
        public string Bundle { get; set; }
        public string Code { get; set; }
        public string Price { get; set; }
        public bool Show { get; set; }

    }
}
