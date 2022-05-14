﻿using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Batches
{
    public class BatchRepository : IBatchRepository
    {
       
        private readonly AhiomaDbContext _context;

        public BatchRepository(AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.Batches.FindAsync(id);
             _context.Batches.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Batch>> GetAsyncAll()
        {
            var data = await _context.Batches.ToListAsync();
            return data;
        }

        public async Task<Batch> GetById(long? id)
        {
            var data = await _context.Batches.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<long> Insert(Batch model)
        {
            _context.Batches.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(Batch model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
