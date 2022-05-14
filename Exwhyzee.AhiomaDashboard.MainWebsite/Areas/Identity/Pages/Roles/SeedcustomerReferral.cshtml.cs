using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Identity.Pages.Roles
{
    public class SeedcustomerReferralModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserProfileRepository _account;
        private readonly AhiomaDbContext _context;

        public SeedcustomerReferralModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserProfileRepository account, AhiomaDbContext context)
        {
            _userManager = userManager;
            _account = account;
            _roleManager = roleManager;
            _context = context;
        }

        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var user = new IdentityUser { UserName = "customerreferral@ahioma.com", Email = "customerreferral@ahioma.com", PhoneNumber = "900900000000", LockoutEnabled = false };
            var result = await _userManager.CreateAsync(user, "ahiomaorder@123");
            if (result.Succeeded)
            {
               
                    var role = new IdentityRole("Ahiomaorder");
                    await _roleManager.CreateAsync(role);
                    await _userManager.AddToRoleAsync(user, "Ahiomaorder");

                

                //create wallet
                Wallet wallet = new Wallet();
                wallet.UserId = user.Id;
                wallet.Balance = 0;
                wallet.WithdrawBalance = 0;
                wallet.CreationTime = DateTime.UtcNow.AddHours(1);
                wallet.LastUpdateTime = DateTime.UtcNow.AddHours(1);

               
                UserProfile profile = new UserProfile();
                profile.UserId = user.Id;
                profile.DateRegistered = DateTime.UtcNow.AddHours(1);
                profile.Surname = "Customer";
                profile.FirstName = "Referral";
                profile.OtherNames = "Account";
                profile.DateRegistered = DateTime.UtcNow.AddHours(1);

                _context.Wallets.Add(wallet);
                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();

                var profileupdate = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == wallet.UserId);
                profileupdate.IdNumber = profileupdate.Id.ToString("0000000");
                _context.Attach(profileupdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
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
