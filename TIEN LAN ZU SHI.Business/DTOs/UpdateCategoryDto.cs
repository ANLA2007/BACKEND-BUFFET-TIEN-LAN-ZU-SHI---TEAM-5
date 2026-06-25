using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
      public class UpdateCategoryDto
      {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "El nombre de la categoría debe tener entre 5 y 100 caracteres")]
        public string CategoryName { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no debe exceder los 500 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Debe especificar si la categoría está activa o no")]
        public bool State { get; set; }
      }
}
