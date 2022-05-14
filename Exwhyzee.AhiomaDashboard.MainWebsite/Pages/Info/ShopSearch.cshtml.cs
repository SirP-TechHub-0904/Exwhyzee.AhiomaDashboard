using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Dtos;
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

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages.Info
{
    public class ShopSearchModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly ICategoryRepository _category;
        public ShopSearchModel(SignInManager<IdentityUser> signInManager, ICategoryRepository category, IHostingEnvironment hostingEnv, ILogger<IndexModel> logger, IUserProfileRepository account, AhiomaDbContext context,
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
            _category = category;
        }

        public Tenant Tenant { get; set; }
        public PaginatedList<ProductDto> Product { get; set; }

        public PaginatedList<ProductDto> NewProductList { get; set; }
        public long? TenantId { get; set; }
        public int Count { get; set; }

        public string Name { get; set; }

        public int PageSize { get; set; }
        public int? CurrentPage { get; set; }
        public IList<StoreCategoryDto> StoreCategories { get; set; }
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        [BindProperty]
        public string CustomerRef { get; set; }

        public string searchStrings { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef, string searchString, string name, int? pageIndex)
        {
            CustomerRef = customerRef;
            Name = name;
            if (name == null)
            {
                return RedirectToPage("/Info/PageNotFound");
            }
            Tenant = await _context.Tenants
                .Include(x=>x.Market)
                .Include(x=>x.TenantAddresses)
                .FirstOrDefaultAsync(x=>x.TenentHandle == name);
            if (Tenant == null)
            {
                return RedirectToPage("/Info/PageNotFound");
            }
            //check if shop category is more than 3

            CustomerRef = customerRef;
            TempData["search"] = searchString;


            searchStrings = searchString;

            IQueryable<Product> productIQ = from s in _context.Products
                                                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Tenant)
                    .Include(p => p.Tenant.TenantAddresses)
                    .Include(p => p.ProductPictures)
                    .Include(p => p.Tenant.Market).OrderByDescending(x => x.CreatedOnUtc)
                    .Where(x => x.Published == true && x.TenantId == Tenant.Id)
                                            select s;



            if (!String.IsNullOrEmpty(searchString))
            {
              
                    if (CountString(searchString) > 1)
                    {
                        string[] searchStringCollection = searchString.Split(' ');

                        IQueryable<Product> listProduct = Enumerable.Empty<Product>().AsQueryable();
                        List<Product> alist = new List<Product>();
                        foreach (var item in searchStringCollection)
                        {
                            productIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(item.ToUpper()));
                            var li = productIQ.ToList();
                            alist.AddRange(li);
                        }
                        var mainlist = alist.Select(x => x.Id).Distinct();
                        IQueryable<Product> ipro = from s in _context.Products
                                             .Include(p => p.Category)
                        .Include(p => p.Manufacturer)
                        .Include(p => p.Tenant)
                        .Include(p => p.Tenant.TenantAddresses)
                        .Include(p => p.ProductPictures)
                        .Include(p => p.Tenant.Market)
                                              .Where(x => mainlist.Contains(x.Id))
                                               .OrderByDescending(x => x.CreatedOnUtc)
                                                   select s;
                        productIQ = ipro.AsQueryable();
                        var list = productIQ.ToList();
                    }
                    else
                    {
                        productIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString.ToUpper()));


                    }
                
            }
            var NewProductList = productIQ.Select(x => new ProductDto
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

            });
            //NewProductList = NewProductList.Where(x => x.ImageThumbnail != "noimage").ToList();

            //Product = NewProductList.OrderBy(a => Guid.NewGuid()).ToList();
            //Product = products.OrderBy(a => Guid.NewGuid()).ToList();
            //add sizes
            // var random = new Random();
            // var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            // var d = chars.Select(c => chars[random.Next(chars.Length)]).Take(8).ToArray();

            //// string sda = Guid.NewGuid().ToString("n").Substring(0, 40);
            // TempData["sd"] = d;
            //var gNewProductLists = NewProductLists.OrderBy(a => Guid.NewGuid()).ToList();
            var Producti = NewProductList;

            Count = NewProductList.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;
            CurrentPage = pageIndex ?? 1;
            Product = await PaginatedList<ProductDto>.CreateAsync(
                Producti.AsNoTracking(), pageIndex ?? 1, pageSize);
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

        public int CountString(string searchString)
        {
            int result = 0;

            searchString = searchString.Trim();

            if (searchString == "")
                return 0;

            while (searchString.Contains("  "))
                searchString = searchString.Replace("  ", " ");

            foreach (string y in searchString.Split(' '))

                result++;


            return result;
        }
    }

}
