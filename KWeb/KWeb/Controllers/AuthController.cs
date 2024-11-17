using KWeb.ModelsDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Login()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public ActionResult ValidateUser(Usuario usuario)
        {
            ModelState.Remove("Phone");
            if (ModelState.IsValid)
            {              
                var existingUser = _context.Usuarios
                    .FirstOrDefault(u => u.Email == usuario.Email && u.Password == usuario.Password);

                if (existingUser != null)
                {                   
                    return RedirectToAction("Index", "Home");
                }

            }
            ModelState.AddModelError("", "Credenciales invalidas.");

            return View("login", usuario);
        }

        public ActionResult Register()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public ActionResult Create(Usuario usuario)
        {
            bool usuarioExistente = _context.Usuarios.Any(u => u.Email == usuario.Email);

            if (usuarioExistente)
            {
                ModelState.AddModelError("", "El usuario ya existe.");
            }

            if (ModelState.IsValid)
            {

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Usuario creado.";

                return RedirectToAction("Register");

            }
            return View("Register", usuario);
           
        }

        public ActionResult RegainAccess()
        {
            return View();
        }



    }
}