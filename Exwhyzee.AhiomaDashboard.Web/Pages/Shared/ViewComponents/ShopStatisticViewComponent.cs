using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
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

namespace Exwhyzee.AhiomaDashboard.Web.Pages.Shared.ViewComponents
{
    public class ShopStatisticViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;

        public ShopStatisticViewComponent(
            UserManager<IdentityUser> userManager,
            AhiomaDbContext context,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            ITenantRepository tenant,
            IUserProfileRepository account
            )
        {
            _userManager = userManager;
            _context = context; _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tenant = tenant;
        }




        public async Task<IViewComponentResult> InvokeAsync(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            var tenant = await _context.Tenants.Where(x => x.CreationUserId == user.Id).CountAsync();
            TempData["count"] = tenant;
            return View();
        }
    }

}
