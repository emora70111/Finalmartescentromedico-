
using KWeb.Models.Request;
using KWeb.ModelsDB;
using KWeb.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly CONSULTORIO_VIDA_SALUDEntities _context;

        public AuthController()
        {
            _context = new CONSULTORIO_VIDA_SALUDEntities();
        }

        public ActionResult Login()
        {
            return View(new UserLoginRequest());
        }

        [HttpPost]
        public ActionResult Login(UserLoginRequest request)
        {        

            if (ModelState.IsValid)
            {
                var existingUser = _context.Users
                    .FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);

                if (existingUser != null)
                {
                    RoleOption options = RoleOptions.GetRoleOptions(existingUser.Role);
                    HttpContext.Session["options"] = JsonConvert.SerializeObject(options);
                    HttpContext.Session["user"] = JsonConvert.SerializeObject(existingUser, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    return RedirectToAction("Index", "Main");
                }

            }
            TempData["ErrorMessage"] = "Credenciales invalidas.";
            return View(request);
        }

        public ActionResult Register()
        {
            return View(new UserRegisterRequest());
        }

        [HttpPost]
        public ActionResult Register(UserRegisterRequest request)
        {

            bool usuarioExistente = _context.Users.Any(u => u.Email == request.Email);

            if (usuarioExistente)
            {
                TempData["ErrorMessage"] = "El correo electrónico ya está registrado.";
                return View(request);
            }

            if (ModelState.IsValid)
            {
                var nuevoUsuario = new Users
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Password = request.Password, 
                    Role = request.Role,
                    IsActive = true
                };

                _context.Users.Add(nuevoUsuario);        
                _context.SaveChanges();

                if (request.Role == Enum.GetName(typeof(RolesEnum), RolesEnum.Patient))
                {
                    var nuevoPaciente = new Patients
                    {
                        UserID = nuevoUsuario.UserID, 
                        BirthDate = request.BirthDate.Value 
                    };

                    _context.Patients.Add(nuevoPaciente);
                }
                else if (request.Role == "Doctor")
                {
                    var nuevoDoctor = new Doctors
                    {
                        UserID = nuevoUsuario.UserID,
                        Specialty = request.Specialty
                    };

                    _context.Doctors.Add(nuevoDoctor);
                }

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Usuario creado con éxito.";
                return RedirectToAction("Register");
            }

            return View(request);
        }


        public ActionResult RegainAccess()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SignOut()
        {
            HttpContext.Session.Remove("user");
            HttpContext.Session.Remove("options");

            return RedirectToAction("Login", "Auth");
        }

    }
}