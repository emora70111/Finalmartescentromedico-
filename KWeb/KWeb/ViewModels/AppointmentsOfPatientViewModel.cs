using KWeb.Models.Request;
using KWeb.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.ViewModels
{
    public class AppointmentsOfPatientViewModel
    {        
        public List<Doctors> Doctors{ get; set; }
        public AppointmentRequest AppointmentRequest { get; set; }
    }
}