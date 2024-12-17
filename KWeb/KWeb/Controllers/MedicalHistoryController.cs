using KWeb.ModelsDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KWeb.Controllers
{
    public class MedicalHistoryController : Controller
    {
        private readonly CONSULTORIO_VIDA_SALUDEntities _context;

        public MedicalHistoryController()
        {
            _context = new CONSULTORIO_VIDA_SALUDEntities();
        }
        public ActionResult Index()
        {
            var userJson = HttpContext.Session["user"] as string;
            var user = JsonConvert.DeserializeObject<Users>(userJson);

            int PatientId = user.Patients.First().PatientID;
            var medicalHistory = _context.MedicalHistory.Where(m => m.Appointments.PatientID == PatientId).ToList();

            return View(medicalHistory);
        }
    }
}