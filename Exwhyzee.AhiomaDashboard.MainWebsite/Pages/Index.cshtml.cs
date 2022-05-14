using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceDetectorNET;
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
    public class IndexModel : PageModel
    {
       

        private readonly ILogger<IndexModel> _logger;
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUserProfileRepository _account;
        private readonly ITenantRepository _tenant;
        private readonly IHostingEnvironment _hostingEnv;

        public IndexModel(SignInManager<IdentityUser> signInManager, IHostingEnvironment hostingEnv, ILogger<IndexModel> logger, IUserProfileRepository account, AhiomaDbContext context,
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
        }


        public IList<Product> Product { get; set; }
        public IQueryable<Category> Category { get; set; }
        public IQueryable<PromoCategory> PromoCategory { get; set; }
        public IQueryable<Market> Markets { get; set; }
        public IList<ProductDto> NewProductList { get; set; }
        public long? TenantId { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task OnGetAsync(string customerRef, long? id)
        {
            CustomerRef = customerRef;
            IQueryable<Category> iCategory = from s in _context.Categories
                                               .Where(x=>x.ShowOnHomePage).OrderBy(x => x.DisplayOrder).Take(14)
                                            select s;
            Category = iCategory.AsQueryable();
            IQueryable<Market> imkt = from s in _context.Markets
                                             select s;

            Markets = imkt.AsQueryable();

            IQueryable<PromoCategory> iPromoCategory = from s in _context.PromoCategories
                                             .Where(x => x.Show == true).OrderBy(x => x.Date).Take(2)
                                             select s;
            PromoCategory = iPromoCategory.AsQueryable();
        }
        //
        public string ValidImage(long id)
        {
            string imgpath = "noimage";
            try
            {
                var pic = _context.ProductPictures.Where(x => x.ProductId == id).ToList();
                foreach(var i in pic)
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
            catch(Exception c)
            {
                return imgpath;
            }
        }
    }
}
 //if (System.IO.File.Exists(fullPaththum))
 //               {
 //                   System.IO.File.Delete(fullPaththum);

 //               }

//template  https://themesbrand.com/skote/layouts/vertical/index-dark.html
//https://sb-revo.mybigcommerce.com