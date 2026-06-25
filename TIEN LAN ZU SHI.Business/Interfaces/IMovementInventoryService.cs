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
    public interface IMovementInventoryService
    {
        Task<ServiceResponse<PagedResponse<IEnumerable<MovementInventory>>>> GetAllAsync(PaginationParams pagination);
        Task<ServiceResponse<MovementInventory>> CreateAsync(CreateMovementInventoryDto newMovementInventory);
        Task<ServiceResponse<MovementInventory>> GetPortionByIdAsync(int id);

        Task<ServiceResponse<List<MovementInventory>>> GetByMovementTypeAsync(string movementType);
    }
}
