using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Categories
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Content,mSuperAdmin")]
    public class SubCreateModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public SubCreateModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public async Task<IActionResult> OnGet(long id)
        {
           var Cat = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            CategoryId = Cat.Id;
            CategoryName = Cat.Name;
            return Page();
        }

        [BindProperty]
        public SubCategory SubCategory { get; set; }

        

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            SubCategory.CreatedOnUtc = DateTime.UtcNow.AddHours(1);
            SubCategory.UpdatedOnUtc = DateTime.UtcNow.AddHours(1);
            _context.SubCategories.Add(SubCategory);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = SubCategory.CategoryId });
        }
    }
}
