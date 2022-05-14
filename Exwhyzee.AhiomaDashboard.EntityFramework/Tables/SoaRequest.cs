using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
   public class SoaRequest
    {
        public long Id { get; set; }
        [Required]

        public string FullName { get; set; }

        [Required]

        public string PhoneNumber { get; set; }

        [Required]
        public string ShopAddress { get; set; }
        [Required]

        public string BusinessName { get; set; }
        [Required]

        public string Email { get; set; }
        public DateTime Date { get; set; }

        public long? UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public string IdNumber { get; set; }
        public bool ProcessedByUser { get; set; }
        public DateTime DateProcessedByUser { get; set; }
        public DateTime DateSentToSoa { get; set; }
        public string SentToSoaBy { get; set; }

        public SoaRequestStatus SoaRequestStatus { get; set; }
    }
}
