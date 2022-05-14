using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Store.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Store,mSuperAdmin")]

    public class ShopOrdersModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShopOrdersModel(AhiomaDbContext context,
            UserManager<IdentityUser> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }
        public IList<OrderItem> OrderItem { get; set; }


        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var store = await _context.Tenants.FirstOrDefaultAsync(x => x.UserId == user.Id);
            OrderItem = await _context.OrderItems.Include(x => x.Product).Include(x => x.Order.UserProfile).Include(x => x.Order).Include(x => x.Product.ProductPictures).Include(x => x.Product.Tenant).Where(x => x.Product.TenantId == store.Id).ToListAsync();
        }
    }
}
