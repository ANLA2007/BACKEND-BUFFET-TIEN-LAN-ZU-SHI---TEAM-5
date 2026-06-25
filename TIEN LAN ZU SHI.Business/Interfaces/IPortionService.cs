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
    public interface IPortionService
    {
        Task<ServiceResponse<PagedResponse<IEnumerable<Portion>>>> GetAllAsync(PaginationParams pagination);
        Task<ServiceResponse<Portion>> GetByIdAsync(int ids);
        Task<ServiceResponse<Portion>> GetByNameAsync(string name);
        Task<ServiceResponse<Portion>> CreateAsync(CreatePortionDto newPortion);
        Task<ServiceResponse<Portion>> UpdateAsync(int id, UpdatePortionDto portion);
        Task<ServiceResponse<Portion>> SetStateAsync(int portionId, bool state);
        Task<ServiceResponse<Portion>> SetReconsumableAsync(int portionId, bool state);
    }
}
