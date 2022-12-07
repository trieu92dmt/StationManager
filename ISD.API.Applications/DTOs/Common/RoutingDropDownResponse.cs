using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.Common
{
    public class RoutingDropDownResponse
    {
        [StringLength(50)]
        public string StepCode { get; set; }
        [StringLength(100)]
        public string StepName { get; set; }
    }
}
