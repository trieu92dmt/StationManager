using Core.Interfaces.Databases;
using Core.Models;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.Scale
{
    public class SaveScaleCommand : IRequest<ApiResponse>
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
        public bool isIntegrated { get; set; } = false;
        //Cân không tích hợp
        public bool isTruckScale { get; set; } = false;
        //List màn hình
        public List<string> Screens { get; set; } = new List<string>();
    }

    public class SaveScaleCommadHandler : IRequestHandler<SaveScaleCommand, ApiResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<ScreenModel> _screenRepo;
        private readonly IRepository<Screen_Scale_MappingModel> _screenScaleRepo;

        public SaveScaleCommadHandler(IUnitOfWork unitOfWork, IRepository<ScaleModel> scaleRepo, IRepository<ScreenModel> screenRepo, IRepository<Screen_Scale_MappingModel> screenScaleRepo)
        {
            _unitOfWork = unitOfWork;
            _scaleRepo = scaleRepo;
            _screenRepo = screenRepo;
            _screenScaleRepo = screenScaleRepo;
        }

        public async Task<ApiResponse> Handle(SaveScaleCommand request, CancellationToken cancellationToken)
        {
            //Tạo response
            var response = new ApiResponse()
            {
                Code = 200,
                IsSuccess = true,
                Message = String.Format(CommonResource.Msg_Success, "Thêm mới cân"),
                Data = true
            };

            //Duyệt list scale đầu vào
            //Checkk tồn tại
            var scale = await _scaleRepo.FindOneAsync(x => x.ScaleCode == request.ScaleCode);

            //Query screen
            var screens = _screenRepo.GetQuery().AsNoTracking();

            if (scale != null)
            {
                response.IsSuccess = false;
                response.Message = $"Scale {request.ScaleCode} đã tồn tại";
                response.Data = false;
                return response;
            }

            //Không tồn tại thì tạo mới
            scale = new ScaleModel
            {
                ScaleId = Guid.NewGuid(),
                Plant = request.Plant,
                ScaleCode = request.ScaleCode,
                ScaleName = request.ScaleName,
                ScaleType = request.isIntegrated,
                isCantai = request.isTruckScale,
                Actived = true
            };

            var mapping = new List<Screen_Scale_MappingModel>();
            //Thêm mapping cân và màn hình
            foreach (var item in request.Screens)
            {
                mapping.Add(new Screen_Scale_MappingModel
                {
                    Screen_Scale_Mapping_Id = Guid.NewGuid(),
                    ScreenId = screens.FirstOrDefault(x => x.ScreenCode == item).ScreenId,
                    ScaleId = scale.ScaleId
                });
            }


            _scaleRepo.Add(scale);
            _screenScaleRepo.AddRange(mapping);
            await _unitOfWork.SaveChangesAsync();

            //Trả response
            return response;

        }
    }
}
