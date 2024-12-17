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
        private readonly EmailSystem emailSystem;

        public UsersController()
        {
            _context = new CONSULTORIO_VIDA_SALUDEntities();
            emailSystem = new EmailSystem();

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

        [HttpPost]
        public async Task<ActionResult> Edit(Users request)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(request.UserID);

                if (user != null)
                {
                    user.FirstName = request.FirstName;
                    user.LastName = request.LastName;
                    user.Email = request.Email;
                    user.Phone = request.Phone;
                    user.Password = request.Password;

                    if(user.IsActive != request.IsActive)
                    {
                        user.IsActive = request.IsActive;

                        if (!user.IsActive) emailSystem.InactivatedUserAppointment(user.Email, user);
                    }

                    _context.Entry(user).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Usuario actualizado con éxito.";
                }
                else
                {
                    TempData["ErrorMessage"] = "El usuario no existe.";
                }

                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Datos inválidos.";
            return RedirectToAction("Index");
        }

    }
}