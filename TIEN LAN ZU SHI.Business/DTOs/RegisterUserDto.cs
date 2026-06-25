using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Business.DTOs
{
    public class RegisterUserDto
    {
        [Required, MinLength(5)]
        public string UserName { get; set; }
        
        [Required,EmailAddress]
        public string Email {  get; set; }
        [Required, MinLength(5)]
        public string Password { get; set; }
        [Compare("Password",ErrorMessage = "LAS CONTRASEÑAS NO COINCIDEN")]
        public string ConfirmPassword {  get; set; }
    }
}
