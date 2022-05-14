using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class TransferQueue
    {
        public long Id { get; set; }
        public long TransactionId { get; set; }
        public string uid { get; set; }
        public string account_bank { get; set; }
        public string account_number { get; set; }
        public string fullname { get; set; }
        public DateTime Date { get; set; }
        public string IDNUmber { get; set; }
        public int amount { get; set; }
        public string narration { get; set; }
        public string currency { get; set; }
        public string reference { get; set; }
        public string callback_url { get; set; }
        public string debit_currency { get; set; }
        public string response { get; set; }
        public QueueStatus QueueStatus { get; set; }
        
    }
}
