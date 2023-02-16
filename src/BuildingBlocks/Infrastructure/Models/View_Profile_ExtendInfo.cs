﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Keyless]
    public partial class View_Profile_ExtendInfo
    {
        public Guid ProfileId { get; set; }
        public int ProfileCode { get; set; }
        [StringLength(20)]
        public string ProfileForeignCode { get; set; }
        [StringLength(255)]
        public string ProfileName { get; set; }
        [StringLength(50)]
        public string PersonInChargeCompanyCode { get; set; }
        public string PersonInCharge { get; set; }
        public string RoleInCharge { get; set; }
        [StringLength(50)]
        public string CustomerGroupCompanyCode { get; set; }
        public string CustomerGroupName { get; set; }
    }
}