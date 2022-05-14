using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.Data.Repository.TenantAddresses;
using Exwhyzee.AhiomaDashboard.Data.Repository.TenantSocialMedias;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.Data.Dtos;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Logistic.Pages.Dashboard
{
  
        [Authorize(Roles = "Logistic")]

        public class LogisticProfileModel : PageModel
        {

            private readonly AhiomaDbContext _context;
            private readonly IHostingEnvironment _hostingEnv;
            private readonly UserManager<IdentityUser> _userManager;
            public LogisticProfileModel(AhiomaDbContext context, IHostingEnvironment env,
                UserManager<IdentityUser> userManager)
            {

                _context = context;
                _hostingEnv = env;
                _userManager = userManager;
            }



            public LogisticProfile Logistic { get; set; }
            public int Count { get; set; }
            public IList<LogisticVehicle> LogisticVehicle { get; set; }


            public async Task<IActionResult> OnGetAsync()
            {
            var user = await _userManager.GetUserAsync(User);
                Logistic = await _context.LogisticProfiles.Include(x => x.UserProfile).Include(x => x.UserProfile.User).Include(x => x.LogisticVehicles).FirstOrDefaultAsync(x => x.UserId == user.Id);
                return Page();
            }



        }
    }
