using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
    public class CreateCategoryDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "El nombre de la categoria debe tener entre 5 y  100 caracteres")]
        public string CategoryName { get; set; }
        [StringLength(500, ErrorMessage = "La descripcion no debe exceder los  500 caracteres")]
        public string Description { get; set; }
    }
}
