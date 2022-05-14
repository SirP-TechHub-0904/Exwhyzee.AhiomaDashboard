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
using Exwhyzee.AhiomaDashboard.Data.Repository.Markets;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.IO;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.CategoryPromo
{
    [Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class AddOldPriceModel : PageModel
    {
        private readonly AhiomaDbContext _market;
        private readonly IUserProfileRepository _account;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;


        public AddOldPriceModel(AhiomaDbContext market, IHostingEnvironment hostingEnv, IUserProfileRepository account, AhiomaDbContext context)
        {
            _market = market;
            _context = context;
            _account = account;
            _hostingEnv = hostingEnv;
        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public long Cid { get; set; }


        public async Task<IActionResult> OnGetAsync(long? id, long cid)
        {
            if (id == null)
            {
                return NotFound();
            }

            Cid = cid;
            Product = await _market.Products.FindAsync(id);

            if (Product == null)
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
                var marketUpdate = await _context.Products.FindAsync(Product.Id);
                marketUpdate.Price = Product.Price;
                marketUpdate.OldPrice = Product.OldPrice;
              
                _context.Entry(marketUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;

            }

            return RedirectToPage("./Details", new { id = Cid });
        }

    }
}
