using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.TenantSocialMedias
{
   public interface ITenantSocialMediaRepository
    {
        Task<long> Insert(TenantSocialMedia model);
        Task<TenantSocialMedia> GetById(long? id);
        Task Delete(long? id);
        Task Update(TenantSocialMedia model);
        Task<List<TenantSocialMedia>> GetAsyncAll(long? id);

    }
}
