using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace TIEN_LAN_ZU_SHI.DataAccess.Interfaces
{
    public interface IAuthRepository
    {
        Task<RepositoryResponse<User>> RegisterAsync(User user);
        Task<RepositoryResponse<User>> GetByUserNameAsync(string username);
        Task<RepositoryResponse<User>> GetByEmailAsync(string email);
        Task<RepositoryResponse<IEnumerable<string>>> GetRolesByUserIdAsync(int userId);
      
    }
}
