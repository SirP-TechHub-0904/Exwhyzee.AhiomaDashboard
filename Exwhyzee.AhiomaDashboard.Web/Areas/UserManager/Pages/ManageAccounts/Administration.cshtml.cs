using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.Web.Services;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.Users
{
    [Authorize(Roles = "UserManager,mSuperAdmin")]
    public class AdministrationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public AdministrationModel(
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

        public IList<UserProfile> Profile { get; set; }
        public IList<UserProfile> ProfileList { get; set; }

        public async Task OnGetAsync()
        {
         

           var ProfileSort = await _context.UserProfiles.Include(x => x.User).Where(x => x.Roles.Contains("Store") || x.Roles.Contains("Customer") || x.Roles.Contains("SOA")).OrderByDescending(x => x.DateRegistered).Select(x=>x.Id).ToListAsync();
            ProfileList = await _context.UserProfiles.Include(x => x.User).OrderByDescending(x => x.DateRegistered).ToListAsync();
            Profile = ProfileList.Where(x => !ProfileSort.Contains(x.Id)).ToList();
           

        }

        public async Task<IActionResult> OnPostUpdateUserStatus(string uid, int statusType)
        {
            return RedirectToPage();
        }

    }
}
