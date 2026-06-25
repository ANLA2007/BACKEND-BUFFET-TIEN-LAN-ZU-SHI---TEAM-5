using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Core.Entities
{
    public class Customer
    {
        public  int Id { get; set; }
        public  string FirstName {  get; set; }
        public string? LastName { get; set; }
        public string ? Email {  get; set; }
        public bool State {  get; set; }
    }
}
