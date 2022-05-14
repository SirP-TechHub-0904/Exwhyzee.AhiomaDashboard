using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.Web.Areas.User.Pages.Account
{
    [Authorize]
    public class DeliveryAddressModel : PageModel
    {
        private readonly AhiomaDbContext _context;
        private readonly IUserProfileRepository _account;
        private readonly UserManager<IdentityUser> _userManager;


        public DeliveryAddressModel(AhiomaDbContext context, UserManager<IdentityUser> userManager, IUserProfileRepository account

            )
        {
            _context = context;
            _userManager = userManager;
            _account = account;
        }
        public UserProfile UserProfile { get; set; }
        [BindProperty]
        public UserAddress UserAddress { get; set; }

        [BindProperty]
        public string Surname { get; set; }
        [BindProperty]
        public string FirstName { get; set; }
        public IList<ProductCart> ProductCartsBUser { get; set; }


        public List<SelectListItem> AddressListing { get; set; }
        public List<SelectListItem> StateListing { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }

        [BindProperty]
        public bool NoAddress { get; set; }
        [BindProperty]
        public long AddressUId { get; set; }

        [BindProperty]
        public long AddressUpdateId { get; set; }

        
        public async Task<IActionResult> OnGetAsync(string customerRef)
        {
            CustomerRef = customerRef;
            var user = await _userManager.GetUserAsync(User);
            UserProfile = await _context.UserProfiles.Include(x=>x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
            var cartsessionid = HttpContext.Session.GetString("CartUserId");
           var ProductCarts = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.CartTempId == cartsessionid).ToListAsync();
            try
            {
                foreach (var p in ProductCarts)
                {
                    p.UserProfileId = UserProfile.Id;
                    p.CartTempId = "";
                    p.CartStatus = Enums.CartStatus.Active;
                    _context.Attach(p).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
            }catch(Exception d)
            {

            }
            ProductCartsBUser = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == UserProfile.Id).ToListAsync();

            var address = await _context.UserAddresses.FirstOrDefaultAsync(x => x.UserId == UserProfile.UserId);
            
            HttpContext.Session.Remove("CartUserId");
            if (address == null)
            {
                NoAddress = true;
            }
            else if(address.Address == null)
            {
                return RedirectToPage("./NewUserInfo", new { id = address.Id, uxi = user.Id });
            }
            var chch = UserProfile.UserAddresses.FirstOrDefault(x => x.Default == true);
            if(chch == null)
            {
                var ichch = UserProfile.UserAddresses.FirstOrDefault();
                if(ichch == null)
                {
                    var addressli = await _context.UserAddresses.Where(x => x.UserId == UserProfile.UserId).OrderByDescending(x => x.Default).ToListAsync();
                    AddressListing = addressli.Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.Id.ToString(),
                                              Text = a.Address + " in " + a.LocalGovernment + " " + a.State
                                          }).ToList();


                    var statei = await _account.GetStates();
                    StateListing = statei.Select(a =>
                                          new SelectListItem
                                          {
                                              Value = a.StateName,
                                              Text = a.StateName
                                          }).ToList();
                    return Page();
                }
                var data = await _context.UserAddresses.FirstOrDefaultAsync(x=>x.UserProfileId == UserProfile.Id);
                
                data.Default = true;

                _context.Entry(data).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
           
            var addressl = await _context.UserAddresses.Where(x => x.UserId == UserProfile.UserId).OrderByDescending(x=>x.Default).ToListAsync();
            AddressListing = addressl.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.Id.ToString(),
                                      Text = a.Address +" in "+ a.LocalGovernment +" "+ a.State
                                  }).ToList();


            var state = await _account.GetStates();
            StateListing = state.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.StateName,
                                      Text = a.StateName
                                  }).ToList();
            return Page();
        }
        [BindProperty]
        public long AddressId { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            var data = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == AddressId);
            IQueryable<UserAddress> check = from s in _context.UserAddresses
                                           .Where(x => x.UserId == data.UserId && x.Default == true)

                                            select s;
            foreach (var i in check)
            {
                i.Default = false;
                _context.Entry(i).State = EntityState.Modified;

            }
            data.Default = true;

            _context.Entry(data).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToPage("/Account/DeliveryMethod", new { area = "User" });
        }

        public async Task<IActionResult> OnPostAddAddress()
        {
            try
            {
                string userid = _userManager.GetUserId(HttpContext.User);
                var profiledata = await _account.GetByUserId(userid);
                UserAddress.UserId = userid;
                UserAddress.UserProfileId = profiledata.Id;
                UserAddress.Default = true;
                _context.UserAddresses.Add(UserAddress);
                IQueryable<UserAddress> check = from s in _context.UserAddresses
                                               .Where(x => x.UserId == userid && x.Default == true)

                                                select s;
                foreach (var i in check)
                {
                    i.Default = false;
                    _context.Entry(i).State = EntityState.Modified;

                }
                await _context.SaveChangesAsync();

                TempData["success"] = "Address Successfully Updated";
                return RedirectToPage("/Account/DeliveryMethod", new { area = "User" });
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostUpdateAddress()
        {
            try
            {

                var maddress = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == AddressUpdateId);
                maddress.Address = UserAddress.Address;
                maddress.State = UserAddress.State;
                maddress.LocalGovernment = UserAddress.LocalGovernment;
                _context.Entry(maddress).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["success"] = "Address Updated";
                return RedirectToPage("/Account/DeliveryMethod", new { area = "User" });
            }
            catch (Exception c)
            {
                TempData["error"] = "Update failed";
                return RedirectToPage();
            }
        }
    }
}
