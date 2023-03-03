using MES.Application.DTOs.Common;
using MES.Application.DTOs.MES.Scale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.XKLXH
{
    public class GetTruckWeighInfoResponse
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
        //Số cân đầu ra
        public decimal? OutputWeight { get; set; }
        //Trọng lượng hàng hóa
        public decimal? GoodsWeight { get; set; }
        //Confirm quantity
        public decimal? ConfirmQty { get; set; }
        //SL bao
        public int? BagQuantity { get; set; }
        //Đơn trọng
        public decimal? SingleWeight { get; set; }
        //SL bao - đơn trọng
        public string BagQuantityAndSingleWeight => $"{BagQuantity.Value.ToString()} - {SingleWeight.Value.ToString()}";
        //Material desc
        public string MaterialDesc { get; set; }
        //Material
        public string Material { get; set; }
        //Số cân đầu vào
        public decimal? InputWeight { get; set; }
        //Plant
        public string Plant { get; set; }
    }

    public class GetListTruckWeighInfoResponse
    {
        public List<GetTruckWeighInfoResponse> TruckWeightInfos { get; set; } = new List<GetTruckWeighInfoResponse>();
        //Phân trang
        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }

    public class SavedDataTruckWeighReponse : GetTruckWeighInfoResponse
    {
        //Thời gian ghi nhận
        public DateTime? RecordTime { get; set; }
    }

    public class ListSavedDataTruckWeighResponse
    {
        public List<SavedDataTruckWeighReponse> SavedDataTruckWeighs { get; set; } = new List<SavedDataTruckWeighReponse>();
        //Phân trang
        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }
}
