using IntegrationNS.Application.DTOs;
using Core.SeedWork.Repositories;
using Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IntegrationNS.Application.Commands.NKMHs
{
    public class NKHTIntegrationCommand
    {
        public string PlantCode { get; set; }
        public string SalesOrderFrom { get; set; }
        public string SalesOrderTo { get; set; }
        public string OutboundDeliveryFrom { get; set; }
        public string OutboundDeliveryTo { get; set; }
        public string ShipToPartyFrom { get; set; }
        public string ShipToPartyTo { get; set; }
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }
        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }

        //Search dữ liệu đã lưu
        public string WeightHeadCode { get; set; }
        public List<string> WeightVotes { get; set; } = new List<string>();
        public DateTime? WeightDateFrom { get; set; }
        public DateTime? WeightDateTo { get; set; }
        public Guid? CreateBy { get; set; }


        public string Status { get; set; }
    }
}
