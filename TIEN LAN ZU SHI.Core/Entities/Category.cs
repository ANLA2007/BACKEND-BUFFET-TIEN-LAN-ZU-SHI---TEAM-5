using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Core.Entities
{
    public  class Category
    {
        public  int Id { get; set; }
        public  string CategoryName { get; set; }
        public string? Description { get; set; }
        public bool State {  get; set; }
    }
}
