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

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "Admin,SOA,Store,mSuperAdmin,Editor,SubAdmin")]

    public class IndexModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;

        public IndexModel(SignInManager<IdentityUser> signInManager, IUserProfileRepository account, AhiomaDbContext context,
            ITenantRepository tenant,
           UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _account = account;
            _context = context;
            _tenant = tenant;
        }

        public PaginatedList<Product> Product { get; set; }
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
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;
        public string UID { get; set; }
        public UserProfile Profile { get; set; }
        public IList<ProductCheck> ProductCheck { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id, string sortOrder,
            string currentFilter, string searchString, int? pageIndex, string uid)
        {

            //
            string uidsdata = _userManager.GetUserId(HttpContext.User);
            var profiledata = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == uidsdata);
            ProductCheck = await _context.ProductChecks.Include(x=>x.Product).Where(x => x.UserCode == profiledata.IdNumber).ToListAsync();

            int pageSize = _context.Settings.FirstOrDefault().PageSize;



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

            if (!String.IsNullOrEmpty(uid))
            {
                IQueryable<Product> productIQ = from s in _context.Products
                                                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(x => x.ProductPictures)
                .Include(x => x.Tenant.UserProfile)
                .Include(x => x.Tenant).Where(x => x.ThirdPartyUserId == uid && x.TenantId == id).OrderByDescending(x=>x.CreatedOnUtc)
                                                select s;
                if (!String.IsNullOrEmpty(searchString))
                {

                    productIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString)

                        || (s.Price.ToString() != null) && s.Price.ToString().ToUpper().Contains(searchString)

                         || (s.Tenant.BusinessName != null) && s.Tenant.BusinessName.ToUpper().Contains(searchString)
                         || (s.Commision.ToString() != null) && s.Commision.ToString().ToUpper().Contains(searchString)
                         || (s.CreatedOnUtc != null) && s.CreatedOnUtc.ToString().ToUpper().Contains(searchString)
                        );

                }

                switch (sortOrder)
                {

                    case "Date":
                        productIQ = productIQ.OrderBy(s => s.CreatedOnUtc);
                        break;
                    case "name":
                        productIQ = productIQ.OrderBy(s => s.Name);
                        break;
                    case "publish":
                        productIQ = productIQ.OrderBy(s => s.Published);
                        break;
                    case "shop":
                        productIQ = productIQ.OrderBy(s => s.Tenant.BusinessName);
                        break;
                    case "quantity":
                        productIQ = productIQ.OrderBy(s => s.Quantity);
                        break;
                    case "price":
                        productIQ = productIQ.OrderBy(s => s.Price);
                        break;
                    case "commission":
                        productIQ = productIQ.OrderBy(s => s.Commision);
                        break;
                    case "name_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Name);
                        break;
                    case "publish_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Published);
                        break;
                    case "shop_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Tenant.BusinessName);
                        break;
                    case "quantity_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Quantity);
                        break;
                    case "price_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Price);
                        break;
                    case "commission_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Commision);
                        break;
                    default:
                        productIQ = productIQ.OrderByDescending(s => s.CreatedOnUtc);
                        break;
                }
                Count = productIQ.Count();
                PageSize = pageSize;

                Product = await PaginatedList<Product>.CreateAsync(
                    productIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
                TenantId = id;
                CurrentPage = pageIndex ?? 1;
                Profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == uid);
                TempData["Third"] = uid;
                Tenant = Tenant;
                return Page();
            }

            if (id != null)
            {
                IQueryable<Product> productIQ = from s in _context.Products
                                                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(x => x.ProductPictures)
                .Include(x => x.Tenant.UserProfile)
                .Include(x => x.Tenant).Where(x => x.TenantId == id).OrderByDescending(x => x.CreatedOnUtc)
                                                select s;
                if (!String.IsNullOrEmpty(searchString))
                {
                   
                        productIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString)

                            || (s.Price.ToString() != null) && s.Price.ToString().ToUpper().Contains(searchString)

                             || (s.Tenant.BusinessName != null) && s.Tenant.BusinessName.ToUpper().Contains(searchString)
                             || (s.Commision.ToString() != null) && s.Commision.ToString().ToUpper().Contains(searchString)
                             || (s.CreatedOnUtc != null) && s.CreatedOnUtc.ToString().ToUpper().Contains(searchString)
                            );
                    
                }

                switch (sortOrder)
                {
                   
                    case "Date":
                        productIQ = productIQ.OrderBy(s => s.CreatedOnUtc);
                        break;
                    case "name":
                        productIQ = productIQ.OrderBy(s => s.Name);
                        break;
                    case "publish":
                        productIQ = productIQ.OrderBy(s => s.Published);
                        break;
                    case "shop":
                        productIQ = productIQ.OrderBy(s => s.Tenant.BusinessName);
                        break;
                    case "quantity":
                        productIQ = productIQ.OrderBy(s => s.Quantity);
                        break;
                    case "price":
                        productIQ = productIQ.OrderBy(s => s.Price);
                        break;
                    case "commission":
                        productIQ = productIQ.OrderBy(s => s.Commision);
                        break;
                    case "name_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Name);
                        break;
                    case "publish_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Published);
                        break;
                    case "shop_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Tenant.BusinessName);
                        break;
                    case "quantity_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Quantity);
                        break;
                    case "price_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Price);
                        break;
                    case "commission_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Commision);
                        break;
                    default:
                        productIQ = productIQ.OrderBy(s => s.CreatedOnUtc);
                        break;
                }
                Count = productIQ.Count();
                PageSize = pageSize;

                Product = await PaginatedList<Product>.CreateAsync(
                    productIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
                TenantId = id;
                CurrentPage = pageIndex ?? 1;
                if (Tenant.BusinessName != null)
                {

                    TempData["title"] = "All Products of " + Tenant.BusinessName ?? "";
                }
                else
                {
                    TempData["title"] = "All Products of ";
                }
                return Page();
            }
            string uids = _userManager.GetUserId(HttpContext.User);
            var tenant = await _tenant.GetById(id);
            if(tenant != null)
            {
                TenantId = tenant.Id;
            }
            var user = await _userManager.FindByIdAsync(uids);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var adminrole = await _userManager.IsInRoleAsync(user, "Admin");
            if (adminrole.Equals(true))
            {
                TempData["title"] = "All Products";
                IQueryable<Product> productIQ = from s in _context.Products
                                             .Include(p => p.Category)
             .Include(p => p.Manufacturer)
             .Include(x => x.ProductPictures)
             .Include(x => x.Tenant.UserProfile)
             .Include(x => x.Tenant)
                                                 select s;
                if (!String.IsNullOrEmpty(searchString))
                {

                    productIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString)

                        || (s.Price.ToString() != null) && s.Price.ToString().ToUpper().Contains(searchString)

                         || (s.Tenant.BusinessName != null) && s.Tenant.BusinessName.ToUpper().Contains(searchString)
                         || (s.Commision.ToString() != null) && s.Commision.ToString().ToUpper().Contains(searchString)
                         || (s.CreatedOnUtc != null) && s.CreatedOnUtc.ToString().ToUpper().Contains(searchString)
                        );


                }
                switch (sortOrder)
                {

                    case "Date":
                        productIQ = productIQ.OrderBy(s => s.CreatedOnUtc);
                        break;
                    case "name":
                        productIQ = productIQ.OrderBy(s => s.Name);
                        break;
                    case "publish":
                        productIQ = productIQ.OrderBy(s => s.Published);
                        break;
                    case "shop":
                        productIQ = productIQ.OrderBy(s => s.Tenant.BusinessName);
                        break;
                    case "quantity":
                        productIQ = productIQ.OrderBy(s => s.Quantity);
                        break;
                    case "price":
                        productIQ = productIQ.OrderBy(s => s.Price);
                        break;
                    case "commission":
                        productIQ = productIQ.OrderBy(s => s.Commision);
                        break;
                    case "name_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Name);
                        break;
                    case "publish_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Published);
                        break;
                    case "shop_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Tenant.BusinessName);
                        break;
                    case "quantity_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Quantity);
                        break;
                    case "price_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Price);
                        break;
                    case "commission_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Commision);
                        break;
                    default:
                        productIQ = productIQ.OrderByDescending(s => s.CreatedOnUtc);
                        break;
                }


                Count = productIQ.Count();
                PageSize = pageSize;

                Product = await PaginatedList<Product>.CreateAsync(
                    productIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
                CurrentPage = pageIndex ?? 1;

                return Page();
            }
                var soarole = await _userManager.IsInRoleAsync(user, "SOA");
                
            if (soarole.Equals(true))
            {
               
                IQueryable<Product> productIQ = from s in _context.Products
                                                 .Include(p => p.Category)
                 .Include(p => p.Manufacturer)
                 .Include(x => x.ProductPictures)
                 .Include(x => x.Tenant.UserProfile)
                 .Include(x => x.Tenant).Where(x => x.Tenant.CreationUserId == user.Id)
                                                select s;
                if (!String.IsNullOrEmpty(searchString))
                {

                    productIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString)

                        || (s.Price.ToString() != null) && s.Price.ToString().ToUpper().Contains(searchString)

                         || (s.Tenant.BusinessName != null) && s.Tenant.BusinessName.ToUpper().Contains(searchString)
                         || (s.Commision.ToString() != null) && s.Commision.ToString().ToUpper().Contains(searchString)
                         || (s.CreatedOnUtc != null) && s.CreatedOnUtc.ToString().ToUpper().Contains(searchString)
                        );


                }
                switch (sortOrder)
                {

                    case "Date":
                        productIQ = productIQ.OrderBy(s => s.CreatedOnUtc);
                        break;
                    case "name":
                        productIQ = productIQ.OrderBy(s => s.Name);
                        break;
                    case "publish":
                        productIQ = productIQ.OrderBy(s => s.Published);
                        break;
                    case "shop":
                        productIQ = productIQ.OrderBy(s => s.Tenant.BusinessName);
                        break;
                    case "quantity":
                        productIQ = productIQ.OrderBy(s => s.Quantity);
                        break;
                    case "price":
                        productIQ = productIQ.OrderBy(s => s.Price);
                        break;
                    case "commission":
                        productIQ = productIQ.OrderBy(s => s.Commision);
                        break;
                    case "name_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Name);
                        break;
                    case "publish_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Published);
                        break;
                    case "shop_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Tenant.BusinessName);
                        break;
                    case "quantity_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Quantity);
                        break;
                    case "price_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Price);
                        break;
                    case "commission_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Commision);
                        break;
                    default:
                        productIQ = productIQ.OrderByDescending(s => s.CreatedOnUtc);
                        break;
                }

                Count = productIQ.Count();
                PageSize = pageSize;
                
                Product = await PaginatedList<Product>.CreateAsync(
                    productIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
                CurrentPage = pageIndex ?? 1;
                TempData["title"] = "All Products By SOA ("+profile.Fullname+")";
                return Page();
            }
            var storerole = await _userManager.IsInRoleAsync(user, "Store");
            if (storerole.Equals(true))
            {
                var storeid = await _tenant.GetByLogin(uids);
                Tenant = await _context.Tenants.Include(x => x.UserProfile).FirstOrDefaultAsync(x => x.Id == storeid.Id);
               

                IQueryable<Product> productIQ = from s in _context.Products
                                                 .Include(p => p.Category)
                 .Include(p => p.Manufacturer)
                 .Include(x => x.ProductPictures)
                 .Include(x => x.Tenant.UserProfile)
                 .Include(x => x.Tenant).Where(x => x.TenantId == storeid.Id)
                                                select s;
                if (!String.IsNullOrEmpty(searchString))
                {

                    productIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString)

                        || (s.Price.ToString() != null) && s.Price.ToString().ToUpper().Contains(searchString)

                         || (s.Tenant.BusinessName != null) && s.Tenant.BusinessName.ToUpper().Contains(searchString)
                         || (s.Commision.ToString() != null) && s.Commision.ToString().ToUpper().Contains(searchString)
                         || (s.CreatedOnUtc != null) && s.CreatedOnUtc.ToString().ToUpper().Contains(searchString)
                        );


                }
                switch (sortOrder)
                {

                    case "Date":
                        productIQ = productIQ.OrderBy(s => s.CreatedOnUtc);
                        break;
                    case "name":
                        productIQ = productIQ.OrderBy(s => s.Name);
                        break;
                    case "publish":
                        productIQ = productIQ.OrderBy(s => s.Published);
                        break;
                    case "shop":
                        productIQ = productIQ.OrderBy(s => s.Tenant.BusinessName);
                        break;
                    case "quantity":
                        productIQ = productIQ.OrderBy(s => s.Quantity);
                        break;
                    case "price":
                        productIQ = productIQ.OrderBy(s => s.Price);
                        break;
                    case "commission":
                        productIQ = productIQ.OrderBy(s => s.Commision);
                        break;
                    case "name_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Name);
                        break;
                    case "publish_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Published);
                        break;
                    case "shop_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Tenant.BusinessName);
                        break;
                    case "quantity_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Quantity);
                        break;
                    case "price_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Price);
                        break;
                    case "commission_desc":
                        productIQ = productIQ.OrderByDescending(s => s.Commision);
                        break;
                    default:
                        productIQ = productIQ.OrderByDescending(s => s.CreatedOnUtc);
                        break;
                }

                Count = productIQ.Count();
                PageSize = pageSize;

                Product = await PaginatedList<Product>.CreateAsync(
                    productIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
                CurrentPage = pageIndex ?? 1;// Product = Product.Where(x => x.TenantId == storeid.Id).ToList();
                TenantId = storeid.Id;
                TempData["title"] = "All Products By Shop (" + storeid.BusinessName + ")";
                return Page();
            }
          


            IQueryable<Product> productIQs = from s in _context.Products
                                                 .Include(p => p.Category)
                 .Include(p => p.Manufacturer)
                 .Include(x => x.ProductPictures)
                 .Include(x => x.Tenant.UserProfile)
                 .Include(x => x.Tenant).OrderByDescending(x => x.CreatedOnUtc)
                                             select s;
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
                         || (s.Commision.ToString() != null) && s.Commision.ToString().ToUpper().Contains(searchString)
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
                case "quantity":
                    productIQs = productIQs.OrderBy(s => s.Quantity);
                    break;
                case "price":
                    productIQs = productIQs.OrderBy(s => s.Price);
                    break;
                case "commission":
                    productIQs = productIQs.OrderBy(s => s.Commision);
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
                case "quantity_desc":
                    productIQs = productIQs.OrderByDescending(s => s.Quantity);
                    break;
                case "price_desc":
                    productIQs = productIQs.OrderByDescending(s => s.Price);
                    break;
                case "commission_desc":
                    productIQs = productIQs.OrderByDescending(s => s.Commision);
                    break;
                default:
                    productIQs = productIQs.OrderByDescending(s => s.CreatedOnUtc);
                    break;
            }


            Count = productIQs.Count();
            PageSize = pageSize;

            Product = await PaginatedList<Product>.CreateAsync(
                productIQs.AsNoTracking(), pageIndex ?? 1, pageSize);
            CurrentPage = pageIndex ?? 1;
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


    }
}
