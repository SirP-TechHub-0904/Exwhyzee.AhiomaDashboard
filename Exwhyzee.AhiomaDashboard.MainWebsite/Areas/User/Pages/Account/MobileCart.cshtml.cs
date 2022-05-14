using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    public class MobileCartModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public MobileCartModel(AhiomaDbContext context, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager
            )
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IList<ProductCart> ProductCarts { get; set; }
        public int productCount { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef)
        {
            CustomerRef = customerRef;
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                
                ProductCarts = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == profile.Id && x.CartStatus == Enums.CartStatus.Active && x.TrackOrderId != null).ToListAsync();
                productCount = ProductCarts.Count();
                decimal sum = 0;
                foreach (var p in ProductCarts)
                {
                    var eachtotal = p.Quantity * p.Product.Price;
                    sum = sum + eachtotal;
                }
                TempData["sum"] = sum;
                if (sum == 0)
                {
                    TempData["sumnone"] = "none";
                }
            }
            else
            {
                var cartsessionid = HttpContext.Session.GetString("CartUserId");
                if (cartsessionid != null)
                {
                    ProductCarts = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.CartTempId == cartsessionid && x.CartStatus == Enums.CartStatus.Active && x.TrackOrderId != null).ToListAsync();
                    productCount = ProductCarts.Count();
                    decimal sum = 0;
                    foreach (var p in ProductCarts)
                    {
                        var eachtotal = p.Quantity * p.Product.Price;
                        sum = sum + eachtotal;
                    }
                    TempData["sum"] = sum;
                    if (sum == 0)
                    {
                        TempData["sumnone"] = "none";
                    }
                }
                else
                {
                    TempData["sum"] = 0;
                    productCount = 0;
                    TempData["sumnone"] = "none";
                }
            }
            
                
                
            
                return Page();
        }
    }
}
