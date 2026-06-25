using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace TIEN_LAN_ZU_SHI.DataAccess.Interfaces
{
    public interface ICustomerRepository
    {
        Task<RepositoryResponse<PagedResponse<IEnumerable<Customer>>>> GetAllAsync(PaginationParams pagination);


        Task<RepositoryResponse<Customer>> GetByIdAsync(int id);

        Task<RepositoryResponse<Customer>> GetByNameAsync(string name);



        Task<RepositoryResponse<Customer>> AddAsync(Customer customer);


        Task<RepositoryResponse<Customer>> UpdateAsync(int id, Customer customer);

        Task<RepositoryResponse<Customer>> SetStateAsync(int customerId, bool state);



            //Task<bool> Desactive(int id);

}   }
