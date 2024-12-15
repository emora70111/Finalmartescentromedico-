using KWeb.Models.Request;
using KWeb.ModelsDB;
using KWeb.Utils;
using KWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly CONSULTORIO_VIDA_SALUDEntities _context;

        public UsersController()
        {
            _context = new CONSULTORIO_VIDA_SALUDEntities();
        }

        public async Task<ActionResult> Index()
        {
            var result = await _context.Users.ToListAsync();

            var viewModel = new UsersViewModel
            {
                Users = result,
                UserRequest = new UserRequest()
            };

            return View(viewModel);         
        }

        [HttpPost]
        public async Task<ActionResult> Register(UserRequest request)
        {

            bool usuarioExistente = _context.Users.Any(u => u.Email == request.Email);

            if (usuarioExistente)
            {
                TempData["ErrorMessage"] = "El correo electrónico ya está registrado.";
          
                return View("Index", new UsersViewModel
                {
                    Users = await _context.Users.ToListAsync(),
                    UserRequest = request
                });
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

                return RedirectToAction("Index");
            }

            return View("Index", new UsersViewModel
            {
                Users = await _context.Users.ToListAsync(),
                UserRequest = request
            });
        }
    }
}