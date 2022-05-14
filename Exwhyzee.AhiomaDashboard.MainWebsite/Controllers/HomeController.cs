using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Dtos;
using Exwhyzee.AhiomaDashboard.Data.Flutter.BankInfo;
using Exwhyzee.AhiomaDashboard.Data.Repository.Flutter;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserProfileRepository _account;
        private readonly AhiomaDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IFlutterTransactionService _flutterTransactionAppService;

        public HomeController(IUserProfileRepository account, IFlutterTransactionService flutterTransactionAppService, SignInManager<IdentityUser> signInManager, AhiomaDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _account = account;
            _signInManager = signInManager;
            _userManager = userManager;
            _flutterTransactionAppService = flutterTransactionAppService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public List<SelectListItem> LgaList { get; set; }
        public async Task<IActionResult> GetLgaList(string id)
        {
            List<LocalGoverment> lga = new List<LocalGoverment>();

            var query = await _account.GetLGA(id);


            LgaList = query.Select(a =>
                                new SelectListItem
                                {
                                    Value = a.LGAName,
                                    Text = a.LGAName
                                }).ToList();
            return new JsonResult(LgaList);
        }

        public List<SelectListItem> CompanyList { get; set; }
        public async Task<IActionResult> GetCompanyList(long id)
        {
            List<LogisticVehicle> logis = new List<LogisticVehicle>();

            var query = await _context.LogisticVehicle.Where(x => x.LogisticProfileId == id).ToListAsync();


            CompanyList = query.Select(a =>
                                new SelectListItem
                                {
                                    Value = a.Id.ToString(),
                                    Text = a.VehicleName +" ("+a.VehicleType+")"
                                }).ToList();
            return new JsonResult(CompanyList);
        }

        //
        public List<SelectListItem> SubList { get; set; }

        public async Task<IActionResult> SubCategoryList(long id)
        {
            List<SubCategory> category = new List<SubCategory>();

            var query = await _context.SubCategories.Where(x => x.CategoryId == id).ToListAsync();


            SubList = query.Select(a =>
                                new SelectListItem
                                {
                                    Value = a.Id.ToString(),
                                    Text = a.Name
                                }).ToList();
            return new JsonResult(SubList);
        }

        [HttpPost]
        public List<string> OnChange(string name)
        {
            //return db.Customers.Where(C => C.CustomerID == id).ToString();
            return _context.UserProfiles.Where(x => x.Fullname.Contains(name)).Select(x=>x.Fullname).ToList();
        }

        public string GetUserInfoByPhone(string phone)
        {
            //return db.Customers.Where(C => C.CustomerID == id).ToString();
            var user = _context.Users.FirstOrDefault(x => x.PhoneNumber == phone);
            var profile = _context.UserProfiles.FirstOrDefault(x => x.UserId == user.Id);
            return profile.Fullname+ "("+profile.IdNumber+")";
        }

        //public async Task<BankInformation> GetAccount(string number, string bank)
        //{
        //    var checkaccount = await _flutterTransactionAppService.AccountInfomation(number, bank);
        //    string account = checkaccount.data.name
        //    //return db.Customers.Where(C => C.CustomerID == id).ToString();
        //    return "";
        //}


        public async Task<ActionResult> UpdateProduct(long ProductId)
        {
            string data = "";
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == ProductId);
            if(product.Published == true)
            {
                product.Published = false;
            }
            else
            {
                product.Published = true;
            }
            _context.Attach(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                data = "successful";
            }
            catch(Exception o) {
                data = "unsuccessful";
            }
            
            return new JsonResult(data);
        }
       
        public async Task<ActionResult> Addtocart(long id, string itemcolor = "", string itemsize = "", int quantity = 0, string ilink = "")
        {
            if (quantity > 0)
            {
                HttpContext.Session.SetString("XXXXX", Guid.NewGuid().ToString());
                string trackcart = HttpContext.Session.GetString("XXXXX");
                trackcart = trackcart.Replace("-", "");
                var newcartsessionid = "";
                var cartsessionid = "";
                long profileId = 0;


                string CheckTrackOrderId = HttpContext.Session.GetString("TrackOrderId");
                if (CheckTrackOrderId == null)
                {
                    HttpContext.Session.SetString("TrackOrderId", Guid.NewGuid().ToString());
                }
                string TrackOrderId = HttpContext.Session.GetString("TrackOrderId");
                if (_signInManager.IsSignedIn(User))
                {
                    var user = await _userManager.GetUserAsync(User);
                    var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == user.Id);
                    profileId = profile.Id;
                }
                else
                {
                    cartsessionid = HttpContext.Session.GetString("CartUserId");
                   // return RedirectToPage("/Login", new { returnUrl = "~/" });
                   // return new JsonResult("Login");

                }
                if (cartsessionid == null)
                {
                    HttpContext.Session.SetString("CartUserId", Guid.NewGuid().ToString());
                    newcartsessionid = HttpContext.Session.GetString("CartUserId");
                    cartsessionid = newcartsessionid;
                }


                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (product != null)
                {

                    var cartitem = await _context.ProductCarts.Where(x => x.CartStatus == Enums.CartStatus.Active).ToListAsync();
                    ProductCart logcartitem = new ProductCart();
                    if (_signInManager.IsSignedIn(User))
                    {
                        logcartitem = cartitem.FirstOrDefault(x => x.UserProfileId == profileId && x.ProductId == product.Id);
                    }
                    else
                    {
                        logcartitem = cartitem.FirstOrDefault(x => x.CartTempId == cartsessionid && x.ProductId == product.Id);
                    }
                    if (logcartitem != null)
                    {
                        logcartitem.Quantity = logcartitem.Quantity + quantity;
                        _context.Attach(logcartitem).State = EntityState.Modified;

                    }
                    else
                    {
                        ProductCart cart = new ProductCart();
                        cart.ProductId = id;
                        cart.TrackOrderId = TrackOrderId;
                        cart.CartStatus = Enums.CartStatus.Active;
                        cart.Quantity = quantity;
                        cart.TrackCartId = trackcart;
                        cart.Itemcolor = itemcolor;
                        cart.ItemSize = itemsize;
                        cart.Date = DateTime.UtcNow.AddHours(1);
                        if (_signInManager.IsSignedIn(User))
                        {
                            cart.UserProfileId = profileId;
                        }
                        else
                        {
                            cart.CartTempId = cartsessionid;
                        }
                        _context.ProductCarts.Add(cart);
                    }
                    await _context.SaveChangesAsync();
                }




                return new JsonResult("success");
            }
            else
            {
                return new JsonResult("fail");

            }
        }

        public async Task<ActionResult> CartList()
        {
            var cartsessionid = HttpContext.Session.GetString("CartUserId");
            var cart = await _context.ProductCarts.Include(x=>x.Product).Where(x => x.CartTempId == cartsessionid && x.CartStatus == Enums.CartStatus.Active).ToListAsync();

            return new JsonResult(cart);

        }

    }
}
