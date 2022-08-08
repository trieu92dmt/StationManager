using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using WEIGHT.EntityModels.Core;
using WEIGHT.EntityModels.Datas;
using WEIGHT.EntityModels.Models;
using WEIGHT.EntityModels.Responses;
using WEIGHT.EntityModels.ViewModels;

namespace WEIGHT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeightController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public readonly string ConnectionString = MongoDbConfig.ConnectionString;
        public readonly string DatabaseName = MongoDbConfig.DatabaseName;
        public readonly string ResourceMonitor = MongoDbConfig.CollectionName;


        public WeightController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>API "Save thông tin "WeightDetail"</summary>
        /// <remarks>
        /// BODY
        ///  
        ///     {
        ///       "weightTime": "2022-08-05T08:15:27.929Z",
        ///       "weightPerWeighing": 0,
        ///       "weightStationCode": "string"
        ///     }
        ///     
        /// OUT PUT
        /// 
        ///     {
        ///         "statusCode": 200,
        ///         "message": "Lưu thông tin cân thành công.",
        ///         "data": true    
        ///     }
        ///</remarks>
        [HttpPost("save-weight-detail")]
        public async Task<IActionResult> SaveWeightDetail(SaveWeightDetailViewModel request)
        {
            _context.WeightDetailModel.Add(new WeightDetailModel
            {
                WeightTime = request.WeightTime,
                WeightStationCode = request.WeightStationCode,
                WeightPerWeighing = request.WeightPerWeighing,
                RawData = request.RawData
            });

            await _context.SaveChangesAsync();

            return Ok(new ApiSuccessReponse<bool> { Data = true, Message = string.Format(CommonResource.MSG_SUCCESS, "Lưu thông tin cân") });
        }

        /// <summary>API "Save thông tin "ResourceMonitor"</summary>
        /// <remarks>
        /// BODY
        ///  
        ///     {
        ///       "weighingStationCode": "string",
        ///       "checkingTime": "2022-08-05T08:13:48.096Z",
        ///       "weightNumberDisplayed": 0
        ///     }
        ///     
        /// OUT PUT
        /// 
        ///     {
        ///         "statusCode": 200,
        ///         "message": "Lưu thông tin cân thành công.",
        ///         "data": true    
        ///     }
        ///</remarks>
        [HttpPost("save-resource-monitor")]
        public IActionResult SaveResourceMonitor(SaveResourceMonitorModel request)
        {
            //Get connection string
            var settings = MongoClientSettings.FromConnectionString(ConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);

            //Get database 
            var database = client.GetDatabase(DatabaseName);

            //Get collection
            var rM = database.GetCollection<ResourceMonitorModel>(ResourceMonitor);

            //Insert data
            rM.InsertOne(new ResourceMonitorModel
            {
                CheckingTime = request.CheckingTime,
                WeighingStationCode = request.WeighingStationCode,
                WeightNumberDisplayed = request.WeightNumberDisplayed
            });

            return Ok(new ApiSuccessReponse<bool> { Data = true, Message = string.Format(CommonResource.MSG_SUCCESS, "Lưu thông tin cân") });

        }

        /// <summary>API "Tìm thông tin theo weight time</summary>
        [HttpGet("list-weight-detail")]
        public IActionResult ListWeightDetail([FromQuery]ListWeightDetailViewModel vm)
        {
            //Query weight detail order by decs weight time
            var response = _context.WeightDetailModel.OrderByDescending(x => x.WeightTime).AsQueryable();
            var result = response.Take(vm.Length).Select(v => new WeightDetailResponse
            {
                WeightTime = v.WeightTime,
                WeightStationCode = v.WeightStationCode,
                WeightPerWeighing = (double)v.WeightPerWeighing,
                RawData = v.RawData

            }).ToList();

            //Search by Weight time
            if (vm.WeightTime.HasValue) result = result.Where(x => x.WeightTime.Date == vm.WeightTime.Value.Date).ToList();

            //Total recorđ
            var totalRecord = result.Count();

            //Get stt
            if (result.Any())
            {
                int i = 0;
                foreach (var item in result)
                {
                    i++;
                    item.STT = i;
                }
            }

            return Ok(new ApiSuccessReponse<List<WeightDetailResponse>> { Data = result, Message = string.Format(CommonResource.MSG_SUCCESS, "Tìm kiếm thông tin cân"), TotalRecord = totalRecord });
        }


        /// <summary>API "Tìm thông tin theo resource monitor</summary>
        [HttpGet("list-resource-monitor")]
        public IActionResult ListResourceMonitor(DateTime? CheckingTime)
        {
            //Get connection string
            var settings = MongoClientSettings.FromConnectionString(ConnectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);

            //Get database 
            var database = client.GetDatabase(DatabaseName);

            //Get collection
            var rM = database.GetCollection<ResourceMonitorModel>(ResourceMonitor);

            var respose = rM.AsQueryable().Select(e => new ResourceMonitorResponse { 
                CheckingTime = e.CheckingTime,
                WeighingStationCode = e.WeighingStationCode,
                WeightNumberDisplayed = e.WeightNumberDisplayed
            }).OrderByDescending(x => x.CheckingTime).ToList();

            if (CheckingTime.HasValue) respose = respose.Where(x => x.CheckingTime.Date == CheckingTime.Value.Date).ToList();

            return Ok(new ApiSuccessReponse<List<ResourceMonitorResponse>> { Data = respose, Message = string.Format(CommonResource.MSG_SUCCESS, "Tìm kiếm thông tin cân") });
        }
    }
}
