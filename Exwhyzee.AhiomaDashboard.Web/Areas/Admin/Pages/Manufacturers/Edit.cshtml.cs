using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.AhiomaDashboard.Data.Repository.Manufacturers;
using Microsoft.AspNetCore.Authorization;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Manufacturers
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class EditModel : PageModel
    {
        private readonly IManufacturerRepository _context;

        public EditModel(IManufacturerRepository context)
        {
            _context = context;
        }

        [BindProperty]
        public Manufacturer Manufacturer { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Manufacturer = await _context.GetById(id);

            if (Manufacturer == null)
            {
                return NotFound();
            }
            return Page();
        }

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
                await _context.Update(Manufacturer);
            }
            catch (DbUpdateConcurrencyException)
            {
                
                    throw;
                
            }

            return RedirectToPage("./Index");
        }

        //private bool ManufacturerExists(long id)
        //{
        //    return _context.Manufacturers.Any(e => e.Id == id);
        //}
    }
}
