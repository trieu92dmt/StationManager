using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Applications.DTOs.TestTarget
{
    public class TestTargetDetailsResponse
    {
        public int TargetCode { get; set; }
        public string TargetName { get; set; }
        public string StepCode { get; set; }
        public bool? Actived { get; set; }
        public string Tolerance { get; set; }

    }
}
