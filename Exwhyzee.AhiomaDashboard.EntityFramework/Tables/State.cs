using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class State
    {
        public long Id { get; set; }


        public string StateName { get; set; }

        public virtual ICollection<LocalGoverment> LocalGov { get; set; }


    }
}
