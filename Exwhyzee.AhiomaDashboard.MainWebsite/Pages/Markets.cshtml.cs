using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    [AllowAnonymous]
    public class MarketsModel : PageModel
    {

            private readonly AhiomaDbContext _context;
            public MarketsModel(
                AhiomaDbContext context)
            {
                
                _context = context;
            }
        [BindProperty]
        public string CustomerRef { get; set; }

        public IList<Market> Markets { get; set; }
            public async Task OnGetAsync(string customerRef)
            {
            CustomerRef = customerRef;
            Markets = await _context.Markets.ToListAsync();
            }



        }
    }
