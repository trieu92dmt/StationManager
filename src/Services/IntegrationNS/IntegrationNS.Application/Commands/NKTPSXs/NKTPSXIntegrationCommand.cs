using IntegrationNS.Application.DTOs;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IntegrationNS.Application.Commands.NKTPSXs
{
    public class NKTPSXIntegrationCommand
    {
        //Plant
        public string Plant { get; set; }
        //Order Type
        public string OrderType { get; set; }
        //Production Order From
        public string WorkOrderFrom { get; set; }
        //Production Order To
        public string WorkOrderTo { get; set; }
        //Sale order from
        public string SaleOrderFrom { get; set; }
        //Sale order to
        public string SaleOrderTo { get; set; }
        //Material from
        public string MaterialFrom { get; set; }
        //Material to
        public string MaterialTo { get; set; }
        //Scheduled Start from
        public DateTime? ScheduledStartFrom { get; set; }
        //Scheduled Start to
        public DateTime? ScheduledStartTo { get; set; }

        //Dữ liệu đã lưu
        //Đầu cân
        public string WeightHeadCode { get; set; }
        //Số phiếu cân 
        public List<string> WeightVotes { get; set; } = new List<string>();
        //Ngày thực hiện cân from
        public DateTime? WeightDateFrom { get; set; }
        //Ngày thực hiện cân to
        public DateTime? WeightDateTo { get; set; }
        //Create by
        public Guid? CreateBy { get; set; }
        public string Status { get; set; }

    }
}
