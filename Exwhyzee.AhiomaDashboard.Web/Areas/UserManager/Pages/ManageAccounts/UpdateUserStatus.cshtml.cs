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

    public class UpdateUserStatusModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IMessageRepository _message;

        public UpdateUserStatusModel(
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
        public AccountStatus AccountStatus { get; set; }
        public string Note { get; set; }
        public string ENote { get; set; }
        public UserProfile Profile { get; set; }
        public async Task<IActionResult> OnGetAsync(string uid)
        {
            Profile = await _account.GetByUserId(uid);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string iUserId = _userManager.GetUserId(HttpContext.User);
            var profile = await _account.GetByUserId(UserID);
            profile.Status = AccountStatus;
            profile.Note = Note;
            profile.LastAdminUpdated = DateTime.UtcNow.AddHours(1);
            profile.ApprovedBy = iUserId;
            _context.Attach(profile).State = EntityState.Modified;
            string note = "";
            if(profile.Status == AccountStatus.Active)
            {
                note = "Active";
            }
            else if (profile.Status == AccountStatus.Suspended)
            {
                note = "Suspended";
            }
            else if (profile.Status == AccountStatus.Disabled)
            {
                note = "Disabled";
            }
            else if (profile.Status == AccountStatus.Pending)
            {
                note = "Pending";
            }
            try
            {
                await _context.SaveChangesAsync();

                //await _emailSender.SendToOne(profile.User.Email, "Account Update", "Hi",
                //       $" Your account has been "+note+". Kindly https://ahioma.com to enjoy more features.");
                //await _emailSender.SMSToOne(profile.User.PhoneNumber,
                //      $" Your account has been " + note + ". Kindly https://ahioma.com to enjoy more features.");
                var ahiapayUrl = Url.Page(
                      "/Account/Index",
                      pageHandler: null,
                      values: new { area = "User" },
                      protocol: Request.Scheme);
                //try
                //{
                //    MailMessage mail = new MailMessage();
                //    StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "SOAREF.html"));
               
                //    string mailmsg = sr.ReadToEnd();
                //    mailmsg = mailmsg.Replace("|Fname|", profile.Fullname);
                //    mailmsg = mailmsg.Replace("Congratulations ", "");
                //    mailmsg = mailmsg.Replace("|RegName|", profile.Note);
                //    mailmsg = mailmsg.Replace(" Registered with your Referral Code", "");

                //    mail.Body = mailmsg;
                //    sr.Close();

                //    AddMessageDto i = new AddMessageDto();
                //    i.Content = mailmsg;
                //    i.Recipient = profile.User.Email;
                //    i.NotificationType = Enums.NotificationType.Email;
                //    i.NotificationStatus = Enums.NotificationStatus.NotSent;
                //    i.Retries = 0;
                //    i.Title = "Profile Update";
                //    var stsa = await _message.AddMessage(i);
                //}
                //catch (Exception c) { }

            }
            catch (DbUpdateConcurrencyException)
            {
              
                    throw;
                
            }
            TempData["success"] = "success";
           // Profile = await _account.GetByUserId(profile.UserId);
            return RedirectToPage("./UpdateUserStatus", new { uid = profile.UserId });
        }

    }
}
