using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
    public class CreatePortionDto
    {
        [Required(ErrorMessage = "El ID de la categoría es obligatorio")]
        public int CategoryId { get; set; }
        //[Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        //[StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre del producto  debe tener entre 5 y 100 caracteres")]
        //public string CategoryName { get; set; }


        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "El nombre del producto debe tener entre 5 y 100 caracteres")]
        public string PortionName { get; set; }

        [Required(ErrorMessage = "El precio de la porción es obligatorio")]
        [Range(0.01, 10000, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PortionPrice { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no debe exceder los 500 caracteres")]
        public string Description { get; set; }
        [Required]
        public bool Reconsumable { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "El nombre del producto debe tener entre 5 y 100 caracteres")]
        public string PortionType { get; set; }

        [Required]
        public bool State { get; set; }


    }
}
