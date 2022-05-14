using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class Wallet
    {
        public long Id { get; set; }
        public string UserId { get; set; }
         [Column(TypeName = "decimal(18, 2)")]
        public decimal Balance { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal WithdrawBalance { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }

    public class WalletHistory
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long WalletId { get; set; }
        public long UserProfileId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public UserProfile Profile { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal LedgerBalance { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AvailableBalance { get; set; }
        public DateTime CreationTime { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Reason { get; set; }
        public string TransactionType { get; set; }
        public string From { get; set; }
    }
}
