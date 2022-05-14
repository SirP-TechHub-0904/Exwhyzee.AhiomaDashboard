using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Shared.ViewComponents
{
    public class RoleDataViewComponent : ViewComponent
    {

        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleDataViewComponent(
            RoleManager<IdentityRole> roleManager
            )
        {
            
            _roleManager = roleManager;
        }

        public IdentityRole RoleData { get; set; }

        public async Task<IViewComponentResult> InvokeAsync(string name, string Userid, string syle, string fullname)
        {
            RoleData = await _roleManager.FindByNameAsync(name);
            TempData["UserID"] = Userid;
            TempData["style"] = syle;
            TempData["fullname"] = fullname;
            return View(RoleData);
        }
    }
}
