using KWeb.ModelsDB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KWeb.Controllers
{
    public class CitasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitasController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Agendar()
        {
            ViewBag.Doctores = _context.Doctores.ToList();
            ViewBag.Pacientes = _context.Pacientes.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Agendar(int pacienteId, int doctorId, DateTime fechaHora)
        {
            var cita = new Cita
            {
                PacienteId = pacienteId,
                DoctorId = doctorId,
                FechaHora = fechaHora
            };

            _context.Citas.Add(cita);
            _context.SaveChanges();

            return RedirectToAction("Confirmacion");
        }

        public ActionResult Confirmacion()
        {
            return View();
        }

        public ActionResult MisCitas(int pacienteId)
        {
            var citas = _context.Citas
                .Where(c => c.PacienteId == pacienteId)
                .Include(c => c.Doctor)
                .ToList();

            return View(citas);
        }
    }
}