using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSendService _emailSender; 
        private readonly IMessageRepository _message;


        public ForgotPasswordConfirmation(UserManager<IdentityUser> userManager, IEmailSendService emailSender, IMessageRepository message)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _message = message;
        }
        [BindProperty]
        public string Email { get; set; }
        public void OnGet(string email)
        {
            TempData["mail"] = email;
            Email = email;
        }


      
        public async Task<IActionResult> OnPostAsync()
        {
            return RedirectToPage();
        }
            public async Task<IActionResult> OnPostResendMail()
            {
                if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    TempData["success"] = "An mail has be sent to the email";
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
                //    Email,
                //    "Reset Password", "Hi",
                //    $" Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                var imEmailContent = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.";


                AddMessageDto i = new AddMessageDto();
                i.Content = imEmailContent;
                i.Recipient = user.Email;
                i.NotificationType = Enums.NotificationType.Email;
                i.NotificationStatus = Enums.NotificationStatus.NotSent;
                i.Retries = 0;
                i.Title = "Forgot Password";
                var stsa = await _message.AddMessage(i);


                TempData["success"] = "An mail has be sent to the email";
                ViewData["mail"] = Email;
                return RedirectToPage("./ForgotPasswordConfirmation", new { email = Email });
            }

            return Page();
        }
    }
}
