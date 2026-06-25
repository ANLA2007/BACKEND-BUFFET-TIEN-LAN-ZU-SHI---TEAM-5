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
    public interface ISaleService
    {
        Task<ServiceResponse<SaleResponseDto>> InsertAsync(CreateSaleDto dto);
        Task<ServiceResponse<List<SaleResponseDto>>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ServiceResponse<SaleTransaction>> GetBySaleIdAsync(int saleId);
        Task<ServiceResponse<PagedResponse<IEnumerable<SaleTransaction>>>> GetAllAsync(PaginationParams pagination);
        Task<ServiceResponse<List<SaleDetail>>> GetDetailsBySaleIdAsync(int saleId);




    }
}