using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Dtos
{
    public class AdvertUserProfileDto
    {
        public long Id { get; set; }

        public string IdNumber { get; set; }

        public string UserId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Decimal Amount { get; set; }
    }

}
