using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.TenantSettings
{
    public class TenantSettingRepository : ITenantSettingRepository
    {
       
        private readonly AhiomaDbContext _context;

        public TenantSettingRepository(AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.TenantSettings.FindAsync(id);
             _context.TenantSettings.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TenantSetting>> GetAsyncAll()
        {
            var data = await _context.TenantSettings.ToListAsync();
            return data;
        }

        public async Task<TenantSetting> GetById(long? id)
        {
            var data = await _context.TenantSettings.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<long> Insert(TenantSetting model)
        {
            _context.TenantSettings.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(TenantSetting model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
