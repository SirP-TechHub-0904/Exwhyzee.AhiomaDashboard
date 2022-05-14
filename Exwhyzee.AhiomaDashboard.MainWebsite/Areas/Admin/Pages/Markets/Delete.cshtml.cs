using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Microsoft.AspNetCore.Authorization;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Markets
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class DeleteModel : PageModel
    {
        private readonly IMarketRepository _context;

        public DeleteModel(IMarketRepository context)
        {
            _context = context;
        }

        [BindProperty]
        public Market Market { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Market = await _context.GetById(id);

            if (Market == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _context.Delete(id);

          
            return RedirectToPage("./Index");
        }
    }
}
