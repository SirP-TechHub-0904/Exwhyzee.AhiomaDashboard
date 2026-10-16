using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class ApiSyncMap
    {
        public long Id { get; set; }                  // local tracking Id

        public string EntityType { get; set; }        // "Category", "Product", "Variant", "Image"
        public long LocalEntityId { get; set; }       // your EF Core Id
        public string RemoteEntityId { get; set; }    // returned from Ahioma API (string Ids)

        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastSyncedOnUtc { get; set; }
        public bool Success { get; set; }             // did sync succeed?
        public string ErrorMessage { get; set; }      // store failure reasons if any
    }
}
