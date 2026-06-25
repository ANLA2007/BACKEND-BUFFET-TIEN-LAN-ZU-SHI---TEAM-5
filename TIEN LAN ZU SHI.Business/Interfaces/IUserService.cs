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
    public interface IUserService
    {
        Task<ServiceResponse<PagedResponse<IEnumerable<User>>>> GetAllAsync(PaginationParams pagination);
        Task<ServiceResponse<User>> GetByIdAsync(int ids);
        Task<ServiceResponse<User>> GetByNameAsync(string name);
        Task<ServiceResponse<User>> GetByEmailAsync(string email);
        Task<ServiceResponse<User>> CreateAsync(CreateUserDto newUser);
        Task<ServiceResponse<User>> UpdateAsync(int id, UpdateUserDto user);

        Task<ServiceResponse<User>> SetStateAsync(int userId, bool state);
        Task<ServiceResponse<UserRole>> AssignUserRoleAsync(int userId, int roleId);

    }
}
