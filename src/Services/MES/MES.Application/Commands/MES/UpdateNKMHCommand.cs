using ISD.Core.Exceptions;
using ISD.Core.Interfaces.Databases;
using ISD.Core.Properties;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.MES
{
    public class UpdateNKMHCommand : IRequest<bool>
    {
        //Id nhập kho mua hàng
        public Guid NKMHId { get; set; }
        //Purchase Order
        public string PurchaseOrderCode { get; set; }
        //PO Item
        public string POItem { get; set; }
        //Material
        public string Material { get; set; }
        //Storage Location
        public string SlocCode { get; set; }
        //Confirm Quantity
        public decimal? ConfirmQty { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackaging { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        //Đánh dấu xóa
        public bool? isDelete { get; set; }
    }

    public class UpdateNKMHCommandHandler : IRequestHandler<UpdateNKMHCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<GoodsReceiptModel> _nkmhRepo;
        private readonly IRepository<PurchaseOrderDetailModel> _poDetailRepo;
        private readonly IRepository<StorageLocationModel> _slocRepo;

        public UpdateNKMHCommandHandler(IUnitOfWork unitOfWork, IRepository<GoodsReceiptModel> nkmhRepo, IRepository<PurchaseOrderDetailModel> poDetailRepo,
                                        IRepository<StorageLocationModel> slocRepo)
        {
            _unitOfWork = unitOfWork;
            _nkmhRepo = nkmhRepo;
            _poDetailRepo = poDetailRepo;
            _slocRepo = slocRepo;
        }

        public async Task<bool> Handle(UpdateNKMHCommand request, CancellationToken cancellationToken)
        {
            //Check tồn tại nkmh
            var nkmh = await _nkmhRepo.FindOneAsync(x => x.GoodsReceiptId == request.NKMHId);

            if (nkmh == null)
                throw new ISDException(string.Format(CommonResource.Msg_NotFound, "Nhập kho mua hàng"));

            //Lấy ra po detail
            var poDetail = _poDetailRepo.GetQuery().Include(x => x.PurchaseOrder).FirstOrDefault(x => x.POLine == request.POItem && x.PurchaseOrder.PurchaseOrderCodeInt == long.Parse(request.PurchaseOrderCode));

            //Lấy ra storage location
            var sloc = await _slocRepo.FindOneAsync(x => x.StorageLocationCode == request.SlocCode);

            //Cập nhật
            //PODetailId
            nkmh.PurchaseOrderDetailId = poDetail.PurchaseOrderDetailId;
            //Material Code
            nkmh.MaterialCode = request.Material;
            //Material Code Int
            nkmh.MaterialCodeInt = long.Parse(request.Material);
            //Storage Location
            nkmh.SlocCode = request.SlocCode;
            //Sloc Name
            nkmh.SlocName = sloc.StorageLocationName;
            //Confirm Quantity
            nkmh.ConfirmQty = request.ConfirmQty;
            //Sl kèm bao bì
            nkmh.QuantityWithPackaging = request.QuantityWithPackaging;
            //Số phương tiện
            nkmh.VehicleCode = request.VehicleCode;
            //Số cân đầu rea
            nkmh.OutputWeight = request.OutputWeight;
            //Ghi chú
            nkmh.Description = request.Description;
            //Hình ảnh
            //Đánh dấu xóa
            if (request.isDelete == true)
                nkmh.Status = "DEL";
            //Hủy đánh dấu xóa
            if (request.isDelete == false)
            {
                nkmh.Status = "NOT";
            }    

            return true;
        }
    }
}
