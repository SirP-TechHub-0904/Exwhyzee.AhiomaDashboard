using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System.Net;
using System.IO;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Admin.Pages.Images
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Content,mSuperAdmin")]

    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;

        public IndexModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context)
        {
            _context = context;
        }

        public IList<ProductPicture> ProductPictures { get; set; }
        public int Count { get; set; }

        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public PaginatedList<ProductPictureDto> Picture { get; set; }
        public int? CurrentPage { get; set; }
        public string SearchString { get; set; }
        public async Task OnGetAsync(string searchString, int? pageIndex)
        {
            IQueryable<ProductPicture> ProductPictures = from s in _context.ProductPictures

                                                         select s;
            if (!String.IsNullOrEmpty(searchString))
            {

                ProductPictures = ProductPictures.Where(s => (s.PictureUrlThumbnail != null) && s.PictureUrlThumbnail.ToUpper().Contains(searchString)

                    || (s.PictureUrl.ToString() != null) && s.PictureUrl.ToString().ToUpper().Contains(searchString)
                    );

            }

            var jiop = await ProductPictures.CountAsync();
            var Images = ProductPictures.Select(x => new ProductPictureDto
            {
                Id = x.Id,
                PictureUrl = x.PictureUrl,
                PictureUrlThumbnail = x.PictureUrlThumbnail,
                ProductId = x.ProductId,
                CreatedDateTimeUtc = x.CreatedDateTimeUtc,
                ImageSize = GetFileSizes("https://ahioma.com/" + x.PictureUrl),
                ThumbnailImageSize = GetFileSizes("https://ahioma.com/" + x.PictureUrlThumbnail)


            }).OrderByDescending(v => v.Id);

            SearchString = searchString;
            var fghj = Images.AsQueryable();
            int pageSize = 50;
            PageSize = pageSize;
            Count = await Images.AsQueryable().CountAsync();
            Picture = await PaginatedList<ProductPictureDto>.CreateAsync(Images.AsNoTracking(), pageIndex ?? 1, pageSize);

            CurrentPage = pageIndex;

        }
        [BindProperty]
        public long Sid { get; set; }

        [BindProperty]
        public long Eid { get; set; }
        public async Task<IActionResult> OnPostUpdateImageSize()
        {
            try
            {
                IQueryable<ProductPicture> ProductPictures = from s in _context.ProductPictures
                                                             .Where(x => x.Id < Sid && x.Id > Eid)
                                                             select s;

                var sf = ProductPictures.ToList();
                foreach (var i in ProductPictures)
                {
                   var io = _context.ProductPictures.FirstOrDefault(x=>x.Id == i.Id);
                    io.ImageSize = GetFileSize("https://ahioma.com/" + io.PictureUrl);
                    io.ThumbnailImageSize = GetFileSize("https://ahioma.com/" + io.PictureUrlThumbnail);
                    _context.Entry(io).State = EntityState.Modified;
                    
                }
                
                await _context.SaveChangesAsync();

                var sfw = ProductPictures.ToList();

                TempData["count"] = ProductPictures.Count();
            }
            catch (Exception c)
            {

            }
            return RedirectToPage("/Images/Statuspage", new { area = "Admin" });
        }

        public static string GetFileSize(string url)
        {
            long result = 0;

            WebRequest req = WebRequest.Create(url);
            req.Method = "HEAD";
            try
            {
                using (WebResponse resp = req.GetResponse())
                {
                    if (long.TryParse(resp.Headers.Get("Content-Length"), out long contentLength))
                    {
                        result = contentLength;
                    }
                }
            }
            catch (Exception c)
            {

            }
            return result.ToString();
        }

        //your filePath  ex: /document/mypersonal/ram.png
        public static decimal GetFileSizes(string url)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.OpenRead(url);
                long totalSizeBytes = Convert.ToInt64(webClient.ResponseHeaders["Content-Length"]);
                decimal sizedf = Math.Round(((decimal)totalSizeBytes / (decimal)1024), 2);
                return sizedf;
            }
            catch (Exception y)
            {
                return 0;
            }
        }
    }
}
