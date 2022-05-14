using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class Setting
    {
        public long Id { get; set; }
        public int PageSize { get; set; }
        public bool ActivateFreeDelivery { get; set; }
        public bool EnableSMS { get; set; }
        //public string ModalMessage { get; set; }
        //public bool ShowModal { get; set; }
    }
}
