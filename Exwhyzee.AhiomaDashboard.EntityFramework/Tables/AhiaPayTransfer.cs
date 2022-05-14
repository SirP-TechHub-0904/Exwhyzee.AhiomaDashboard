using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class AhiaPayTransfer
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Sender { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public DateTime DateOfTransfer { get; set; }

        public TransferEnum Status { get; set; }

        public string TransferReference { get; set; }

        public string Description { get; set; }
        public string Log { get; set; }
    }
}
