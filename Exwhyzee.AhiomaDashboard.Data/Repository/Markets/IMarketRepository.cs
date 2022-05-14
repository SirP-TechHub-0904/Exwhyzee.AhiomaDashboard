using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Markets
{
   public interface IMarketRepository
    {
        Task<long> Insert(Market model);
        Task<Market> GetById(long? id);
        Task Delete(long? id);
        Task Update(Market model);
        Task<List<Market>> GetAsyncAll();

    }
}
