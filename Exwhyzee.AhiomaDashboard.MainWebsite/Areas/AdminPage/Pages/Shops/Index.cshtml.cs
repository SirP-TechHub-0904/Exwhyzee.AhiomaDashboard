using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.AdminPage.Pages.Shops
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


        public IList<Tenant> Profile { get; set; }

        public async Task OnGetAsync()
        {
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
            IQueryable<AdminShopReferral> getids = from s in _context.AdminShopReferrals
                                                   .Where(x => x.MainReferalId == subadminid)
                                                   select s;
            var idlist = getids.Select(x => x.SubReferalId);

            IQueryable<Tenant> Shops = from s in _context.Tenants
                                       .Include(x=>x.User)
                                       .Include(x=>x.UserProfile)
                                                  .Where(x=> idlist.Contains(x.UserId)).OrderByDescending(x => x.CreationTime)
                                                   select s;


            Profile = Shops.ToList();
        }
        public async Task<IActionResult> OnPostUpdateUserStatus(string uid, int statusType)
        {
            return RedirectToPage();
        }

    }
}
