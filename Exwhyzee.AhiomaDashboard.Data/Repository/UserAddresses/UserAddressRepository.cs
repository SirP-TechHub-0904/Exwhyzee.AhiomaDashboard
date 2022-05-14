using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.UserAddresses
{
    public class UserAddressRepository : IUserAddressRepository
    {
       
        private readonly AhiomaDbContext _context;

        public UserAddressRepository(AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.UserAddresses.FindAsync(id);
             _context.UserAddresses.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserAddress>> GetAsyncAll()
        {
            var data = await _context.UserAddresses.ToListAsync();
            return data;
        }

        public async Task<UserAddress> GetById(long? id)
        {
            var data = await _context.UserAddresses.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<long> Insert(UserAddress model)
        {
            _context.UserAddresses.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(UserAddress model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
