using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.UserProfileSocialMedias
{
    public class UserProfileSocialMediaRepository : IUserProfileSocialMediaRepository
    {
       
        private readonly AhiomaDbContext _context;

        public UserProfileSocialMediaRepository(AhiomaDbContext context)
        {
            _context = context;
        }
        public async Task Delete(long? id)
        {
            var data = await _context.UserProfileSocialMedias.FindAsync(id);
             _context.UserProfileSocialMedias.Remove(data);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserProfileSocialMedia>> GetAsyncAll()
        {
            var data = await _context.UserProfileSocialMedias.ToListAsync();
            return data;
        }

        public async Task<UserProfileSocialMedia> GetById(long? id)
        {
            var data = await _context.UserProfileSocialMedias.FirstOrDefaultAsync(x => x.Id == id);
            return data;
        }

        public async Task<long> Insert(UserProfileSocialMedia model)
        {
            _context.UserProfileSocialMedias.Add(model);
            await _context.SaveChangesAsync();
            return model.Id;
        }

        public async Task Update(UserProfileSocialMedia model)
        {
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
