using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
    public class CreateSaleDetailDto
    {
        [Required(ErrorMessage = "El Id del producto es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El Id del producto ingresado no es valido")]
        public int PortionId { get; set; }

        [Required(ErrorMessage = "La cantidad del producto es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad del producto ingresada no es valida")]
        public int Quantity {  get; set; }

        [Required(ErrorMessage = "El tipo de porción es obligatorio")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "El tipo de porción debe tener entre 5 y 20 caracteres")]
        public string PortionType { get; set; }
    }
}
