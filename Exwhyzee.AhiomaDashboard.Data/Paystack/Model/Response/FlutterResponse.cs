﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.Data.Paystack.Model.Response
{
    
    public class FlutterResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string link { get; set; }
    }
}
