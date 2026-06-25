using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace TIEN_LAN_ZU_SHI.Business.Interfaces
{
    public interface IInvoiceService
    {
        Task<ServiceResponse<List<Invoice>>> InvoiceQueue();
        Task<ServiceResponse<Invoice>> ToPrint();
    }
}
