using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Pages.Shared.ViewComponents
{
    public class CustomerRefViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;

        private readonly IUserProfileRepository _account;


        public CustomerRefViewComponent(
            UserManager<IdentityUser> userManager,
            IUserProfileRepository account
, AhiomaDbContext context)
        {
            _userManager = userManager;
            _account = account;
            _context = context;
        }

        public string UserInfo { get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string LoggedInUser = _userManager.GetUserId(HttpContext.User);

            var profile = await _account.GetByUserId(LoggedInUser);
            TempData["name"] = profile.IdNumber;
            try
            {
                if(User.Identity.IsAuthenticated && User.IsInRole("mSuperAdmin"))
                {
                    IQueryable<UserProfile> profiles = from s in _context.UserProfiles
                                                
                .Where(x => x.LastUserUpdated <= DateTime.Now.AddDays(-30))
                .Where(x => x.Status == Enums.AccountStatus.Pending)
                                                       select s;
                    TempData["profiles"] = profiles.Count();
                }
                                            
                }catch(Exception d)
            {

            }
            return View();
        }
    }
}
