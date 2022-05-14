using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
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
    public class PromoPageModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;
        private readonly ICategoryRepository _category;
        private readonly IHostingEnvironment _hostingEnv;

        public PromoPageModel(SignInManager<IdentityUser> signInManager, IHostingEnvironment hostingEnv, ICategoryRepository category, ILogger<IndexModel> logger, IUserProfileRepository account, AhiomaDbContext context,
            ITenantRepository tenant,
           UserManager<IdentityUser> userManager)
        {
            _category = category;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _account = account;
            _context = context;
            _tenant = tenant;
            _hostingEnv = hostingEnv;
        }

        public Tenant Tenant { get; set; }
        public Category Category { get; set; }
        public PaginatedList<ProductDto> Product { get; set; }
        public PaginatedList<ProductDto> NewProductList { get; set; }
        public int Count { get; set; }
        public long Cid { get; set; }
        public string Des { get; set; }
        public string Name { get; set; }

        public int PageSize { get; set; }
        public int? CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef, long id, int? pageIndex)
        {
            CustomerRef = customerRef;
            if (id == null)
            {
                return RedirectToPage("./Info/PageNotFound");
            }
            
            IQueryable<PromoProduct> productIQ = from s in _context.PromoProducts
                                            .Include(p => p.PromoCategory)
            .Include(p => p.Product)

            .Include(p => p.Product.Tenant)
            .Include(p => p.Product.Tenant.Market)
            .Include(p => p.Product.ProductPictures)
            .Where(x => x.PromoCategoryId == id)
                                                 select s;


            Cid = id;
            var NewProductLists = productIQ.Select(x => new ProductDto
            {
                Id = x.Product.Id,
                Name = x.Product.Name ?? "",
                Category = x.Product.Category,
                Manufacturer = x.Product.Manufacturer,
                ProductPictures = x.Product.ProductPictures,
                Market = x.Product.Tenant.Market,
                Tenant = x.Product.Tenant,
                Published = x.Product.Published,
                ImageThumbnail = x.Product.ProductPictures.FirstOrDefault().PictureUrlThumbnail,
                Price = x.Product.Price,
                OldPrice = x.Product.OldPrice,
                ShortDescription = x.Product.ShortDescription

            }).Where(x => !String.IsNullOrEmpty(x.Name)).AsQueryable();



            Count = productIQ.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;
            CurrentPage = pageIndex;
            Product = await PaginatedList<ProductDto>.CreateAsync(
                NewProductLists.AsNoTracking(), pageIndex ?? 1, pageSize);
            return Page();
        }

    }
    
}