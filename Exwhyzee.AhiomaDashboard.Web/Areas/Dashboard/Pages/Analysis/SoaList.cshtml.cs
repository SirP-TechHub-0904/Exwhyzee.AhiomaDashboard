using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data.Migrations;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Dashboard.Pages.Analysis
{


    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin")]

    public class SoaListModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;
        public SoaListModel(
                UserManager<IdentityUser> userManager,
                AhiomaDbContext context, IUserProfileRepository account
                )
        {
            _userManager = userManager;
            _context = context;
            _account = account;

        }

        public IList<UserProfile> Profile { get; set; }
        public IQueryable<UserProfile> Profiles { get; set; }
        public int CountSoa { get; set; }
        public int CountStores { get; set; }
        public async Task OnGetAsync()
        {
            Profiles = await _account.GetAsyncAllByRole("SOA");
            Profile = Profiles.Where(x => x.Status == Enums.AccountStatus.Active).ToList();
            CountSoa = Profile.Count();
            CountStores = await _context.Tenants.Where(x=>x.TenantStatus == Enums.TenantEnum.Enable).CountAsync();
        }

        public async Task<IActionResult> OnPostUpdateUserStatus(string uid, int statusType)
        {
            return RedirectToPage();
        }

    }
}
