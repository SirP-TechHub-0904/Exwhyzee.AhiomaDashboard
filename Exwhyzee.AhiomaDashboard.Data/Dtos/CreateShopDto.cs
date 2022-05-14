using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.Data.Dtos
{
    public class CreateShopDto
    {

        public long Id { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string TenentHandle { get; set; }
        public string BusinessName { get; set; }
        public string BusinessDescription { get; set; }
        public DateTime CreationTime { get; set; }
        public string CreationUserId { get; set; }
        public long MarketId { get; set; }
        public string State { get; set; }
        public string LocalGovernment { get; set; }
        public string ContactAddress { get; set; }
        public string AltPhoneNumber { get; set; }
        public string[] CategoryListId { get; set; }


    }
}
