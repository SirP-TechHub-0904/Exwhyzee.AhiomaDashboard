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

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Authorize]
    public class DeliveryMethodModel : PageModel
    {

        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public DeliveryMethodModel(AhiomaDbContext context, UserManager<IdentityUser> userManager

            )
        {
            _context = context;
            _userManager = userManager;
        }
        public UserProfile UserProfile { get; set; }
        public UserAddress UserAddress { get; set; }
        public IList<ProductCart> ProductCartsBUser { get; set; }

        [BindProperty]
        public string DeliveryMethod { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef)
        {
            CustomerRef = customerRef;
            var user = await _userManager.GetUserAsync(User);
            UserProfile = await _context.UserProfiles.Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
            UserAddress = await _context.UserAddresses.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Default == true);
            ProductCartsBUser = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == UserProfile.Id).ToListAsync();



            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
           var ProductCarts = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == profile.Id).ToListAsync();

            foreach(var p in ProductCarts)
            {
                p.DeliveryMethod = DeliveryMethod;   
                _context.Attach(p).State = EntityState.Modified;

            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/CheckoutPayment", new { area = "User", customerRef = CustomerRef });
        }

    }
}