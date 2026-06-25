using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
    public class UpdateUserDto
    {
       
        [Required(ErrorMessage = "El username es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        public string UserName { get; set; }

  

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [StringLength(100, ErrorMessage = "El correo no debe exceder los 100 caracteres")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public bool State { get; set; }
    }
}
