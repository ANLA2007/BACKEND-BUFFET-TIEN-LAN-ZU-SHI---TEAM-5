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
    public interface ICategoryService
    {
        Task<ServiceResponse<PagedResponse<IEnumerable<Category>>>> GetAllAsync(PaginationParams pagination);
        Task<ServiceResponse<Category>> GetByIdAsync(int ids);
        Task<ServiceResponse<Category>> GetByNameAsync(string name);
        Task<ServiceResponse<Category>> CreateAsync(CreateCategoryDto newCategory);
        Task<ServiceResponse<Category>> UpdateAsync(int id, UpdateCategoryDto category);

        Task<ServiceResponse<Category>> SetStateAsync(int categoryId, bool state);
    }
}
