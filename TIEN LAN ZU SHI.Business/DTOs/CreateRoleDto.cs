using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
   public class CreateRoleDto
    {
    
        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "El nombre del rol debe tener entre 5 y 100 caracteres")]
        public string RoleName { get; set; }
    }
}
