using Exwhyzee.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class UserProfile
    {
        public long Id { get; set; }

        public string IdNumber { get; set; }
        
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public string Surname { get; set; }

        public string FirstName { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }

        public DateTime? DOB { get; set; }
        public AccountStatus Status { get; set; }

        public string OtherNames { get; set; }
        public string Roles { get; set; }
        [Display(Name = "Next of Kin")]
        public string NextOfKin { get; set; }
        [Display(Name = "Next of Kin Phone Number")]
        public string NextOfKinPhoneNumber { get; set; }
        public bool FirstTimeLogin { get; set; }

        public ICollection<UserReferee> UserReferees { get; set; }
        public ICollection<UserAddress> UserAddresses { get; set; }
        public ICollection<UserProfileSocialMedia> UserProfileSocialMedias { get; set; }

        [Display(Name = "Profile Image")]
        public string ProfileUrl { get; set; }
        [Display(Name = "Id Card Front")]
        public string IDCardFront { get; set; }
        [Display(Name = "Id Card Back")]
        public string IDCardBack { get; set; }
        public string Note { get; set; }
        public DateTime DateRegistered { get; set; }
        public string Logs { get; set; }
        public string ReferralLink { get; set; }
        public bool ReferralApproval { get; set; }
        public string Fullname
        {
            get
            {
                return Surname + " " + FirstName + " " + OtherNames;
            }
        }

        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string VerificationCode { get; set; }
        public bool BankAccountApproved { get; set; }
        public bool DisableDeposit { get; set; }
        public bool DisableAhiaPay { get; set; }
        public bool DisableAhiaPayTransfer { get; set; }
        public bool DisableBankTransfer { get; set; }
        public bool DisableBuy { get; set; }
        public bool DisableAdsCrediting { get; set; }
        public bool ActivateForAdvert { get; set; }
        public bool DisableUtility { get; set; }

        
        public DateTime AddForAdvert { get; set; }
        public string TokenAccount { get; set; }
        public DateTime LastUserUpdated { get; set; }
        public DateTime LastAdminUpdated { get; set; }
        public string ApprovedBy { get; set; }
        public string BeneficaryToken { get; set; }
    }
}
