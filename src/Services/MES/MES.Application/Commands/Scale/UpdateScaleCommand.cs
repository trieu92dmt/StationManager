using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using MES.Application.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MES.Application.Commands.Scale
{
    public class UpdateScaleCommand : IRequest<bool>
    {
        public List<ScaleUpdate> Scales { get; set; } = new List<ScaleUpdate>();
    }

    public class ScaleUpdate
    {
        //Id
        public Guid ScaleId { get; set; }
        //Scale code
        public string ScaleCode { get; set; }
        //ScaleName
        [Required]
        public string ScaleName { get; set; }
        //Cân tích hợp
        public bool isIntegrated { get; set; }
        //Cân xe tải
        public bool isTruckScale { get; set; }
        //Trạng thái
        public bool isActived { get; set; }
        //Màn hình đã chọn
        public List<string> Screens { get; set; } = new List<string>();
    }

    public class UpdateScaleCommandHandler : IRequestHandler<UpdateScaleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ScaleModel> _scaleRepo;
        private readonly IRepository<Screen_Scale_MappingModel> _mappingRepo;
        private readonly IRepository<ScreenModel> _screenRepo;
        private readonly IWeighSessionService _weiSsService;

        public UpdateScaleCommandHandler(IUnitOfWork unitOfWork, IRepository<ScaleModel> scaleRepo, IRepository<Screen_Scale_MappingModel> mappingRepo,
                                         IRepository<ScreenModel> screenRepo, IWeighSessionService weiSsService)
        {
            _unitOfWork = unitOfWork;
            _scaleRepo = scaleRepo;
            _mappingRepo = mappingRepo;
            _screenRepo = screenRepo;
            _weiSsService = weiSsService;
        }

        public async Task<bool> Handle(UpdateScaleCommand request, CancellationToken cancellationToken)
        {
            //Call service cập nhật
            var rs = await _weiSsService.UpdateScale(request);

            //Get query screen
            var screenQuery = _screenRepo.GetQuery().AsNoTracking();

            if (rs)
            {
                //Thêm mapping giữa màn hình và cân
                //Lấy ra danh sách đã mapping
                var mappings = _mappingRepo.GetQuery().Where(x => x.ScaleCode == request.Scales[0].ScaleCode);
                //Xóa những mapping đã có
                _mappingRepo.RemoveRange(mappings);
                //Tạo mới mapping
                var listMap = new List<Screen_Scale_MappingModel>();
                foreach (var item in request.Scales[0].Screens)
                {
                    listMap.Add(new Screen_Scale_MappingModel
                    {
                        Screen_Scale_Mapping_Id = Guid.NewGuid(),
                        ScaleCode = request.Scales[0].ScaleCode,
                        ScreenCode = item,
                        Actived = true
                    });
                }

                //Thêm mới
                _mappingRepo.AddRange(listMap);
            }    
            
            await _unitOfWork.SaveChangesAsync();

            //Trả response
            return true;
        }
    }
}
