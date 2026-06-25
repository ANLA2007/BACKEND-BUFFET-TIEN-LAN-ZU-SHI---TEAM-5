using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
   public class LoginResponseDto
   {
        public int Id {  get; set; }
        public string UserName {  get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        //public string Password { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
