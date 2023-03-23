using Shared.WeighSession;
using WeighSession.API.DTOs;

namespace WeighSession.API.Repositories.Interfaces
{
    public interface IWeighSessionRepository
    {
        /// <summary>
        /// Dropdown đầu cân
        /// </summary>
        /// <param name="keyWord">Từ khóa</param>
        /// <param name="plantCode">Mã nhà máy</param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<List<WeightHeadResponse>> GetWeightHeadAsync(string keyWord, string plantCode, string type);

        /// <summary>
        /// Lấy số cân
        /// </summary>
        /// <param name="scaleCode">Mã đầu cân</param>
        /// <returns></returns>
        Task<GetWeighNumResponse> GetWeighNum(string scaleCode);

        /// <summary>
        /// Lấy chi tiết cân
        /// </summary>
        /// <param name="scaleCode"></param>
        /// <returns></returns>
        Task<ScaleDetailResponse> GetScaleByCode(string scaleCode);

        /// <summary>
        /// Lấy chi tiết đợt cân
        /// </summary>
        /// <param name="scaleCode"></param>
        /// <returns></returns>
        Task<WeighSessionDetailResponse> GeWeighSessionByScaleCode(string scaleCode);

        /// <summary>
        /// Giám sát hoạt động cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<SearchScaleMonitorResponse>> SearchScaleMonitor(SearchScaleMinitorRequest request);

    }
}
