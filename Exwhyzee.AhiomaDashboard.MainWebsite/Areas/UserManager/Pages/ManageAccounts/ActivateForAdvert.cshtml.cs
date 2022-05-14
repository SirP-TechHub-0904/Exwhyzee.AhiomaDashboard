using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.UserManager.Pages.ManageAccounts
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare,SubAdmin")]

    public class ActivateForAdvertModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;

        public ActivateForAdvertModel(
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
        
        public async Task<IActionResult> OnGetAsync(long id)
        {
            try
            {
                var iProfile = await _context.UserProfiles.FindAsync(id);
                if (iProfile.ActivateForAdvert == true)
                {
                    iProfile.ActivateForAdvert = false;
                }
                else
                {
                    iProfile.ActivateForAdvert = true;
                }
                _context.Entry(iProfile).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                TempData["success"] = "Added to Advert";
            }
            catch(Exception f)
            {
                TempData["success"] = "Unable to Add Advert";
            }
            return RedirectToPage("./AllUsers");
        }

    }
}
