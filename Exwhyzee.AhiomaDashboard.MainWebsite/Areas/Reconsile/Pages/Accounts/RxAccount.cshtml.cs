using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.Reconsile.Pages.Accounts
{

    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin")]

    public class RxAccountModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AhiomaDbContext _context;

        public RxAccountModel(
            UserManager<IdentityUser> userManager, AhiomaDbContext context)
        {
            _userManager = userManager;
            _context = context;

        }


        public async Task<ActionResult> OnGetAsync()
        {
            
            return Page();
        }

        public async Task<ActionResult> OnPostAsync(string Email)
        {
            var emuser = await _userManager.FindByEmailAsync(Email);

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == emuser.Id);
            if (profile == null)
            {
                TempData["error"] = "Invalid User";
                return Page();
            }
            var user = await _userManager.FindByIdAsync(profile.UserId);
            if (user == null)
            {
                TempData["error"] = "Invalid User";
                return Page();
            }
            try
            {
                try
                {
                    var w = await _context.WalletHistories.Where(x => x.UserId == user.Id).ToListAsync();
                    foreach (var i in w)
                    {
                        _context.WalletHistories.Remove(i);

                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception c) { }
                //
                try
                {
                    var w = await _context.Orders.Where(x => x.UserProfileId == profile.Id).ToListAsync();
                    foreach (var i in w)
                    {
                        var wf = await _context.OrderItems.Where(x => x.OrderId == i.Id).ToListAsync();
                        foreach (var ifd in wf)
                        {
                            var wse = await _context.Transactions.Where(x => x.OrderItemId == ifd.Id).ToListAsync();
                            foreach (var ifs in wse)
                            {
                                _context.Transactions.Remove(ifs);

                            }
                            await _context.SaveChangesAsync();
                            _context.OrderItems.Remove(ifd);

                        }

                        _context.Orders.Remove(i);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception c) { }
                //
                try
                {
                    var w = await _context.Transactions.Where(x => x.UserId == user.Id).ToListAsync();
                    foreach (var i in w)
                    {
                        _context.Transactions.Remove(i);

                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception c) { }
                try
                {
                    var w = await _context.AhiaPayTransfers.Where(x => x.UserId == user.Id).ToListAsync();
                    foreach (var i in w)
                    {
                        _context.AhiaPayTransfers.Remove(i);

                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception c) { }
                try
                {
                    var w = await _context.UserProfileSocialMedias.Where(x => x.UserId == user.Id).ToListAsync();
                    foreach (var i in w)
                    {
                        _context.UserProfileSocialMedias.Remove(i);

                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception c) { }
                try
                {
                    var w = await _context.UserReferees.Where(x => x.UserId == user.Id).ToListAsync();
                    foreach (var i in w)
                    {
                        _context.UserReferees.Remove(i);

                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception c) { }
             
                //
                try
                {
                    var wal = await _context.Wallets.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    _context.Wallets.Remove(wal);
                    await _context.SaveChangesAsync();
                }
                catch (Exception d) { }
                //
                try
                {
                    var tennt = await _context.Tenants.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    var w = await _context.Products.Where(x => x.TenantId == tennt.Id).ToListAsync();
                    foreach (var i in w)
                    {
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
                            var wdsff = await _context.ProductUploadShops.Where(x => x.TenantId == tennt.Id).ToListAsync();
                            foreach (var itg in wdsff)
                            {
                                _context.ProductUploadShops.Remove(itg);

                            }
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
                        catch(Exception s) { }
                       
                        _context.Products.Remove(i);

                    }

                  
                }
                catch (Exception c) { }
                //
                try
                {
                    var tennt = await _context.Tenants.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    var w = await _context.TenantAddresses.Where(x => x.TenantId == tennt.Id).ToListAsync();
                    foreach (var i in w)
                    {
                        _context.TenantAddresses.Remove(i);

                    }
                    await _context.SaveChangesAsync();
                    var wd = await _context.TenantSocialMedias.Where(x => x.TenantId == tennt.Id).ToListAsync();
                    foreach (var i in wd)
                    {
                        _context.TenantSocialMedias.Remove(i);

                    }
                    await _context.SaveChangesAsync();

                    var weed = await _context.StoreCategories.Where(x => x.TenantId == tennt.Id).ToListAsync();
                    foreach (var i in weed)
                    {
                        _context.StoreCategories.Remove(i);

                    }
                    await _context.SaveChangesAsync();
                    var tennts = await _context.TenantSettings.FirstOrDefaultAsync(x => x.TenantId == tennt.Id);
                    _context.TenantSettings.Remove(tennts);
                    _context.Tenants.Remove(tennt);
                    await _context.SaveChangesAsync();
                }
                catch (Exception c) { }
                //
                try
                {
                    var wal = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    var csp = await _context.ProductCarts.Where(x => x.UserProfileId == wal.Id).ToListAsync();
                    foreach (var itg in csp)
                    {
                        _context.ProductCarts.Remove(itg);

                    }
                    await _context.SaveChangesAsync();
                    var cspd = await _context.UserAddresses.Where(x => x.UserProfileId == wal.Id).ToListAsync();
                    foreach (var itg in csp)
                    {
                        _context.ProductCarts.Remove(itg);

                    }
                    await _context.SaveChangesAsync();

                    var cspdd = await _context.Markets.Where(x => x.UserId == wal.UserId).ToListAsync();
                    foreach (var itg in cspdd)
                    {
                        _context.Markets.Remove(itg);

                    }
                    await _context.SaveChangesAsync();
                    _context.UserProfiles.Remove(wal);
                    await _context.SaveChangesAsync();
                }
                catch (Exception d) { }

                

                try
                {
                    var rolesd = await _userManager.GetRolesAsync(user);
                    var wal = await _userManager.RemoveFromRolesAsync(user, rolesd);

                }
                catch (Exception d) { }
                try
                {
                    var wal = await _userManager.DeleteAsync(user);

                }
                catch (Exception d) { }

                TempData["success"] = "success";
            }
            catch (Exception d) { }

            return Page();
        }

    }
}
