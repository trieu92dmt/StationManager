using Core.Interfaces.Databases;
using Core.Models;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.Scale
{
    public class SaveScaleCommad : IRequest<ApiResponse>
    {
        public List<Scale> Scales { get; set; } = new List<Scale>();
    }

    public class Scale 
    {
        //Plant
        [Required]
        public string Plant { get; set; }
        //Mã đầu cân
        [Required]
        public string ScaleCode { get; set; }
        //Tên đầu cân
        [Required]
        public string ScaleName { get; set; }
        //Cân tích hợp
        public bool isIntegrated { get; set; }
        //Cân không tích hợp
        public bool isTruckScale { get; set; }
    }

    public class SaveScaleCommadHandler : IRequestHandler<SaveScaleCommad, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ScaleModel> _scaleRepo;

        public SaveScaleCommadHandler(IUnitOfWork unitOfWork, IRepository<ScaleModel> scaleRepo)
        {
            _unitOfWork = unitOfWork;
            _scaleRepo = scaleRepo;
        }

        public async Task<ApiResponse> Handle(SaveScaleCommad request, CancellationToken cancellationToken)
        {
            //Tạo response
            var response = new ApiResponse()
            {
                Code = 200,
                IsSuccess = true,
                Message = String.Format(CommonResource.Msg_Success, "Thêm mới cân"),
                Data = true
            };

            var scales = new List<ScaleModel>();

            //Duyệt list scale đầu vào
            foreach (var item in request.Scales)
            {
                //Checkk tồn tại
                var scale = await _scaleRepo.FindOneAsync(x => x.ScaleCode == item.ScaleCode);
                if (scale != null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Scale {item.ScaleCode} đã tồn tại";
                    response.Data = false;
                    return response;
                }

                //Không tồn tại thì tạo mới
                scale = new ScaleModel
                {
                    ScaleId = Guid.NewGuid(),
                    Plant = item.Plant,
                    ScaleCode = item.ScaleCode,
                    ScaleName = item.ScaleName,
                    ScaleType = item.isIntegrated,
                    isCantai = item.isTruckScale,
                    Actived = true
                };

                scales.Add(scale);
            }

            _scaleRepo.AddRange(scales);
            await _unitOfWork.SaveChangesAsync();

            //Trả response
            return response;

        }
    }
}
