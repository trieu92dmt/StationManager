using Core.Extensions;
using MES.Application.Commands.Scale;
using Newtonsoft.Json;
using Shared.Models;
using Shared.WeighSession;
using System.Text;

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

        /// <summary>
        /// Service call api lấy chi tiết đợt cân
        /// </summary>
        /// <param name="ScaleCode">Đầu cân</param>
        /// <returns></returns>
        Task<List<DetailWeighSsResponse>> GetListDetailWeighSession(string WeighSessionCode);

        /// <summary>
        /// Service call api search cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ScaleListResponse> SearchScale(SearchScaleCommand request);

        /// <summary>
        /// Service call api save cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResponse> SaveScale(SaveScaleCommand request);

        /// <summary>
        /// Service call api update cân
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> UpdateScale(UpdateScaleCommand request);
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

        public async Task<ScaleListResponse> SearchScale(SearchScaleCommand request)
        {
            //GET data weigh session
            var domainWS = new ConfigManager().WeighSessionUrl;
            var url = $"{domainWS}search-scale";

            //Convert request to json
            var json = JsonConvert.SerializeObject(request);
            //Conver json to dictionary<string, string> => form
            var requestBody = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

            var scaleData = await _httpClient.PostAsync(url, requestBody);
            var scaleResponse = await scaleData.Content.ReadAsStringAsync();

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var response = JsonConvert.DeserializeObject<ScaleListResponse>(scaleResponse, jsonSettings);

            return response;

        }

        public async Task<ApiResponse> SaveScale(SaveScaleCommand request)
        {
            //GET data weigh session
            var domainWS = new ConfigManager().WeighSessionUrl;
            var url = $"{domainWS}save-scale";

            //Convert request to json
            var json = JsonConvert.SerializeObject(request);
            //Conver json to dictionary<string, string> => form
            var requestBody = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

            var scaleData = await _httpClient.PostAsync(url, requestBody);
            var scaleResponse = await scaleData.Content.ReadAsStringAsync();

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var response = JsonConvert.DeserializeObject<ApiResponse>(scaleResponse, jsonSettings);

            return response;
        }

        public async Task<bool> UpdateScale(UpdateScaleCommand request)
        {
            //GET data weigh session
            var domainWS = new ConfigManager().WeighSessionUrl;
            var url = $"{domainWS}update-scale";

            //Convert request to json
            var json = JsonConvert.SerializeObject(request);
            //Conver json to dictionary<string, string> => form
            var requestBody = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

            var scaleData = await _httpClient.PostAsync(url, requestBody);
            var scaleResponse = await scaleData.Content.ReadAsStringAsync();

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var response = JsonConvert.DeserializeObject<ApiSuccessResponse<bool>>(scaleResponse, jsonSettings);

            return response.IsSuccess;
        }

        public async Task<List<DetailWeighSsResponse>> GetListDetailWeighSession(string WeighSessionCode)
        {
            //GET data weigh session
            var domainWS = new ConfigManager().WeighSessionUrl;
            var url = $"{domainWS}get-detail-weiss?WeighSessionCode={WeighSessionCode}";

            var weighSessionData = await _httpClient.GetAsync(url);
            var weighSessionResponse = await weighSessionData.Content.ReadAsStringAsync();

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var result = JsonConvert.DeserializeObject<ApiSuccessResponse<List<DetailWeighSsResponse>>>(weighSessionResponse, jsonSettings);

            if (!result.IsSuccess)
                return null;

            return result?.Data;
        }
    }
}