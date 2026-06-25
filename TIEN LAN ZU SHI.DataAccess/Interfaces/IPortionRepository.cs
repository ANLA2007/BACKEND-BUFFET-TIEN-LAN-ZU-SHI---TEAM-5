using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace TIEN_LAN_ZU_SHI.DataAccess.Interfaces
{
    public interface IPortionRepository
    {
        Task<RepositoryResponse<PagedResponse<IEnumerable<Portion>>>> GetAllAsync(PaginationParams pagination);


        Task<RepositoryResponse<Portion>>GetByIdAsync(int id);

        Task<RepositoryResponse<Portion>> GetByNameAsync(string name);


        Task<RepositoryResponse<Portion>> AddAsync(Portion portion);
        Task<RepositoryResponse<Portion>> UpdateAsync(int id, Portion portion);


        Task<RepositoryResponse<Portion>> SetStateAsync(int portionId, bool state);

        Task<RepositoryResponse<Portion>> SetReconsumableAsync(int portionId, bool state);


        //Task<RepositoryResponse<int>>Updatesync(Product product);


        //Task<bool> Desactive(int id);
    }
}
