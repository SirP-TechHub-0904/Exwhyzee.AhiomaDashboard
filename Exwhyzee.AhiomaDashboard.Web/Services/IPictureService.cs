using Exwhyzee.AhiomaDashboard.EntityFramework.Tables;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Web.Services
{
    public interface IPictureService
    {
        Task<bool> InsertPicture(byte[] pictureBinary, string mimeType, long ProductId, string path, string extension);
    }
}
