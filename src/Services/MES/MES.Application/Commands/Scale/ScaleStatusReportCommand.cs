using Core.Extensions;
using MediatR;
using Newtonsoft.Json;
using Shared.WeighSession;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.Scale
{
    public class ScaleStatusReportCommand : IRequest<List<ScaleStatusReportResponse>>
    {
        //Plant
        public string Plant { get; set; }
        //Đầu cân
        public List<string> ScaleCode { get; set; }
    }

    public class ScaleStatusReportCommandHandler : IRequestHandler<ScaleStatusReportCommand, List<ScaleStatusReportResponse>>
    {
        private readonly HttpClient _httpClient;

        public ScaleStatusReportCommandHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<List<ScaleStatusReportResponse>> Handle(ScaleStatusReportCommand request, CancellationToken cancellationToken)
        {
            //GET data weigh session
            var domainWS = new ConfigManager().WeighSessionUrl;
            var url = $"{domainWS}scale-status-report";

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

            var response = JsonConvert.DeserializeObject<List<ScaleStatusReportResponse>>(scaleResponse, jsonSettings);

            return response;
        }
    }
}
