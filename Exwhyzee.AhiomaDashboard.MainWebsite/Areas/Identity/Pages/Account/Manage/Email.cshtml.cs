using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using System.IO;
using System.Net.Mail;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Microsoft.AspNetCore.Hosting;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Identity.Pages.Account.Manage
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public partial class EmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSendService _emailSender;
        private readonly IUserProfileRepository _account;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IMessageRepository _message;
        private readonly AhiomaDbContext _context;

        public EmailModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSendService emailSender, IUserProfileRepository account, IHostingEnvironment hostingEnv, IMessageRepository message, AhiomaDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _account = account;
            _hostingEnv = hostingEnv;
            _message = message;
            _context = context;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            //[Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { userId = userId, email = Input.NewEmail, code = code },
                    protocol: Request.Scheme);
                await _emailSender.SendToOne(
                    Input.NewEmail,
                    "Confirm your email", "Hi",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                StatusMessage = "Confirmation link to change email sent. Please check your email.";
                return RedirectToPage();
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            try
            {
                var profileupdate = await _context.UserProfiles.FirstOrDefaultAsync(x=>x.UserId == user.Id);
                profileupdate.TokenAccount = code;
                _context.Attach(profileupdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception d) { }
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code, returnUrl = "/" },
                protocol: Request.Scheme);

            var ahiapayUrl = Url.Page(
               "/Account/Index",
               pageHandler: null,
               values: new { area = "User" },
               protocol: Request.Scheme);
            MailMessage mail = new MailMessage();
            try
            {
                StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "NewCustomer.html"));
                //create the mail message 
               

                string links = $" <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Mail Confirmation Link</a>.";
                string mailmsg = sr.ReadToEnd();
                mailmsg = mailmsg.Replace("{customer}", user.Email);
                mailmsg = mailmsg.Replace("{link}", links);
                mailmsg = mailmsg.Replace("{linkhref}", callbackUrl);
                mailmsg = mailmsg.Replace("{ahiapay}", ahiapayUrl);

                mail.Body = mailmsg;
                sr.Close();

                AddMessageDto i = new AddMessageDto();
                i.Content = mailmsg;
                i.Recipient = user.Email;
                i.NotificationType = Enums.NotificationType.Email;
                i.NotificationStatus = Enums.NotificationStatus.NotSent;
                i.Retries = 0;
                i.Title = "Account Confirmation";
                var stsa = await _message.AddMessage(i);
            }
            catch (Exception c) { }

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
        }
    }
}
