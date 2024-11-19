using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Sobre Nosotros";
            ViewBag.Historia = "Bienvenido al Centro Médico. Somos un equipo dedicado a proporcionar atención médica de alta calidad a nuestra comunidad. Desde nuestros inicios, hemos trabajado incansablemente para ofrecer servicios de salud accesibles y confiables para todos.";
            ViewBag.Mision = "Nuestra misión es brindar atención médica integral de calidad, enfocada en la prevención, diagnóstico y tratamiento de diversas condiciones de salud. Trabajamos con un enfoque humano, siempre velando por el bienestar de nuestros pacientes y ofreciendo un servicio accesible para todos.";
            ViewBag.Servicios = new List<string>
    {
        "Consultas médicas generales",
        "Urgencias 24/7",
        "Atención pediátrica",
        "Exámenes de laboratorio",
        "Cirugía menor",
        "Consejería de salud mental"
    };

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Appoinment()
        {
            return View();  // ASP.NET MVC buscará automáticamente la vista "Appoinment.cshtml"
        }
    }
}