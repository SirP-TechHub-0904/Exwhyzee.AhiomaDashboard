using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.Data.Flutter.Model
{
    public class BankVerify
    {
        public string status { get; set; }
        public string message { get; set; }
        public ICollection<Data> data { get; set; }
    }

public class Data
    {
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
      
    }

   

}
