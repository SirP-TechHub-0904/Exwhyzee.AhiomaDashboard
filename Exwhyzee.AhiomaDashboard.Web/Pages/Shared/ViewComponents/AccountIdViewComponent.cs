using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Pages.Shared.ViewComponents
{
    public class AccountIdViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
       
        private readonly IUserProfileRepository _account;


        public AccountIdViewComponent(
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
          string LoggedInUser = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(uid);
            var profile = await _account.GetByUserId(uid);
            TempData["name"] = profile.IdNumber;
            return View();
        }
    }
}
