using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using System.IO;
using System.Net.Mail;
using Microsoft.AspNetCore.Hosting;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSendService _emailSender;
        private readonly IMessageRepository _message;
        private readonly IHostingEnvironment _hostingEnv;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSendService emailSender, IMessageRepository message, IHostingEnvironment hostingEnv)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _message = message;
            _hostingEnv = hostingEnv;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
       

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    TempData["success"] = "invalid email";
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                //await _emailSender.SendToOne(
                //    Input.Email,
                //    "Reset Password", "Hi",
                //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

              var imEmailContent = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";
                MailMessage mail = new MailMessage();
                StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "ResetPassword.html"));
                //create the mail message 
                

                string links = $" <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Reset Password</a>.";
                string mailmsg = sr.ReadToEnd();
                mailmsg = mailmsg.Replace("{customer}", user.Email);
                mailmsg = mailmsg.Replace("{link}", links);
                mailmsg = mailmsg.Replace("{linkhref}", callbackUrl);

                mail.Body = mailmsg;
                sr.Close();


                AddMessageDto i = new AddMessageDto();
                i.Content = imEmailContent;
                i.Recipient = user.Email;
                i.NotificationType = Enums.NotificationType.Email;
                i.NotificationStatus = Enums.NotificationStatus.NotSent;
                i.Retries = 0;
                i.Title = "Forgot Password";
                var stsa = await _message.AddMessage(i);

                TempData["success"] = "An mail has be sent to the email";
                ViewData["mail"] = Input.Email;
                return RedirectToPage("./ForgotPasswordConfirmation", new { email = Input.Email });
            }

            return Page();
        }
    }
}
