using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 5 y 100 caracteres.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 5 y 100 caracteres.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no debe exceder los 100 caracteres.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; }

        //public bool State { get; set; }
    }
}
