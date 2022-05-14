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
    public class Search1Model : PageModel
    {
        private readonly IProductRepository _product;
        private readonly ICategoryRepository _category;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        public Search1Model(IProductRepository product, IHostingEnvironment hostingEnv, AhiomaDbContext context, ICategoryRepository category)
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
        public IEnumerable<ProductDto> ProductExact { get; set; }
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

        public int PageSize { get; set; }
        public int PerPage { get; set; } = 11;
        public int? CurrentPage { get; set; }
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

                //

                //correctproductIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper() == searchString.ToUpper()).ToList();

                //var checktenant = await _context.Tenants.FirstOrDefaultAsync(x => x.TenentHandle == searchString);
                //if (checktenant != null)
                //{
                //    productIQ = productIQ.Where(s => (s.Tenant.TenentHandle != null) && s.Tenant.TenentHandle.ToUpper().Contains(searchString.ToUpper()));
                //}
                //else
                //{
                //    if (CountString(searchString) > 1)
                //    {
                //        string[] searchStringCollection = searchString.Split(' ');

                //        IQueryable<Product> listProduct = Enumerable.Empty<Product>().AsQueryable();
                //        List<Product> alist = new List<Product>();
                //        foreach (var item in searchStringCollection)
                //        {
                //            productIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(item.ToUpper()));
                          
                //            var li = productIQ.ToList();
                //            alist.AddRange(li);
                //        }
                //        foreach (var item in searchStringCollection)
                //        {
                //            productIQ = productIQ.Where(s => (s.MetaKeywords != null) && s.MetaKeywords.ToUpper().Contains(item.ToUpper()));

                //            var li = productIQ.ToList();
                //            alist.AddRange(li);
                //        }

                //        foreach (var item in searchStringCollection)
                //        {
                //            productIQ = productIQ.Where(s => (s.Category.Name != null) && s.Category.Name.ToUpper().Contains(item.ToUpper()));

                //            var li = productIQ.ToList();
                //            alist.AddRange(li);
                //        }
                //        foreach (var item in searchStringCollection)
                //        {
                //            productIQ = productIQ.Where(s => (s.SubCategory.Name != null) && s.SubCategory.Name.ToUpper().Contains(item.ToUpper()));

                //            var li = productIQ.ToList();
                //            alist.AddRange(li);
                //        }
                //        var mainlist = alist.Select(x => x.Id).Distinct();
                //        IQueryable<Product> ipro = from s in _context.Products
                //                             .Include(p => p.Category)
                //        .Include(p => p.Manufacturer)
                //        .Include(p => p.Tenant)
                //        .Include(p => p.Tenant.TenantAddresses)
                //        .Include(p => p.ProductPictures)
                //        .Include(p => p.Tenant.Market)
                //                              .Where(x => mainlist.Contains(x.Id))
                //                               .OrderByDescending(x => x.CreatedOnUtc)
                //                                   select s;
                //        productIQ = ipro.AsQueryable();
                //        var list = productIQ.ToList();
                //    }
                //    else
                //    {
                       
                //        IQueryable<Product> listProduct = Enumerable.Empty<Product>().AsQueryable();
                //        List<Product> alist = new List<Product>();
                        
                //            productIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString.ToUpper()));

                //            var li = productIQ.ToList();
                //            alist.AddRange(li);
                      
                //            productIQ = productIQ.Where(s => (s.MetaKeywords != null) && s.MetaKeywords.ToUpper().Contains(searchString.ToUpper()));

                //            var lil = productIQ.ToList();
                //            alist.AddRange(lil);


                        
                //            productIQ = productIQ.Where(s => (s.Category.Name != null) && s.Category.Name.ToUpper().Contains(searchString.ToUpper()));

                //            var lli = productIQ.ToList();
                //            alist.AddRange(lli);
                       

                //            productIQ = productIQ.Where(s => (s.SubCategory.Name != null) && s.SubCategory.Name.ToUpper().Contains(searchString.ToUpper()));

                //            var llli = productIQ.ToList();
                //            alist.AddRange(llli);
                        

                //        var mainlist = alist.Select(x => x.Id).Distinct();
                //        IQueryable<Product> ipro = from s in _context.Products
                //                             .Include(p => p.Category)
                //        .Include(p => p.Manufacturer)
                //        .Include(p => p.Tenant)
                //        .Include(p => p.Tenant.TenantAddresses)
                //        .Include(p => p.ProductPictures)
                //        .Include(p => p.Tenant.Market)
                //                              .Where(x => mainlist.Contains(x.Id))
                //                               .OrderByDescending(x => x.CreatedOnUtc)
                //                                   select s;
                //        productIQ = ipro.AsQueryable();
                //        var list = productIQ.ToList();

                //    }
                //}
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

            var NewProductList2 = correctproductIQ.Select(x => new ProductDto
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

            List<ProductDto> malist = new List<ProductDto>();
           var loi = NewProductList;
           var lia = NewProductList2;

            var lis = NewProductList.Count();
            var lias = NewProductList2.Count();
            malist.AddRange(loi);
            malist.AddRange(lia);


            var dd = malist.Count();

            //
            var mainlist1 = malist.Select(x => x.Id).Distinct();
            IQueryable<Product> outipro = from s in _context.Products
                                 .Include(p => p.Category)
            .Include(p => p.Manufacturer)
            .Include(p => p.Tenant)
            .Include(p => p.Tenant.TenantAddresses)
            .Include(p => p.ProductPictures)
            .Include(p => p.Tenant.Market)
                                  .Where(x => mainlist1.Contains(x.Id))
                                   .OrderByDescending(x => x.CreatedOnUtc)
                                       select s;

            var outiprom = outipro.Select(x => new ProductDto
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

            var outputproductIQ = outiprom.AsQueryable();

            //


            var Producti = NewProductList;
            var ilistproduct = outputproductIQ;




            Count = NewProductList.Count();
            int pageSize = _context.Settings.FirstOrDefault().PageSize;
            PageSize = pageSize;
            CurrentPage = pageIndex ?? 1;
            Product = await PaginatedList<ProductDto>.CreateAsync(
                ilistproduct.AsNoTracking(), pageIndex ?? 1, pageSize);
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