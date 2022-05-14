using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.TenantSocialMedias
{
    public class TenantSocialMediaRepository : ITenantSocialMediaRepository
    {
       
        private readonly AhiomaDbContext _context;

        public TenantSocialMediaRepository(AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.TenantSocialMedias.FindAsync(id);
             _context.TenantSocialMedias.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TenantSocialMedia>> GetAsyncAll(long? id)
        {
            var data = await _context.TenantSocialMedias.Where(x=>x.TenantId == id).ToListAsync();
            return data;
        }

        public async Task<TenantSocialMedia> GetById(long? id)
        {
            var data = await _context.TenantSocialMedias.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<long> Insert(TenantSocialMedia model)
        {
            _context.TenantSocialMedias.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(TenantSocialMedia model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
