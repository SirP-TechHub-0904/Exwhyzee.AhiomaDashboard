using Exwhyzee.AhiomaDashboard.EntityFramework.Data;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.UtilityBill
{
    static class Constants
    {
        public const string url = "https://tranzit.com.ng/api/v2/";
        public const string api_key = "ad7ec35c069b5637d1343b631eab2ed5";
    }
    public class BillRepository : IBillRepository
    {
        private readonly AhiomaDbContext _context;

        public BillRepository(AhiomaDbContext context)
        {
            _context = context;
        }

        public async Task<string> Airtime(string network, string number, string amount)
        {

            try
            {
                string middle = $"airtime.php";
                string apiurl = Constants.url + middle + "?api_key=" + Constants.api_key + "&network=" + network + "&number=" + number + "&amount=" + amount;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                var client = new RestClient(apiurl);
                var request = new RestRequest(Method.GET);

                IRestResponse response = client.Execute(request);
                var contents = response.Content.ToString();
                return contents;
            }
            catch (Exception g)
            {
                return null;
            }
        }

        public Task<string> Cable(string promocode, string cardnumber)
        {
            throw new NotImplementedException();
        }

        public Task<string> Date(string network, string number, string productCode)
        {
            throw new NotImplementedException();
        }

        public Task<string> Electricity(string promocode, string cardnumber, string amount)
        {
            throw new NotImplementedException();
        }

        public Task<string> ValidateCableCard(string promocode, string cardnumber)
        {
            throw new NotImplementedException();
        }

        public Task<string> ValidateElectricityCard(string promocode, string cardnumber)
        {
            throw new NotImplementedException();
        }
    }
}
