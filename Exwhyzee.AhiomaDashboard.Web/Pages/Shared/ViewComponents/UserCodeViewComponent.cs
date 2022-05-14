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


    public class UserCodeViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly IUserProfileRepository _account;


        public UserCodeViewComponent(
            UserManager<IdentityUser> userManager,
            IUserProfileRepository account
            )
        {
            _userManager = userManager;
            _account = account;

        }

        public string UserInfo { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                string LoggedInUser = _userManager.GetUserId(HttpContext.User);
                var user = await _userManager.FindByIdAsync(LoggedInUser);
                var profile = await _account.GetByUserId(LoggedInUser);
                TempData["code"] = profile.IdNumber;
            }
            catch (Exception s)
            {
                TempData["code"] = "0000000";
            }
            return View();
        }
    }

}
