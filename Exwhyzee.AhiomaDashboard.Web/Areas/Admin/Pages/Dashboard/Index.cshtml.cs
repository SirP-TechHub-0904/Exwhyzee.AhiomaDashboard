using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class IndexModel : PageModel
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;
        public IndexModel(UserManager<IdentityUser> userManager, AhiomaDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public string UserId { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            TempData["name"] = profile.Fullname;
            UserId = profile.UserId;
        }

    }

}
