using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.MainWebsite.Services;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
     [AllowAnonymous]
    public class NewshopModel : PageModel
    {

        private readonly IMessageRepository _message;

        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly IEmailSendService _emailSender;


        public NewshopModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, IEmailSendService emailSender, IMessageRepository message)
        {
            _context = context;
            _emailSender = emailSender;
            _message = message;
        }


        [BindProperty]
        public SoaRequest Input { get; set; }


        public string CustomerRef { get; set; }
        public async Task OnGetAsync()
        {



        }
        public async Task<IActionResult> OnPostAsync()
        {

            if (ModelState.IsValid)
            {
                Input.Date = DateTime.UtcNow.AddHours(1);
                _context.SoaRequests.Add(Input);
                await _context.SaveChangesAsync(); 
                 TempData["success"] = "Request have been submited. You will contacted within 24 hours";
                await _emailSender.SendToOne(Input.Email, "Shop Registration Request", "Hi",
                     $" We have received your request. you will be contacted soon <a href=''>Konw more about ahioma FAQ (Frequently Asked Questions)</a>.");
                ////email
                string mEmailContent = "";
                try
                {
                    mEmailContent = await _message.GetMessage(Enums.ContentType.SoaRequestEmail);
                }
                catch (Exception c) { }


                ////update content Data
                mEmailContent = mEmailContent.Replace("|Date|", Input.Date.ToString("dd/MM/yyyy hh:mm tt"));
                mEmailContent = mEmailContent.Replace("|Surname|", Input.FullName);
                mEmailContent = mEmailContent.Replace("|BusinessName|", Input.BusinessName);
                mEmailContent = mEmailContent.Replace("|Description|", "We have received your request. you will be contacted soon");


                AddMessageDto email = new AddMessageDto();
                email.Content = mEmailContent;
                email.Recipient = "ahiomaad@gmail.com";
                email.NotificationType = Enums.NotificationType.Email;
                email.NotificationStatus = Enums.NotificationStatus.NotSent;
                email.Retries = 0;
                email.Title = "SOA Request";

                var sts = await _message.AddMessage(email);

                //
                AddMessageDto email2 = new AddMessageDto();
                email2.Content = mEmailContent;
                email2.Recipient = "Felixobagha@yahoo.com";
                email2.NotificationType = Enums.NotificationType.Email;
                email2.NotificationStatus = Enums.NotificationStatus.NotSent;
                email2.Retries = 0;
                email2.Title = "SOA Request";

               await _message.AddMessage(email2);

                //
                AddMessageDto email3 = new AddMessageDto();
                email3.Content = mEmailContent;
                email3.Recipient = "ahiomaad@gmail.com";
                email3.NotificationType = Enums.NotificationType.Email;
                email3.NotificationStatus = Enums.NotificationStatus.NotSent;
                email3.Retries = 0;
                email3.Title = "SOA Request";

                await _message.AddMessage(email3);
                //
                AddMessageDto email4 = new AddMessageDto();
                email4.Content = mEmailContent;
                email4.Recipient = "peterahioma2020@gmail.com";
                email4.NotificationType = Enums.NotificationType.Email;
                email4.NotificationStatus = Enums.NotificationStatus.NotSent;
                email4.Retries = 0;
                email4.Title = "SOA Request";

                await _message.AddMessage(email4);

                ////email
                string mSmsContent = "";
                try
                {
                    mSmsContent = await _message.GetMessage(Enums.ContentType.SoaRequestSms);
                }
                catch (Exception c) { }

                //update content Data
                mSmsContent = mSmsContent.Replace("|Name|", Input.FullName);
                mSmsContent = mSmsContent.Replace("|Date|", Input.Date.ToString("dd/MM/yyyy hh:mm tt"));
                mSmsContent = mSmsContent.Replace("|||", "\r\n");

                AddMessageDto adminsms = new AddMessageDto();
                adminsms.Content = "Admin Notic - " + mSmsContent;
                adminsms.Recipient = "07060530000,08165529721";
                adminsms.NotificationType = Enums.NotificationType.SMS;
                adminsms.NotificationStatus = Enums.NotificationStatus.NotSent;
                adminsms.Retries = 0;
                adminsms.Title = "SOA Request";
                //
                var stsss = await _message.AddMessage(adminsms);


                return RedirectToPage("./Success");
            }
            else
            {
                // If we got this far, something failed, redisplay form
                return Page();
            }
        }
    }
}