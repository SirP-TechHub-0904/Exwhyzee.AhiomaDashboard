using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class LocalGoverment
    {
        public long Id { get; set; }
        public string LGAName { get; set; }

        public long StatesId { get; set; }
        public State States { get; set; }
    }
}
