using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Dashboard.Pages.Analysis
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin")]

    public class ShopListModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public ShopListModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }
        public IList<Tenant> Stores { get; set; }
        public UserProfile SOAProfile { get; set; }

        public async Task OnGetAsync(string userid, DateTime fdate, DateTime sdate)
        {
            SOAProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userid);
            Stores = await _context.Tenants.Include(x => x.User).Include(x => x.Market).Include(x => x.TenantAddresses).Where(x => x.CreationUserId == userid).Where(x => x.CreationTime.Date >= fdate).Where(x => x.CreationTime.Date <= sdate).ToListAsync();


        }
    }
}

