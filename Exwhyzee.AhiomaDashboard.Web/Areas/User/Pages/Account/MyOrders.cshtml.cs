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

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Authorize]
    public class MyOrdersModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public MyOrdersModel(AhiomaDbContext context, SignInManager<IdentityUser> signInManager,
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
           // Orders = await _context.Orders.Include(x=>x.OrderItems).Include(x => x.UserProfile).Where(x => x.UserProfileId == profile.Id).ToListAsync();
            var iOrderItems = _context.OrderItems.Include(x => x.Product).Include(x => x.Order.UserProfile).Include(x => x.Order).Include(x => x.Product.ProductPictures).Where(x => x.Order.UserProfileId == profile.Id).OrderByDescending(x=>x.DateOfOrder);
            OrderItems = await iOrderItems.Where(x => x.Status == Enums.OrderStatus.Completed || x.Status == Enums.OrderStatus.Pending || x.Status == Enums.OrderStatus.Processing).OrderByDescending(x=>x.DateOfOrder).ToListAsync();
            return Page();
        
        }
    }
}
