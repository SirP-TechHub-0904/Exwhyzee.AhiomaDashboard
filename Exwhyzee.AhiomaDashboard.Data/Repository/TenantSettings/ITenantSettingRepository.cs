using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.TenantSettings
{
   public interface ITenantSettingRepository
    {
        Task<long> Insert(TenantSetting model);
        Task<TenantSetting> GetById(long? id);
        Task Delete(long? id);
        Task Update(TenantSetting model);
        Task<List<TenantSetting>> GetAsyncAll();

    }
}
