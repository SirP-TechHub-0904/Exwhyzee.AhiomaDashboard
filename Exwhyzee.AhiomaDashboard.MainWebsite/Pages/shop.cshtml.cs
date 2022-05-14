using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    public class shopModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;
        private readonly IHostingEnvironment _hostingEnv;

        public shopModel(SignInManager<IdentityUser> signInManager, IHostingEnvironment hostingEnv, ILogger<IndexModel> logger, IUserProfileRepository account, AhiomaDbContext context,
            ITenantRepository tenant,
           UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _account = account;
            _context = context;
            _tenant = tenant;
            _hostingEnv = hostingEnv;
        }

        public Tenant Tenant { get; set; }
        public IList<Product> Product { get; set; }

        public IList<ProductDto> NewProductList { get; set; }
        public long? TenantId { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef, string name)
        {
            CustomerRef = customerRef;
            if (name == null)
            {
                return RedirectToPage("./Info/PageNotFound");
            }

            Tenant = await _tenant.GetByIdHandle(name);
               

            if (Tenant == null)
            {
                return RedirectToPage("./Info/PageNotFound");

            }


            
        
            Product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.ProductPictures)
                .Include(p => p.Tenant.Market).OrderByDescending(x => x.CreatedOnUtc)
                .Where(x => x.Published == true && x.TenantId == Tenant.Id)
                .ToListAsync();

        


      
            NewProductList = Product.Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category,
                Manufacturer = x.Manufacturer,
                ProductPictures = x.ProductPictures,
                Market = x.Tenant.Market,
                Tenant = x.Tenant,
                ImageThumbnail = ValidImage(x.Id),
                Published = x.Published,
                Price = x.Price,
                ShortDescription = x.ShortDescription

            }).ToList();
            NewProductList = NewProductList.Where(x => x.ImageThumbnail != "noimage").ToList();
            return Page();
        }
        //
        public string ValidImage(long id)
        {
            string imgpath = "noimage";
            try
            {
                var pic = _context.ProductPictures.Where(x => x.ProductId == id).ToList();
                foreach (var i in pic)
                {

                    string webRootPath = _hostingEnv.WebRootPath;
                    var fullPaththum = webRootPath + i.PictureUrlThumbnail;
                    if (System.IO.File.Exists(fullPaththum))
                    {
                        imgpath = i.PictureUrlThumbnail;
                        break;
                    }
                }
                return imgpath;
            }
            catch (Exception c)
            {
                return imgpath;
            }
        }
    }
}