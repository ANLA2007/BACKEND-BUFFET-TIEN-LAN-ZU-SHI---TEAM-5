using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace TIEN_LAN_ZU_SHI.DataAccess.Interfaces
{
    public interface IRoleRepository
    {
        Task<RepositoryResponse<PagedResponse<IEnumerable<Role>>>> GetAllAsync(PaginationParams pagination);


        Task<RepositoryResponse<Role>> GetByIdAsync(int id);
        Task<RepositoryResponse<Role>> GetByNameAsync(string name);


        Task<RepositoryResponse<Role>> AddAsync(Role role);


        Task<RepositoryResponse<Role>> UpdateAsync(int id, Role role);

        Task<RepositoryResponse<Role>> SetStateAsync(int roleId, bool state);
    }
}
