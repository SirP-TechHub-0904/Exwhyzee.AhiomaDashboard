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

namespace Exwhyzee.AhiomaDashboard.Web.Pages.Shared.ViewComponents
{
    public class ShopProductsViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
       private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;


        public ShopProductsViewComponent(
            UserManager<IdentityUser> userManager,
            IUserProfileRepository account, Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context
            )
        {
            _userManager = userManager;
            _account = account;
          _context = context;
        }

        public string UserInfo{ get; set; }

        public async Task<IViewComponentResult> InvokeAsync(long id, long tid)
        {
            var item = await _context.OrderItems.Include(x => x.Order).Include(x => x.Order.UserProfile).Include(x => x.Product.Tenant).Include(x => x.Product.ProductPictures).Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == id && x.Product.TenantId == tid);
         
            return View(item);
        }
    }
}
