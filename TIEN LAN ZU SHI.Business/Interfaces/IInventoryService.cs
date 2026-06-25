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
    public interface IInventoryService
    {
        Task<ServiceResponse<PagedResponse<IEnumerable<Inventory>>>> GetAllAsync(PaginationParams pagination);
        Task<ServiceResponse<Inventory>> CreateAsync(CreateInventoryDto newInventory);
        Task<ServiceResponse<Inventory>> GetByPortionAndDateAsync(int portionId, DateTime date);
        Task<ServiceResponse<Inventory>> GetPortionByIdAsync(int id);

    }
}
