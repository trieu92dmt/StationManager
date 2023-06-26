using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany
{
    public class DetailRouteResponse
    {
        public Guid RouteId { get; set; }
        public string RouteCode { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public decimal Distance { get; set; }
        public string Description { get; set; }
    }
}
