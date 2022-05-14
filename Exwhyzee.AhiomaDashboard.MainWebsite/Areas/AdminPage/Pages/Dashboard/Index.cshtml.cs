using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Flutter.Balance;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.AdminPage.Pages.Dashboard
{
    [Authorize(Roles = "mSuperAdmin,SubAdmin,Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,

            IUserProfileRepository account, AhiomaDbContext context,
            IEmailSendService emailSender)
        {
            _userManager = userManager;
            _context = context;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public int iShop { get; set; }
        public int iShopAll { get; set; }
        public int iProduct { get; set; }
        public int iProductAll { get; set; }
        public int iSoa { get; set; }
        public int iSoaAll { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IList<UserProfile> Profile { get; set; }

        public async Task OnGetAsync(DateTime? DateOne, DateTime? DateTwo)
        {
            if(DateOne == null)
            {
                DateOne = DateTime.UtcNow.AddHours(1).Date;
            }
            if (DateTwo == null)
            {
                DateTwo = DateTime.UtcNow.AddHours(1).Date;
            }

            StartDate = DateOne;
            EndDate = DateTwo;
            //walatapeter@gmail.com
            string uids = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(uids);
            var adminrole = await _userManager.IsInRoleAsync(user, "mSuperAdmin");
            var admini = await _userManager.IsInRoleAsync(user, "Admin");
            string subadminid = "";
            if (adminrole.Equals(true))
            {
                var userad = await _userManager.FindByEmailAsync("MRTOBIEMMANUEL@gmail.com");
                subadminid = userad.Id;
            }
            else if (admini.Equals(true))
            {
                var userad = await _userManager.FindByEmailAsync("MRTOBIEMMANUEL@gmail.com");
                subadminid = userad.Id;
            }
            else
            {
                subadminid = user.Id;
            }
            IQueryable<AdminReferral> getids = from s in _context.AdminReferrals
                                                   .Where(x => x.MainReferalId == subadminid)
                                               select s;
            var idlist = getids.Select(x => x.SubReferalId);

            IQueryable<UserProfile> SOA = from s in _context.UserProfiles
                                       .Include(x => x.User)
                                       .Include(x => x.UserAddresses)
                                                  .Where(x => idlist.Contains(x.UserId))
                                            select s;

            IQueryable<AdminShopReferral> getshopids = from s in _context.AdminShopReferrals
                                                   .Where(x => x.MainReferalId == subadminid)
                                               select s;


            var shopidlist = getshopids.Select(x => x.SubReferalId);
            var shopidlistByTenantId = getshopids.Select(x => x.SubReferalId);
            //
           
            //
            IQueryable<Tenant> Shops = from s in _context.Tenants
                                      
                                                  .Where(x => shopidlist.Contains(x.UserId))
                                            select s;
            var shopids = Shops.Select(x => x.Id);
            IQueryable<Product> product = from s in _context.Products

                                                 .Where(x => shopids.Contains(x.TenantId))
                                          select s;

            iProduct = product.Where(x => x.CreatedOnUtc.Date >= DateOne).Where(x => x.CreatedOnUtc.Date <= DateTwo).Count();
            iProductAll = product.Count();

            iShop = Shops.Where(x => x.CreationTime.Date >= DateOne).Where(x => x.CreationTime.Date <= DateTwo).Count();
            iShopAll = Shops.Count();


            iSoa = SOA.Where(x => x.DateRegistered.Date >= DateOne).Where(x => x.DateRegistered.Date <= DateTwo).Count();
            iSoaAll = SOA.Count();


        }
        public async Task<IActionResult> OnPostUpdateUserStatus(string uid, int statusType)
        {
            return RedirectToPage();
        }

    }
}
