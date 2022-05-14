using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Categories;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceStack;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
    public class SearchModel : PageModel
    {
        private readonly IProductRepository _product;
        private readonly ICategoryRepository _category;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        public SearchModel(IProductRepository product, IHostingEnvironment hostingEnv, AhiomaDbContext context, ICategoryRepository category)
        {
            _context = context;
            _product = product;
            _category = category;
            _hostingEnv = hostingEnv;
        }

        public long TenantId { get; set; }
        //public async Task<IActionResult> OnGet(long tid)

        //{
        //    var category = await _category.GetAsyncCategoryByStoreAll(tid);
        //    ViewData["CategoryId"] = new SelectList(category, "CategoryId", "CategoryName");
        //    ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers, "Id", "Name");
        //    ViewData["ProductTypeId"] = new SelectList(_context.SubCategories, "Id", "Name");
        //    TenantId = tid;
        //    return Page();
        //}


        public PaginatedList<ProductDto> Product { get; set; }
        public IQueryable<ProductDto> ProductDtoExact { get; set; }
        public IQueryable<Product> ProductExact { get; set; }
        public IQueryable<Product> ProductShop { get; set; }
        public IQueryable<Product> SubGeneralProduct { get; set; }
        public IQueryable<Product> GeneralProduct { get; set; }


        public IQueryable<ProductDto> ProductDtoContain { get; set; }
        public IQueryable<Product> ProductContain { get; set; }


        public IList<Category> Categories { get; set; }


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
        public int Count { get; set; }
        public long Cid { get; set; }
        public string catidloggers { get; set; }
        public string hdsearcws { get; set; }
        public string searchStrings { get; set; }

        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int PageSize { get; set; }
        public int PerPage { get; set; } = 11;
        public int? CurrentPage { get; set; }
        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGet(string customerRef, string searchString, string catidlogger, string hdsearcw, int? pageIndex)
        {
            CustomerRef = customerRef;
            TempData["search"] = searchString;
            List<Product> correctproductIQ = new List<Product>();

            catidloggers = catidlogger;
            hdsearcws = hdsearcw;
            searchStrings = searchString;

            IQueryable<Product> productIQ = from s in _context.Products
                                                    .Include(p => p.Category)
                    .Include(p => p.Manufacturer)
                    .Include(p => p.Tenant)
                    .Include(p => p.Tenant.TenantAddresses)
                    .Include(p => p.ProductPictures)
                    .Include(p => p.Tenant.Market).OrderByDescending(x => x.CreatedOnUtc)
                    .Where(x => x.Published == true)
                                            select s;



            if (!String.IsNullOrEmpty(searchString))
            {
                ProductExact = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper() == searchString.ToUpper()).AsQueryable();
                ProductContain = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString.ToUpper())).AsQueryable();
                ProductShop = productIQ.Where(s => (s.Tenant.BusinessName != null) && s.Tenant.BusinessName.ToUpper().Contains(searchString.ToUpper())).AsQueryable();
            }

            SubGeneralProduct = ProductExact.Union(ProductContain);
            GeneralProduct = SubGeneralProduct.Union(ProductShop);

            ProductDtoExact = GeneralProduct.Select(x => new ProductDto
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

            Count = ProductDtoExact.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;
            CurrentPage = pageIndex ?? 1;
            Product = await PaginatedList<ProductDto>.CreateAsync(
                ProductDtoExact.AsNoTracking(), pageIndex ?? 1, pageSize);

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