using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.TenantAddresses
{
   public interface ITenantAddressRepository
    {
        Task<long> Insert(TenantAddress model);
        Task<TenantAddress> GetById(long? id);
        Task Delete(long? id);
        Task Update(TenantAddress model);
        Task<List<TenantAddress>> GetAsyncAll();

    }
}
