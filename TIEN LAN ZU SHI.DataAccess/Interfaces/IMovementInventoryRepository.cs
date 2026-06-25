using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace TIEN_LAN_ZU_SHI.DataAccess.Interfaces
{
    public  interface IMovementInventoryRepository
    {
        Task<RepositoryResponse<PagedResponse<IEnumerable<MovementInventory>>>> GetAllAsync(PaginationParams pagination);

        Task<RepositoryResponse<MovementInventory>> AddAsync(MovementInventory movementInventory);
        Task<RepositoryResponse<MovementInventory>> GetPortionByIdAsync(int id);

        Task<RepositoryResponse<List<MovementInventory>>> GetByMovementTypeAsync(string movementType);

    }
}
