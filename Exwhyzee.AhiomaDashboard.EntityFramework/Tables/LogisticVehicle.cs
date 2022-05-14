using Exwhyzee.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Exwhyzee.AhiomaDashboard.EntityFramework.Tables
{
    public class LogisticVehicle
    { 
        public long Id { get; set; }
        [Display(Name = "Vehicle Name")]

        public string VehicleName { get; set; }
        [Display(Name = "Vehicle Plate Number")]

        public string VehiclePlateNumber { get; set; }
        [Display(Name = "Vehicle Phone Number")]

        public string VehiclePhoneNumber { get; set; }
        [Display(Name = "Vehicle Size")]

        public string VehicleSize { get; set; }
        [Display(Name = "Vehicle Weight")]

        public string VehicleWeight { get; set; }
        [Display(Name = "Vehicle Type")]

        public string VehicleType { get; set; }

        public string State { get; set; }
        public string LGA { get; set; }
        [Display(Name = "Least Amount")]

        public decimal LeastAmount { get; set; }
        [Display(Name = "Vehicle Status")]

        public VehicleEnum VehicleStatus { get; set; }
        [Display(Name = "Logistic Profile")]

        public long LogisticProfileId { get; set; }
        public LogisticProfile LogisticProfile { get; set; }
        public string VehicleImage { get; set; }
    }
}
