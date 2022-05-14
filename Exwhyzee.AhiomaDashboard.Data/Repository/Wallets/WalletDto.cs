using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Wallets
{
   public class WalletDto
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Fullname { get; set; }
        public string IdNumber { get; set; }
        public decimal WithdrawBalance { get; set; }
        public decimal LedgerBalance { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
