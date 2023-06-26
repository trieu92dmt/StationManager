using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StationManager.Application.DTOs.CarCompany
{
    public class DetailEmployeeResponse
    {
        public Guid EmployeeId { get; set; }
        public int EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string PositionCode { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
    }
}
