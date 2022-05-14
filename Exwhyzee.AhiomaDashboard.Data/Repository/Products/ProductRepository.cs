using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Products
{
    public class ProductRepository : IProductRepository
    {
       
        private readonly AhiomaDbContext _context;

        public ProductRepository(AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.Products.FindAsync(id);
             _context.Products.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductImg(long? id)
        {
            var data = await _context.ProductPictures.FindAsync(id);
            _context.ProductPictures.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAsyncAll()
        {
            var data = await _context.Products.ToListAsync();
            return data;
        }

        public async Task<Product> GetById(long? id)
        {
            var data = await _context.Products.Include(x=>x.Tenant).FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<List<Product>> GetByStoreAsyncAll(long? id)
        {
         
                var data = await _context.Products.Where(x=>x.TenantId == id).ToListAsync();
                return data;
            
        }

        public async Task<List<Product>> GetByStoreByUserAsyncAll(string id)
        {

            IQueryable<Product> productIQ = from s in _context.Products
                                                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(x => x.ProductPictures)
                .Include(x => x.Tenant.UserProfile)
                .Include(x => x.Tenant).Where(x => x.Tenant.CreationUserId == id)
                                            select s;

            var data = await productIQ.ToListAsync();
            return data;
        }

        public async Task<int> GetByStoreCount(long? id)
        {

            var data = await _context.Products.Where(x => x.TenantId == id).CountAsync();
            return data;

        }

        public async Task<ProductPicture> GetProductImgByProductId(long? id)
        {
            var data = await _context.ProductPictures.FirstOrDefaultAsync(x => x.ProductId == id && x.IsDefault == true);
            return data;
        }

        public async Task<List<ProductPicture>> GetProductPictureAsyncAll(long? id)
        {
            var data = await _context.ProductPictures.Where(x=>x.ProductId == id).ToListAsync();
            return data;
        }

        public async Task<long> Insert(Product model)
        {
            _context.Products.Add(model);
            await _context.SaveChangesAsync();
            var product = await _context.Products.FindAsync(model.Id);
            Purchase purchase = new Purchase();
            purchase.DateEntered = DateTime.UtcNow.AddHours(1);
            purchase.ProductId = product.Id;
            purchase.Quantity = product.Quantity;
            purchase.UnitSellingPrice = product.Price;
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task<long> InsertImg(ProductPicture model)
        {
            _context.ProductPictures.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(Product model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
