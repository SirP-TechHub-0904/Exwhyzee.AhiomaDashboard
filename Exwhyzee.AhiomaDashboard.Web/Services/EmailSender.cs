using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Services
{
   
    public class EmailSender : IEmailSender
    {
       
 private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly IHostingEnvironment _hostingEnv;
        public EmailSender(IHostingEnvironment hostingEnv,
            AhiomaDbContext context, UserManager<IdentityUser> userManager)
        {
            _hostingEnv = hostingEnv;
            _context = context;
            _userManager = userManager;
        }


        // Our private configuration variables
        private string host;
        private int port;
        private bool enableSSL;
        private string userName;
        private string password;

        // Get our parameterized configuration
        public EmailSender(string host, int port, bool enableSSL, string userName, string password)
        {
            this.host = host;
            this.port = port;
            this.enableSSL = enableSSL;
            this.userName = userName;
            this.password = password;
        }

        // Use our configuration to send the email by using SmtpClient
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //var data = ComposeMessage(htmlMessage);
            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = enableSSL
            };
            return client.SendMailAsync(
                new MailMessage(userName, email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }

        public Task SendBulkEmailAsync(string[] emails, string subject, string
      htmlMessage)
        {
            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = enableSSL
            };

            MailMessage mailMessage = new MailMessage();
            mailMessage.Body = htmlMessage;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;

            foreach (string emailAdress in emails)
            {
              
                    mailMessage.To.Add(emailAdress);
            }


            return client.SendMailAsync(mailMessage); ;
        }
        private string ComposeMessage(string message)
        {
            var path = Path.Combine(_hostingEnv.WebRootPath, "Email.html");
            string readText = File.ReadAllText(path);
            using (StreamReader sr = new StreamReader(path))
            {
                String line = sr.ReadToEnd();
                Console.WriteLine(line);
            }
            return readText;
        }
        //{
        //    var path = Path.Combine(_hostingEnv.WebRootPath, "Email/Email.html");
        //    var mailstring = System.IO.File.OpenRead(path);

        //    if (string.IsNullOrEmpty(template))
        //    {
        //        throw new NotSupportedException("Emtpy Template");
        //    }

        //    var splitMessages = message.Split(";??");

        //    if (splitMessages.Length != 4)
        //    {
        //        throw new NotSupportedException("Message Split Index error");
        //    }

        //    var mailTemplate = template
        //        .Replace("{mailHeader}", splitMessages[1])
        //        .Replace("{greeting}", splitMessages[2])
        //        .Replace("{messageBody}", splitMessages[3])
        //        .Replace("{currentYear}", DateTime.Now.Year.ToString());

        //    var message = new MailMessage()
        //    {
        //        Subject = splitMessages[0],
        //        Body = mailTemplate,
        //        IsBodyHtml = true,
        //        From = new MailAddress(_mailSetting.Username),
        //    };

        //    if (messageStore.AddressType == AddressType.Bulk)
        //    {
        //        foreach (var address in messageStore.EmailAddress.Split(','))
        //        {
        //            message.To.Add(address);
        //        }
        //    }
        //    else if (messageStore.AddressType == AddressType.Single)
        //    {
        //        message.To.Add(messageStore.EmailAddress);
        //    }
        //    else
        //    {
        //        throw new NotSupportedException("AddressType");
        //    }

        //    return message;
        //}
        public async void InvokeAsync(string id)
        {
            //string url = HttpContext.Current.Request.Url.AbsoluteUri;
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var item = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
               // string ip = HttpContext.Connection.RemoteIpAddress.ToString();
                string mc = GetMacAddress();
                item.Note = item.Note + "<br>log:" + DateTime.UtcNow + " <ip> nill"  + " <mc> " + mc;
                _context.Attach(item).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

        }
        private string GetMacAddress()
        {
            string macAddresses = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return macAddresses;
        }


    }
}
