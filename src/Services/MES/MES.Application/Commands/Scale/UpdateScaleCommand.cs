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
    public class UpdateScaleCommand : IRequest<bool>
    {
        public List<ScaleUpdate> Scales { get; set; } = new List<ScaleUpdate>();
    }

    public class ScaleUpdate
    {
        //Id
        [Required]
        public Guid ScaleId { get; set; }
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

        public UpdateScaleCommandHandler(IUnitOfWork unitOfWork, IRepository<ScaleModel> scaleRepo, IRepository<Screen_Scale_MappingModel> mappingRepo,
                                         IRepository<ScreenModel> screenRepo)
        {
            _unitOfWork = unitOfWork;
            _scaleRepo = scaleRepo;
            _mappingRepo = mappingRepo;
            _screenRepo = screenRepo;
        }

        public async Task<bool> Handle(UpdateScaleCommand request, CancellationToken cancellationToken)
        {
            //Get query screen
            var screenQuery = _screenRepo.GetQuery().AsNoTracking();

            //Lấy ra scale cần chỉnh sửa
            var scale = await _scaleRepo.FindOneAsync(x => x.ScaleId == request.Scales[0].ScaleId);

            //cập nhật
            //Tên đầu cân
            scale.ScaleName = request.Scales[0].ScaleName;
            //Loại cân
            scale.ScaleType = request.Scales[0].isIntegrated;
            //Là cân xe tải ?
            scale.isCantai = request.Scales[0].isTruckScale;
            scale.Actived = request.Scales[0].isActived;


            //Thêm mapping giữa màn hình và cân
            //Lấy ra danh sách đã mapping
            var mappings = _mappingRepo.GetQuery().Where(x => x.ScaleId == scale.ScaleId);
            //Xóa những mapping đã có
            _mappingRepo.RemoveRange(mappings);
            //Tạo mới mapping
            var listMap = new List<Screen_Scale_MappingModel>();
            foreach (var item in request.Scales[0].Screens) 
            {
                listMap.Add(new Screen_Scale_MappingModel
                {
                    Screen_Scale_Mapping_Id = Guid.NewGuid(),
                    ScaleId = request.Scales[0].ScaleId,
                    ScreenId = screenQuery.FirstOrDefault(x => x.ScreenCode == item).ScreenId,
                    Actived =true
                });
            }
            //Thêm mới
            _mappingRepo.AddRange(listMap);

            await _unitOfWork.SaveChangesAsync();

            //Trả response
            return true;
        }
    }
}
