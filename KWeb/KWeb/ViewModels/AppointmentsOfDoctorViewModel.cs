using KWeb.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.ViewModels
{
    public class AppointmentsOfDoctorViewModel
    {
        public List<Appointments> AppointmentsScheduled{ get; set; }
        public List<Appointments> AppointmentsToday { get; set; }
    }
}