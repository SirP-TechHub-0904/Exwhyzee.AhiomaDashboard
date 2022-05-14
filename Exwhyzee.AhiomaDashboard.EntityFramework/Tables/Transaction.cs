using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class Transaction
    {
        public Transaction()
        {
            DateOfTransaction = DateTime.UtcNow.AddHours(1);
        }
        public long Id { get; set; }
        public long WalletId { get; set; }

        public string UserId { get; set; }
        public long? UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }

        public string Note { get; set; }
        public string Sender { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public DateTime DateOfTransaction { get; set; }

        public TransactionTypeEnum TransactionType { get; set; }
        public TransactionShowEnum TransactionShowEnum { get; set; }
        public TransactionSection TransactionSection { get; set; }
        public EntityStatus Status { get; set; }
        public PayoutStatus PayoutStatus { get; set; }
        public string TransactionReference { get; set; }
        public string Description { get; set; }
        public string TrackCode { get; set; }
        public long? OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; }
        public string Color { get; set; }
        public string From { get; set; }
    }
}
