using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Wallets
{
   public interface IWalletRepository
    {
        Task<long> Insert(Wallet model);
        Task<Wallet> GetById(long? id);
        Task<Wallet> GetWallet(string userId);
        Task Delete(long? id);
        Task Update(Wallet model);
        Task<List<Wallet>> GetAsyncAll();
        Task<long> CreateAhiaPay(AhiapayDto data);

        Task<long> CreateAhiaPayAdmin(AhiapayAdminDto data);


    }
}
