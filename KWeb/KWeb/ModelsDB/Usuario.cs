using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KWeb.ModelsDB
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato valido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El numero de teléfono es obligatorio")]
        [Phone(ErrorMessage = "El número de telefono no tiene un formato valido")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string Password{ get; set; }
    }
}