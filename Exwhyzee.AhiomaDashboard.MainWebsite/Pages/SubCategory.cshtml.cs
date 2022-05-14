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
    public class SubCategoryModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;
        private readonly ICategoryRepository _category;
        private readonly IHostingEnvironment _hostingEnv;

        public SubCategoryModel(SignInManager<IdentityUser> signInManager, IHostingEnvironment hostingEnv, ICategoryRepository category, ILogger<IndexModel> logger, IUserProfileRepository account, AhiomaDbContext context,
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
        public SubCategory SubCategory { get; set; }
        public Category Category { get; set; }
        public IList<Product> Product { get; set; }
        public PaginatedList<ProductDto> iNewProductList { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public int Count { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int? CurrentPage { get; set; }
        public long? SubId { get; set; }
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;
        public async Task<IActionResult> OnGetAsync(string customerRef, long? id, string desc, string name, string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            SubId = id;
           CurrentFilter = searchString;
            CurrentSort = sortOrder;

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CustomerRef = customerRef;
            if (id == null)
            {
                return RedirectToPage("./Info/PageNotFound");
            }
            SubCategory = await _context.SubCategories.FirstOrDefaultAsync(x => x.Id == id);
            Category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == SubCategory.CategoryId);

            if (SubCategory == null)
            {
                return RedirectToPage("./Info/PageNotFound");

            }

            IQueryable<Product> product = from s in _context.Products
                                            .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .Include(p => p.ProductPictures)
            .Include(p => p.Tenant.Market).OrderByDescending(x => x.CreatedOnUtc)
            .Where(x => x.Published == true && x.SubCategoryId == SubCategory.Id)
                                            select s;


            var NewProductList = product.Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category,
                Manufacturer = x.Manufacturer,
                ProductPictures = x.ProductPictures,
                Market = x.Tenant.Market,
                Tenant = x.Tenant,
                ImageThumbnail = x.ProductPictures.FirstOrDefault().PictureUrlThumbnail,
                Published = x.Published,
                Price = x.Price,
                ShortDescription = x.ShortDescription

            }).AsQueryable();
            //var mNewProductList = NewProductList.Where(x => x.ImageThumbnail != "noimage").OrderBy(a => Guid.NewGuid()).ToList();

            Count = NewProductList.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;

            iNewProductList = await PaginatedList<ProductDto>.CreateAsync(
                NewProductList.AsNoTracking(), pageIndex ?? 1, pageSize);
            CurrentPage = pageIndex ?? 1;
            return Page();
        }
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