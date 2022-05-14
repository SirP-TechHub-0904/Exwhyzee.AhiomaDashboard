using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.UserProfileSocialMedias
{
   public interface IUserProfileSocialMediaRepository
    {
        Task<long> Insert(UserProfileSocialMedia model);
        Task<UserProfileSocialMedia> GetById(long? id);
        Task Delete(long? id);
        Task Update(UserProfileSocialMedia model);
        Task<List<UserProfileSocialMedia>> GetAsyncAll();

    }
}
