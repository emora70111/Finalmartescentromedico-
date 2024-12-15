using KWeb.ModelsDB;
using KWeb.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KWeb.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly CONSULTORIO_VIDA_SALUDEntities _context;

        public SchedulesController()
        {
            _context = new CONSULTORIO_VIDA_SALUDEntities();
        }
        public ActionResult Index()
        {

            var userJson = HttpContext.Session["user"] as string;
            var user = JsonConvert.DeserializeObject<Users>(userJson);

            var schedules = _context.Schedules
             .Where(s => s.DoctorID == user.UserID)
             .Select(s => new ScheduleViewModel
             {
                 ScheduleID = s.ScheduleID,
                 Status = _context.Appointments.FirstOrDefault(a => a.Time == s.StartTime) == null ? "Disponible" : "Ocupado",
                 StartTime = s.StartTime,
                 EndTime = s.EndTime,
                 DayOfWeek = s.DayOfWeek
             })
             .ToList();

            return View(schedules);
        }

        [HttpPost]
        public ActionResult Configure(Schedules schedule)
        {
            try
            {
                var userJson = HttpContext.Session["user"] as string;
                var user = JsonConvert.DeserializeObject<Users>(userJson);

                schedule.DoctorID = user.UserID;

                var conflict = _context.Schedules.Any(s => s.DoctorID == user.UserID &&
                    s.DayOfWeek == schedule.DayOfWeek &&
                    ((schedule.StartTime >= s.StartTime && schedule.StartTime < s.EndTime) ||
                     (schedule.EndTime > s.StartTime && schedule.EndTime <= s.EndTime)));

                if (conflict)
                {
                    TempData["ErrorMessage"] = "El horario se superpone a otro horario.";

                    return RedirectToAction("Index");
                }

                TimeSpan currentStartTime = schedule.StartTime;
                TimeSpan blockEndTime;

                while (currentStartTime < schedule.EndTime)
                {
                    blockEndTime = currentStartTime.Add(TimeSpan.FromMinutes(30));

                    // Asegurarse de no exceder el tiempo de finalización
                    if (blockEndTime > schedule.EndTime)
                        blockEndTime = schedule.EndTime;

                    // Crear el nuevo horario
                    var newSchedule = new Schedules
                    {
                        DoctorID = schedule.DoctorID,
                        DayOfWeek = schedule.DayOfWeek,
                        StartTime = currentStartTime,
                        EndTime = blockEndTime
                    };

                    _context.Schedules.Add(newSchedule);
                    currentStartTime = blockEndTime;
                }


                _context.SaveChanges();

                TempData["SuccessMessage"] = "Horarios agregados exitosamente en bloques de 30 minutos.";

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("Index");

        }

        [HttpGet]

        public JsonResult GetSchedulesByDoctor(int doctorId, int dayOfWeek)
        {            
            string[] daysOfWeek = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"};
            string dayName = daysOfWeek[dayOfWeek];

            var schedules = _context.Schedules
                .Where(s => s.DoctorID == doctorId && s.DayOfWeek == dayName)
                .ToList()
                .Select(s => new ScheduleViewModel
                {
                    ScheduleID = s.ScheduleID,
                    Status = _context.Appointments.FirstOrDefault(a => a.Time == s.StartTime) == null ? "Disponible" : "Ocupado",
                    DoctorID = s.DoctorID,
                    DayOfWeek = s.DayOfWeek,   
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                })
                .ToList();



            return Json(schedules, JsonRequestBehavior.AllowGet);
        }

    }
}