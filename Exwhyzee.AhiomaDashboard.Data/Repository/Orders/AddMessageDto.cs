using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Orders
{
    public class AddMessageDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Recipient { get; set; }
        public bool IsAdmin { get; set; }
        public NotificationStatus NotificationStatus { get; set; }
        public int Retries { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
