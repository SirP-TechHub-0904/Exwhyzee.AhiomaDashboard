using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.Reconsile.Pages.Accounts
{

    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin")]

    public class ProductRxModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;

        public ProductRxModel(
            UserManager<IdentityUser> userManager, AhiomaDbContext context)
        {
            _userManager = userManager;
            _context = context;

        }


        public async Task<ActionResult> OnGetAsync()
        {
            
           

            return Page();
        }

        public async Task<ActionResult> OnPostAsync(long pid)
        {

            
                try
                {
                    
                        var wf = await _context.OrderItems.Where(x => x.ProductId == pid).ToListAsync();
                        foreach (var ifd in wf)
                        {
                            _context.OrderItems.Remove(ifd);

                        }

                        await _context.SaveChangesAsync();
                    
                }
                catch (Exception c) { }
               
               
                    var i = await _context.Products.FirstOrDefaultAsync(x => x.Id == pid);
           
                try
                {
                    var wd = await _context.ProductPictures.Where(x => x.ProductId == i.Id).ToListAsync();
                    foreach (var itg in wd)
                    {
                        _context.ProductPictures.Remove(itg);

                    }
                    await _context.SaveChangesAsync();
                    var wds = await _context.ProductColors.Where(x => x.ProductId == i.Id).ToListAsync();
                    foreach (var itg in wds)
                    {
                        _context.ProductColors.Remove(itg);

                    }
                    await _context.SaveChangesAsync();
                    var wdsf = await _context.ProductSizes.Where(x => x.ProductId == i.Id).ToListAsync();
                    foreach (var itg in wdsf)
                    {
                        _context.ProductSizes.Remove(itg);

                    }
                    await _context.SaveChangesAsync();

                    await _context.SaveChangesAsync();
                    var cp = await _context.ProductChecks.Where(x => x.ProductId == i.Id).ToListAsync();
                    foreach (var itg in cp)
                    {
                        _context.ProductChecks.Remove(itg);

                    }
                    await _context.SaveChangesAsync();
                    var csp = await _context.ProductCarts.Where(x => x.ProductId == i.Id).ToListAsync();
                    foreach (var itg in csp)
                    {
                        _context.ProductCarts.Remove(itg);

                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception s) { }

                _context.Products.Remove(i);
                await _context.SaveChangesAsync();
            

                  
                //
               

                TempData["success"] = "success";
           

            return Page();
        }

    }
}
