using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Business.DTOs;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace TIEN_LAN_ZU_SHI.Business.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<User>> RegisterAsync(RegisterUserDto newUser);
        Task<ServiceResponse<User>> GetByUserNameAsync(string username);
        Task<ServiceResponse<User>> GetByEmailAsync(string email);
        Task<ServiceResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest);
       


    }
}