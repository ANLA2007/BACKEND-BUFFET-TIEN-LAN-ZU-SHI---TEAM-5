using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIEN_LAN_ZU_SHI.Core.Common
{
    public enum MessageCodes
    {
        Sucess = 0,
        NotFound = 1,
        ErrorValidation = 2,
        Unauthenticated = 3,
        Unauthorized = 4,
        ErrorDataBase = 5,
        NoData = 6,
        Conflict = 7
    }
}
