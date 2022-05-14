using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.Net;
using System.IO;
using System.Web;
using System.Net.Mail;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.MessagingPage
{
    public class ComposeMailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;

        private readonly IHostingEnvironment _hostingEnv;



        public ComposeMailModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager, IHostingEnvironment hostingEnv, AhiomaDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _hostingEnv = hostingEnv;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public Messaging Messaging { get; set; }
        [BindProperty]
        public IList<MailContent> MailContents { get; set; }
        [BindProperty]
        public IList<MailProduct> MailProducts { get; set; }
        [BindProperty]
        public string NewEmails { get; set; }

        [BindProperty]
        public long MessageId { get; set; }

        [BindProperty]
        public bool SendTest { get; set; }
        public async Task<IActionResult> OnGet(long id)
        {
            MessageId = id;
            Messaging = await _context.Messagings.Include(x => x.MailProducts).Include(x => x.MailContents).FirstOrDefaultAsync(x => x.Id == id);
            MailProducts = await _context.MailProducts.Include(x => x.Product).Where(x => x.MessagingId == id).ToListAsync();
            MailContents = await _context.MailContents.Where(x => x.MessagingId == id).ToListAsync();
            return Page();
        }


        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            string error = "";
            try
            {
                IList<string> emails = null;
                if (!string.IsNullOrEmpty(Messaging.Contacts))
                {
                    emails = Messaging.Contacts.Split(new string[] { "\r\n", " " }, StringSplitOptions.RemoveEmptyEntries);

                }
                IList<string> Moreemails = null;
                if (NewEmails != null)
                {
                    NewEmails = NewEmails.Replace("\r\n", ",");
                    if (!string.IsNullOrEmpty(NewEmails))
                    {
                        Moreemails = NewEmails.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);

                    }
                }
                IList<string> allemails = null;
                if (SendTest != true)
                {
                    var emailsplit = emails.Distinct().ToList();
                    if (Moreemails != null)
                    {
                        var newemailsplit = Moreemails.Distinct().ToList();
                        allemails = emailsplit.Concat(newemailsplit.ToList()).ToList();
                    }
                    else
                    {
                        allemails = emailsplit.ToList();
                    }
                }
                else
                {
                    var emailsplit = emails.Distinct().ToList();
                    var newemailsplit = Moreemails.Distinct().ToList();
                    allemails = newemailsplit.ToList();
                }
                if (allemails == null || allemails.Count() < 1)
                {
                    TempData["error"] = "Add emails";
                    return RedirectToPage("./ComposeMail", new { id = MailContent.MessagingId });
                }
                Messaging.Count = allemails.Count().ToString();
                foreach (var email in allemails)
                {
                    BulkMailList im = new BulkMailList();
                    im.Subject = Messaging.Subject;
                    im.Date = DateTime.UtcNow.AddHours(1);
                    im.Email = email;
                    im.Sent = false;
                    im.Title = Messaging.Title;
                     


                    string response = "";

                    try
                    {
                        MailMessage mail = new MailMessage();

                        var Content = await _context.MailContents.Where(x => x.MessagingId == Messaging.Id).ToListAsync();
                        string mailmsg = Messaging.Message;
                        foreach (var msg in Content)
                        {
                            string newMainString = "";
                            try
                            {
                                if (msg.NewString == "Firstname")
                                {

                                    var profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == email);
                                    if (profile != null)
                                    {
                                        newMainString = profile.FirstName;
                                    }
                                }
                                else if (msg.NewString == "Surname")
                                {

                                    var profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == email);
                                    if (profile != null)
                                    {
                                        newMainString = profile.Surname;
                                    }
                                }
                                else if (msg.NewString == "Othername")
                                {

                                    var profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == email);
                                    if (profile != null)
                                    {
                                        newMainString = profile.OtherNames;
                                    }
                                }
                                else if (msg.NewString == "Fullname")
                                {

                                    var profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == email);
                                    if (profile != null)
                                    {
                                        newMainString = profile.Fullname;
                                    }
                                }
                                else if (msg.NewString == "DOB")
                                {

                                    var profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Email == email);
                                    if (profile != null)
                                    {
                                        newMainString = profile.DOB.ToString();
                                    }
                                }
                                else if (msg.NewString == "BusinessName")
                                {

                                    var profile = await _context.Tenants.Include(x => x.User).Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.User.Email == email);
                                    if (profile != null)
                                    {
                                        newMainString = profile.BusinessName;
                                    }
                                }
                                else if (msg.NewString == "SOAName")
                                {

                                    var profile = await _context.Tenants.Include(x => x.User).Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.User.Email == email);
                                    if (profile != null)
                                    {
                                        var soa = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Id == profile.CreationUserId);
                                        if (soa != null)
                                        {
                                            newMainString = soa.Fullname;
                                        }
                                    }
                                }
                                else if (msg.NewString == "SOAPhone")
                                {

                                    var profile = await _context.Tenants.Include(x => x.User).Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.User.Email == email);
                                    if (profile != null)
                                    {
                                        var soa = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.User.Id == profile.CreationUserId);
                                        if (soa != null)
                                        {
                                            newMainString = soa.User.PhoneNumber;
                                        }
                                    }
                                }

                            }
                            catch (Exception f)
                            {
                                error = error + "<br>" + f.InnerException.Message.ToString();
                            }
                            mailmsg = mailmsg.Replace(msg.OldString, newMainString);
                        }
                        string startcard = "<div style=\"margin-right: -15px; margin-left: -15px;\">";
                        string endcard = "</div>";
                        string card = "";
                        string allcard = "";
                        string productstring = "";
                        var Productss = await _context.MailProducts.Where(x => x.MessagingId == Messaging.Id).ToListAsync();

                        foreach (var msg in Productss)
                        {
                            string newMainString = "";
                            try
                            {
                                //*IMAGE* *PRODUCTNAME* *AMOUNT**SHOPLINK**PRODUCTLINK*
                                var product = await _context.Products.Include(x => x.Tenant).Include(x => x.ProductPictures).FirstOrDefaultAsync(x => x.Id == msg.ProductId);
                                if (product != null)
                                {
                                    //                                                                <a asp-page="/Info/ShopPage" asp-route-customerRef="" asp-route-name="@data.Tenant.TenentHandle">

                                    var home = Url.Page(
                     "/Index",
                     pageHandler: null,
                     values: new { area = "" },
                     protocol: Request.Scheme);

                                    var link = Url.Page(
                     "/ProductDetails",
                     pageHandler: null,
                     values: new { area = "", id = product.Id, name = product.Name, shop = product.Tenant.BusinessName },
                     protocol: Request.Scheme);

                                    var shoplink = Url.Page(
                     "/Info/ShopPage",
                     pageHandler: null,
                     values: new { area = "", name = product.Tenant.TenentHandle },
                     protocol: Request.Scheme);

                                    card = "<div style=\"position: relative; min-height: 1px; padding-right: 15px; padding-left: 15px;float: left;width: 40%;max-height: 400px;margin-bottom:10px;\"> <div class=\"card\" style=\" box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2); transition: 0.3s; width: 100%;\"><a href=\"*PRODUCTLINK*\"><img src=\"*IMAGE*\" alt=\"\" style=\"width:100%;\"></a><div class=\"\" style=\"padding: 2px 16px;min-height: 90px;\"><a href=\"*PRODUCTLINK*\"><h6 style=\"padding:2px;margin:1px;text-align:center;\"><b>*PRODUCTNAME*</b></h6></a><p style=\"padding:2px;margin:1px;text-align:center;\">&#8358;*AMOUNT*</p><a href=\"*SHOPLINK*\"><h6 style=\"padding:2px;margin:1px;text-align:center;\"><b>*SHOPNAME*</b></h6></a></div></div></div>";
                                    card = card.Replace("*IMAGE*", home + product.ProductPictures.FirstOrDefault().PictureUrlThumbnail);
                                    card = card.Replace("*PRODUCTNAME*", product.Name);
                                    card = card.Replace("*AMOUNT*", product.Price.ToString());
                                    card = card.Replace("*SHOPLINK*", shoplink);
                                    card = card.Replace("*PRODUCTLINK*", link);
                                    card = card.Replace("*SHOPNAME*", product.Tenant.BusinessName);
                                    //
                                    allcard = allcard + card;
                                }
                            }
                            catch (Exception f)
                            {
                                error = error + "<br>" + f.InnerException.Message.ToString();
                            }

                        }

                        if (!String.IsNullOrEmpty(allcard))
                        {
                            productstring = startcard + allcard + endcard;
                        }
                        mailmsg = mailmsg.Replace("{Products}", productstring);
                        mail.Body = mailmsg;
                        try
                        {
                            string xapiurl = $"http://notify.ahioma.com/api/Messages/addmail";
                            string xapiUrl = String.Format(xapiurl);
                            WebRequest xrequestObj = WebRequest.Create(xapiUrl);
                            xrequestObj.Method = "POST";
                            xrequestObj.ContentType = "application/json";
                            using (var streamWriter = new StreamWriter(xrequestObj.GetRequestStream()))
                            {
                                string jsonModel = Newtonsoft.Json.JsonConvert.SerializeObject(im);
                                streamWriter.Write(jsonModel);
                                streamWriter.Flush();
                                streamWriter.Close();
                            }
                            HttpWebResponse xresponse = (HttpWebResponse)xrequestObj.GetResponse();

                            Stream responseStream = xresponse.GetResponseStream();
                            error = error +im.Email +" added <br> ";
                        }
                        catch (Exception f)
                        {
                            error = error + im.Email + " fail <br> ";
                        }
                        //set the addresses 

                    }
                    catch (Exception ex)
                    {
                       
                    }
                    
                }
                Messaging.Status = error;
                _context.Attach(Messaging).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["success"] = "Submitted all " + error;
                return RedirectToPage("./StatusPage");
            }
            catch (Exception v)
            {
                TempData["success"] = error;
                return RedirectToPage("./ComposeMail", new { id = Messaging.Id });
            }

        }

        [BindProperty]
        public MailContent MailContent { get; set; }
        public async Task<IActionResult> OnPostMailContent()
        {
            try
            {
                _context.MailContents.Add(MailContent);
                await _context.SaveChangesAsync();
                TempData["success"] = "Successful";
                return RedirectToPage("./ComposeMail", new { id = MailContent.MessagingId });
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage("./ComposeMail", new { id = MailContent.MessagingId });
            }
        }

        [BindProperty]
        public MailProduct MailProduct { get; set; }
        public async Task<IActionResult> OnPostMailProduct()
        {
            try
            {
                _context.MailProducts.Add(MailProduct);
                await _context.SaveChangesAsync();
                TempData["success"] = "Successful";
                return RedirectToPage("./ComposeMail", new { id = MailProduct.MessagingId });
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage("./ComposeMail", new { id = MailProduct.MessagingId });
            }
        }


        [BindProperty]
        public long DeleteId { get; set; }
        public async Task<IActionResult> OnPostRemoveMailContent()
        {
            try
            {
                var data = await _context.MailContents.FindAsync(DeleteId);
                _context.MailContents.Remove(data);
                await _context.SaveChangesAsync();
                return RedirectToPage("./ComposeMail", new { id = MailContent.MessagingId });
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage("./ComposeMail", new { id = MailContent.MessagingId });
            }
        }

        public async Task<IActionResult> OnPostRemoveMailProduct()
        {
            try
            {
                var data = await _context.MailProducts.FindAsync(DeleteId);
                _context.MailProducts.Remove(data);
                await _context.SaveChangesAsync();
                return RedirectToPage("./ComposeMail", new { id = MailProduct.MessagingId });
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage("./ComposeMail", new { id = MailProduct.MessagingId });
            }
        }
    }

}
