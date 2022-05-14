using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.SOA.Pages.Orders
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SOA,mSuperAdmin")]

    public class OrderDetailsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
       


        public OrderDetailsModel(
                UserManager<IdentityUser> userManager,
                RoleManager<IdentityRole> roleManager,
               
                SignInManager<IdentityUser> signInManager,AhiomaDbContext context
                )
        {
            _userManager = userManager;
            _context = context;
           
            _roleManager = roleManager;
            _signInManager = signInManager;

        }
        [BindProperty]
        public long OrderId { get; set; }
        [BindProperty]
        public string UserID { get; set; }
        [BindProperty]
        public Order Order { get; set; }
        public IList<OrderItem> OrderItems { get; set; }
        public string UserName { get; set; }
        public string Note { get; set; }
        
        public async Task<IActionResult> OnGetAsync(long id)
        {
            Order = await _context.Orders
                .Include(x => x.OrderItems)
                .Include(x => x.LogisticVehicle)
                .Include(x => x.Transaction)
                .Include(x => x.UserProfile.User)
                .Include(x => x.LogisticVehicle.LogisticProfile)
                .Include(x => x.UserProfile)
                .Include(x => x.UserProfile.UserAddresses)
                .FirstOrDefaultAsync(x => x.Id == id);
            OrderItems = await _context.OrderItems.Include(x => x.Product).Include(x => x.Product.ProductPictures)
                .Include(x => x.Product.Tenant)
                .Include(x => x.Product.Tenant.TenantAddresses)
                .Where(x => x.OrderId == id).ToListAsync();

            var iUserName = await _context.UserProfiles.FirstOrDefaultAsync(x=>x.UserId == Order.Transaction.UserId);
            UserName = iUserName.Fullname + "(" + iUserName.IdNumber + ")";


            return Page();
        }
    }
}
