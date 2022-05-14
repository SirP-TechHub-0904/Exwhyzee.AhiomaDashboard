using Exwhyzee.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class Market
    {
        public long Id { get; set; }
        public string Name { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
       
        [Display(Name = "Short Name")]

        public string ShortName { get; set; }
        public DateTime Date { get; set; }
        public string State { get; set; }
        public string LocalGovernment { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public int SortOrder { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public MarketType MarketType { get; set; }
    }
}
