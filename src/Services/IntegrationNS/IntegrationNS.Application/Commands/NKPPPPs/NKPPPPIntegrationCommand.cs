using IntegrationNS.Application.DTOs;
using ISD.Core.SeedWork.Repositories;
using ISD.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IntegrationNS.Application.Commands.NKPPPPs
{
    public class NKPPPPIntegrationCommand
    {
        //Plant
        public string Plant { get; set; }
        //Material
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }
        //Component
        public string Component { get; set; }
        //Production Order
        public string WorkorderFrom { get; set; }
        public string WorkorderTo { get; set; }
        //Sales Order
        public string SalesOrderFrom { get; set; }
        public string SalesOrderTo { get; set; }
        //Order Type
        public string OrderType { get; set; }
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
