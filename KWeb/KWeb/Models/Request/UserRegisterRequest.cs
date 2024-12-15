using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KWeb.Models.Request
{
    public class UserRegisterRequest : IValidatableObject
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no debe exceder los 100 caracteres.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no debe exceder los 100 caracteres.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(150, ErrorMessage = "El correo electrónico no debe exceder los 150 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El formato del número de teléfono no es válido.")]
        [StringLength(15, ErrorMessage = "El número de teléfono no debe exceder los 15 caracteres.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [RegularExpression("^(Patient|Doctor|Administrator)$", ErrorMessage = "El rol debe ser Patient, Doctor o Administrator.")]
        public string Role { get; set; }
        public string Specialty { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role == "Doctor" && string.IsNullOrEmpty(Specialty))
            {
                yield return new ValidationResult("La especialidad es obligatoria para los doctores.", new[] { nameof(Specialty) });
            }

            if (Role == "Patient" && !BirthDate.HasValue)
            {
                yield return new ValidationResult("La fecha de nacimiento es obligatoria para los pacientes.", new[] { nameof(BirthDate) });
            }
        }
    }
}