using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Core.Entities
{
    public class Inventory
    {
        public  int Id {  get; set; }
        public  int PortionId {  get; set; }
        public DateTime Date { get; set; }
        public int DailyStock {  get; set; }
        public int AccumulatedStock { get; set; }
        public decimal SalePrice {  get; set; }
      
    }
}
