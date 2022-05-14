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
    public class CategoryModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;
        private readonly ICategoryRepository _category;
        private readonly IHostingEnvironment _hostingEnv;

        public CategoryModel(SignInManager<IdentityUser> signInManager, IHostingEnvironment hostingEnv, ICategoryRepository category, ILogger<IndexModel> logger, IUserProfileRepository account, AhiomaDbContext context,
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
        public PaginatedList<Product> Product { get; set; }
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
        public async Task<IActionResult> OnGetAsync(string customerRef, long? id, string des, string name, int? pageIndex)
        {
            CustomerRef = customerRef;
            if (id == null)
            {
                return RedirectToPage("./Info/PageNotFound");
            }
            Category = await _category.GetById(id);
            Cid = Category.Id;
            Des = des;
            Name = name;
            if (Category == null)
            {
                return RedirectToPage("./Info/PageNotFound");

            }



            IQueryable<Product> productIQ = from s in _context.Products
                                                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.ProductPictures)
                .Include(p => p.Tenant.Market).OrderByDescending(x => x.CreatedOnUtc)
                .Where(x => x.Published == true && x.CategoryId == Category.Id).Where(x => !String.IsNullOrEmpty(x.Name))
                                            select s;



            var NewProductLists = productIQ.Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name ?? "",
                Category = x.Category,
                Manufacturer = x.Manufacturer,
                ProductPictures = x.ProductPictures,
                Market = x.Tenant.Market,
                Tenant = x.Tenant,
                Published = x.Published,
                Price = x.Price,
                ShortDescription = x.ShortDescription

            }).Where(x => !String.IsNullOrEmpty(x.Name)).AsQueryable();


            var gNewProductLists = NewProductLists;
            var ogNewProductLists = NewProductLists.Where(x=> String.IsNullOrEmpty(x.Name));
           var Producti = productIQ;

            Count = productIQ.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;
            CurrentPage = pageIndex;
            Product = await PaginatedList<Product>.CreateAsync(
                productIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
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