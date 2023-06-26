using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.Common
{
    public class DistrictByProvinceResponse
    {
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
    }
}
