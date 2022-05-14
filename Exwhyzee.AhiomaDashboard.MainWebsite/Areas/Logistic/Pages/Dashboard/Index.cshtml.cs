using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Logistic.Pages.Dashboard
{
  
        [Authorize(Roles = "Admin,mSuperAdmin,CustomerCare")]

        public class IndexModel : PageModel
        {
            private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

            public IndexModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
            {
                _context = context;
            }

            public IList<LogisticProfile> Logistic { get; set; }

            public async Task OnGetAsync()
            {
                Logistic = await _context.LogisticProfiles.Include(x=>x.LogisticVehicles).Include(x => x.UserProfile).Include(x => x.User).OrderByDescending(x => x.CreationTime).ToListAsync();
            }
        }
    
}
