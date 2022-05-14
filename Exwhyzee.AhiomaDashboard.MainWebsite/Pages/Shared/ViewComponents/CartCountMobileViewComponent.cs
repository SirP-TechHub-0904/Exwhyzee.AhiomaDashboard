
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

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class CartCountMobileViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        //private readonly IEmailSender _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;

        public CartCountMobileViewComponent(
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
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(name);
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);

                var cart = await _context.ProductCarts.Where(x => x.UserProfileId == profile.Id && x.CartStatus == Enums.CartStatus.Active && x.TrackOrderId != null).CountAsync();
                TempData["cart"] = cart;
            }
            else
            {
                var cartsessionid = HttpContext.Session.GetString("CartUserId");
                if(cartsessionid != null)
                {
                    var cart = await _context.ProductCarts.Where(x => x.CartTempId == cartsessionid && x.CartStatus == Enums.CartStatus.Active && x.TrackOrderId != null).CountAsync();
                    TempData["cart"] = cart;
                }
                else
                {
                    TempData["cart"] = 0;
                }
                
            }


           

            return View();
        }
    }
}
