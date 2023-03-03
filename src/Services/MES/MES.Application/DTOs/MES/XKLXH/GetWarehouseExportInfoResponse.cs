using MES.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.XKLXH
{
    public class GetWarehouseExportInfoResponse
    {
        //STT
        public int STT { get; set; }
        //Id
        public Guid XKLXHId { get; set; }
        //Od
        public string OutboundDelivery { get; set; }
        //Od item
        public string OutboundDeliveryItem { get; set; }
        //Od/Od item
        public string OdPerItem => $"{OutboundDelivery} | {OutboundDeliveryItem}";
        //Số xe tải
        public string TruckNumber { get; set; }
        //Confirm quantity
        public decimal? ConfirmQty { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //SL bao - đơn trọng
        public string BagQuantityAndSingleWeight => $"{BagQuantity.Value.ToString()} - {SingleWeight.Value.ToString()}";
        //Số lượng kèm bao bì
        public decimal? QuantityWithPackage { get; set; }
        //Material desc
        public string MaterialDesc { get; set; }
        //Material
        public string Material { get; set; }
        //Plant
        public string Plant { get; set; }
    }

    public class ListWarehouseExportResponse
    {
        public List<GetWarehouseExportInfoResponse> WarehouseExports { get; set; } = new List<GetWarehouseExportInfoResponse>();
        //Phân trang
        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }

    public class SavedWarehouseExportResponse : GetWarehouseExportInfoResponse
    {
        //Thời gian ghi nhận
        public DateTime? RecordTime { get; set; }
    }

    public class ListSavedWarehouseExportResponse
    {
        public List<SavedWarehouseExportResponse> SavedWarehouseExports { get; set; } = new List<SavedWarehouseExportResponse>();
        //Phân trang
        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }
}
