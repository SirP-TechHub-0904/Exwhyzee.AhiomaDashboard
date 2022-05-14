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
    public class ShopPageModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly ICategoryRepository _category;
        public ShopPageModel(SignInManager<IdentityUser> signInManager, ICategoryRepository category, IHostingEnvironment hostingEnv, ILogger<IndexModel> logger, IUserProfileRepository account, AhiomaDbContext context,
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
        public async Task<IActionResult> OnGetAsync(string customerRef, string name, int? pageIndex)
        {
            CustomerRef = customerRef;

            Name = name;
            if (name == null)
            {
                return RedirectToPage("/Info/PageNotFound");
            }
            Tenant = await _tenant.GetByIdHandle(name);
            if (Tenant == null)
            {
                return RedirectToPage("/Info/PageNotFound");
            }
            //check if shop category is more than 3
            bool isMobile = false;
            try
            {
                string u = HttpContext.Request.Headers["User-Agent"];
                System.Text.RegularExpressions.Regex b = new System.Text.RegularExpressions.Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
                System.Text.RegularExpressions.Regex v = new System.Text.RegularExpressions.Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
                if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
                {
                    isMobile = true;
                }
            }catch(Exception c) { }
            
            StoreCategories = await _category.GetAsyncCategoryByStoreAll(Tenant.Id);
            if (StoreCategories.Count() > 3 && isMobile == false)
            {
                TempData["SortByCategory"] = "move";
            }
            else
            {


                IQueryable<Product> productIQ = from s in _context.Products
                                                   .Include(p => p.Category)
                   .Include(p => p.Manufacturer)
                   .Include(p => p.ProductPictures)
                   .Include(p => p.Tenant.Market).OrderByDescending(x => x.CreatedOnUtc)
                   .Where(x => x.Published == true && x.TenantId == Tenant.Id)
                                                select s;


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
                    OldPrice = x.OldPrice,
                    ShortDescription = x.ShortDescription

                });
                var Producti = NewProductList;
                
                Count = NewProductList.Count();
                int pageSize = _context.Settings.FirstOrDefault().PageSize;
                PageSize = pageSize;
                CurrentPage = pageIndex;
                Product = await PaginatedList<ProductDto>.CreateAsync(
                    Producti.AsNoTracking(), pageIndex ?? 1, pageSize);
            }
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
    }

}
