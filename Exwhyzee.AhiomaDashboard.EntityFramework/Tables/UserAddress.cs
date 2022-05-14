using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class UserAddress
    { 
        public long Id { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string LocalGovernment { get; set; }
        public string Longitude { get; set; }
        public string Latitiude { get; set; }
        public string Zipcode { get; set; }
        public string UserId { get; set; }
        public long? UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public bool Default { get; set; }
    }
}
