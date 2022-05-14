﻿using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Transactions
{
   public class TransactionListDto
    {
       
        public long Id { get; set; }
        public long WalletId { get; set; }

        public string UserId { get; set; }
        public string Fullname { get; set; }
        public string IdNumber { get; set; }

        public string Note { get; set; }
        public string Sender { get; set; }
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
    }
}
