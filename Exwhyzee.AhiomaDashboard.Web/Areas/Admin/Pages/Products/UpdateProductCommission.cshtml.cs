using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "mSuperAdmin,Editor")]

    public class UpdateProductCommissionModel : PageModel
    {
        private readonly IProductRepository _product;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;

        public UpdateProductCommissionModel(IProductRepository product, AhiomaDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _product = product;
            _hostingEnv = env;
        }

        public Product Product { get; set; }
        public int ProductId { get; set; }
        public async Task<IActionResult> OnGetAsync(long id)
        {

            Product = await _product.GetById(id);
               

            return Page();


        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var product = await _product.GetById(ProductId);
                if(product.Published == true)
                {
                    product.Published = false;
                }
                else
                {
                    product.Published = true;
                }
                await _product.Update(product);
               
            }
            catch (Exception e)
            {

            }
            return RedirectToPage("./Index", new { id = Product.Id });


        }
    }
}
