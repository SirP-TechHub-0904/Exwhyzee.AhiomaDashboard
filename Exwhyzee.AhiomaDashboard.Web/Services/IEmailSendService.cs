using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Services
{
   public interface IEmailSendService
    {
        Task<bool> SendToOne(string email, string subject, string title, string htmlMessage);
        Task<bool> SMSToOne(string phone, string Message);
        Task<bool> SendToMany(string emails, string subject, string title, string htmlMessage);
        
        Task<bool> NewSendToOne(string email, string subject, MailMessage mail);
      Task<bool> AdminSendToMany(string emails, string subject, string title, string htmlMessage);


    }
}
