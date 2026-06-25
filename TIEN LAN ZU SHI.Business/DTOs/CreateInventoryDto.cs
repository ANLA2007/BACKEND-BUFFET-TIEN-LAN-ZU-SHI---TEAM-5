using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
    public class CreateInventoryDto
    {
        [Required(ErrorMessage = "El Id del producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El Id del producto debe ser mayor que 0.")]
        public int PortionId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "El stock diario es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock diario no puede ser negativo.")]
        public int DailyStock { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta debe ser mayor que 0.")]
        [DataType(DataType.Currency)]
        public decimal SalePrice { get; set; }
    }
}
