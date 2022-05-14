using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.UtilityBill
{
    public interface IBillRepository
    {
        Task<string> Date(string network, string number, string productCode);
        Task<string> Airtime(string network, string number, string amount);
        Task<string> ValidateCableCard(string promocode, string cardnumber);
        Task<string> Cable(string promocode, string cardnumber);

        Task<string> ValidateElectricityCard(string promocode, string cardnumber);
        Task<string> Electricity(string promocode, string cardnumber, string amount);
    }
}
