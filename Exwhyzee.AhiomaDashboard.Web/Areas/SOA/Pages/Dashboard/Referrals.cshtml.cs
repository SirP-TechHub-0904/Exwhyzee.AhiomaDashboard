using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.SOA.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SOA,mSuperAdmin")]

    public class ReferralsModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ReferralsModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IList<UserProfile> Profile { get; set; }
        public UserProfile SOAProfile { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            SOAProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var soauser = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            Profile = await _context.UserProfiles.Include(x => x.User).Where(x => x.ReferralLink == soauser.IdNumber && x.Roles.Contains("SOA")).ToListAsync();

        }
    }
}

