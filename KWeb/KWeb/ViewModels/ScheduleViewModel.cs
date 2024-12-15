using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.ViewModels
{
    public class ScheduleViewModel
    {
        public int ScheduleID { get; set; }
        public int DoctorID { get; set; }
        public string Status { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DayOfWeek { get; set; }
    }
}