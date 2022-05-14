using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Markets
{
    public class MarketRepository : IMarketRepository
    {
       
        private readonly AhiomaDbContext _context;

        public MarketRepository(AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.Markets.FindAsync(id);
             _context.Markets.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Market>> GetAsyncAll()
        {
            var data = await _context.Markets.Include(x=>x.User).ToListAsync();
            return data;
        }

        public async Task<Market> GetById(long? id)
        {
            var data = await _context.Markets.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<long> Insert(Market model)
        {
            _context.Markets.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(Market model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
