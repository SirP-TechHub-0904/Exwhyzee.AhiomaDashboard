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
    public class StateMarketsModel : PageModel
    {

        private readonly AhiomaDbContext _context;
        public StateMarketsModel(
            AhiomaDbContext context)
        {

            _context = context;
        }


        public IList<Market> Markets { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task OnGetAsync(string customerRef, string state, string Market)
        {
            CustomerRef = customerRef;
            TempData["mkt"] = Market;
            Markets = await _context.Markets.Where(x => x.State == state).ToListAsync();
        }



    }
}
