using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Identity.Pages.Roles
{
    public class SeedAccountModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserProfileRepository _account;

        public SeedAccountModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserProfileRepository account)
        {
            _userManager = userManager;
            _account = account;
            _roleManager = roleManager;
        }

        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var user = new IdentityUser { UserName = "jinmcever@gmail.com", Email = "jinmcever@gmail.com", PhoneNumber = "070000000000000", LockoutEnabled = false };
            var result = await _userManager.CreateAsync(user, "jinmcever@123");
            if (result.Succeeded)
            {
               
                    var role = new IdentityRole("mSuperAdmin");
                    await _roleManager.CreateAsync(role);
                    await _userManager.AddToRoleAsync(user, "mSuperAdmin");

                

                //create wallet
                Wallet wallet = new Wallet();
                wallet.UserId = user.Id;
                wallet.Balance = 0;
                wallet.CreationTime = DateTime.UtcNow.AddHours(1);
                wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);


                UserProfile profile = new UserProfile();
                profile.UserId = user.Id;
                profile.DateRegistered = DateTime.UtcNow.AddHours(1);
                profile.Surname = "Major";
                profile.FirstName = "Admin";
                profile.OtherNames = "Account";
                profile.DateRegistered = DateTime.UtcNow.AddHours(1);

                string id = await _account.CreateAccount(wallet, profile, null, null, null);

                return LocalRedirect(Url.Content("~/"));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return LocalRedirect(Url.Content("~/"));
        }


    }

}
