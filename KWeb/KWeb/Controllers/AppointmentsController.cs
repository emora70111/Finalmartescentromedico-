using KWeb.Models.Request;
using KWeb.ModelsDB;
using KWeb.Utils;
using KWeb.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KWeb.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly CONSULTORIO_VIDA_SALUDEntities _context;

        public AppointmentsController()
        {
            _context = new CONSULTORIO_VIDA_SALUDEntities();
        }

        public ActionResult OfDoctor()
        {
            return View();
        }
        public async Task<ActionResult> OfPatient()
        {
            var userJson = HttpContext.Session["user"] as string;
            var user = JsonConvert.DeserializeObject<Users>(userJson);

            var appointments = await _context.Appointments.Where(a => a.PatientID == user.UserID).ToListAsync();
            var doctors = await _context.Doctors.ToListAsync();

            var viewModel = new AppointmentsOfPatientViewModel
            {     
                Doctors = doctors,
                AppointmentRequest = new AppointmentRequest()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Register(AppointmentsOfPatientViewModel vm)
        {
            AppointmentRequest model = vm.AppointmentRequest;

            var userJson = HttpContext.Session["user"] as string;
            var user = JsonConvert.DeserializeObject<Users>(userJson);

            model.PatientID = user.UserID;

            if (ModelState.IsValid)
            {
                var newAppointment = new Appointments
                {
                    PatientID = model.PatientID,
                    DoctorID = model.DoctorID,
                    Date = model.Date,
                    Time = model.Time,
                    Status = Enum.GetName(typeof(AppointmentsStatusEnum), AppointmentsStatusEnum.Scheduled),
                    Notes = model.Notes
                };

                _context.Appointments.Add(newAppointment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "La cita se agendo correctamente.";

                return RedirectToAction("OfPatient");
            }

            TempData["ErrorMessage"] = "No se pudo agendar la cita.";

            return RedirectToAction("OfPatient");
            
        }


        //public ActionResult Agendar()
        //{
        //    ViewBag.Doctores = _context.Doctores.ToList();
        //    ViewBag.Pacientes = _context.Pacientes.ToList();
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Agendar(int pacienteId, int doctorId, DateTime fechaHora)
        //{
        //    var cita = new Cita
        //    {
        //        PacienteId = pacienteId,
        //        DoctorId = doctorId,
        //        FechaHora = fechaHora
        //    };

        //    _context.Citas.Add(cita);
        //    _context.SaveChanges();

        //    return RedirectToAction("Confirmacion");
        //}

        //public ActionResult Confirmacion()
        //{
        //    return View();
        //}

        //public ActionResult MisCitas(int pacienteId)
        //{
        //    var citas = _context.Citas
        //        .Where(c => c.PacienteId == pacienteId)
        //        .Include(c => c.Doctor)
        //        .ToList();

        //    return View(citas);
        //}
    }
}