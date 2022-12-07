using ISD.API.Applications.DTOs.Common;
using ISD.API.Applications.DTOs.WorkOrder;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.CommandQC
{
    public class CommandQCResponse
    {
        public int STT { get; set; }
        //Mã
        public Guid CommandQCId { get; set; }
        public Guid? WorkOrderCardId { get; set; }
        //Mã lệnh QC
        public int CommandQCCode { get; set; }
        //Loại
        public string QCType { get; set; }
        public string QCTypeName { get; set; }
        //MÃ BTP/TP/NVL
        public string ProductCodeNames { get; set; }
        //Mã LSX
        public string WorkOrderCode { get; set; }
        //Công đoạn
        public string StepCode { get; set; }
        //Tên công đoạn
        public string StepName { get; set; }
        //Thời gian tạo
        public DateTime? CreateTime { get; set; }
        //Ngày QC
        public DateTime? QCTime { get; set; }
        //Kết quả
        public string ResultQC { get; set; }
    }

    public class CommandQCListResponse
    {
        public List<CommandQCResponse> CommandQCs { get; set; } = new List<CommandQCResponse>();

        public PagingResponse PagingRep { get; set; } = new PagingResponse();
    }
}
