
using KWeb.Models.Request;
using KWeb.ModelsDB;
using KWeb.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace KWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly CONSULTORIO_VIDA_SALUDEntities _context;
        private readonly EmailSystem emailSystem;
 
        public AuthController()
        {
            _context = new CONSULTORIO_VIDA_SALUDEntities();
            emailSystem = new EmailSystem();

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
                    if (!existingUser.IsActive)
                    {
                        TempData["ErrorMessage"] = "Usuario inactivo.";
                        return View(request);

                    }

                    RoleOption options = RoleOptions.GetRoleOptions(existingUser.Role);
                    HttpContext.Session["options"] = JsonConvert.SerializeObject(options);
                    HttpContext.Session["user"] = JsonConvert.SerializeObject(existingUser, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    if(existingUser.Role == Enum.GetName(typeof(RolesEnum), RolesEnum.Administrator))
                    {
                        return RedirectToAction("Index", "Users");

                    }
                    if (existingUser.Role == Enum.GetName(typeof(RolesEnum), RolesEnum.Doctor))
                    {
                        return RedirectToAction("OfDoctor", "appointments");

                    }

                    if (existingUser.Role == Enum.GetName(typeof(RolesEnum), RolesEnum.Patient))
                    {
                        return RedirectToAction("OfPatient", "appointments");

                    }

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

        public ActionResult PasswordRecovery()
        {
            return View(new PasswordRecoveryRequest());
        }

        [HttpPost]
        public ActionResult PasswordRecovery(PasswordRecoveryRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                TempData["ErrorMessage"] = "El correo electrónico no está registrado.";
                return View(new PasswordRecoveryRequest());
            }

            string code = Global.GenerateRandomNumber();

            bool codeExists;

             do
            {
                codeExists = _context.PasswordRecovery
                                     .Any(pr => pr.Code == code);

                if (codeExists)
                {
                    code = Global.GenerateRandomNumber();
                }

            } while (codeExists);  

            var newPasswordRecovery = new PasswordRecovery
            {
                Code = code,
                Date = DateTime.Now,
                UserID = user.UserID,
                Status = true,
            };

             _context.PasswordRecovery.Add(newPasswordRecovery);

             try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                 foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                         Console.WriteLine($"Property: {error.PropertyName}, Error: {error.ErrorMessage}");
                    }
                }

                TempData["ErrorMessage"] = "Hubo un error al guardar los datos.";
                return View(new PasswordRecoveryRequest());
            }

             emailSystem.RecoveryPassword(request.Email, code);

            return RedirectToAction("ValidateCode", request);
        }

        public ActionResult ValidateCode(PasswordRecoveryRequest request)
        {
            return View(request);
        }

        [HttpPost]
        public async Task<ActionResult> ValidateCodePost(PasswordRecoveryRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                TempData["ErrorMessage"] = "El correo electrónico no está registrado.";
                return RedirectToAction("ValidateCode", request);
            }

            var processRecovery = _context.PasswordRecovery
                .Where(p => p.UserID == user.UserID && p.Status == true)
                .OrderByDescending(p => p.Date) 
                .FirstOrDefault();

            if (processRecovery == null)
            {
                TempData["ErrorMessage"] = "Esta cuenta no esta en recuperacion.";
                return RedirectToAction("ValidateCode", request);

            }


            if(processRecovery.Code != request.Code)
            {
                TempData["ErrorMessage"] = "Codigo incorrecto.";
                return RedirectToAction("ValidateCode", request);
            }

            _context.SaveChanges();

            return RedirectToAction("ChangePassword", request);
        }

        public ActionResult ChangePassword(PasswordRecoveryRequest request)
        {
            return View(request);
        }

        [HttpPost]
        public ActionResult ChangePasswordPost(PasswordRecoveryRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
            {
                TempData["ErrorMessage"] = "El correo electrónico no está registrado.";
                return RedirectToAction("ValidateCode", request);
            }

            var processRecovery = _context.PasswordRecovery
                .Where(p => p.UserID == user.UserID && p.Status == true)
                .OrderByDescending(p => p.Date)
                .FirstOrDefault();

            if (processRecovery == null)
            {
                TempData["ErrorMessage"] = "Esta cuenta no esta en recuperacion.";
                return RedirectToAction("ValidateCode", request);

            }


            if (processRecovery.Code != request.Code)
            {
                TempData["ErrorMessage"] = "Codigo incorrecto.";
                return RedirectToAction("ValidateCode", request);
            }

            user.Password = request.NewPassword;
            processRecovery.Status = false;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Contraseña cambiada con éxito.";
            return RedirectToAction("ChangePassword", request);

        }

    }
}