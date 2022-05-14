using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Manufacturers
{
   public interface IManufacturerRepository
    {
        Task<long> Insert(Manufacturer model);
        Task<Manufacturer> GetById(long? id);
        Task Delete(long? id);
        Task Update(Manufacturer model);
        Task<List<Manufacturer>> GetAsyncAll();

    }
}
