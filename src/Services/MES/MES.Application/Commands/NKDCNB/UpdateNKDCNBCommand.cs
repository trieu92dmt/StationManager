using ISD.Core.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NKDCNB
{
    public class UpdateNKDCNBCommand : IRequest<ApiResponse>
    {
        public List<UpdateNKDCNB> UpdateNKDCNBs { get; set; } = new List<UpdateNKDCNB>();
    }

    public class UpdateNKDCNB
    {
        //Id
        public Guid NKDCNBID { get; set; }
        //Plant
        public string Plant { get; set; }
        //Shipping point
        public string ShippingPoint { get; set; }
        //OD
        public string OutboundDelivery { get; set; }
        //OD item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //Storage Location
        public string Sloc { get; set; }
        //Batch
        public string Batch { get; set; }
        //Confirm Quantity
        public decimal? ConfirmQty { get; set; }
        //SL kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Số lần cân
        public decimal? QuantityWeight { get; set; }
        //Số phiếu cân
        public string WeightVote { get; set; }
        //Thời gian bắt đầu
        public DateTime? StartTime { get; set; }
        //Thời gian kết thúc
        public DateTime? EndTime { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Đánh dấu xóa
        public bool? isDelete { get; set; }
    }

    public class UpdateNKDCNBCommandHandler : IRequestHandler<UpdateNKDCNBCommand, ApiResponse>
    {
        public UpdateNKDCNBCommandHandler()
        {
        }

        public Task<ApiResponse> Handle(UpdateNKDCNBCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
