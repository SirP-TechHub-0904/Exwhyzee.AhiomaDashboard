using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Products
{
   public interface IProductRepository
    {
        Task<long> Insert(Product model);
        Task<Product> GetById(long? id);
        Task Delete(long? id);
        Task Update(Product model);
        Task<List<Product>> GetAsyncAll();
        Task<List<Product>> GetByStoreAsyncAll(long? id);
        Task<List<Product>> GetByStoreByUserAsyncAll(string id);
        Task<int> GetByStoreCount(long? id);

        Task<long> InsertImg(ProductPicture model);
        Task<ProductPicture> GetProductImgByProductId(long? id);
        Task DeleteProductImg(long? id);
        Task<List<ProductPicture>> GetProductPictureAsyncAll(long? id);

    }
}
