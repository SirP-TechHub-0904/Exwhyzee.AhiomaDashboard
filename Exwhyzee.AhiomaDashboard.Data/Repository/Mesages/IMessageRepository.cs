using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Mesages
{
    public interface IMessageRepository
    {
        Task<bool> AddMessage(AddMessageDto sms);
        Task<string> GetMessage(ContentType contentType);
    }
}
