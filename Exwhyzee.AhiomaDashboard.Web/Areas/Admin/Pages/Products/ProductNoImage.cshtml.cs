using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.Data.Repository.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.AspNetCore.Hosting;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "Admin,SOA,Store,mSuperAdmin,Editor")]

    public class ProductNoImageModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant; private readonly IHostingEnvironment _hostingEnv;
        public ProductNoImageModel(SignInManager<IdentityUser> signInManager, IUserProfileRepository account, AhiomaDbContext context,
            ITenantRepository tenant, IHostingEnvironment hostingEnv,
           UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _account = account;
            _context = context;
            _hostingEnv = hostingEnv;
            _tenant = tenant;
        }

        public PaginatedList<ProductDto> Product { get; set; }
        public long? TenantId { get; set; }
        public Tenant Tenant { get; set; }

        //paging 
        //public int CurrentPage { get; set; } = 1;
        //public int CurrentPageFive { get; set; }
        //public int CurrentPageLastFIve { get; set; }

        public int Count { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));


        //public bool EnableNext => CurrentPage < TotalPages;

        public string Date { get; set; }
        public string Name { get; set; }
        public string Publish { get; set; }
        public string Shop { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string Commission { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int? CurrentPage { get; set; }
        public string UID { get; set; }
        public UserProfile Profile { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id, string sortOrder,
            string currentFilter, string searchString, int? pageIndex, string uid)
        {
            Name = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            Publish = String.IsNullOrEmpty(sortOrder) ? "publish_desc" : "";
            Shop = String.IsNullOrEmpty(sortOrder) ? "shop_desc" : "";
            Price = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            Quantity = String.IsNullOrEmpty(sortOrder) ? "quantity_desc" : "";
            Commission = String.IsNullOrEmpty(sortOrder) ? "commission_desc" : "";
            Date = sortOrder == "Date" ? "date_desc" : "Date";

            CurrentFilter = searchString;
            CurrentSort = sortOrder;
            UID = uid;
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            Tenant = await _context.Tenants.Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.Id == id);

        


            IQueryable<Product> productIQsnn = from s in _context.Products
                                                 .Include(p => p.Category)
                 .Include(p => p.Manufacturer)
                 .Include(x => x.ProductPictures)
                 .Include(x => x.Tenant.UserProfile)
                 .Include(x => x.Tenant).OrderByDescending(x => x.CreatedOnUtc)
                                             select s;

            var productIQs = productIQsnn.Select(x => new ProductDto
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
            productIQs = productIQs.Where(x => x.ImageThumbnail == null).AsQueryable();
            if (!String.IsNullOrEmpty(searchString))
            {
                if (searchString == "true")
                {
                    productIQs = productIQs.Where(s => s.Published == true);
                }
                else if (searchString == "false")
                {
                    productIQs = productIQs.Where(s => s.Published == false);
                }
                else
                {

                    productIQs = productIQs.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString)

                        || (s.Price.ToString() != null) && s.Price.ToString().ToUpper().Contains(searchString)

                         || (s.Tenant.BusinessName != null) && s.Tenant.BusinessName.ToUpper().Contains(searchString)
                         || (s.CreatedOnUtc != null) && s.CreatedOnUtc.ToString().ToUpper().Contains(searchString)
                        );

                }
            }
            switch (sortOrder)
            {

                case "Date":
                    productIQs = productIQs.OrderBy(s => s.CreatedOnUtc);
                    break;
                case "name":
                    productIQs = productIQs.OrderBy(s => s.Name);
                    break;
                case "publish":
                    productIQs = productIQs.OrderBy(s => s.Published);
                    break;
                case "shop":
                    productIQs = productIQs.OrderBy(s => s.Tenant.BusinessName);
                    break;
                case "price":
                    productIQs = productIQs.OrderBy(s => s.Price);
                    break;
                case "name_desc":
                    productIQs = productIQs.OrderByDescending(s => s.Name);
                    break;
                case "publish_desc":
                    productIQs = productIQs.OrderByDescending(s => s.Published);
                    break;
                case "shop_desc":
                    productIQs = productIQs.OrderByDescending(s => s.Tenant.BusinessName);
                    break;
                case "price_desc":
                    productIQs = productIQs.OrderByDescending(s => s.Price);
                    break;
                default:
                    productIQs = productIQs.OrderByDescending(s => s.CreatedOnUtc);
                    break;
            }


            Count = productIQs.Count();
            int pageSizes = 20;
            PageSize = pageSizes;

            Product = await PaginatedList<ProductDto>.CreateAsync(
                productIQs.AsNoTracking(), pageIndex ?? 1, pageSizes);
            CurrentPage = pageIndex;
            return Page();
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
