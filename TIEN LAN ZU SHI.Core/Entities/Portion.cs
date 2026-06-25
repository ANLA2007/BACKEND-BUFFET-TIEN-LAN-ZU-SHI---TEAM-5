using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Core.Entities
{
    public class Portion
    {
        public int Id { get; set; }
        public  int CategoryId {  get; set; }
        public string PortionName {  get; set; }
        public decimal PortionPrice {  get; set; }
        public string Description {  get; set; }
        public bool State {  get; set; }
        public bool Reconsumable {  get; set; }

        public string PortionType {  get; set; }
        public string CategoryName { get; set; }
    }
}
