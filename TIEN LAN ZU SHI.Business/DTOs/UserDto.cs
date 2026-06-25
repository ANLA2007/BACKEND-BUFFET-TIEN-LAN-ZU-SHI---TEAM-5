using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TIEN_LAN_ZU_SHI.Business.DTOs;

namespace PROYECTO_TIEN_LAN_ZU_SHI.DTOs
{
    public class UserDto
    {
        public required int Id { get; set; }
        public required string UserName { get; set; }

     
        public string? Email { get; set; }
        //public string? PasswordHash { get; set; }

        
        public bool State { get; set; }


        //[BindNever]
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        //public List<string> Roles { get; set; } = new List<string>();
        [BindNever]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Roles { get; set; }




    }
}
