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

namespace Exwhyzee.AhiomaDashboard.Web.Pages
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
        public IQueryable<Market> Markets { get; set; }
        public IList<ProductDto> NewProductList { get; set; }
        public long? TenantId { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef, long? id)
        {

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var superrole = await _userManager.IsInRoleAsync(user, "mSuperAdmin");
                var adminrole = await _userManager.IsInRoleAsync(user, "Admin");
                var storerole = await _userManager.IsInRoleAsync(user, "Store");
                var soarole = await _userManager.IsInRoleAsync(user, "SOA");
                var customerrole = await _userManager.IsInRoleAsync(user, "Customer");
                var logisticrole = await _userManager.IsInRoleAsync(user, "Logistic");
                var SubAdmin = await _userManager.IsInRoleAsync(user, "SubAdmin");


                if (superrole.Equals(true))
                {
                    return RedirectToPage("./Analysis/Index", new { area = "Dashboard" });
                }
                else if (SubAdmin.Equals(true))
                {
                    return RedirectToPage("./Dashboard/Index", new { area = "AdminPage" });

                }
                else if (storerole.Equals(true))
                {
                    var profile = await _account.GetByUserId(user.Id);
                    if (profile.FirstTimeLogin == false)
                    {
                        return RedirectToPage("./Account/UpdatePassword", new { area = "Identity" });
                    }
                    else
                    {
                        return RedirectToPage("./Dashboard/Index", new { area = "Store" });

                    }

                }
                else if (soarole.Equals(true))
                {
                    return RedirectToPage("./Dashboard/Index", new { area = "SOA" });

                }

                else if (logisticrole.Equals(true))
                {
                    return RedirectToPage("./Dashboard/Index", new { area = "Logistic" });

                }
                else if (customerrole.Equals(true))
                {
                    return RedirectToPage("./Dashboard/Index", new { area = "Customer" });

                }
            }
            else
            {
                return RedirectToPage("/Login");
            }
            return RedirectToPage("/Login");
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