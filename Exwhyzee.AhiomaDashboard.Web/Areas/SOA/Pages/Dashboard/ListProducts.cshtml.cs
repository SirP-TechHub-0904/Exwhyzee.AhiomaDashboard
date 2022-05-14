using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.SOA.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SOA,mSuperAdmin")]

    public class ListProductsModel : PageModel
    {

        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ListProductsModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IList<Product> Product { get; set; }
        public UserProfile Profile { get; set; }
        public Tenant Tenant { get; set; }

        public async Task OnGetAsync(string id, long tenantid)
        {
            var user = await _userManager.FindByIdAsync(id);
            Profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == user.Id);
            Tenant = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == tenantid);

            IQueryable<Product> productIQ = from s in _context.Products
                                                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(x => x.ProductPictures)
                .Include(x => x.Tenant.UserProfile)
                .Include(x => x.Tenant).Where(x => x.ThirdPartyUserId == id && x.TenantId == tenantid)
                                            select s;

            Product = await productIQ.ToListAsync();
        }
    }
}
