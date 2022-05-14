using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    [Authorize]
    public class BuyNowModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;
        private readonly UserManager<IdentityUser> _userManager;


        public BuyNowModel(AhiomaDbContext context, UserManager<IdentityUser> userManager, IUserProfileRepository account

            )
        {
            _context = context;
            _userManager = userManager;
            _account = account;
        }
        public UserProfile UserProfile { get; set; }
        [BindProperty]
        public UserAddress UserAddress { get; set; }

        [BindProperty]
        public string Surname { get; set; }
        [BindProperty]
        public string FirstName { get; set; }
        public IList<ProductCart> ProductCartsBUser { get; set; }


        public List<SelectListItem> StateListing { get; set; }

        [BindProperty]
        public bool NoAddress { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef, long ProductId, string ItemSize, string ItemColor, int Quantity)
        {

            
            CustomerRef = customerRef;
            var productcheck = await _context.Products.Include(x=>x.Tenant).Include(x => x.Tenant.Market).FirstOrDefaultAsync(x => x.Id == ProductId);
            if (Quantity < 0)
            {
                TempData["size"] = "Low on Stock";
                return RedirectToPage("/ProductDetails", new { CustomerRef = customerRef, id = ProductId, name = productcheck.ShortDescription, shop = productcheck.Tenant.BusinessName, mktstate = productcheck.Tenant.Market.Name, mktaddress = productcheck.Tenant.Market.Address });

            }
            if (productcheck.UseColor == true && String.IsNullOrEmpty(ItemColor))
            {
                TempData["color"] = "select product color and size";
                //long? id, string name, string shop, string mktstate, string mktaddress
                return RedirectToPage("/ProductDetails", new { CustomerRef = customerRef, id = ProductId, name = productcheck.ShortDescription, shop = productcheck.Tenant.BusinessName, mktstate = productcheck.Tenant.Market.Name, mktaddress = productcheck.Tenant.Market.Address });
            }
            if (productcheck.UseSize == true && String.IsNullOrEmpty(ItemSize))
            {
                TempData["size"] = "select product size and color";
                return RedirectToPage("/ProductDetails", new { CustomerRef = customerRef, id = ProductId, name = productcheck.ShortDescription, shop = productcheck.Tenant.BusinessName, mktstate = productcheck.Tenant.Market.Name, mktaddress = productcheck.Tenant.Market.Address });

            }
            var user = await _userManager.GetUserAsync(User);
            UserProfile = await _context.UserProfiles.Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (UserProfile.DisableBuy == true)
            {
                TempData["size"] = "You have been Suspended from buying";
                return RedirectToPage("/ProductDetails", new { CustomerRef = customerRef, id = ProductId, name = productcheck.ShortDescription, shop = productcheck.Tenant.BusinessName, mktstate = productcheck.Tenant.Market.Name, mktaddress = productcheck.Tenant.Market.Address });

            }
            HttpContext.Session.SetString("XXXXX", Guid.NewGuid().ToString());
            string trackcart = HttpContext.Session.GetString("XXXXX");
            trackcart = trackcart.Replace("-", "");
            string CheckTrackOrderId = HttpContext.Session.GetString("TrackOrderId");
            if (CheckTrackOrderId == null)
            {
                HttpContext.Session.SetString("TrackOrderId", Guid.NewGuid().ToString());
            }
            string TrackOrderId = HttpContext.Session.GetString("TrackOrderId");


            if (productcheck != null)
            {

                var cartitem = await _context.ProductCarts.Where(x => x.CartStatus == Enums.CartStatus.Active).ToListAsync();
                ProductCart logcartitem = new ProductCart();
               
                    logcartitem = cartitem.FirstOrDefault(x => x.UserProfileId == UserProfile.Id && x.ProductId == productcheck.Id);
               
                if (logcartitem != null)
                {
                    logcartitem.Quantity = logcartitem.Quantity + Quantity;
                    _context.Attach(logcartitem).State = EntityState.Modified;

                }
                else
                {
                    ProductCart cart = new ProductCart();
                    cart.ProductId = productcheck.Id;
                    cart.CartStatus = Enums.CartStatus.Active;
                    cart.Quantity = Quantity;
                    cart.TrackCartId = trackcart;
                    cart.Itemcolor = ItemColor;
                    cart.ItemSize = ItemSize;
                    cart.TrackOrderId = TrackOrderId;
                    cart.Date = DateTime.UtcNow.AddHours(1);
                    cart.UserProfileId = UserProfile.Id;
                    _context.ProductCarts.Add(cart);
                }
                await _context.SaveChangesAsync();
            }




            ProductCartsBUser = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == UserProfile.Id && x.CartStatus == Enums.CartStatus.Active).ToListAsync();

            var address = await _context.UserAddresses.FirstOrDefaultAsync(x => x.UserId == UserProfile.UserId);
      

            if (address == null)
            {
                NoAddress = true;
            }
            var state = await _context.UserAddresses.Where(x => x.UserId == UserProfile.UserId).OrderBy(x => x.Default).ToListAsync();
            StateListing = state.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id.ToString(),
                                      Text = a.Address + " in " + a.LocalGovernment + " " + a.State
                                  }).ToList();
            return RedirectToPage("/Account/DeliveryAddress", new { area = "User" });
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
        //    profile.Surname = Surname;
        //    profile.FirstName = FirstName;
        //    _context.Attach(profile).State = EntityState.Modified;

        //    var address = await _context.UserAddresses.FirstOrDefaultAsync(x => x.UserId == user.Id);
        //    address.Address = UserAddress.Address;
        //    address.State = UserAddress.State;
        //    address.LocalGovernment = UserAddress.LocalGovernment;
        //    _context.Attach(address).State = EntityState.Modified;

        //    await _context.SaveChangesAsync();

        //    return RedirectToPage("/Account/DeliveryMethod", new { area = "User" });
        //}
    }
}
