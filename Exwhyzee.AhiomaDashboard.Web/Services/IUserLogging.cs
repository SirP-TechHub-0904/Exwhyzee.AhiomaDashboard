using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Services
{
    public interface IUserLogging
    {
        Task<string> LogData(string userid, string data1, string data2);

    }
}
