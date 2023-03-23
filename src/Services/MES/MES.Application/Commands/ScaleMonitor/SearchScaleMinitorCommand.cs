using Core.Exceptions;
using Core.Extensions;
using Core.Properties;
using Core.SeedWork;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using MES.Application.Queries;
using Newtonsoft.Json;
using Shared.Models;
using Shared.WeighSession;
using System.Text;

namespace MES.Application.Commands.ScaleMonitor
{
    public class SearchScaleMinitorCommand : IRequest<SearchScaleMonitorResponse2>
    {
        public PagingQuery Paging { get; set; } = new PagingQuery();
        //Plant
        public string PlantFrom { get; set; }
        public string PlantTo { get; set; }
        //Đầu cân
        public string WeightHeadCodeFrom { get; set; }
        public string WeightHeadCodeTo { get; set; }
        //Loại
        public string Type { get; set; }
        //Ngày ghi nhận
        public DateTime? RecordTimeFrom { get; set; }
        public DateTime? RecordTimeTo { get; set; }
    }

    public class SearchScaleMinitorCommandHandler : IRequestHandler<SearchScaleMinitorCommand, SearchScaleMonitorResponse2>
    {
        private readonly IRepository<ScaleMonitorModel> _scaleMonitorRepo;
        private readonly HttpClient _httpClient;

        public SearchScaleMinitorCommandHandler(IRepository<ScaleMonitorModel> scaleMonitorRepo, IHttpClientFactory httpClientFactory)
        {
            _scaleMonitorRepo = scaleMonitorRepo;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<SearchScaleMonitorResponse2> Handle(SearchScaleMinitorCommand request, CancellationToken cancellationToken)
        {
            //GET data weigh session
            var domainWS = new ConfigManager().WeighSessionUrl;
            var url = $"{domainWS}get-weigh-monitor-by-scale-code";

            //Convert request to json
            var json = JsonConvert.SerializeObject(request);
            //Conver json to dictionary<string, string> => form
            var requestBody = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");

            var scaleMonitorData = await _httpClient.PostAsync(url, requestBody);
            var scaleMonitorResponse = await scaleMonitorData.Content.ReadAsStringAsync();

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            var responseWS = JsonConvert.DeserializeObject<SearchScaleMonitorResponse2>(scaleMonitorResponse, jsonSettings);

            return responseWS;
        }
    }
}
