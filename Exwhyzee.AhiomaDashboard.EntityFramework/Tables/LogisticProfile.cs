using Exwhyzee.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class LogisticProfile
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public long? UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public DateTime CreationTime { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string CompanyDocument { get; set; }
        public LogisticEnum LogisticStatus { get; set; }
        public string Referee { get; set; }
        public string CustomerCareNumber { get; set; }
        public ICollection<LogisticVehicle> LogisticVehicles { get; set; }
    }
}
