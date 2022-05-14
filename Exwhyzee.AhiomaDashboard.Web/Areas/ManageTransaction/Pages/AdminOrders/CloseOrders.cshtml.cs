using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.ManageTransaction.Pages.AdminOrders
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin,CustomerCare,Logistic")]
    public class CloseOrdersModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public CloseOrdersModel(AhiomaDbContext context, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager
            )
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IList<Order> Orders { get; set; }
        public IList<OrderItem> OrderItems { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var iOrderItems = _context.OrderItems.Include(x => x.Product).Include(x => x.Order.UserProfile).Include(x => x.Order).Include(x => x.Product.ProductPictures).OrderByDescending(x => x.DateOfOrder);
            OrderItems = await iOrderItems.Where(x => x.Status == Enums.OrderStatus.Cancel || x.Status == Enums.OrderStatus.Reversed || x.Status == Enums.OrderStatus.None).OrderByDescending(x => x.DateOfOrder).ToListAsync();

            return Page();
        
        }
    }
}
