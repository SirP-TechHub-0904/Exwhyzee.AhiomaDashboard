using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class Messaging
    {
        public long Id { get; set; }
        public MessageStatus MassageType { get; set; }
        public DateTime Date { get; set; }
        public string Contacts { get; set; }
        public string Message { get; set; }
      
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string Count { get; set; }
        public ICollection<MailContent> MailContents { get; set; }
        public ICollection<MailProduct> MailProducts { get; set; }
    }
}
