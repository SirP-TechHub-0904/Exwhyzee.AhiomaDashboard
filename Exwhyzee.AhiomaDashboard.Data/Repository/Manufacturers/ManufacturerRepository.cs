using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Manufacturers
{
    public class ManufacturerRepository : IManufacturerRepository
    {
       
        private readonly AhiomaDbContext _context;

        public ManufacturerRepository(AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.Manufacturers.FindAsync(id);
             _context.Manufacturers.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Manufacturer>> GetAsyncAll()
        {
            var data = await _context.Manufacturers.ToListAsync();
            return data;
        }

        public async Task<Manufacturer> GetById(long? id)
        {
            var data = await _context.Manufacturers.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<long> Insert(Manufacturer model)
        {
            _context.Manufacturers.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(Manufacturer model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
