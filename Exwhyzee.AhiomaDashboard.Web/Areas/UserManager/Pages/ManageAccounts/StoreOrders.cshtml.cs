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

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.ManageAccounts
{
    [Authorize(Roles = "Admin,Store,SOA,mSuperAdmin,CustomerCare")]

    public class StoreOrdersModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public StoreOrdersModel(AhiomaDbContext context, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager
            )
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IList<OrderItem> Orders { get; set; }

                public Tenant Tenant { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            Tenant = await _context.Tenants.Include(x => x.UserProfile).FirstOrDefaultAsync(x => x.UserProfileId == profile.Id);
           

            Orders = await _context.OrderItems.Include(x=>x.Order).Include(x => x.Product).Include(x => x.Product.ProductPictures).Include(x => x.Order.UserProfile).ToListAsync();
            return Page();

        }
    }
}
