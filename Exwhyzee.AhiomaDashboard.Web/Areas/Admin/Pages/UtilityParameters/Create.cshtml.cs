using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.UtilityParameters
{
    [Authorize(Roles = "mSuperAdmin")]

    public class CreateModel : PageModel
    {
        
        private readonly AhiomaDbContext _context;
       
        public CreateModel(AhiomaDbContext context)
        {
            _context = context;
        }


        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public UtilityParameter Data { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            
            _context.UtilityParameters.Add(Data);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
