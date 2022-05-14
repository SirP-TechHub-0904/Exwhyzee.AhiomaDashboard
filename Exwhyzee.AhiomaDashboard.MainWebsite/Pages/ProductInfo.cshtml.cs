
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{


    public class ProductInfoModel : PageModel
    {
        private readonly IProductRepository _product;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IUserProfileRepository _account;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductInfoModel(IProductRepository product, AhiomaDbContext context, IHostingEnvironment env,
            UserManager<IdentityUser> userManager, IUserProfileRepository account)
        {
            _context = context;
            _product = product;
            _hostingEnv = env;
            _userManager = userManager;
            _account = account;
        }


        [BindProperty]
        public Product Product { get; set; }

        public IList<ProductPicture> Pictures { get; set; }

        [BindProperty]
        public long ProductId { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef, long? id, string name, string desc)
        {
            CustomerRef = customerRef;
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Include(p => p.Manufacturer)
                .Include(p => p.ProductPictures)
                .Include(p => p.Tenant.Market)
                .Include(p => p.Tenant.TenantAddresses)
                .Include(p => p.Tenant.Market)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .Include(p => p.Tenant.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Product == null)
            {
                return NotFound();
            }

            Pictures = await _product.GetProductPictureAsyncAll(id);
            ProductId = Product.Id;


            return Page();
        }



    }
}
