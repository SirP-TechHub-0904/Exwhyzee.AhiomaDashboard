using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Web;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.MessagingPage
{
    public class ComposeMessageModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;


        public ComposeMessageModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager, AhiomaDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public Messaging Messaging { get; set; }
        [BindProperty]
        public string NewNumbers { get; set; }
        public async Task<IActionResult> OnGet(long id)
        {
            Messaging = await _context.Messagings.FirstOrDefaultAsync(x=>x.Id == id);
            return Page();
        }

      
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
string error = "";
            try
            {
                Messaging.Contacts = Messaging.Contacts + "," + NewNumbers;
                Messaging.Contacts = Messaging.Contacts.Replace("\r\n", ",");
                string response = "";
               
                try
                {
                    Messaging.Contacts = Messaging.Contacts.Replace("\r\n", ",");
                    IList<string> inumbers = Messaging.Contacts.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                    double units = inumbers.Count() * 2.8;
                   
                    // var getApiBal = await db.ApiSettings.FirstOrDefaultAsync(x => x.IsDefault == true);
                    string apiSendingbal = "http://account.kudisms.net/api/?username=peterahioma2020@gmail.com&password=nation@123&action=balance"; 

                    HttpWebRequest httpWebRequestBal = (HttpWebRequest)WebRequest.Create(apiSendingbal);
                    httpWebRequestBal.Method = "GET";
                    httpWebRequestBal.ContentType = "application/json";
                    httpWebRequestBal.Timeout = 25000;

                    //getting the respounce from the request
                    HttpWebResponse httpWebResponseBal = (HttpWebResponse)await httpWebRequestBal.GetResponseAsync();
                    Stream responseStreamBal = httpWebResponseBal.GetResponseStream();
                    StreamReader streamReaderBal = new StreamReader(responseStreamBal);
                    string responsebal = await streamReaderBal.ReadToEndAsync();
                    ////response = response.Remove(0, 11);
                    //// response = response.Substring(0, 5);
                    ///string inputStr =  "($23.01)";      
                    responsebal = Regex.Match(responsebal, @"\d+.+\d").Value;
                    responsebal = responsebal.Substring(0, responsebal.IndexOf(','));
                    double AdminBal = Convert.ToDouble(responsebal);
                    if (units > AdminBal)
                    {
                        TempData["error"] = "Unit not sufficient:: Unit::" + AdminBal ;
                        return RedirectToPage("./StatusPage");
                    }


                    var getApi = "http://account.kudisms.net/api/?username=peterahioma2020@gmail.com&password=nation@123&message=@@message@@&sender=@@sender@@&mobiles=@@recipient@@";
                    string apiSending = getApi.Replace("@@sender@@", "Ahioma").Replace("@@recipient@@", HttpUtility.UrlEncode(Messaging.Contacts)).Replace("@@message@@", HttpUtility.UrlEncode(Messaging.Message));

                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(apiSending);
                    httpWebRequest.Method = "GET";
                    httpWebRequest.ContentType = "application/json";

                    //getting the respounce from the request
                    //HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                    //Stream responseStream = httpWebResponse.GetResponseStream();
                    //StreamReader streamReader = new StreamReader(responseStream);
                    //response = await streamReader.ReadToEndAsync();
                    response = "OK";
                }
                catch (Exception c)
                {
                   error = c.InnerException.Message.ToString();
                }

                if (response.ToUpper().Contains("OK") || response.ToUpper().Contains("1701"))
                {
                    Messaging.Status = "Sent error::" + error + " response::" + response;
                }
                else
                {
                    Messaging.Status = "Failed " + error;
                }

                _context.Attach(Messaging).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["success"] = "sent " + error;
                return RedirectToPage("./StatusPage");
            }
            catch (Exception v)
            {
                TempData["success"] = error;
                return RedirectToPage("./ComposeMessage", new { id = Messaging.Id });
            }

        }
    }
}
