using KWeb.ModelsDB;
using KWeb.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KWeb.Controllers
{
    public class MainController : Controller
    {
        private readonly CONSULTORIO_VIDA_SALUDEntities _context;

        public MainController()
        {
            _context = new CONSULTORIO_VIDA_SALUDEntities();
        }
        // GET: Main
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AcercaDe()
        {
            var results = _context.Doctors.ToList();
            return View(results);
        }
    }
}