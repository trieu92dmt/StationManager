using Core.Interfaces.Databases;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.XKLXH
{
    public class SaveTruckWeighInfoCommand : IRequest<bool>
    {
        public string Type { get; set; } //Type là ghi nhận hay chỉnh sửa (Save/Update)
        public List<SaveTruckWeighInfo> TruckWeighInfos { get; set; } = new List<SaveTruckWeighInfo>();
    }

    public class SaveTruckWeighInfo
    {
        //Id
        public Guid NKLXHId { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
    }

    public class SaveTruckWeighInfoCommandHandler : IRequestHandler<SaveTruckWeighInfoCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ExportByCommandModel> _nklxhRepo;

        public SaveTruckWeighInfoCommandHandler(IUnitOfWork unitOfWork, IRepository<ExportByCommandModel> nklxhRepo)
        {
            _unitOfWork = unitOfWork;
            _nklxhRepo = nklxhRepo;
        }

        public async Task<bool> Handle(SaveTruckWeighInfoCommand request, CancellationToken cancellationToken)
        {
            //Update từng field
            foreach (var item in request.TruckWeighInfos)
            {
                //Lấy ra nklxh
                var nklxh = await _nklxhRepo.FindOneAsync(x => x.ExportByCommandId == item.NKLXHId);

                //Update
                nklxh.OutputWeight = item.OutputWeight ?? 0;
                nklxh.GoodsWeight = Math.Abs(nklxh.OutputWeight.Value - nklxh.InputWeight.Value);
                if (request.Type == "SAVE")
                    nklxh.RecordTime2 = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
