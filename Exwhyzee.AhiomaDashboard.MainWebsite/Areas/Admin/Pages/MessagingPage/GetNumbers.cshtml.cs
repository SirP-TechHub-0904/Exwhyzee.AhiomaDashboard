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
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.MessagingPage
{
    public class GetNumbersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AhiomaDbContext _context;


        public GetNumbersModel(
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
        public string Role { get; set; }
        public async Task<IActionResult> OnGet()
        {
            try
            {
                var getApi = "http://account.kudisms.net/api/?username=peterahioma2020@gmail.com&password=nation@123&action=balance";
                

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(getApi);
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Timeout = 25000;

                //getting the respounce from the request
                HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                string response = await streamReader.ReadToEndAsync();
                ////response = response.Remove(0, 11);
                //// response = response.Substring(0, 5);
                ///string inputStr =  "($23.01)";      
                response = Regex.Match(response, @"\d+.+\d").Value;
                response = response.Substring(0, response.IndexOf(','));

                TempData["balance"] = response;

            }catch(Exception d)
            { 
            
            }
            return Page();
        }

        [BindProperty]
        public Messaging Messaging { get; set; }

        public IList<UserProfile> Profile { get; set; }
        [BindProperty]
        public string Numbers { get; set; }
        [BindProperty]

        public string Title { get; set; }
        [BindProperty]

        public string Count { get; set; }
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if(Role == "All SOA")
                {
                    IQueryable<UserProfile> profileIQ = from s in _context.UserProfiles
                                               .Include(p => p.User)
                                                .Where(x => x.Roles.Contains("SOA"))
                                                    select s;
                    var itemContacts = profileIQ.Select(x => x.User.PhoneNumber);
                    int c1 = profileIQ.Count();
                    int cs1 = itemContacts.Count();


                    Numbers = string.Join(",", itemContacts.ToList());
                    Numbers = Numbers.Replace(" ", "");
                    Title = "All SOA";
                    Count = profileIQ.Count().ToString();
                }
                else if (Role == "Active SOA")
                {

                    IQueryable<UserProfile> profileIQ = from s in _context.UserProfiles
                                             .Include(p => p.User)
                                              .Where(x => x.Status == Enums.AccountStatus.Active && x.Roles.Contains("SOA"))
                                                        select s;
                    var itemContacts = profileIQ.Select(x => x.User.PhoneNumber);
                    int c1 = profileIQ.Count();
                    int cs1 = itemContacts.Count();


                    Numbers = string.Join(",", itemContacts.ToList());
                    Numbers = Numbers.Replace(" ", "");
                    Title = "Active SOA";
                    Count = profileIQ.Count().ToString();
                }
                else if (Role == "Non Active SOA")
                {

                    IQueryable<UserProfile> profileIQ = from s in _context.UserProfiles
                                             .Include(p => p.User)
                                              .Where(x => x.Status != Enums.AccountStatus.Active && x.Roles.Contains("SOA"))
                                                        select s;
                    var itemContacts = profileIQ.Select(x => x.User.PhoneNumber);
                    int c1 = profileIQ.Count();
                    int cs1 = itemContacts.Count();


                    Numbers = string.Join(",", itemContacts.ToList());
                    Numbers = Numbers.Replace(" ", "");
                    Title = "Non Active SOA";
                    Count = profileIQ.Count().ToString();
                }
                else if (Role == "All Shop")
                {


                    IQueryable<Tenant> profileIQ = from s in _context.Tenants
                                            .Include(p => p.User).Include(x => x.UserProfile)
                                            
                                                        select s;
                    var itemContacts = profileIQ.Select(x => x.User.PhoneNumber);
                    int c1 = profileIQ.Count();
                    int cs1 = itemContacts.Count();


                    Numbers = string.Join(",", itemContacts.ToList());
                    Numbers = Numbers.Replace(" ", "");
                    Title = "All Shops";
                    Count = profileIQ.Count().ToString();
                }
                else if (Role == "Active Shop")
                {
                    IQueryable<Tenant> profileIQ = from s in _context.Tenants
                                           .Include(p => p.User).Include(x => x.UserProfile)
                                           .Where(x => x.TenantStatus == Enums.TenantEnum.Enable)
                                                   select s;
                    var itemContacts = profileIQ.Select(x => x.User.PhoneNumber);
                    int c1 = profileIQ.Count();
                    int cs1 = itemContacts.Count();


                    Numbers = string.Join(",", itemContacts.ToList());
                    Numbers = Numbers.Replace(" ", "");
                    Title = "Active Shop";
                    Count = profileIQ.Count().ToString();
                }
                else if (Role == "Non Active Shop")
                {
                    IQueryable<Tenant> profileIQ = from s in _context.Tenants
                                          .Include(p => p.User).Include(x => x.UserProfile)
                                          .Where(x => x.TenantStatus != Enums.TenantEnum.Enable)
                                                   select s;
                    var itemContacts = profileIQ.Select(x => x.User.PhoneNumber);
                    int c1 = profileIQ.Count();
                    int cs1 = itemContacts.Count();


                    Numbers = string.Join(",", itemContacts.ToList());
                    Numbers = Numbers.Replace(" ", "");
                    Title = "Non Active Shop";
                    Count = profileIQ.Count().ToString();
                }
                else if (Role == "All Customer")
                {
                    IQueryable<UserProfile> profileIQ = from s in _context.UserProfiles
                                           .Include(p => p.User)
                                            .Where(x => x.Roles == "Customer")
                                                        select s;
                    var itemContacts = profileIQ.Select(x => x.User.PhoneNumber);
                    int c1 = profileIQ.Count();
                    int cs1 = itemContacts.Count();


                    Numbers = string.Join(",", itemContacts.ToList());
                    Numbers = Numbers.Replace(" ", "");
                    Title = "All Customer";
                    Count = profileIQ.Count().ToString();
                }
               
                else if (Role == "All Logistics")
                {
                    IQueryable<LogisticProfile> profileIQ = from s in _context.LogisticProfiles
                                      .Include(p => p.User).Include(x => x.UserProfile)
                                                        select s;
                    var itemContacts = profileIQ.Select(x => x.User.PhoneNumber);
                    int c1 = profileIQ.Count();
                    int cs1 = itemContacts.Count();


                    Numbers = string.Join(",", itemContacts.ToList());
                    Numbers = Numbers.Replace(" ", "");
                    Title = "All Logistics";
                    Count = profileIQ.Count().ToString();
                }
                else if (Role == "All")
                {
                    IQueryable<UserProfile> profileIQ = from s in _context.UserProfiles
                                           .Include(p => p.User)
                                                        select s;
                    var itemContacts = profileIQ.Select(x => x.User.PhoneNumber);
                    int c1 = profileIQ.Count();
                    int cs1 = itemContacts.Count();


                    Numbers = string.Join(",", itemContacts.ToList());
                    Numbers = Numbers.Replace(" ", "");
                    Title = "All Users";
                    Count = profileIQ.Count().ToString();
                }

                Messaging.MassageType = Enums.MessageStatus.SMS;
                Messaging.Contacts = Numbers;
                Messaging.Title = Title;
                Messaging.Count = Count;
                Messaging.MassageType = Enums.MessageStatus.SMS;
                Messaging.Date = DateTime.UtcNow.AddHours(1);
                _context.Messagings.Add(Messaging);
                await _context.SaveChangesAsync();
                return RedirectToPage("./ComposeMessage", new { id = Messaging.Id });
            }
            catch(Exception v)
            {
                return Page();
            }

            
        }
    }
}
