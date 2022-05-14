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

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Authorize]
    public class UpdateAddressModel : PageModel
    {
            private readonly AhiomaDbContext _context;
            private readonly IUserProfileRepository _account;
            private readonly UserManager<IdentityUser> _userManager;


            public UpdateAddressModel(AhiomaDbContext context, UserManager<IdentityUser> userManager, IUserProfileRepository account

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

            public async Task<IActionResult> OnGetAsync()
            {
                var user = await _userManager.GetUserAsync(User);
                UserProfile = await _context.UserProfiles.Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
                var cartsessionid = HttpContext.Session.GetString("CartUserId");
                var ProductCarts = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.CartTempId == cartsessionid).ToListAsync();
                try
                {
                    foreach (var p in ProductCarts)
                    {
                        p.UserProfileId = UserProfile.Id;
                        p.CartTempId = "";
                        p.CartStatus = Enums.CartStatus.Active;
                        _context.Attach(p).State = EntityState.Modified;
                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception d)
                {

                }
                ProductCartsBUser = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == UserProfile.Id).ToListAsync();

                var address = await _context.UserAddresses.FirstOrDefaultAsync(x => x.UserId == UserProfile.UserId);

                HttpContext.Session.Remove("CartUserId");

               
                var state = await _account.GetStates();
                StateListing = state.Select(a =>
                                      new SelectListItem
                                      {
                                          Value = a.StateName,
                                          Text = a.StateName
                                      }).ToList();
            UserAddress = address;
                return Page();
            }

            public async Task<IActionResult> OnPostAsync()
            {
                var user = await _userManager.GetUserAsync(User);
              
                var address = await _context.UserAddresses.FirstOrDefaultAsync(x => x.UserId == user.Id);
                address.Address = UserAddress.Address;
                address.State = UserAddress.State;
                address.LocalGovernment = UserAddress.LocalGovernment;
                _context.Attach(address).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return RedirectToPage("/Account/DeliveryMethod", new { area = "User" });
            }
        
    }
}
