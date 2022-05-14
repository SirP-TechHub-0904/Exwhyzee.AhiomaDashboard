using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Tenants
{
    public class TenantRepository : ITenantRepository
    {
       
        private readonly AhiomaDbContext _context;

        public TenantRepository(AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.Tenants.FindAsync(id);
             _context.Tenants.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Tenant>> GetAsyncAll()
        {
            var data = await _context.Tenants.ToListAsync();
            return data;
        }

        public async Task<List<Tenant>> GetAsyncAllBySOA(string id)
        {
            var data = await _context.Tenants.Where(x=>x.CreationUserId == id).ToListAsync();
            return data;
        }

        public async Task<Tenant> GetById(long? id)
        {
            var data = await _context.Tenants.Include(x=>x.TenantAddresses).Include(x => x.UserProfile).Include(x => x.Market).Include(x => x.UserProfile.User).Include(x=>x.TenantSocialMedias).FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<Tenant> GetByIdHandle(string handle)
        {
            var data = await _context.Tenants.Include(x => x.TenantAddresses).Include(x => x.Market).Include(x => x.UserProfile).Include(x => x.UserProfile.User).Include(x => x.TenantSocialMedias).FirstOrDefaultAsync(x => x.TenentHandle == handle);
            return data;
        }

        public async Task<Tenant> GetByLogin(string id)
        {
            var data = await _context.Tenants.Include(x => x.TenantAddresses).Include(x => x.UserProfile).Include(x => x.UserProfile.User).Include(x => x.TenantSocialMedias).FirstOrDefaultAsync(x => x.UserId == id);
            return data;
        }

        public async Task<long> Insert(Tenant model)
        {
            _context.Tenants.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(Tenant model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
