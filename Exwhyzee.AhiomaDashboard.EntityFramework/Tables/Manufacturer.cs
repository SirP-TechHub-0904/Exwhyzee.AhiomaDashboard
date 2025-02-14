﻿using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class Manufacturer
    {
       
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public string Address { get; set; }

        public EntityStatus Status { get; set; }


    }
}
