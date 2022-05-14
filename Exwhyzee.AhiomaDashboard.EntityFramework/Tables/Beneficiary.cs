using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class Beneficiary
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public UtilityParam ParamType { get; set; }
        public string BeneficiaryValue { get; set; }
        public string Title { get; set; }
    }
}
