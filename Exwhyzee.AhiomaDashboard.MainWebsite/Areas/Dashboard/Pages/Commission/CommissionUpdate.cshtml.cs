using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Dashboard.Pages.Commission
{
    [Authorize(Roles = "Admin,mSuperAdmin")]
    public class CommissionUpdateModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;

        public CommissionUpdateModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,

            IUserProfileRepository account, AhiomaDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public Tenant Profile { get; set; }
        public PaginatedList<Product> Product { get; set; }
        public long? TenantId { get; set; }
      
        //paging 
        //public int CurrentPage { get; set; } = 1;
        //public int CurrentPageFive { get; set; }
        //public int CurrentPageLastFIve { get; set; }

        public int Count { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public int? CurrentPage { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id, int? pageIndex)
        {
            if (id == null)
            {
                return NotFound();
            }
            Profile = await _context.Tenants.Include(x => x.UserProfile).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);

            IQueryable<Product> products = from s in _context.Products
                                                  .Where(x => x.TenantId == id)
                                           select s;
            Count = products.Count();
            TenantId = id;
            CurrentPage = pageIndex;
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;
            Product = await PaginatedList<Product>.CreateAsync(
                   products.AsNoTracking(), pageIndex ?? 1, pageSize);
            return Page();
        
        }
    }
}
