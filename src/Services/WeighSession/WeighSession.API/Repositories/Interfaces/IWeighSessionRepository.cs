using Core.SeedWork;
using Shared.Models;
using Shared.WeighSession;
using WeighSession.API.DTOs;

namespace WeighSession.API.Repositories.Interfaces
{
    public interface IWeighSessionRepository
    {
        /// <summary>
        /// Dropdown đầu cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<WeightHeadResponse>> GetWeightHeadAsync(GetDropdownWeighHeadRequest request);

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
        Task<SearchScaleMonitorResponse2> SearchScaleMonitor(SearchScaleMinitorRequest request);

        /// <summary>
        /// Search danh sách cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ScaleListResponse> SearchScale(SearchScaleRequest request);

        /// <summary>
        /// Lưu cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResponse> SaveScale(SaveScaleRequest request);

        /// <summary>
        /// Cập nhật cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> UpdateScale(UpdateScaleRequest request);

        /// <summary>
        /// BC trạng thái cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<ScaleStatusReportResponse>> ScaleStatusReport(ScaleStatusReportRequest request);

    }
}
