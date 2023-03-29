using Core.Interfaces.Databases;
using Core.Properties;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using MES.Application.Services;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using System.ComponentModel.DataAnnotations;

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
        private readonly IWeighSessionService _weiSsService;

        public SaveScaleCommadHandler(IUnitOfWork unitOfWork, IRepository<ScaleModel> scaleRepo, IRepository<ScreenModel> screenRepo, 
                                      IRepository<Screen_Scale_MappingModel> screenScaleRepo, IWeighSessionService weiSsService)
        {
            _unitOfWork = unitOfWork;
            _scaleRepo = scaleRepo;
            _screenRepo = screenRepo;
            _screenScaleRepo = screenScaleRepo;
            _weiSsService = weiSsService;
        }

        public async Task<ApiResponse> Handle(SaveScaleCommand request, CancellationToken cancellationToken)
        {
            //REsponse call service
            var serviceResponse = await _weiSsService.SaveScale(request);

            if (serviceResponse.IsSuccess)
            {
                var mapping = new List<Screen_Scale_MappingModel>();
                //Thêm mapping cân và màn hình
                foreach (var item in request.Screens)
                {
                    mapping.Add(new Screen_Scale_MappingModel
                    {
                        Screen_Scale_Mapping_Id = Guid.NewGuid(),
                        ScaleCode = request.ScaleCode,
                        ScreenCode = item
                    });
                }

                _screenScaleRepo.AddRange(mapping);
            }    

            await _unitOfWork.SaveChangesAsync();

            //Trả response
            return serviceResponse;

        }
    }
}
