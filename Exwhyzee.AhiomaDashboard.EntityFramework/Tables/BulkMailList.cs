using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class BulkMailList
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }

        public string Title { get; set; }
        public string Subject { get; set; }
        public string Response { get; set; }

        public bool Sent { get; set; }
    }
}
