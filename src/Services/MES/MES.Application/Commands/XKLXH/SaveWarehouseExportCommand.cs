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
    public class SaveWarehouseExportCommand : IRequest<bool>
    {
        public string Type { get; set; } //Type là ghi nhận hay chỉnh sửa (Save/Update)
        public List<SaveWarehouseExport> WarehouseExports { get; set; } = new List<SaveWarehouseExport>();
    }

    public class SaveWarehouseExport
    {
        //Id
        public Guid NKLXHId { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Confirm quantity
        public decimal? ConfirmQty { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
    }

    public class SaveWarehouseExportCommandHandler : IRequestHandler<SaveWarehouseExportCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ExportByCommandModel> _nklxhRepo;
        public SaveWarehouseExportCommandHandler(IUnitOfWork unitOfWork, IRepository<ExportByCommandModel> nklxhRepo)
        {
            _unitOfWork = unitOfWork;
            _nklxhRepo = nklxhRepo;
        }

        public async Task<bool> Handle(SaveWarehouseExportCommand request, CancellationToken cancellationToken)
        {
            //Update từng field
            foreach (var item in request.WarehouseExports)
            {
                //Lấy ra nklxh
                var nklxh = await _nklxhRepo.FindOneAsync(x => x.ExportByCommandId == item.NKLXHId);

                //Update
                //SL bao
                nklxh.BagQuantity2 = item.BagQuantity ?? 0;
                //Đơn trọng
                nklxh.SingleWeight2 = item.SingleWeight ?? 0;
                nklxh.ConfirmQty = item.ConfirmQty ?? 0;
                //SL kèm bao bì
                nklxh.QuantityWithPackaging = item.QuantityWithPackage ?? 0;
                //Nếu là tạo mới thì lưu recordTime3 là ngày tạo
                if (request.Type == "SAVE")
                    nklxh.RecordTime3 = DateTime.Now;
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
