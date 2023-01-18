using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.MES
{
    public class UpdateNKMHCommand
    {
        //Id nhập kho mua hàng
        public Guid NKMHId { get; set; }
        //Purchase Order
        public string PurchaseOrderCode { get; set; }
        //PO Item
        public string POLine { get; set; }
        //Material
        public string MaterialCode { get; set; }
        //Storage Location
        public string SlocCode { get; set; }
        //Batch
        public string Batch { get; set; }
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
}
