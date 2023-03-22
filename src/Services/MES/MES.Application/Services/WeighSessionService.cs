using Core.Extensions;
using Newtonsoft.Json;
using Shared.Models;
using Shared.WeighSession;

namespace MES.Application.Services
{
    public interface IWeighSessionService
    {
        /// <summary>
        /// Service call api lấy chi tiết cân
        /// </summary>
        /// <param name="ScaleCode">Đầu cân</param>
        /// <returns></returns>
        Task<ScaleDetailResponse> GetDetailScale(string ScaleCode);

        /// <summary>
        /// Service call api lấy đợt cân
        /// </summary>
        /// <param name="ScaleCode">Đầu cân</param>
        /// <returns></returns>
        Task<WeighSessionDetailResponse> GetDetailWeighSession(string ScaleCode);
    }
    public class WeighSessionService : IWeighSessionService
    {
        private readonly HttpClient _httpClient;
        public WeighSessionService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<ScaleDetailResponse> GetDetailScale(string ScaleCode)
        {
            //GET data weigh session
            var domainWS = new ConfigManager().WeighSessionUrl;
            var url = $"{domainWS}get-scale-by-code?ScaleCode={ScaleCode}";

            var scaleData = await _httpClient.GetAsync(url);
            var scaleResponse = await scaleData.Content.ReadAsStringAsync();

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var result = JsonConvert.DeserializeObject<ApiSuccessResponse<ScaleDetailResponse>>(scaleResponse, jsonSettings);

            if (!result.IsSuccess)
                return null;

            return result?.Data;
        }

        public async Task<WeighSessionDetailResponse> GetDetailWeighSession(string ScaleCode)
        {
            //GET data weigh session
            var domainWS = new ConfigManager().WeighSessionUrl;
            var url = $"{domainWS}get-weigh-session-by-scale-code?ScaleCode={ScaleCode}";

            var weighSessionData = await _httpClient.GetAsync(url);
            var weighSessionResponse = await weighSessionData.Content.ReadAsStringAsync();

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var result = JsonConvert.DeserializeObject<ApiSuccessResponse<WeighSessionDetailResponse>>(weighSessionResponse, jsonSettings);

            if (!result.IsSuccess)
                return null;

            return result?.Data;
        }
    }
}
