using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.CheckProduct
{
    public class CreateModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public ProductCheck Data { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //string uidsdata = _userManager.GetUserId(HttpContext.User);
            var profiledata = await _context.UserProfiles.FirstOrDefaultAsync(x => x.IdNumber == Data.UserCode);
            var prod = await _context.Products.FirstOrDefaultAsync(x => x.Id == Data.ProductId);
            Data.TenantId = prod.TenantId;
            Data.UserProfileId = profiledata.Id;
            _context.ProductChecks.Add(Data);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
