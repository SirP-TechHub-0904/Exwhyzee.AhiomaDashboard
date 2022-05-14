using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{

    [AllowAnonymous]
    public class SupportModel : PageModel
    {

        private readonly AhiomaDbContext _context;
        public SupportModel(
                        AhiomaDbContext context)
        {

            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }



        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
            public string Subject { get; set; }
            public string PhoneNumber { get; set; }
            public string Message { get; set; }
            public string AhiomaId { get; set; }
            public string Name { get; set; }
        }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task OnGetAsync()
        {
          
        }


        public async Task<IActionResult> OnPostAsync()
        {
            string message = "";
            try
            {

                MailMessage mail = new MailMessage();

                //set the addresses 
                mail.From = new MailAddress("support@ahioma.com"); //IMPORTANT: This must be same as your smtp authentication address.
                mail.To.Add("support@ahioma.com");

                //set the content 

                mail.Subject = "Ahioma Support Mail";

                string mess = "Name: " + Input.Name + ", Email Address: " + Input.Email + ", Phone: " + Input.PhoneNumber + ", Ahioma ID: " + Input.AhiomaId + ", Message: " + Input.Message;
                mail.Body = mess;
                //send the message 
                SmtpClient smtp = new SmtpClient("mail.ahioma.com");

                //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                NetworkCredential Credentials = new NetworkCredential("support@ahioma.com", "Ahioma@247");
                smtp.Credentials = Credentials;
                smtp.Send(mail);
                TempData["mssg"] = message = "Mail Sent Successfull. our Customer Care will get back to you soon";
            }
            catch (Exception ex)
            {

                TempData["mssg"] = message = "Mail not Sent. Try Again.";
            }
            return Page();
        }
    }
}
