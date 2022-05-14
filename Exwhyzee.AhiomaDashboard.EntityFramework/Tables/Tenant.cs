using Exwhyzee.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class Tenant
    {
        public long Id { get; set; }
        public string TenentHandle {get;set;}
        public string BusinessName {get;set; }
        public string BusinessDescription { get;set; }
        public string LogoUri { get;set; }
        public string BannerUri { get;set; }
        public bool DoYouHaveOtherBranches { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public long? UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreationUserId { get; set; }
        public long MarketId { get; set; }
        public Market Market { get; set; }
        public int Commission { get; set; }

        public DeliveryEnum DeliveryType { get; set; }
        public TenantEnum TenantStatus { get; set; }
        public PaymentEnum PaymentType { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<TenantAddress> TenantAddresses { get; set; }
        public ICollection<TenantSocialMedia> TenantSocialMedias { get; set; }
    }
}
