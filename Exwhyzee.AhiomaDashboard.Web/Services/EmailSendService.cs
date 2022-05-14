using Exwhyzee.AhiomaDashboard.Data.Repository.Mesages;
using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Exwhyzee.AhiomaDashboard.Web.Services
{
    public class EmailSendService : IEmailSendService
    {
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IMessageRepository _message;

        public EmailSendService(IHostingEnvironment hostingEnv, IMessageRepository message)
        {
            _hostingEnv = hostingEnv;
            _message = message;
        }
        public async Task<bool> SendToMany(string emails, string subject, string title, string htmlMessage)
        {
            try
            {


                StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "Email.html"));
                //create the mail message 
                MailMessage mail = new MailMessage();

                string mailmsg = sr.ReadToEnd();
                mailmsg = mailmsg.Replace("{Subject}", subject);
                mailmsg = mailmsg.Replace("{Message}", htmlMessage);
                mailmsg = mailmsg.Replace("{Title}", title);
                mail.Body = mailmsg;
                sr.Close();


                //set the addresses 
                mail.From = new MailAddress("noreply@ahioma.com", "Ahioma"); //IMPORTANT: This must be same as your smtp authentication address.

                AddMessageDto sms = new AddMessageDto();
                sms.Content = mailmsg;
                sms.Recipient = emails;
                sms.NotificationType = Enums.NotificationType.Email;
                sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                sms.Retries = 0;
                sms.IsAdmin = true;
                sms.Title = title;

                var stss = await _message.AddMessage(sms);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<bool> SendToOne(string email, string subject, string title, string htmlMessage)
        {
            try
            {


                StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "Email.html"));
                //create the mail message 
                MailMessage mail = new MailMessage();

                string mailmsg = sr.ReadToEnd();
                mailmsg = mailmsg.Replace("{Subject}", subject);
                mailmsg = mailmsg.Replace("{Message}", htmlMessage);
                mailmsg = mailmsg.Replace("{Title}", title);

                mail.Body = mailmsg;
                sr.Close();

                AddMessageDto sms = new AddMessageDto();
                sms.Content = mailmsg;
                sms.Recipient = email;
                sms.NotificationType = Enums.NotificationType.Email;
                sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                sms.Retries = 0;
                sms.Title = title;

                var stss = await _message.AddMessage(sms);
                
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<bool> SMSToOne(string phone, string Message)
        {

            AddMessageDto sms = new AddMessageDto();
            sms.Content = Message;
            sms.Recipient = phone;
            sms.NotificationType = Enums.NotificationType.SMS;
            sms.NotificationStatus = Enums.NotificationStatus.NotSent;
            sms.Retries = 0;
            sms.Title = "SMS";

            var stss = await _message.AddMessage(sms);
            // response = "ok";
            return true;


        }

        public async Task<bool> NewSendToOne(string email, string subject, MailMessage mail)
        {
            try
            {


                //set the addresses 
                mail.From = new MailAddress("noreply@ahioma.com", "Ahioma"); //IMPORTANT: This must be same as your smtp authentication address.
                mail.To.Add(email);

                //set the content 
                mail.Subject = subject;

                mail.IsBodyHtml = true;
                //send the message 
                SmtpClient smtp = new SmtpClient("mail.ahioma.com");

                //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
                NetworkCredential Credentials = new NetworkCredential("noreply@ahioma.com", "Admin@123");
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = Credentials;
                smtp.Port = 25;    //alternative port number is 8889
                smtp.EnableSsl = false;
                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public async Task<bool> AdminSendToMany(string emails, string subject, string title, string htmlMessage)
        {
            try
            {


                StreamReader sr = new StreamReader(System.IO.Path.Combine(_hostingEnv.WebRootPath, "Email.html"));
                //create the mail message 
                MailMessage mail = new MailMessage();

                string mailmsg = sr.ReadToEnd();

                mailmsg = mailmsg.Replace("{Message}", htmlMessage);
                mail.Body = mailmsg;
                sr.Close();


                //set the addresses 
                mail.From = new MailAddress("noreply@ahioma.com", "Ahioma"); //IMPORTANT: This must be same as your smtp authentication address.

                AddMessageDto sms = new AddMessageDto();
                sms.Content = mailmsg;
                sms.Recipient = emails;
                sms.NotificationType = Enums.NotificationType.Email;
                sms.NotificationStatus = Enums.NotificationStatus.NotSent;
                sms.Retries = 0;
                sms.IsAdmin = true;
                sms.Title = title;

                var stss = await _message.AddMessage(sms);
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
