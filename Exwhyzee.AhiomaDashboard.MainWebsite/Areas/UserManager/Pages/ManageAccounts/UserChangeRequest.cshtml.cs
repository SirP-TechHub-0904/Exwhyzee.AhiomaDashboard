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

using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.UserManager.Pages.Users
{
    [Authorize(Roles = "UserManager,mSuperAdmin")]
    public class UserChangeRequestModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public UserChangeRequestModel(
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

        public IList<RequestPhoneEmailChange> Data { get; set; }

        public async Task OnGetAsync()
        {
            Data = await _context.RequestPhoneEmailChanges.Include(x => x.Profile).OrderByDescending(x => x.Date).ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateUserStatus(string uid, int statusType)
        {
            return RedirectToPage();
        }

    }
}
