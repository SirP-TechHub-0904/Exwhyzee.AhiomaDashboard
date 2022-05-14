using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Batches
{
   public interface IBatchRepository
    {
        Task<long> Insert(Batch model);
        Task<Batch> GetById(long? id);
        Task Delete(long? id);
        Task Update(Batch model);
        Task<List<Batch>> GetAsyncAll();

    }
}
