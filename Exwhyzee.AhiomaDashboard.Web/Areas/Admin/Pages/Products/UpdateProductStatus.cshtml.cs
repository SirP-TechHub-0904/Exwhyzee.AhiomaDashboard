using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Products
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SOA,Store,mSuperAdmin,Editor")]

    public class UpdateProductStatusModel : PageModel
    {
        private readonly IProductRepository _product;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;

        public UpdateProductStatusModel(IProductRepository product, AhiomaDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _product = product;
            _hostingEnv = env;
        }
        public async Task<IActionResult> OnGetAsync(long id, long tid, string uid)
        {
            try
            {
                var product = await _product.GetById(id);
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
            if (tid > 0)
            {
                return RedirectToPage("./Index", new { tid = tid, uid = uid });
            }
            else
            {
                return RedirectToPage("./Index");
            }

        }
    }
}
