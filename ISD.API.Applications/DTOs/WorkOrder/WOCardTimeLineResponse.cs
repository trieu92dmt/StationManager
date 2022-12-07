using ISD.API.Constant.Common;
using Newtonsoft.Json;

namespace ISD.API.Applications.DTOs.WorkOrder
{
    public class WOCardTimeLineResponse
    {
        //1.Lệnh sản xuất
        public string WorkOrderCode { get; set; }
        //2.Thành phẩm/Bán thành phẩm
        public string ProductCode { get; set; }
        //3.Tên thành phẩm/Bán thành phẩm
        public string ProductName { get; set; }
        //4.Ngày phát lệnh
        public DateTime? DocumentDate { get; set; }
        public string DocumentDateStr => DocumentDate.HasValue ? DocumentDate.Value.ToString(ISDDateTimeFormat.DdMmyyyy) : null;
        //5.Số lượng phát lệnh
        public decimal? Quantity { get; set; }
        //6.Công đoạn hiện tại
        public string CurrentStep { get; set; }
        //7.Hướng dẫn sản xuất
        public string ProductionGuide { get; set; }
        //8.NVL sử dụng

        //9.TG đứng tại công đoạn hiện tại
        public string CurrentStepTime { get; set; }
        //10.Chuyển công đoạn
        public List<StageTranferTimeLine> StageTranferTimeLine { get; set; } = new List<StageTranferTimeLine>();
        //11.Ghi nhận sản lượng
        public List<OutputRecordTimeLine> OutputRecordTimeLine { get; set; } = new List<OutputRecordTimeLine>();

        //24.QC công đoạn sản xuất
        public List<QCTimeLine> QCTimeLine { get; set; } = new List<QCTimeLine>();

        //32.Xác nhận hoàn thành công đoạn
        public List<ConfirmStageTimeLine> ConfirmStageTimeLine { get; set; } = new List<ConfirmStageTimeLine>();

        public List<WorkOderCardTimeLineDetailResponse> WorkOderCardTimeLines { get; set; } = new List<WorkOderCardTimeLineDetailResponse>();
    }

    public class StageTranferTimeLine
    {
        //1.Từ công đoạn
        public string CurrentStep { get; set; }
        //2.Đến công đoạn
        public string NextStep { get; set; }
        //3.Người thực hiện
        public string TranferBy { get; set; }
        //4.Thời gian(8:00)
        public string TranferTime { get; set; }
        //5.Thời gian(10/10/2022)
        public string TranferDate { get; set; }
    }

    public class OutputRecordTimeLine
    {
        //17.Công đoạn
        public string CurrentStep { get; set; }
        //18.Số lượng ghi nhận
        public decimal? Quantity { get; set; }
        //19.Khuôn(Mã và serial)
        public List<MoldOutputTimeLine> MoldOutputTimeLines { get; set; } = new List<MoldOutputTimeLine>();
        //20.Người thực hiện
        public string RecordBy { get; set; }
        //21.Thời gian ghi nhận(Từ…đến…)
        public string RecordTime { get; set; }
        //23.Thời gian(10/10/2022)
        public string RecordDate { get; set; }
    }
    public class MoldOutputTimeLine
    {
        public string MoldCode { get; set; }
        public string Serial { get; set; }
    }

    public class QCTimeLine
    {
        //25.Công đoạn
        public string CurrentStep { get; set; }
        //26.Kết quả
        public string ResultQC { get; set; }
        //27.Chỉ tiêu kiểm tra
        public string TestTargetName { get; set; }
        //28.Kết quả chi tiết
        public string ResultDetail { get; set; }
        //29.Người thực hiện
        public string QCBy { get; set; }
        //30.Thời gian(8:00)
        public string QCTime { get; set; }
        //31.Thời gian(10/10/2022)
        public string QCDate { get; set; }
    }

    public class ConfirmStageTimeLine
    {
        //33.Công đoạn hoàn thành
        public string StepFinish { get; set; }
        //34.Số lượng hoàn thành
        public decimal? Quantiy { get; set; }
        //35.Người thực hiện
        public string ConfirmBy { get; set; }
        //36.Thời gian(8:00)
        public string ConfirmTime { get; set; }
        //37.Thời gian(10/10/2022)
        public string ConfirmDate { get; set; }
    }

    public class WorkOderCardTimeLineDetailResponse
    {
        public string Title { get; set; }
        public string Color { get; set; }
        public int Type { get; set; }
        public DateTime? LastTime { get; set; }
        public string LastTimeStr => LastTime.HasValue ? LastTime.Value.ToString(ISDDateTimeFormat.DdMmyyyy) : string.Empty;  
        public List<object> Description { get; set; } = new List<object>();
        public string DescriptionStr => Description.Any() ? JsonConvert.SerializeObject(Description) : null;
    }
}
