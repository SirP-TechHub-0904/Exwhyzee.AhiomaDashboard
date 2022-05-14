using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Wallets
{
    public class AhiapayDto
    {
        public long Id { get; set; }

        public string Sender { get; set; }
        public long Senderwalletid { get; set; }

        public string ReceiverId { get; set; }
        public long Receiverwalletid { get; set; }

        public decimal Amount { get; set; }
        public string TransactionCode { get; set; }
        public string From { get; set; }
        public DateTime DateOfTransaction { get; set; }

        public TransactionTypeEnum TransactionStatus { get; set; }



        public string Note { get; set; }
        public string ReceiverPhone { get; set; }
    }


    public class AhiapayAdminDto
    {
        public long Id { get; set; }

        public string AhiaPayType { get; set; }

        public string ReceiverId { get; set; }
        public long Receiverwalletid { get; set; }

        public decimal Amount { get; set; }
        public string TransactionCode { get; set; }
        public DateTime DateOfTransaction { get; set; }

        public TransactionTypeEnum TransactionStatus { get; set; }



        public string Note { get; set; }
        public string ReceiverPhone { get; set; }
    }

}
