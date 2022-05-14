using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.SOA.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "SOA,mSuperAdmin")]

    public class AddSoaModel : PageModel
    {
        private readonly Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AddSoaModel(Exwhyzee.AhiomaDashboard.EntityFramework.Data.AhiomaDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [BindProperty]
        public string UserIdNumber { get; set; }
        public string StatusMessage { get; set; }
        [BindProperty]
        public long TenantIdGet { get; set; }
        public Tenant Tenant { get; set; }
        public IList<ProductUploadShop> ProductUploadShop { get; set; }
        public async Task<IActionResult> OnGet(long id)
        {
            TenantIdGet = id;
            Tenant = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == id);
            ProductUploadShop = await _context.ProductUploadShops.Include(x => x.Tenant).Include(x => x.UserProfile).Where(x => x.TenantId == id).ToListAsync();
            if (Tenant == null)
            {
                return NotFound();
            }
            return Page();
        }

        [BindProperty]
        public ProductUploadShop ShopUpload { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _context.UserProfiles.FirstOrDefaultAsync(x => x.IdNumber == UserIdNumber);
            var accuser = await _userManager.FindByIdAsync(user.UserId);
            //var checkrole = await _userManager.IsInRoleAsync(accuser, "SOA");
            if (user == null)
            {
                return NotFound();
            }
            //if(checkrole == false)
            //{
            //    Tenant = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == ShopUpload.TenantId);
            //    ProductUploadShop = await _context.ProductUploadShops.Include(x => x.Tenant).Where(x => x.TenantId == ShopUpload.TenantId).ToListAsync();
            //    StatusMessage = "User not an SOA";
            //    return Page();
            //}
            ShopUpload.Date = DateTime.UtcNow.AddHours(1);
            ShopUpload.UserId = user.UserId;
            ShopUpload.UserProfileId = user.Id;

            _context.ProductUploadShops.Add(ShopUpload);
            await _context.SaveChangesAsync();

            return RedirectToPage("./AddSoa", new { id = ShopUpload.TenantId });
        }

        public async Task<JsonResult> OnGetFullname(string phone)
        {
            try
            {
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.IdNumber == phone);
                if (profile != null)
                {
                    string da = profile.Fullname + " (" + profile.IdNumber + ") (" + profile.Roles + ")";

                    return new JsonResult(da);
                }
                else
                {
                    return new JsonResult("not found or wrong number");
                }
            }
            catch (Exception k)
            {
                return new JsonResult("not found or wrong number");
            }
        }
    }
}
