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
    public class ShopNameAddressViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;


        public ShopNameAddressViewComponent(
            UserManager<IdentityUser> userManager, AhiomaDbContext context,
            IUserProfileRepository account
            )
        {
            _userManager = userManager;
            _account = account;
            _context = context;
        }

        public string UserInfo{ get; set; }

        public async Task<IViewComponentResult> InvokeAsync(long tid)
        {
            var tenant = await _context.Tenants.Include(x => x.TenantAddresses).Include(x => x.Market).FirstOrDefaultAsync(x => x.Id == tid);
         
            return View(tenant);
        }
    }
}
