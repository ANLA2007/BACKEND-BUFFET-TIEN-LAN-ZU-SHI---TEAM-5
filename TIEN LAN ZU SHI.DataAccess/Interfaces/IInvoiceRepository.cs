using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TIEN_LAN_ZU_SHI.Core.Common;
using TIEN_LAN_ZU_SHI.Core.Entities;

namespace TIEN_LAN_ZU_SHI.DataAccess.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<RepositoryResponse<ConcurrentQueue<Invoice>>> GetInvoiceQueue();
        Task<RepositoryResponse<Invoice>> ToPrint();
    }
}
