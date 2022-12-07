using ISD.API.Applications.DTOs.Common;
using ISD.API.Applications.DTOs.WorkOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.MatchCardCommand
{
    public class MCCommandResponse
    {
        public int STT { get; set; }
        //Id
        public Guid MatchCardCommandId { get; set; }
        //Mã ghép bài
        public string MatchCardCode { get; set; }
        //Tên hạng mục in

        public string PrintItem { get; set; }
        //Số phiếu thiết kế
        public string DesignVote { get; set; }
        //Phụ trách thiết kế
        public string DesignBy { get; set; }
        //Kiểu in
        public string PrintStyle { get; set; }
        //Công ty gia công in
        public string PrintCompany { get; set; }
        //NCC kẽm
        public string ZincSupplier { get; set; }
        //Ngày ra LSX
        public string DocumentDate { get; set; }
        //Ngày yêu cầu có hàng in
        public string PrintReqDate { get; set; }
    }

    public class MCCommandListResponse
    {
        public List<MCCommandResponse> MCCommands { get; set; } = new List<MCCommandResponse>();

        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }
}
