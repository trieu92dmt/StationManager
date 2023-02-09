using MES.Application.Commands.ReceiptFromProduction;
using MES.Application.DTOs.MES.NKTPSX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface INKTPSXQuery
    {
        /// <summary>
        /// Lấy data lệnh sản xuất
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchWOResponse>> GetWO(SearchNKTPSXCommand command);

        
    }
}
