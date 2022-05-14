using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class MailContent
    {
        public long Id { get; set; }
        public string OldString { get; set; }
        public string NewString { get; set; }

        public long MessagingId { get; set; }
        public Messaging Messaging { get; set; }
    }
}
