﻿using System;
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

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.Users
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,

            IUserProfileRepository account, AhiomaDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public IList<UserProfile> Profile { get; set; }

        public async Task OnGetAsync()
        {
            Profile = await _account.GetAsyncAllActive();
        }

        public async Task<IActionResult> OnPostUpdateUserStatus(string uid, int statusType)
        {
            return RedirectToPage();
        }

    }
}
