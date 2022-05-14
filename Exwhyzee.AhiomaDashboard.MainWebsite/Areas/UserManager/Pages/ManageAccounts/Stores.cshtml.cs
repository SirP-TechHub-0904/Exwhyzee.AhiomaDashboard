using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.UserManager.Pages.ManageAccounts
{
    [Authorize(Roles = "SOA,Admin,mSuperAdmin,CustomerCare")]
    public class StoresModel : PageModel
    {
       

        private readonly IUserProfileRepository _profile;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        public StoresModel(IUserProfileRepository profile, RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _profile = profile;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IList<Tenant> Stores { get; set; }
        public string LoggedInUser { get; set; }
        public bool UserBool { get; set; }

        public async Task OnGetAsync()
        {
            LoggedInUser = _userManager.GetUserId(HttpContext.User);

            Stores = await _profile.GetAsyncStores(LoggedInUser);

            var userdata = await _userManager.FindByIdAsync(LoggedInUser);

            var userprofile = await _profile.GetByUserId(LoggedInUser);
            if(userprofile.Status == Enums.AccountStatus.Active)
            {
                UserBool = true;
            }
            else {

                UserBool = false;
            }
           

        }
    }
}
