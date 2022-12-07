using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.WorkOrder
{
    public class GetWOMoldResponse
    {
        public Guid Id { get; set; }
        public string MoldCode { get; set; }
        public string StepCode { get; set; }
        public Guid? ProductId { get; set; }
    }
}
