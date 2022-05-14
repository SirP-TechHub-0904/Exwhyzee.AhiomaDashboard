using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Info
{
    public class WebpageModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public WebpageModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        public WebPages Pag { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef, string opexyz)
        {
            CustomerRef = customerRef;
            Pag = await _context.WebPages.FirstOrDefaultAsync(m => m.UniqueTitle == opexyz);

            if (Pag == null)
            {
                return RedirectToPage("./PageNotFound");
            }
            return Page();
        }
    }
}