using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.ManageAccounts
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare,SubAdmin")]

    public class UpdateShopStatusModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IMessageRepository _message;

        public UpdateShopStatusModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,

            IUserProfileRepository account, AhiomaDbContext context,
            IEmailSendService emailSender, IHostingEnvironment hostingEnv, IMessageRepository message)
        {
            _userManager = userManager;
            _context = context;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _hostingEnv = hostingEnv;
            _message = message;
        }
        [BindProperty]
        public string UserID { get; set; }
        [BindProperty]
        public TenantEnum AccountStatus { get; set; }
        public string Note { get; set; }
        public string ENote { get; set; }
        public Tenant Profile { get; set; }
        public async Task<IActionResult> OnGetAsync(string uid)
        {
            Profile = await _context.Tenants.FirstOrDefaultAsync(x => x.UserId == uid);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string iUserId = _userManager.GetUserId(HttpContext.User);
            var profile = await _context.Tenants.FirstOrDefaultAsync(x => x.UserId == UserID);
            profile.TenantStatus = AccountStatus;
           
            //profile.LastAdminUpdated = DateTime.UtcNow.AddHours(1);
            //profile.ApprovedBy = iUserId;
            _context.Attach(profile).State = EntityState.Modified;
            string note = "";
            if(profile.TenantStatus == TenantEnum.Enable)
            {
                note = "Enabled";
            }
            else if (profile.TenantStatus == TenantEnum.Suspend)
            {
                note = "Suspended";
            }
            else if (profile.TenantStatus == TenantEnum.Disable)
            {
                note = "Disabled";
            }
           
            try
            {
                await _context.SaveChangesAsync();

               

            }
            catch (DbUpdateConcurrencyException)
            {
              
                    throw;
                
            }
            TempData["success"] = "success";
           // Profile = await _account.GetByUserId(profile.UserId);
            return RedirectToPage("./UpdateShopStatus", new { uid = profile.UserId });
        }

    }
}
