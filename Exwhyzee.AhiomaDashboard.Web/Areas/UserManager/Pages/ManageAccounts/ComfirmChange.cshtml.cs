using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.ManageAccounts
{
    [Authorize(Roles = "mSuperAdmin,UserManager")]

    public class ComfirmChangeModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IUserLogging _log;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IMarketRepository _market;
        private readonly IHostingEnvironment _hostingEnv;

        public ComfirmChangeModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserLogging log,
            IHostingEnvironment hostingEnv,
        SignInManager<IdentityUser> signInManager,
            AhiomaDbContext context,
            IMarketRepository market,
            IUserProfileRepository account,
            IEmailSendService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;
            _hostingEnv = hostingEnv;
            _log = log;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _market = market;
        }

    public UserProfile NewUser { get; set; }
    public UserProfile OldUser { get; set; }
    public UserProfile OldPhone { get; set; }
    public UserProfile NewPhone { get; set; }
    public RequestPhoneEmailChange RequestData { get; set; }

        public async Task<ActionResult> OnGetAsync(long id)
        {
            var item = await _context.RequestPhoneEmailChanges.Include(x=>x.Profile).FirstOrDefaultAsync(x => x.Id == id);
            RequestData = item;
            OldUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == item.OldMail);
            NewUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == item.NewMail);
            OldPhone = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.PhoneNumber == item.OldPhone);
            NewPhone = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.PhoneNumber == item.NewPhone);

            return Page();
        }

        [BindProperty]
        public long RequestId { get; set; }
        public async Task<IActionResult> OnPostChangeEmail()
        {
            var item = await _context.RequestPhoneEmailChanges.FirstOrDefaultAsync(x => x.Id == RequestId);

            try
            {
                var user = await _userManager.FindByEmailAsync(item.OldMail);
                //get code
                var ncode = await _userManager.GenerateChangeEmailTokenAsync(user, user.Email);
               var encode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(ncode));


                string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encode));
                try
                {
                    var ch = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
                    ch.Email = item.NewMail;
                    ch.NormalizedEmail = item.NewMail.ToUpper();
                    ch.UserName = item.NewMail;
                    ch.NormalizedUserName = item.NewMail.ToUpper();

                    _context.Entry(item).State = EntityState.Modified;
                 
                    item.EmailStatus = ChangeDataStatus.Successful;
                    _context.Entry(item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    

                    TempData["success"] = "Email Change Successful";
                    return RedirectToPage("./ComfirmChange", new { id = item.Id });
                }
                //var result = await _userManager.ChangeEmailAsync(user, item.NewMail, code);
              catch(Exception c) { 
                    var items = await _context.RequestPhoneEmailChanges.Include(x => x.Profile).FirstOrDefaultAsync(x => x.Id == item.Id);
                    RequestData = item;
                    OldUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == items.OldMail);
                    NewUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == items.NewMail);
                    OldPhone = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.PhoneNumber == items.OldPhone);
                    NewPhone = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.PhoneNumber == items.NewPhone);

                    TempData["error"] = "Error changing email.";
                    return Page();
                }
              

            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage();
            }
        }


       
        public async Task<IActionResult> OnPostChangePhone()
        {
            var item = await _context.RequestPhoneEmailChanges.FirstOrDefaultAsync(x => x.Id == RequestId);

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == item.UserId);
                //get code
                var ncode = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.Email);
                var encrncode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(ncode));
                try
                {
                    var ch = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
                    ch.PhoneNumber = item.NewPhone;
                    _context.Entry(item).State = EntityState.Modified;

                    item.EmailStatus = ChangeDataStatus.Successful;
                    _context.Entry(item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Email Change Successful";
                    return RedirectToPage("./ComfirmChange", new { id = item.Id });
                }
                //var result = await _userManager.ChangeEmailAsync(user, item.NewMail, code);
                catch (Exception c)
                {
                    var items = await _context.RequestPhoneEmailChanges.Include(x => x.Profile).FirstOrDefaultAsync(x => x.Id == item.Id);
                    RequestData = item;
                    OldUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == items.OldMail);
                    NewUser = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == items.NewMail);
                    OldPhone = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.PhoneNumber == items.OldPhone);
                    NewPhone = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.PhoneNumber == items.NewPhone);

                    TempData["error"] = "Error changing phone.";
                    return Page();
                }
               

            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage();
            }
        }

    }
}
