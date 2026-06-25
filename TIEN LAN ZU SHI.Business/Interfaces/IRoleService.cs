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
   public interface IRoleService
    {
        Task<ServiceResponse<PagedResponse<IEnumerable<Role>>>> GetAllAsync(PaginationParams pagination);
        Task<ServiceResponse<Role>> GetByIdAsync(int ids);
        Task<ServiceResponse<Role>> GetByNameAsync(string name);
        Task<ServiceResponse<Role>> CreateAsync(CreateRoleDto newRole);
        Task<ServiceResponse<Role>> UpdateAsync(int id, UpdateRoleDto role);
        Task<ServiceResponse<Role>> SetStateAsync(int roleId, bool state);
    }
}
