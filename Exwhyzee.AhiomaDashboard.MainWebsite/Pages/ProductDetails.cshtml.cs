using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.Data.Repository.Products;
using Exwhyzee.AhiomaDashboard.Data.Repository.UserProfiles;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Pages
{
  //[Authorize]

    public class ProductDetailsModel : PageModel
    {
        private readonly IProductRepository _product;
        private readonly AhiomaDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IUserProfileRepository _account;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductDetailsModel(IProductRepository product, AhiomaDbContext context, IHostingEnvironment env,
            UserManager<IdentityUser> userManager, IUserProfileRepository account)
        {
            _context = context;
            _product = product;
            _hostingEnv = env;
            _userManager = userManager;
            _account = account;
        }


        [BindProperty]
        public Product Product { get; set; }

        public IList<ProductPicture> Pictures { get; set; }
        public IQueryable<ProductDto> SimilarProduct { get; set; }
        public IQueryable<Review> Reviews { get; set; }



        [BindProperty]
        public long ProductId { get; set; }
        [BindProperty]
        public string CustomerRef { get; set; }
        public async Task<IActionResult> OnGetAsync(string customerRef, long? id, string name, string shop, string mktstate, string mktaddress)
        {
            CustomerRef = customerRef;
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Include(p => p.Manufacturer)
                .Include(p => p.ProductPictures)
                .Include(p => p.Tenant.Market)
                .Include(p => p.Tenant.TenantAddresses)
                .Include(p => p.Tenant.Market)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .Include(p => p.Tenant.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Product == null)
            {
                return NotFound();
            }

            Pictures = await _product.GetProductPictureAsyncAll(id);
            ProductId = Product.Id;

            try
            {
                IQueryable<Product> productIQ = from s in _context.Products
                                              .Include(p => p.Category)
              .Include(p => p.Manufacturer)
              .Include(p => p.ProductPictures)
              .Include(p => p.Tenant.Market).OrderByDescending(x => x.CreatedOnUtc)
              .Where(x => x.Published == true).Where(x => x.CategoryId == Product.CategoryId)
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
                    Quantity = x.Quantity,
                    ImageThumbnail = x.ProductPictures.FirstOrDefault().PictureUrlThumbnail,
                    Published = x.Published,
                    Price = x.Price,
                    ShortDescription = x.ShortDescription

                });
                NewProductList = NewProductList.Where(x => x.ImageThumbnail != "noimage").OrderBy(a => Guid.NewGuid()).Take(12);

                SimilarProduct = NewProductList;

                IQueryable<Review> iReview = from s in _context.Reviews
                                            .Include(p => p.UserProfile).Take(10)
                                                select s;

                Reviews = iReview;
            }
            catch(Exception s) { 
            }
            
            return Page();
        }

       

              public IList<ProductCart> ProductCartsBUser { get; set; }

        public UserProfile UserProfile { get; set; }
        [BindProperty]
        public UserAddress UserAddress { get; set; }
        public List<SelectListItem> StateListing { get; set; }

        public async Task<IActionResult> OnPostBuyNow(long ProductId, string ItemSize, string ItemColor, int Quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            UserProfile = await _context.UserProfiles.Include(x => x.UserAddresses).FirstOrDefaultAsync(x => x.UserId == user.Id);
            ProductCart cart = new ProductCart();
            cart.ProductId = ProductId;
            cart.CartStatus = Enums.CartStatus.Active;
            cart.Quantity = Quantity;
            cart.Itemcolor = ItemColor;
            cart.ItemSize = ItemSize;
            cart.Date = DateTime.UtcNow.AddHours(1);
            cart.UserProfileId = UserProfile.Id;

            _context.ProductCarts.Add(cart);

            await _context.SaveChangesAsync();


            ProductCartsBUser = await _context.ProductCarts.Include(x => x.Product).Include(x => x.Product.ProductPictures).Where(x => x.UserProfileId == UserProfile.Id).ToListAsync();

            var address = await _context.UserAddresses.FirstOrDefaultAsync(x => x.UserId == UserProfile.UserId);


            if (address.Address != null)
            {
                return RedirectToPage("/Account/DeliveryMethod", new { area = "User" });
            }
            var state = await _account.GetStates();
            StateListing = state.Select(a =>
                                  new SelectListItem
                                  {
                                      Value = a.StateName,
                                      Text = a.StateName
                                  }).ToList();
            return RedirectToPage("/Account/DeliveryAddress", new { area = "User" });

        }


        [BindProperty]
        public string UserId { get; set; }
        [BindProperty]
        public int star { get; set; }
        [BindProperty]
        public string Comment { get; set; }

        [BindProperty]
        public string iProductId { get; set; }
        public async Task<IActionResult> OnPostAddReviewDefault()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return RedirectToPage("/Login");
                }

                var iu = await _userManager.FindByNameAsync(UserId);
                if(iu == null)
                {
                    return RedirectToPage("/Login");
                }
                var pr = await _context.UserProfiles.FirstOrDefaultAsync(x => x.UserId == iu.Id);
                Review rv = new Review();
                rv.UserProfileId = pr.Id;
                rv.UserId = iu.Id;
                rv.Rating = star;
                rv.Content = Comment;
                rv.Date = DateTime.UtcNow.AddHours(1);
                _context.Reviews.Add(rv);
                await _context.SaveChangesAsync();
             
                return RedirectToPage("/ProductDetails", new { id = iProductId });
            }
            catch (Exception c)
            {

                return RedirectToPage("/ProductDetails", new { id = iProductId });
            }
        }

    }
}