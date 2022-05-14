using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Tenants
{
   public interface ITenantRepository
    {
        Task<long> Insert(Tenant model);
        Task<Tenant> GetById(long? id);
        Task<Tenant> GetByIdHandle(string handle);
        Task Delete(long? id);
        Task Update(Tenant model);
        Task<List<Tenant>> GetAsyncAll();
        Task<List<Tenant>> GetAsyncAllBySOA(string id);
        Task<Tenant> GetByLogin(string id);

    }
}
