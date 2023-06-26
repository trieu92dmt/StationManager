using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.Common
{
    public class ProviceDistrictResponse
    {
        public string ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public List<DistrictResponse> DistrictResponses { get; set; } = new List<DistrictResponse>();
    }

    public class DistrictResponse
    {
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
    }
}
