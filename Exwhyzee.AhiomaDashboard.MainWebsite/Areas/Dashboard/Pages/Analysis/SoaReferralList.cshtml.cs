using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Dashboard.Pages.Analysis
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin")]

    public class SoaReferralListModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public SoaReferralListModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }
        public IList<UserProfile> Profile { get; set; }
        public UserProfile SOAProfile { get; set; }

        public async Task OnGetAsync(string userid, DateTime fdate, DateTime sdate)
        {
            SOAProfile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userid);
                var soauser = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userid);
            Profile = await _context.UserProfiles.Include(x=>x.User).Where(x => x.ReferralLink == soauser.IdNumber && x.Roles.Contains("SOA")).Where(x => x.DateRegistered.Date >= fdate).Where(x => x.DateRegistered.Date <= sdate).ToListAsync();

            }
        }
}
