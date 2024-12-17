using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.Models.Request
{
    public class ScheduleRequest
    {
        public int DoctorID { get; set; }
        public int DayOfWeek { get; set; }

        public List<ShiftRequest> Shifts { get; set; }

    }

    public class ShiftRequest
    {
        public string Description { get; set; }
        public Nullable<int> BlockDurationMinutes { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public System.TimeSpan EndTime { get; set; }

    }
}