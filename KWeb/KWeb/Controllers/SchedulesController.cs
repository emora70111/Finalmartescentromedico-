using KWeb.Models.Request;
using KWeb.Models.Response;
using KWeb.ModelsDB;
using KWeb.Utils;
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

            int DoctorId = user.Doctors.First().DoctorID;
            var Scheduled = Enum.GetName(typeof(AppointmentsStatusEnum), AppointmentsStatusEnum.Scheduled);

            var schedules = _context.Schedules
            .Where(s => s.DoctorID == DoctorId)

            .Select(s => new ScheduleViewModel
            {
                ScheduleID = s.ScheduleID,
                DayOfWeek = s.DayOfWeek,
                Shifts = _context.Shifts
                    .Where(shift => shift.ScheduleID == s.ScheduleID)
                    .Select(shift => new ShiftViewModel
                    {
                        ShiftID = shift.ShiftID,
                        Description = shift.Description,
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime,
                        BlockDurationMinutes = shift.BlockDurationMinutes,
                        Blocks = _context.Blocks
                            .Where(block => block.ShiftID == shift.ShiftID)
                            .Select(block => new BlockViewModel
                            {
                                BlockID = block.BlockID,
                                ShiftID = block.ShiftID,
                                BlockStartTime = block.BlockStartTime,
                                BlockEndTime = block.BlockEndTime
                                
                            })
                            .ToList()

                    })
                    .ToList()
            })
            .ToList();

            return View(schedules);
        }

        [HttpPost]
        public JsonResult Configure(ScheduleRequest model)
        {
            try
            {
                var userJson = HttpContext.Session["user"] as string;
                var user = JsonConvert.DeserializeObject<Users>(userJson);

                model.DoctorID = user.Doctors.First().DoctorID;

                // Validar conflictos
                foreach (var shift in model.Shifts)
                {
                    bool conflict = _context.Blocks.Any(b =>
                        b.Shifts.Schedules.DoctorID == model.DoctorID &&
                        b.Shifts.Schedules.DayOfWeek == model.DayOfWeek &&
                        ((shift.StartTime < b.BlockEndTime && shift.StartTime >= b.BlockStartTime) || 
                         (shift.EndTime > b.BlockStartTime && shift.EndTime <= b.BlockEndTime) ||  
                         (shift.StartTime <= b.BlockStartTime && shift.EndTime >= b.BlockEndTime)));

                    if (conflict)
                    {
                        return Json(new ApiResponse<string>
                        {
                            Status = 400,
                            Message = $"El turno {shift.Description} se superpone a un horario existente.",
                            Data = $"El turno {shift.Description} se superpone a un horario existente."
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                // Crear el nuevo horario
                var newSchedule = new Schedules
                {
                    DoctorID = model.DoctorID,
                    DayOfWeek = model.DayOfWeek
                };
                _context.Schedules.Add(newSchedule);
                _context.SaveChanges();

                foreach (var shift in model.Shifts)
                {                   
                                    
                    var newShift = new Shifts
                    {
                        ScheduleID = newSchedule.ScheduleID,
                        Description = shift.Description,                      
                        StartTime = shift.StartTime,
                        EndTime = shift.EndTime,
                        BlockDurationMinutes = Global.BlockDurationMinutes,
                    };
                    _context.Shifts.Add(newShift);
                    _context.SaveChanges();

                    TimeSpan currentStartTime = shift.StartTime;
                    TimeSpan blockEndTime;

                    while (currentStartTime < shift.EndTime)
                    {
                        blockEndTime = currentStartTime.Add(TimeSpan.FromMinutes(30));

                        var newBlock= new Blocks
                        {
                            ShiftID = newShift.ShiftID,
                            BlockStartTime = currentStartTime,
                            BlockEndTime = blockEndTime,
                        };

                        _context.Blocks.Add(newBlock);
                        _context.SaveChanges();

                        currentStartTime = blockEndTime;
                    }
                }

                ApiResponse<string> response = new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Horarios agregados exitosamente en bloques de 30 minutos.",
                    Data = "Horarios agregados exitosamente en bloques de 30 minutos."
                };

                return Json(response, JsonRequestBehavior.DenyGet);


            }
            catch (Exception ex)
            {
                 ApiResponse<string> response = new ApiResponse<string>
                {
                    Status = 400,
                    Message = "Error: " + ex.Message,
                    Data = "Error: " + ex.Message
                };

                return Json(response, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpGet]

        public JsonResult GetSchedulesByDoctor(int doctorId, int dayOfWeek)
        {
            var Scheduled = Enum.GetName(typeof(AppointmentsStatusEnum), AppointmentsStatusEnum.Scheduled);

            var schedules = _context.Schedules
           .Where(s => s.DoctorID == doctorId && s.DayOfWeek == dayOfWeek)
           .Select(s => new ScheduleViewModel
           {
               ScheduleID = s.ScheduleID,
               DayOfWeek = s.DayOfWeek,
               Shifts = _context.Shifts
                   .Where(shift => shift.ScheduleID == s.ScheduleID)
                   .Select(shift => new ShiftViewModel
                   {
                       ShiftID = shift.ShiftID,
                       Description = shift.Description,
                       StartTime = shift.StartTime,
                       EndTime = shift.EndTime,
                       BlockDurationMinutes = shift.BlockDurationMinutes,
                       Blocks = _context.Blocks
                           .Where(block => block.ShiftID == shift.ShiftID)
                           .Select(block => new BlockViewModel
                           {
                               BlockID = block.BlockID,
                               ShiftID = block.ShiftID,
                               BlockStartTime = block.BlockStartTime,
                               BlockEndTime = block.BlockEndTime,
                               Status = _context.Appointments
                                    .Any(a =>
                                        a.BlockID == block.BlockID &&
                                        a.Status == Scheduled
                                    ) ? "Ocupado" : "Disponible"
                           })
                           .ToList()
                   })
                   .ToList()
           })
           .ToList();

            ApiResponse<List<ScheduleViewModel>> response = new ApiResponse<List<ScheduleViewModel>>
            {
                Status = 200,
                Message = "Agenda obtenida.",
                Data = schedules
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }
}