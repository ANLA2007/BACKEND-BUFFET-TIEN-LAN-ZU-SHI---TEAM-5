using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
    public class CreateSaleDto
    {
        [Required(ErrorMessage = "El Id del cliente es obligatorio")]
        [Range(1,int.MaxValue,ErrorMessage ="El Id del cliente no es valido")]
        public int CustomerId {  get; set; }

        [Required(ErrorMessage = "La venta debe incluir un detalle de venta")]
        [MinLength(1,ErrorMessage ="El detalle debe incluir al menos un producto")]
        public List<CreateSaleDetailDto> Details { get; set; }
    }
}
