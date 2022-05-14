using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.User.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    /// <summary>
    /// 
    /// </summary>
    public class RequestChangeModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHostingEnvironment _hostingEnv;
        public RequestChangeModel(IUserProfileRepository profile, RoleManager<IdentityRole> roleManager, AhiomaDbContext context,
            UserManager<IdentityUser> userManager,
             IHostingEnvironment hostingEnv
            )
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _hostingEnv = hostingEnv;
        } 
        [BindProperty]
        public RequestPhoneEmailChange Data { get; set; }
       


        public async Task OnGetAsync()
        {
            

        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            try { 
            Data.Date = DateTime.UtcNow.AddHours(1);
                Data.UserId = user.Id;
                Data.UserProfileId = profile.Id;
                Data.EmailStatus = Enums.ChangeDataStatus.Pending;
                Data.PhoneStatus = Enums.ChangeDataStatus.Pending;
                _context.RequestPhoneEmailChanges.Add(Data);
                await _context.SaveChangesAsync();

                TempData["success"] = "Request Successfully. You will get mail in a short time";
                return RedirectToPage();
            }catch(Exception c)
            {
                TempData["error"] = "Request failed";
                return RedirectToPage();
            }
        }
    }
}
