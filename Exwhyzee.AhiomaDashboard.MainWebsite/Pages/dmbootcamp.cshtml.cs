using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestSharp;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    public class Bootcamp
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string HowDidYouHearAboutUs { get; set; }
        public string WillYouBeAvailableOnTheTraining { get; set; }
    }
    public class dmbootcampModel : PageModel
    {

        private readonly AhiomaDbContext _context;

        public dmbootcampModel(AhiomaDbContext context)
        {
            _context = context;
            
        }


        public IActionResult OnGet()
        {
            return Page();
        }

       // [BindProperty]
       // public Category Category { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                string apiurl = $"http://notify.ahioma.com/api/v1/Ahioma/Bootcamp";
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");

                var lad = new Bootcamp
                {
                    Fullname = Input.Fullname,
                    PhoneNumber = Input.PhoneNumber,
                    Email = Input.Email,
                    Location = Input.Location,
                    HowDidYouHearAboutUs = Input.HowDidYouHearAboutUs,
                    WillYouBeAvailableOnTheTraining = Input.WillYouBeAvailableOnTheTraining
                };

                request.AddJsonBody(Input);
                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                if (contents.ToString().Contains("Successful"))
                {
                    TempData["msg"] = "Registration Successfull";
                }
                else
                {
                    TempData["msg"] = "Registration Failed";
                }
            }
            catch (Exception g)
            {
                TempData["msg"] = "Registration Failed";
            }

            return Page();
        }
        [BindProperty]
        public InputModel Input { get; set; }


        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string Fullname { get; set; }
            [Required]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Full Address")]
            public string Location { get; set; }

            [Required]
            [Display(Name = "How Did You Hear About Us")]
            public string HowDidYouHearAboutUs { get; set; }

            [Required]
            [Display(Name = "Will You Be Available On The Training")]
            public string WillYouBeAvailableOnTheTraining { get; set; }
        }
    }


}
