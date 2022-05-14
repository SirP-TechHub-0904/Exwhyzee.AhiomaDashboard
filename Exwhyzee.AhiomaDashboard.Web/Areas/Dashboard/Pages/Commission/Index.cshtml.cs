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

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Dashboard.Pages.Commission
{
  
        [Authorize(Roles = "Admin,mSuperAdmin")]
        public class IndexModel : PageModel
        {
            private readonly UserManager<IdentityUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly SignInManager<IdentityUser> _signInManager;
            private readonly AhiomaDbContext _context;
            private readonly IUserProfileRepository _account;

            public IndexModel(
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

            public IList<Tenant> Profile { get; set; }

            public async Task OnGetAsync()
            {
                Profile = await _context.Tenants.Include(x => x.UserProfile).Include(x => x.User).OrderByDescending(x => x.CreationTime).ToListAsync();
            }

        }
    }
