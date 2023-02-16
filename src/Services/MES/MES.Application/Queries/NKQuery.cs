using MES.Application.Commands.NK;
using MES.Application.DTOs.MES.NK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Queries
{
    public interface INKQuery
    {
        /// <summary>
        /// Lấy input data
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<GetInputDataResponse> GetInputData(SearchNKCommand command);
    }
}
 