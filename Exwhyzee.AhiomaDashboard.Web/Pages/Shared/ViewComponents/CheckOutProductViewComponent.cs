using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Pages.Shared.ViewComponents
{
    public class CheckOutProductViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        //private readonly IEmailSender _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;

        public CheckOutProductViewComponent(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            ITenantRepository tenant,
            AhiomaDbContext context,
            IUserProfileRepository account
            /*IEmailSender emailSender*/)
        {
            _userManager = userManager;
            //_emailSender = emailSender;
            _account = account;
            _roleManager = roleManager;
            _context = context;
            _signInManager = signInManager;
            _tenant = tenant;
        }



        public async Task<IViewComponentResult> InvokeAsync(string name)
        {
            
            var user = await _userManager.FindByNameAsync(name);
        
            var UserProfile = await _context.UserProfiles.Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
            var cart = await _context.ProductCarts.Include(x=>x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == UserProfile.Id && x.CartStatus == Enums.CartStatus.Active && x.TrackOrderId != null).ToListAsync();
            decimal sum = 0;
            decimal delivery = 0;
            foreach (var p in cart)
            {
                var eachtotal = p.Quantity * p.Product.Price;
                sum = sum + eachtotal;
            }

            //
            var addr = UserProfile.UserAddresses.FirstOrDefault(x => x.Default == true && x.State.ToUpper().Contains("IMO"));

            TempData["sum"] = sum;
            var settings = await _context.Settings.FirstOrDefaultAsync();
            if (settings.ActivateFreeDelivery == true && addr != null)
            {
                delivery = 0;
                TempData["Logistic"] = "Free Delivery";
                ViewBag.Logistic = "Free Delivery for Owerri";
            }
            else
            {
                if (cart.Count() < 5)
                {

                    delivery = 1000;
                    TempData["Logistic"] = delivery;
                }
                else if (cart.Count() < 5 && cart.Count() > 9)
                {
                    delivery = 1500;
                    TempData["Logistic"] = delivery;
                }
                else
                {
                    delivery = 2000;
                    TempData["Logistic"] = delivery;
                }
            }
            TempData["Oversum"] = sum + delivery;

            return View(cart);
        }
    }
}
