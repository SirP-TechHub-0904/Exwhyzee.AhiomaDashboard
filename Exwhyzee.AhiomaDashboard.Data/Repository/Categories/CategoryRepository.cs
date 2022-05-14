using Exwhyzee.AhiomaDashboard.Data.Dtos;
using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
       
        private readonly AhiomaDbContext _context;

        public CategoryRepository(AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.Categories.FindAsync(id);
             _context.Categories.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAsyncAll()
        {
            var data = await _context.Categories.Include(x=>x.SubCategories).ToListAsync();
            return data;
        }

        public async Task<List<StoreCategoryDto>> GetAsyncCategoryByStoreAll(long? id)
        {
            var categorystore = await _context.StoreCategories.Include(x=>x.Category).Where(x => x.TenantId == id).ToListAsync();

            var data = categorystore.Select(x => new StoreCategoryDto
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name
            }).ToList();
            return data;
        }

        public async Task<Category> GetById(long? id)
        {
            var data = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<long> Insert(Category model)
        {
            _context.Categories.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(Category model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
