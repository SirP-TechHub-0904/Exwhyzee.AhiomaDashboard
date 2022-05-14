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
    public class SearchQuerryModel : PageModel
    {
        private readonly IProductRepository _product;
        private readonly ICategoryRepository _category;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        public SearchQuerryModel(IProductRepository product, IHostingEnvironment hostingEnv, AhiomaDbContext context, ICategoryRepository category)
        {
            _context = context;
            _product = product;
            _category = category;
            _hostingEnv = hostingEnv;
        }

        public long TenantId { get; set; }
       
        public PaginatedList<ProductDto> Product { get; set; }
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
            IQueryable<Product> list1;
            IQueryable<Product> list2 = Enumerable.Empty<Product>().AsQueryable(); 
            IQueryable<Product> list3;
            IQueryable<Product> MainProductList = Enumerable.Empty<Product>().AsQueryable();


            IQueryable<Product> collectList1 = Enumerable.Empty<Product>().AsQueryable();
            IQueryable<Product> collectList2 = Enumerable.Empty<Product>().AsQueryable();
            IQueryable<Product> list2i = Enumerable.Empty<Product>().AsQueryable();

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
               
                list1 = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper() == searchString.ToUpper()).AsQueryable();

                var oi = list1.ToList();
                if (CountString(searchString) > 1)
                {
                    string[] searchStringCollection = searchString.Split(' ');
                    IQueryable<Product> alist = Enumerable.Empty<Product>().AsQueryable();
                    foreach (var item in searchStringCollection)
                    {
                        var ilist = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper() == item.ToUpper()).AsQueryable();
                        var ysu = ilist.Count();
                        collectList1 = alist.Concat(ilist);

                        var sj = collectList1.Count();
                        var ssj = collectList1.ToList();

                    }
                    IQueryable<Product> ialist = Enumerable.Empty<Product>().AsQueryable();

                    IQueryable<Product> sumlist = Enumerable.Empty<Product>().AsQueryable();
                    foreach (var item in searchStringCollection)
                    {
                        var ilist = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(item.ToUpper()));
                        var ysu = ilist.Count();
                        collectList1 = ialist.Concat(ilist);

                        var rsj = collectList1.Count();
                        var srsj = collectList1.ToList();
                    }

                    collectList1 = sumlist.Concat(alist);

                    var gfdhj = collectList1.Count();
                    collectList1 = sumlist.Concat(ialist);

                    var gfddhj = collectList1.Count();
                    list2i = collectList1;

                    var jd = collectList1.Count();

                    

                }
                //
                list3 = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString.ToUpper())
                           || (s.Tenant.BusinessName != null) && s.Tenant.BusinessName.ToUpper().Contains(searchString.ToUpper())
                           || (s.Tenant.TenentHandle != null) && s.Tenant.TenentHandle.ToUpper().Contains(searchString.ToUpper())
                             || (s.Tenant.Market.Name != null) && s.Tenant.Market.Name.ToUpper().Contains(searchString.ToUpper())
                             || (s.SubCategory.Name != null) && s.SubCategory.Name.ToUpper().Contains(searchString.ToUpper())
                            || (s.Category.Name != null) && s.Category.Name.ToUpper().Contains(searchString.ToUpper()));

                IQueryable<Product> resultalist = Enumerable.Empty<Product>().AsQueryable();

                resultalist.Concat(list2i);
                resultalist.Concat(list3);
                var duplicatemainlist = resultalist.Select(x => x.Id).Distinct();
                IQueryable<Product> ipro = from s in _context.Products

                                      .Where(x => duplicatemainlist.Contains(x.Id))
                                       .OrderByDescending(x => x.CreatedOnUtc)
                                           select s;
                list2 = ipro.AsQueryable();

                var skj = list3.Count();

                #region
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
                //            productIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(item.ToUpper())
                //           || (s.Tenant.BusinessName != null) && s.Tenant.BusinessName.ToUpper().Contains(item.ToUpper())
                //           || (s.Tenant.TenentHandle != null) && s.Tenant.TenentHandle.ToUpper().Contains(item.ToUpper())
                //             || (s.Tenant.Market.Name != null) && s.Tenant.Market.Name.ToUpper().Contains(item.ToUpper())
                //             || (s.SubCategory.Name != null) && s.SubCategory.Name.ToUpper().Contains(item.ToUpper())
                //            || (s.Category.Name != null) && s.Category.Name.ToUpper().Contains(item.ToUpper()));
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
                //        productIQ = productIQ.Where(s => (s.Name != null) && s.Name.ToUpper().Contains(searchString.ToUpper())
                //            || (s.Tenant.BusinessName != null) && s.Tenant.BusinessName.ToUpper().Contains(searchString.ToUpper())
                //            || (s.Tenant.TenentHandle != null) && s.Tenant.TenentHandle.ToUpper().Contains(searchString.ToUpper())
                //              || (s.Tenant.Market.Name != null) && s.Tenant.Market.Name.ToUpper().Contains(searchString.ToUpper())
                //              || (s.SubCategory.Name != null) && s.SubCategory.Name.ToUpper().Contains(searchString.ToUpper())
                //             || (s.Category.Name != null) && s.Category.Name.ToUpper().Contains(searchString.ToUpper()));


                //    }
                //}

                #endregion


                var i1 = list1.ToList();
                var i1e = list1.Count();

                var i1rr = list2.ToList();
                var i1err = list2.Count();

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

           // var NewProductList2 = correctproductIQ.Select(x => new ProductDto
           // {
           //     Id = x.Id,
           //     Name = x.Name,
           //     Category = x.Category,
           //     Manufacturer = x.Manufacturer,
           //     ProductPictures = x.ProductPictures,
           //     Market = x.Tenant.Market,
           //     Tenant = x.Tenant,
           //     ImageThumbnail = x.ProductPictures.FirstOrDefault().PictureUrlThumbnail,
           //     Published = x.Published,
           //     Price = x.Price,
           //     ShortDescription = x.ShortDescription

           // });

           // List<ProductDto> malist = new List<ProductDto>();
           //var loi = NewProductList;
           //var lia = NewProductList2;

           // var lis = NewProductList.Count();
           // var lias = NewProductList2.Count();
           // malist.AddRange(loi);
           // malist.AddRange(lia);


       


            //Count = NewProductList.Count();
            //int pageSize = _context.Settings.FirstOrDefault().PageSize;
            //PageSize = pageSize;
            //CurrentPage = pageIndex ?? 1;
            //Product = await PaginatedList<ProductDto>.CreateAsync(
            //    ilistproduct.AsNoTracking(), pageIndex ?? 1, pageSize);
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