using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class UserReferee
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Display(Name = "Contact Address")]
        public string ContactAddress { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Created Date")]
        public DateTime CreatedDateUtc { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime UpdateDateUtc { get; set; }
    }
}
