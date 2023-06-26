using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany.Request
{
    public class GetListRouteRequest
    {
        public Guid AccountId { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
    }
}
