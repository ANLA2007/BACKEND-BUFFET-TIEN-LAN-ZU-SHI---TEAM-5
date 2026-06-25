using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Core.Entities
{
   public class Sale
    {
        public  int Id {  get; set; }
        public  int CustomerId {  get; set; }
        public int UserId {  get; set; }
        public DateTime DateTime {  get; set; }
        public Decimal Total {  get; set; }

    }
}
