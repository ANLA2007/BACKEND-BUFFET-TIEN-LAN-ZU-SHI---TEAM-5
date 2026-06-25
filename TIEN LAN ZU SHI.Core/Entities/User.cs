using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Core.Entities
{
    public class User
    {
        public  int Id { get; set; }
        public  string UserName {  get; set; }
       
    
        public string? PasswordHash {  get; set; }
        public  string? Email { get; set; }
        public bool State {  get; set; }

        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> Roles { get; set; } = new List<string>();
    }
}
