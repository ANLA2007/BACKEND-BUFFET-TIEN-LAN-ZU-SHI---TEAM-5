using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
    public class CreateMovementInventoryDto
    {
      
        [Required(ErrorMessage = "El Id de la porción es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El Id de la porción debe ser mayor que 0.")]
        public int PortionId { get; set; }

        [Required(ErrorMessage = "El tipo de movimiento es obligatorio.")]
        [RegularExpression("^(BAJA|SALE)$",
            ErrorMessage = "El tipo de movimiento solo puede ser BAJA o SALE.")]
        public string MovementType { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 0.")]
        public int Quantity { get; set; }

       // [MaxLength(255, ErrorMessage = "La justificación no puede superar los 255 caracteres.")]
        //public string? Justification { get; set; }  // SOLO PARA CUANDO DAMOS DE BAJA
    }
}
