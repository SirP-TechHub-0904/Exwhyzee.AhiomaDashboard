using Exwhyzee.AhiomaDashboard.Data.Dtos;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Categories
{
   public interface ICategoryRepository
    {
        Task<long> Insert(Category model);
        Task<Category> GetById(long? id);
        Task Delete(long? id);
        Task Update(Category model);
        Task<List<Category>> GetAsyncAll();
        Task<List<StoreCategoryDto>> GetAsyncCategoryByStoreAll(long? id);

    }
}
