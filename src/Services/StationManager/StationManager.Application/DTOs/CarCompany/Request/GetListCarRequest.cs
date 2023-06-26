using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany.Request
{
    public class GetListCarRequest
    {
        public Guid AccountId { get; set; }
        public string CarNumber { get; set; }
        public string CarTypeCode { get; set; }
    }
}
