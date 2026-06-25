using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace TIEN_LAN_ZU_SHI.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<RepositoryResponse<PagedResponse<IEnumerable<User>>>> GetAllAsync(PaginationParams pagination);


        Task<RepositoryResponse<User>> GetByIdAsync(int id);

        Task<RepositoryResponse<User>> GetByNameAsync(string name);
        Task<RepositoryResponse<User>> GetByEmailAsync(string email);


        Task<RepositoryResponse<User>> AddAsync(User user);


        Task<RepositoryResponse<User>> UpdateAsync(int id,  User user);

        Task<RepositoryResponse<User>> SetStateAsync(int userId, bool state);
        Task<RepositoryResponse<UserRole>> AssignUserRoleAsync(int userId, int roleId);


        //Task<bool> Desactive(int id);
    }
}
