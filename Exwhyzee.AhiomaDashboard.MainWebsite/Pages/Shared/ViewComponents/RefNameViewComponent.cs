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
    public class RefNameViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly IUserProfileRepository _account;


        public RefNameViewComponent(
            UserManager<IdentityUser> userManager,
            IUserProfileRepository account
            )
        {
            _userManager = userManager;
            _account = account;

        }

        public string UserInfo { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(string cid)
        {
            try
            {

                var user = await _userManager.FindByIdAsync(cid);
                var profile = await _account.GetByUserId(user.Id);
                TempData["code"] = profile.Fullname;
                TempData["code1"] = profile.IdNumber;
            }
            catch (Exception s)
            {
                TempData["code"] = "0000000";
                TempData["code1"] = "-----";
            }
            return View();
        }
    }
}
