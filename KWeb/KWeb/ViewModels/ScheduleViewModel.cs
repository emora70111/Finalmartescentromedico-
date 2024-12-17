using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KWeb.ViewModels
{
    public class ScheduleViewModel
    {
        public int ScheduleID { get; set; }
        public int DayOfWeek { get; set; }
        public List<ShiftViewModel> Shifts { get; set; }
    }

    public class ShiftViewModel
    {
        public int ShiftID { get; set; }
        public string Description { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? BlockDurationMinutes { get; set; }
        public List<BlockViewModel> Blocks { get; set; }
    }

    public class BlockViewModel
    {
        public int BlockID { get; set; }
        public int ShiftID { get; set; }
        public TimeSpan BlockStartTime { get; set; }
        public TimeSpan BlockEndTime { get; set; }
        public string Status { get; set; }
    }
}