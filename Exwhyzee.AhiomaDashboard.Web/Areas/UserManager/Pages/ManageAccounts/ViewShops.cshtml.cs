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

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.ManageAccounts
{
      [Authorize(Roles = "mSuperAdmin")]
    public class ViewShopsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
       

        private readonly AhiomaDbContext _context;



        public ViewShopsModel(
            UserManager<IdentityUser> userManager,
           AhiomaDbContext context
            )
        {
            _userManager = userManager;
            _context = context;
          

        }
        public IList<Tenant> Stores { get; set; }
        public string LoggedInUser { get; set; }
        public bool UserBool { get; set; }

        public async Task OnGetAsync(string name)
        {
            var user = await _userManager.FindByNameAsync(name);

            Stores = await _context.Tenants.Include(x=>x.TenantAddresses).Where(x => x.CreationUserId == user.Id).ToListAsync();

        }
    }
}
