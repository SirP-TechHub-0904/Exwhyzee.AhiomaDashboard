using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    public class FAQsModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public FAQsModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        public IList<FaqQuestion> FaqQuestions { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef, string opexyz)
        {
            CustomerRef = customerRef;
            FaqQuestions = await _context.FaqQuestions.Where(m => m.Publish == true).ToListAsync();

            if (FaqQuestions == null)
            {
                return RedirectToPage("./PageNotFound");
            }
            return Page();
        }
    }
}