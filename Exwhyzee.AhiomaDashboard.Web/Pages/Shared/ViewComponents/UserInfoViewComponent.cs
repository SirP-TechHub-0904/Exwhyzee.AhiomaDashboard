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
    public class UserInfoViewComponent:ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        //private readonly IEmailSender _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;

        public UserInfoViewComponent(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            ITenantRepository tenant,
            IUserProfileRepository account
            /*IEmailSender emailSender*/)
        {
            _userManager = userManager;
            //_emailSender = emailSender;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tenant = tenant;
        }

        public string UserInfo{ get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
          string LoggedInUser = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(LoggedInUser);
            var profile = await _account.GetByUserId(LoggedInUser);
            var tenant = await _tenant.GetByLogin(LoggedInUser);
            var storerole = await _userManager.IsInRoleAsync(user, "Store");
            if (storerole.Equals(true))
            {
                UserInfo = tenant.BusinessName.ToUpper() + " STORE";
            }
            else
            {
                UserInfo = profile.Fullname.ToUpper();
            }
            TempData["Userinfo"] = UserInfo;

            return View();
        }
    }
}
