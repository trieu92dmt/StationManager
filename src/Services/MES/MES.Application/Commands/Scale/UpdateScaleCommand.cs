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
    public class UpdateScaleCommand : IRequest<bool>
    {
        public List<ScaleUpdate> Scales { get; set; } = new List<ScaleUpdate>();
    }

    public class ScaleUpdate
    {
        //Id
        public Guid ScaleId { get; set; }
        //ScaleName
        public string ScaleName { get; set; }
        //Cân tích hợp
        public bool isIntegrated { get; set; }
        //Cân xe tải
        public bool isTruckScale { get; set; }
        //Trạng thái
        public bool isActived { get; set; }
    }

    public class UpdateScaleCommandHandler : IRequestHandler<UpdateScaleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ScaleModel> _scaleRepo;

        public UpdateScaleCommandHandler(IUnitOfWork unitOfWork, IRepository<ScaleModel> scaleRepo)
        {
            _unitOfWork = unitOfWork;
            _scaleRepo = scaleRepo;
        }

        public async Task<bool> Handle(UpdateScaleCommand request, CancellationToken cancellationToken)
        {
            //Duyệt list scale đầu vào
            foreach (var item in request.Scales)
            {
                //Lấy ra scale cần chỉnh sửa
                var scale = await _scaleRepo.FindOneAsync(x => x.ScaleId == item.ScaleId);

                //cập nhật
                scale.ScaleName = item.ScaleName;
                scale.ScaleType = item.isIntegrated;
                scale.isCantai = item.isTruckScale;
                scale.Actived = item.isActived;
            }

            await _unitOfWork.SaveChangesAsync();

            //Trả response
            return true;
        }
    }
}
