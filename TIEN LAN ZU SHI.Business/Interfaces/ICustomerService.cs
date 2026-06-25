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
    public interface ICustomerService
    {
        Task<ServiceResponse<PagedResponse<IEnumerable<Customer>>>> GetAllAsync(PaginationParams pagination);
        Task<ServiceResponse<Customer>> GetByIdAsync(int ids);
        Task<ServiceResponse<Customer>> GetByNameAsync(string name);
        Task<ServiceResponse<Customer>> CreateAsync(CreateCustomerDto newCustomer);
        Task<ServiceResponse<Customer>> UpdateAsync(int id, UpdateCustomerDto customer);
        Task<ServiceResponse<Customer>> SetStateAsync(int customerId, bool state);





    }
}
