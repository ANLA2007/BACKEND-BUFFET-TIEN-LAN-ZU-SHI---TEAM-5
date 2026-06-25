using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
    public class UpdatePortionDto
    {
        [Required(ErrorMessage = "El ID de la categoría es obligatorio")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "El nombre de la porcion es obligatorio")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "El nombre del producto debe tener entre 5 y 100 caracteres")]
        public string PortionName { get; set; }

        [Required(ErrorMessage = "El precio de la porción es obligatorio")]
        [Range(0.01, 10000, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal PortionPrice { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no debe exceder los 500 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Debe indicar si el producto es reconsumible o no")]
        public bool Reconsumable { get; set; }

        [Required]
        public bool State { get; set; }

        [Required(ErrorMessage = "El tipo de porcion es obligatorio")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "El tipo de porcion debe tener entre 5 y 100 caracteres")]
        public string PortionType { get; set; }
        //[JsonIgnore]
        //public string CategoryName { get; set; }
    }
}
