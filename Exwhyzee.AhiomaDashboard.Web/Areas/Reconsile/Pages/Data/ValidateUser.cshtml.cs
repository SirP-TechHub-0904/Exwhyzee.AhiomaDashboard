using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Reconsile.Pages.Data
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin")]

    public class ValidateUserModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;

        public ValidateUserModel(
            UserManager<IdentityUser> userManager, AhiomaDbContext context)
        {
            _userManager = userManager;
            _context = context;
           
        }

       
        public async Task<ActionResult> OnGetAsync(string IDnumber, string Password)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.IdNumber == IDnumber);
            if(profile == null)
            {
                TempData["error"] = "Invalid User";
                return Page();
            }
            var user = await _userManager.FindByIdAsync(profile.UserId);
            if (user == null)
            {
                TempData["error"] = "Invalid User";
                return Page();
            }
            try
            {
                var remove = await _userManager.RemovePasswordAsync(user);
                var addpass = await _userManager.AddPasswordAsync(user, Password);
                TempData["success"] = "Password change successful";
            }
            catch(Exception d) { }

            return Page();
        }
    }
}
