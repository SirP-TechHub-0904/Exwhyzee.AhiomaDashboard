using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class RequestPhoneEmailChange
    {
        public long Id { get; set; }
        public string OldMail { get; set; }
        public string NewMail { get; set; }
        public string OldPhone { get; set; }
        public string NewPhone { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
        public string SecurityAnswer { get; set; }
        public string UserId { get; set; }
        public long UserProfileId { get; set; }
        public UserProfile Profile { get; set; }

        public ChangeDataStatus PhoneStatus { get; set; }
        public ChangeDataStatus EmailStatus { get; set; }
    }
}
