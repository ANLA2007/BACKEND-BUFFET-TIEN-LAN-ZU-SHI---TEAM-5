using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
   public class SaleResponseDto
   {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime DateTime { get; set; }
        public Decimal Total { get; set; }

        public List<SaleResponseDetailDto> Details {  get; set; }
    }
}
