using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
    public  class UpdateRoleDto
    {
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "El nombre del rol debe tener entre 5 y 100 caracteres.")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Debe especificar si la categoría está activa o no")]
        public bool State { get; set; }
    }
}
