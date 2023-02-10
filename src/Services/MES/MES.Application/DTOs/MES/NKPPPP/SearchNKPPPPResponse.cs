using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.DTOs.MES.NKPPPP
{
    public class SearchWOResponse
    {
        //Plant
        public string Plant { get; set; }
        //Production Order
        public string WorkOrder { get; set; }
        //Material
        public string Material { get; set; }
        //Material Desc
        public string MaterialDesc { get; set; }
        //Component
        public string Component { get; set; }
        //Component
        public string ComponentDesc { get; set; }
        //Sales Order
        public string SalesOrder { get; set; }
        //OrderType
        public string OrderType { get; set; }
        //Schedule Star Time
        public DateTime? ScheduleStartTime { get; set; }
        //Schedule Finish Time
        public DateTime? ScheduleFinishTime { get; set; }
        //Storage Location
        public string Sloc { get; set; }
        //Batch
        public string Batch { get; set; }
        //Requirement Qty
        public decimal? RequirementQty { get; set; }
        //Withdrawn Qty
        public decimal? WithdrawQty { get; set; }
        //UoM
        public string Unit { get; set; }
    }
}
