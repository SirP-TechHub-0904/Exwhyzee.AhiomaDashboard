using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class DashboardViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly IProductRepository _product;
        private readonly ITenantRepository _tenant;
        private readonly AhiomaDbContext _context;

        public DashboardViewComponent(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IProductRepository product,
            ITenantRepository tenant,
            IUserProfileRepository account,
            AhiomaDbContext context
            /*IEmailSender emailSender*/)
        {
            _userManager = userManager;
            _context = context;
            _product = product;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tenant = tenant;
        }

        public string UserInfo{ get; set; }
        public string LoggedInUser { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(string uid)
        {
            string newid = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(newid);
            var superrole = await _userManager.IsInRoleAsync(user, "mSuperAdmin");

            if (superrole.Equals(true))
            {
                LoggedInUser = uid;
            }
            else
            {
                LoggedInUser = user.Id;
            }

           // string LoggedInUser = _userManager.GetUserId(HttpContext.User);
            var userdata = await _userManager.FindByIdAsync(LoggedInUser);
            var profile = await _account.GetByUserId(LoggedInUser);
            var tenant = await _tenant.GetAsyncAllBySOA(LoggedInUser);
            TempData["shops"] = tenant.Count();
          //  var allproduct = await _product.GetByStoreByUserAsyncAll(LoggedInUser);
            IQueryable<Product> productIQ = from s in _context.Products
                                               .Include(p => p.Category)
               .Include(p => p.Manufacturer)
               .Include(x => x.ProductPictures)
               .Include(x => x.Tenant.UserProfile)
               .Include(x => x.Tenant).Where(x => x.Tenant.CreationUserId == LoggedInUser)
                                            select s;


            TempData["totlaproduct"] = productIQ.Count();
            try
            {
                string roles = await _account.FetchUserRoles(user.Id);
                TempData["roles"] = roles.Replace("Customer, ", "");
            }
            catch (Exception c) { }
            return View(profile);
        }
    }
}
