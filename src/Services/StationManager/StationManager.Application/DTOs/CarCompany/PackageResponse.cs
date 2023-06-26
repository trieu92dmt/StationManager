using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany
{
    public class PackageResponse
    {
        public Guid PackageId { get; set; }
        public string PackageCode { get; set; }
        public string PackageName { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public int CarQuantity { get; set; }
        public int RouteQuantity { get; set; }
        public int TripPerDay { get; set; }
    }
}
