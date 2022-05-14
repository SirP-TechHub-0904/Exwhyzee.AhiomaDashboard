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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Admin.Pages.Products
{
    [Authorize(Roles = "Admin,SOA,Store,mSuperAdmin,Editor")]

    public class ProductSearchModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;

        public ProductSearchModel(SignInManager<IdentityUser> signInManager, IUserProfileRepository account, AhiomaDbContext context,
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
        public string UID { get; set; }
        public string CID { get; set; }
        public string searchSIDDs { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public UserProfile Profile { get; set; }
        public UserProfile UploaderProfile { get; set; }
        public List<SelectListItem> StoreListing { get; set; }
        public async Task<IActionResult> OnGetAsync(long? id, string sortOrder, string searchSIDD,
            string currentFilter, string searchString, int? pageIndex, long? tid, string cid, DateTime? startdata, DateTime? enddate, string shop, string market )
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
            
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            Tenant = await _context.Tenants.Include(x => x.UserProfile).Include(x => x.UserProfile.User).FirstOrDefaultAsync(x => x.Id == tid);
            IQueryable<Tenant> iTenanat = from s in _context.Tenants.OrderBy(x=>x.BusinessName)
                                          select s;
            StoreListing = iTenanat.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id.ToString(),
                                      Text = a.BusinessName
                                  }).ToList();
            searchSIDDs = searchSIDD;
            if (searchSIDD == "SIDD")
            {
                if(startdata == null || enddate == null || String.IsNullOrEmpty(cid) || Tenant == null)
                {
                    StoreListing = iTenanat.Select(a =>
                                 new SelectListItem
                                 {
                                     Value = a.Id.ToString(),
                                     Text = a.BusinessName
                                 }).ToList();
                    TempData["searchSIDD"] = "SIDD";
                    return Page();
                }

                var userprofile = await _context.UserProfiles.Include(x=>x.User).FirstOrDefaultAsync(x => x.IdNumber == cid);

                UploaderProfile = userprofile;

                IQueryable<Product> productIQ = from s in _context.Products
                                                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(x => x.ProductPictures)
                .Include(x => x.Tenant.UserProfile)
                .Include(x => x.Tenant)
                .Where(x => x.ThirdPartyUserId == userprofile.UserId)
                .Where(x => x.TenantId == tid)
                .Where(x => x.CreatedOnUtc.Date >= startdata).Where(x => x.CreatedOnUtc.Date <= enddate)
                                                select s;

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
                int pageSize = _context.Settings.FirstOrDefault().PageSize;
                PageSize = pageSize;

                Product = await PaginatedList<Product>.CreateAsync(
                    productIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
                TenantId = tid;
                CurrentPage = pageIndex;
                CID = cid;
                Profile = await _context.UserProfiles.Include(x => x.User).FirstOrDefaultAsync(x => x.IdNumber == cid);
                StartDate = startdata;
               EndDate = enddate;
                Tenant = Tenant;
               
                return Page();
            }
            Count = 10;
            PageSize = 10;

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


    }
}
