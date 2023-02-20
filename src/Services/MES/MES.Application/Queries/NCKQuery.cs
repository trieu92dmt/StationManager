using MES.Application.Commands.NCK;
using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.NCK;

namespace MES.Application.Queries
{
    public interface INCKQuery
    {
        /// <summary>
        /// Dropdown số phiếu cân
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<List<CommonResponse>> GetDropDownWeightVote(string keyword);

        /// <summary>
        /// Lấy data nhập liệu
        /// </summary>
        /// <param name = "command" ></param>
        /// <returns ></returns>
        Task<List<GetInputDataResponse>> GetInputData(SearchNCKCommand command);

        /// <summary>
        /// Lấy data xck
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task<List<SearchNCKResponse>> GetDataXCK(SearchNCKCommand command);

        /// <summary>
        /// Get data by reservation and reservation item
        /// </summary>
        /// <param name="reservation"></param>
        /// <param name="reservationItem"></param>
        /// <returns></returns>
        //Task<GetDataByRsvAndRsvItemResponse> GetDataByRsvAndRsvItem(string reservation, string reservationItem);
    }

    public class NCKQuery : INCKQuery
    {
        public NCKQuery()
        {
        }

        public Task<List<SearchNCKResponse>> GetDataXCK(SearchNCKCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<List<CommonResponse>> GetDropDownWeightVote(string keyword)
        {
            throw new NotImplementedException();
        }

        public Task<List<GetInputDataResponse>> GetInputData(SearchNCKCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
