using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Core.Entities
{
    public class SaleDetail
    {
        public  int Id {  get; set; }
        public  int SaleId { get; set; }
        public int PortionId {  get; set; }
        public int Quantity {  get; set; }
     
        public decimal LineTotal {  get; set; }
        public decimal SalePrice { get; set; }

        public string PortionType {  get; set; }
    }
}
