using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace TIEN_LAN_ZU_SHI.DataAccess.Interfaces
{
    public interface ISaleRepository
    {
        Task<RepositoryResponse<SaleTransaction>> InsertAsync(Sale master, IEnumerable<SaleDetail> details);
        Task<RepositoryResponse<List<SaleTransaction>>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<RepositoryResponse<SaleTransaction>> GetBySaleIdAsync(int saleId);
         Task<RepositoryResponse<PagedResponse<IEnumerable<SaleTransaction>>>> GetAllAsync(PaginationParams pagination);
        Task<RepositoryResponse<List<SaleDetail>>> GetDetailsBySaleIdAsync(int saleId);
    }
}
