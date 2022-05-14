using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class MobileUserBalanceViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
       
        private readonly IUserProfileRepository _account;
        private readonly AhiomaDbContext _context;


        public MobileUserBalanceViewComponent(
            UserManager<IdentityUser> userManager,
            IUserProfileRepository account
, AhiomaDbContext context)
        {
            _userManager = userManager;
            _account = account;
            _context = context;
        }

        public string UserInfo{ get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (profile != null)
                {
                    var bal = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    TempData["name"] = profile.Fullname;
                    TempData["IDNUMBER"] = profile.IdNumber;
                    TempData["balance"] = bal.WithdrawBalance;
                    TempData["image"] = profile.ProfileUrl;
                }
                else
                {
                    TempData["name"] = "Guest";
                    TempData["IDNUMBER"] = "XXXXXX";
                    TempData["balance"] = "0";
                    TempData["image"] = "/NewMobile/suha/user.png";

                }
            }
            catch(Exception c)
            {
                TempData["name"] = "Guest";
                TempData["IDNUMBER"] = "XXXXXX";
                TempData["balance"] = "0";
                TempData["image"] = "/NewMobile/suha/user.png";
            }
            return View();
        }
    }
}
