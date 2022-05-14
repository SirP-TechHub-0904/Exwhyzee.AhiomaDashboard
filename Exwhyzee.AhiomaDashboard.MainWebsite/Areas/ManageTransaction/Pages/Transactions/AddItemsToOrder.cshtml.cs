using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Exwhyzee.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AhiomaDashboard.MainWebsite.Areas.ManageTransaction.Pages.Transactions
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,mSuperAdmin")]

    public class AddItemsToOrderModel : PageModel
    {
        private readonly AhiomaDbContext _context;

        public AddItemsToOrderModel(AhiomaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public long OrderId { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public IQueryable<OrderItem> OrderItems { get; set; }

        public List<SelectListItem> ColorListing { get; set; }
        public List<SelectListItem> SizeListing { get; set; }
        public async Task<IActionResult> OnGetAsync(long id, long pid = 0)
        {
            OrderId = id;
            Order = await _context.Orders.Include(x => x.UserProfile).Include(x => x.UserProfile.User).Include(x => x.OrderItems).FirstOrDefaultAsync(x => x.Id == id);
            if(Order == null)
            {
                return RedirectToPage("~/");
            }
             OrderItems = from s in _context.OrderItems
                                                  .Include(p => p.Product)
                                                  .Include(p => p.Product.Tenant)
                  .Where(x => x.OrderId == id)
                                              select s;

            //
            var p = await _context.Products.Include(x=>x.Tenant).Include(x => x.ProductColors).Include(x => x.ProductSizes).FirstOrDefaultAsync(x => x.Id == pid);
            if (p != null)
            {
                Product = p;
                if (p.UseColor == true)
                {
                    ColorListing = p.ProductColors.Select(a =>
                                      new SelectListItem
                                      {
                                          Value = a.ItemColor.ToString(),
                                          Text = a.ItemColor
                                      }).ToList();
                }
                if (p.UseSize == true)
                {
                    SizeListing = p.ProductSizes.Select(a =>
                                      new SelectListItem
                                      {
                                          Value = a.ItemSize.ToString(),
                                          Text = a.ItemSize
                                      }).ToList();
                }
            }


            return Page();
        }

        [BindProperty]
        public long ProductId { get; set; }
        [BindProperty]
        public long GetOrderId { get; set; }
       
        public async Task<IActionResult> OnPostFetchProduct()
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == GetOrderId);
            if(order == null)
            {
                TempData["error"] = "Order Not Found";
                return RedirectToPage("./Orders");
            }
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == ProductId);
            if (product == null)
            {
                TempData["error"] = "Product Not Found";
                return RedirectToPage("./AddItemsToOrder", new { id = GetOrderId });
            }
            //check
          var orderItems = from s in _context.OrderItems
                                           .Include(p => p.Product)
                                           .Include(p => p.Product.Tenant)
           .Where(x => x.OrderId == GetOrderId)
                         select s;

            if (orderItems.Select(x => x.ProductId).Contains(product.Id))
            {
                TempData["error"] = "Product alread in order.";
                return RedirectToPage("./AddItemsToOrder", new { id = GetOrderId });
            }
            TempData["success"] = "Product Found";
            return RedirectToPage("./AddItemsToOrder", new { id = GetOrderId, pid = product.Id });
           
        }
        [BindProperty]
        public long CProductId { get; set; }
        [BindProperty]
        public string CColor { get; set; }
        [BindProperty]
        public string CSize { get; set; }
        [BindProperty]
        public int CQuantity { get; set; }
        public async Task<IActionResult> OnPostAddProduct()
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == GetOrderId);
            if (order == null)
            {
                TempData["error"] = "Order Not Found";
                return RedirectToPage("./Orders");
            }
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == CProductId);
            if (product == null)
            {
                TempData["error"] = "Product Not Found";
                return RedirectToPage("./AddItemsToOrder", new { id = GetOrderId });
            }
            decimal? diff = (order.AmountPaid - order.LogisticAmount - (order.AmountMovedToCustomer + order.AmountMovedToAdmin + order.AdditionalPayment)) - order.OrderCostAfterProcessing;
            if ((product.Price * CQuantity) > diff)
            {
                TempData["error"] = "Insufficient fund";
                return RedirectToPage("OrderDetails", new { id = GetOrderId });
            }
            OrderItem item = new OrderItem();
            item.ProductId = CProductId;
            item.Price = product.Price;
            item.OrderId = order.Id;
            item.ReferenceId = order.ReferenceId;
            item.Status = OrderStatus.Processing;
            item.ShopStatus = ShopStatus.Pending;
            item.DateOfOrder = DateTime.UtcNow.AddHours(1);
            item.LogisticStatus = LogisticStatus.Pending;
            item.CustomerStatus = CustomerStatus.Pending;
            item.CustomerRef = "";
            item.Quantity = CQuantity;
            item.ItemSize = CSize;
            item.Itemcolor = CColor;
            item.DeliveryMethod = order.DeliveryMethod;
            item.PaymentMethod = order.PaymentMethod;
            _context.OrderItems.Add(item);
            try
            {
                await _context.SaveChangesAsync();
                TempData["success"] = "Product Added";
                return RedirectToPage("./OrderDetails", new { id = GetOrderId });
            }
            catch (Exception g)
            {
                TempData["error"] = "error";
                return RedirectToPage("./AddItemsToOrder", new { id = GetOrderId, pid = CProductId });
            }
        }

    }
}
