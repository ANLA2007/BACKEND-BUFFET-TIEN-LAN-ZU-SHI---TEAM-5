using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Core.Entities
{
    public class MovementInventory
    {
        public int Id { get; set; }
        public int PortionId { get; set; }
        public string MovementType { get; set; }
        public int Quantity { get; set; }
        public int PreviousDailyStock { get; set; }
        public int NewDailyStock { get; set; }
        public int PreviousAccumulatedStock { get; set; }
        public int NewAccumulatedStock { get; set; }
        public string? Justification { get; set; }
        public DateTime MovementDate { get; set; }
    }
}
