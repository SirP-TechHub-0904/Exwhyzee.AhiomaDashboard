using Exwhyzee.Core.Mvc.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Controllers.Dtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }

        public string Surname { get; set; }

        public string FirstName { get; set; }

        public string OtherNames { get; set; }

        public DateTime DateRegistered { get; set; }
        public string Role { get; set; }

        [Display(Name = "Next of Kin")]
        public string NextOfKin { get; set; }
        [Display(Name = "Next of Kin Phone Number")]
        public string NextOfKinPhoneNumber { get; set; }

        [Display(Name = "Referee Name")]
        public string RefereeName { get; set; }

        [Display(Name = "Referee Phone")]
        public string RefereePhone { get; set; }

        [Display(Name = "Contact Address")]
        public string ContactAddress { get; set; }
        [Display(Name = "Alt Phone Number")]
        public string AltPhoneNumber { get; set; }


        public string State { get; set; }
        public string LocalGovernment { get; set; }
    }
}
