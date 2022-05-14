using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Microsoft.AspNetCore.Authorization;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Dashboard.Pages.Analysis
{
    [Authorize(Roles = "Admin,mSuperAdmin")]

    public class ProductListModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;

        public ProductListModel(SignInManager<IdentityUser> signInManager, IUserProfileRepository account, AhiomaDbContext context,
            ITenantRepository tenant,
           UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _account = account;
            _context = context;
            _tenant = tenant;
        }


        public IList<Product> Product { get; set; }
        public long? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public UserProfile SOAProfile { get; set; }
        public async Task<IActionResult> OnGetAsync(string userid, DateTime fdate, DateTime sdate)
        {
            SOAProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userid);
            int numbeer = 0;
            var shop = _context.Tenants.FirstOrDefault(x => x.CreationUserId == userid);
            if (shop != null)
            {
                Product = await _context.Products.Include(x=>x.Tenant).Where(x => x.Tenant.CreationUserId == SOAProfile.UserId).Where(x => x.CreatedOnUtc.Date >= fdate).Where(x => x.CreatedOnUtc.Date <= sdate)
                    .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(x => x.ProductPictures)
                .Include(x => x.Tenant.UserProfile)
                .Include(x => x.Tenant).OrderByDescending(x => x.CreatedOnUtc).ToListAsync();
             
            }


            return Page();
        }
    }
}
