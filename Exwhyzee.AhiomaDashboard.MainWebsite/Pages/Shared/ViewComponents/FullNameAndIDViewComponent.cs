using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class FullNameAndIDViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
       
        private readonly IUserProfileRepository _account;


        public FullNameAndIDViewComponent(
            UserManager<IdentityUser> userManager,
            IUserProfileRepository account
            )
        {
            _userManager = userManager;
            _account = account;
          
        }

        public string UserInfo{ get; set; }

        public async Task<IViewComponentResult> InvokeAsync(string uid)
        {
            var user = await _userManager.FindByIdAsync(uid);
            var profile = await _account.GetByUserId(uid);
            TempData["name"] = profile.Fullname;
            TempData["IDNUMBER"] = profile.IdNumber;
            return View();
        }
    }
}
