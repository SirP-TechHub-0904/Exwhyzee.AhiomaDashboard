using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.SOA.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SOA,mSuperAdmin")]

    public class ShopsModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserProfileRepository _profile;
        public ShopsModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, UserManager<IdentityUser> userManager, IUserProfileRepository profile)
        {
            _context = context;
            _userManager = userManager;
            _profile = profile;
        }
        public UserProfile SOAProfile { get; set; }
        public IList<Tenant> Stores { get; set; }
        public string LoggedInUser { get; set; }
        public bool UserBool { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            Stores = await _profile.GetAsyncStores(user.Id);

            var soauser = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            Stores = await _context.Tenants.Include(x => x.User).Include(x => x.Market).Include(x => x.TenantAddresses).Where(x=>x.CreationUserId == soauser.UserId).ToListAsync();
            var userprofile = await _profile.GetByUserId(user.Id);
            if (userprofile.Status == Enums.AccountStatus.Active)
            {
                UserBool = true;
            }
            else
            {

                UserBool = false;
            }

        }
    }
}
