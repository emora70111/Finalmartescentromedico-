using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.Models.Request
{
    public class AppointmentRequest
    {
        public int PatientID { get; set; }
        public int DoctorID { get; set; }
        public System.DateTime Date { get; set; }
        public System.TimeSpan Time { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}