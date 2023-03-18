using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.WeighSession
{
    public class WeighSessionResponse
    {
        public int STT { get; set; }
        //Id đợt cân
        public string WeighSessionId { get; set; }
        //Số lần cân
        public int TotalNumberOfWeigh { get; set; }
        //Số phiếu cân
        public string WeightVote { get; set; }
        //Mã đầu cân
        public string ScaleCode { get; set; }
        //Tên đầu cân
        public string ScaleName { get; set; }
        //DateKey
        public string DateKey { get; set; }
        //STT đợt cân
        public int OrderIndex { get; set; }
        //Thời gian bắt đầu
        public DateTime? StartTime { get; set; }
        //Thời gian kết thúc
        public DateTime? EndTime { get; set; }
        //Trọng lượng cân
        public decimal? TotalWeight { get; set; }
        //Confirm quantity
        public decimal? ConfirmQuantity { get; set; }
        //Ghi chú
        public string Description { get; set; }
        //Hình ảnh
        public List<string> Images { get; set; }
        //Người tạo
        public Guid? CreateById { get; set; }
        public string CreateBy { get; set; }
        //Trạng thái
        public string Status { get; set; }
        //Đánh dấu xóa
        public bool isDelete => Status == "DEL" ? true : false;
        //Nghiệp vụ
        public string TransactionType { get; set; }
    }
}
