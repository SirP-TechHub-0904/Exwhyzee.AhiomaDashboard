using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class MailProduct
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public Product Product { get; set; }

        public long MessagingId { get; set; }
        public Messaging Messaging { get; set; }

    }
}
