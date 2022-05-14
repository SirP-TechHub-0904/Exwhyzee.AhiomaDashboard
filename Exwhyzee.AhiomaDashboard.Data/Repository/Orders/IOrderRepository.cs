using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Orders
{
    public interface IOrderRepository
    {
        Task<string> Insert(string source, string tranxRef, 
            string transaction_id, string status, string customerRef, string orderid, 
            string ahiapaystatus, string Ahia_transac_Id, string transactiontype, string from, string skip);

        Task<string> ProcessOrderToLedger(long OrderId);

        Task<string> ProcessOrderToWithdrawable(long OrderId);
        

    }
}
