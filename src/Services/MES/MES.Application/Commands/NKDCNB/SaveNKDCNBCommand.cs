using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.NKDCNB
{
    public class SaveNKDCNBCommand : IRequest<bool>
    {
        public List<SaveNKDCNB> SaveNKDCNBs { get; set; } = new List<SaveNKDCNB>();
    }

    public class SaveNKDCNB
    {
        //Plant
        public string Plant { get; set; }
        //Outbound delivery
        public string OutboundDelivery { get; set; }
        //Outbound delivery Item
        public string OutboundDeliveryItem { get; set; }
        //Material
        public string Material { get; set; }
        //UOM
        public string Unit { get; set; }
        //Storage Location
        public string Sloc { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Trọng lượng cân
        public decimal? Weight { get; set; }
        //Confirm Quantity
        public decimal? ConfirmQty { get; set; }
        //Số lượng kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Số phương tiện
        public string VehicleCode { get; set; }
        //Id - Số xe tải
        public Guid? TruckInfoId { get; set; }
        public string TruckNumber { get; set; }
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public string Image { get; set; }
        //Trạng thái
        public string Status { get; set; }
    }

    public class SaveNKDCNBCommandHandler : IRequestHandler<SaveNKDCNBCommand, bool>
    {
        public SaveNKDCNBCommandHandler()
        {
        }

        public Task<bool> Handle(SaveNKDCNBCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
