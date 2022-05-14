using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class ThirdPartyUploadViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
       private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;


        public ThirdPartyUploadViewComponent(
            UserManager<IdentityUser> userManager,
            IUserProfileRepository account, Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context
            )
        {
            _userManager = userManager;
            _account = account;
          _context = context;
        }

        public string UserInfo{ get; set; }

        public async Task<IViewComponentResult> InvokeAsync()
        {
          string LoggedInUser = _userManager.GetUserId(HttpContext.User);
            var user = await _userManager.FindByIdAsync(LoggedInUser);
            var listshopupload = await _context.ProductUploadShops.Include(x=>x.Tenant).Where(x => x.UserId == LoggedInUser).ToListAsync();
            TempData["uid"] = user.Id;
            return View(listshopupload);
        }
    }
}
