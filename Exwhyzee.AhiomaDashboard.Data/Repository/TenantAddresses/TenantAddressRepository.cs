using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.TenantAddresses
{
    public class TenantAddressRepository : ITenantAddressRepository
    {
       
        private readonly AhiomaDbContext _context;

        public TenantAddressRepository(AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.TenantAddresses.FindAsync(id);
             _context.TenantAddresses.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TenantAddress>> GetAsyncAll()
        {
            var data = await _context.TenantAddresses.ToListAsync();
            return data;
        }

        public async Task<TenantAddress> GetById(long? id)
        {
            var data = await _context.TenantAddresses.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<long> Insert(TenantAddress model)
        {
            _context.TenantAddresses.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(TenantAddress model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
