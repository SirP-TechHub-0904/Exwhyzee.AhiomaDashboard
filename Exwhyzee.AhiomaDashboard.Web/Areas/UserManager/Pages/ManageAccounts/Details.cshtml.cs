using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Authorization;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using System.Net.Mail;
using System.IO;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.UserManager.Pages.Users
{
    [Authorize(Roles = "UserManager,mSuperAdmin,CustomerCare,SubAdmin")]
    public class DetailsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;
        private readonly IMessageRepository _message;
        private readonly IHostingEnvironment _hostingEnv;

        //private readonly IEmailSender _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IMarketRepository _market;
        private readonly ITenantRepository _tenant;

        public DetailsModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IMarketRepository market, ITenantRepository tenant,
            IUserProfileRepository account, AhiomaDbContext context
, IMessageRepository message
, IHostingEnvironment hostingEnv
/*IEmailSender emailSender*/)
        {
            _userManager = userManager;
            _context = context;
            _account = account;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _market = market;
            _tenant = tenant;
            _message = message;
            _hostingEnv = hostingEnv;
        }


        public UserProfile UserProfile { get; set; }
        public Wallet Wallet { get; set; }
        public Tenant Tenant { get; set; }
        public IList<IdentityRole> Roles { get; set; }

        [BindProperty]
        public string Uid { get; set; }
        [BindProperty]
        public string RUid { get; set; }

        public async Task<IActionResult> OnGetAsync(string uid)
        {
            if (uid == null)
            {
                return NotFound();
            }
            Uid = uid;
            UserProfile = await _context.UserProfiles.Include(x => x.User).Include(x => x.UserAddresses).Include(x => x.UserProfileSocialMedias).Include(c => c.UserReferees).FirstOrDefaultAsync(m => m.UserId == uid);

            if (UserProfile == null)
            {
                return NotFound();
            }

            Wallet = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == UserProfile.UserId);
            var soarole = await _userManager.IsInRoleAsync(UserProfile.User, "SOA");
            var storerole = await _userManager.IsInRoleAsync(UserProfile.User, "Store");

            if (soarole.Equals(true))
            {
                TempData["soa"] = "true";
            }

            if (storerole.Equals(true))
            {
                TempData["store"] = "true";
                Tenant = await _tenant.GetByLogin(UserProfile.UserId);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDisableDeposit()
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == RUid);
                if (profile.DisableDeposit == true)
                {
                    profile.DisableDeposit = false;
                }
                else
                {
                    profile.DisableDeposit = true;
                }

                _context.Entry(profile).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception d)
            {
            }
            return RedirectToPage("./Details", new { uid = RUid });

        }

        public async Task<IActionResult> OnPostEnableAdvert()
        {
            try
            {
                var profile = await _context.UserProfiles.FindAsync(RUid);
                if (profile.ActivateForAdvert == true)
                {
                    profile.ActivateForAdvert = false;
                }
                else
                {
                    profile.ActivateForAdvert = true;
                }

                _context.Entry(profile).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception d)
            {
            }
            return RedirectToPage("./Details", new { uid = RUid });

        }

        public async Task<IActionResult> OnPostDisableAhiaPay()
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == RUid);
                if (profile.DisableAhiaPay == true)
                {
                    profile.DisableAhiaPay = false;
                }
                else
                {
                    profile.DisableAhiaPay = true;
                }

                _context.Entry(profile).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception d)
            {
            }
            return RedirectToPage("./Details", new { uid = RUid });

        }
        public async Task<IActionResult> OnPostDisableBankTransfer()
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == RUid);
                if (profile.DisableBankTransfer == true)
                {
                    profile.DisableBankTransfer = false;
                }
                else
                {
                    profile.DisableBankTransfer = true;
                }

                _context.Entry(profile).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception d)
            {
            }
            return RedirectToPage("./Details", new { uid = RUid });

        }
        public async Task<IActionResult> OnPostDisableAhiaPayTransfer()
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == RUid);
                if (profile.DisableAhiaPayTransfer == true)
                {
                    profile.DisableAhiaPayTransfer = false;
                }
                else
                {
                    profile.DisableAhiaPayTransfer = true;
                }

                _context.Entry(profile).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception d)
            {
            }
            return RedirectToPage("./Details", new { uid = RUid });

        }
        public async Task<IActionResult> OnPostDisableBuy()
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == RUid);
                if (profile.DisableBuy == true)
                {
                    profile.DisableBuy = false;
                }
                else
                {
                    profile.DisableBuy = true;
                }

                _context.Entry(profile).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception d)
            {
            }
            return RedirectToPage("./Details", new { uid = RUid });

        }
        public async Task<IActionResult> OnPostDisableAds()
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == RUid);
                if (profile.DisableAdsCrediting == true)
                {
                    profile.DisableAdsCrediting = false;
                }
                else
                {
                    profile.DisableAdsCrediting = true;
                }

                _context.Entry(profile).State = EntityState.Modified;
                await _context.SaveChangesAsync();

            }
            catch (Exception d)
            {
            }
            return RedirectToPage("./Details", new { uid = RUid });

        }

        [BindProperty]
        public string UserEmail { get; set; }

        [BindProperty]
        [Required]
        public string MailSubject { get; set; }
        [Required]
        [BindProperty]
        public string MailContent { get; set; }

        [BindProperty]
        public string FullUserEmail { get; set; }
        [Required]
        [BindProperty]
        public string TitleMail { get; set; }

        public async Task<IActionResult> OnPostSendMail()
        {
            try
            {

                MailMessage mail = new MailMessage();
                StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "Usermail.html"));
                //create the mail message 


                string mailmsg = sr.ReadToEnd();
                mailmsg = mailmsg.Replace("{customer}", FullUserEmail);
                mailmsg = mailmsg.Replace("{link}", TitleMail);
                mailmsg = mailmsg.Replace("{message}", MailContent);

                mail.Body = mailmsg;
                sr.Close();


                AddMessageDto i = new AddMessageDto();
                i.Content = mail.Body;
                i.Recipient = UserEmail;
                i.NotificationType = Enums.NotificationType.Email;
                i.NotificationStatus = Enums.NotificationStatus.NotSent;
                i.Retries = 0;
                i.Title = MailSubject;
                var stsa = await _message.AddMessage(i);

                TempData["success"] = "An mail has be sent to the email";

            }
            catch (Exception d)
            {
            }
            return RedirectToPage("./Details", new { uid = RUid });

        }

    }



}
