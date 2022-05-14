using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.UserAddresses
{
   public interface IUserAddressRepository
    {
        Task<long> Insert(UserAddress model);
        Task<UserAddress> GetById(long? id);
        Task Delete(long? id);
        Task Update(UserAddress model);
        Task<List<UserAddress>> GetAsyncAll();

    }
}
