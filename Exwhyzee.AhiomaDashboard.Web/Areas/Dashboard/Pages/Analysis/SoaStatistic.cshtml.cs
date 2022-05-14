using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations;
using Exwhyzee.AhiomaDashboard.EntityFramework.Dtos;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Dashboard.Pages.Analysis
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin")]

    public class SoaStatisticModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;
        public SoaStatisticModel(
                UserManager<IdentityUser> userManager,
                AhiomaDbContext context, IUserProfileRepository account
                )
        {
            _userManager = userManager;
            _context = context;
            _account = account;

        }

        public IList<SoaStatisticDto> SoaStatisticDto { get; set; }
        public IList<UserProfile> Profile { get; set; }
        public IQueryable<UserProfile> Profiles { get; set; }
        public int CountSoa { get; set; }
        public int CountStores { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public async Task OnGetAsync(DateTime? DateOne, DateTime? DateTwo)
        {
            Profiles = await _account.GetAsyncAllByRole("SOA");
            Profile = Profiles.Where(x => x.Status == Enums.AccountStatus.Active).ToList();

            if (DateOne != null)
            {
                DateOne = DateOne.Value.Date;
            }
            else
            {
                DateOne = DateTime.UtcNow.AddHours(1).Date.AddDays(-5);
            }
            if (DateTwo != null)
            {
                DateTwo = DateTwo.Value.Date;
            }
            else
            {
                DateTwo = DateTime.UtcNow.AddHours(1).Date;
            }
            StartDate = DateOne.Value;
            EndDate = DateTwo.Value;

            var SoaStat = Profile.Select(x => new SoaStatisticDto
            {
                SoaName = x.Fullname,
                SoaId = x.IdNumber,
                ShopCount = GetShopCountAsync(x.UserId, DateOne.Value, DateTwo.Value),
                ProductCount = GetProductCountAsync(x.UserId, DateOne.Value, DateTwo.Value),
                ReferralCount = GetSOAREFCountAsync(x.UserId, DateOne.Value, DateTwo.Value),
                UserId = x.UserId,
                DateStart = DateOne.Value,
                DateEnd = DateTwo.Value

            }).ToList();
            SoaStatisticDto = SoaStat.ToList();
        }

        public int GetShopCountAsync(string userid, DateTime fdate, DateTime sdate)
        {
            var shop = _context.Tenants.Where(x => x.CreationUserId == userid).Where(x => x.CreationTime.Date >= fdate).Where(x => x.CreationTime.Date <= sdate).Count();
            return shop;
        }
        public int GetProductCountAsync(string userid, DateTime fdate, DateTime sdate)
        {
            int numbeer = 0;
            //var shop = _context.Tenants.FirstOrDefault(x => x.CreationUserId == userid);
            //if (shop != null)
            //{
                var product = _context.Products.Include(x => x.Tenant).Where(x => x.Tenant.CreationUserId == userid).Where(x => x.CreatedOnUtc.Date >= fdate).Where(x => x.CreatedOnUtc.Date <= sdate).Count();
                numbeer = product;
            
            return numbeer;
        }
        public int GetSOAREFCountAsync(string userid, DateTime fdate, DateTime sdate)
        {

            var soauser = _context.UserProfiles.FirstOrDefault(x => x.UserId == userid);
            var users = _context.UserProfiles.Where(x => x.ReferralLink == soauser.IdNumber && x.Roles.Contains("SOA")).Where(x => x.DateRegistered.Date >= fdate).Where(x => x.DateRegistered.Date <= sdate).Count();
         return users;
        }

    }
}
