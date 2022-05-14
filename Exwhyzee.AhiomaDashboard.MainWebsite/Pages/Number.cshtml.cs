using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    public class NumberModel : PageModel
    {

        private readonly AhiomaDbContext _context;

     

        public NumberModel(AhiomaDbContext context)
        {
            _context = context;
        }

        public IQueryable<UserProfile> iUsers { get; set; }
       
        public async Task<IActionResult> OnGetAsync()
        {
            

            IQueryable<UserProfile> productIQ = from s in _context.UserProfiles.Include(x=>x.User)
                                                select s;
            var iUserss = productIQ.Count();
            iUsers = productIQ;
            return Page();
        }
       
    }

}
