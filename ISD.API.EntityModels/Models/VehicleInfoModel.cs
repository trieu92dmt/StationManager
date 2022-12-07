﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("VehicleInfoModel", Schema = "ghService")]
    public partial class VehicleInfoModel
    {
        public VehicleInfoModel()
        {
            AccessorySaleOrderModel = new HashSet<AccessorySaleOrderModel>();
            ServiceOrderModel = new HashSet<ServiceOrderModel>();
        }

        [Key]
        public Guid VehicleId { get; set; }
        public Guid? CustomerId { get; set; }
        [StringLength(200)]
        public string LicensePlate { get; set; }
        [StringLength(200)]
        public string SerialNumber { get; set; }
        [StringLength(200)]
        public string EngineNumber { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CurrentKilometers { get; set; }
        [StringLength(200)]
        public string WarrantyNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? BuyDate { get; set; }
        [StringLength(50)]
        public string SaleOrg { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        [StringLength(100)]
        public string ProfitCenterCode { get; set; }
        [StringLength(100)]
        public string ProductHierarchyCode { get; set; }
        [StringLength(200)]
        public string SoBinhDien { get; set; }

        [InverseProperty("Vehicle")]
        public virtual ICollection<AccessorySaleOrderModel> AccessorySaleOrderModel { get; set; }
        [InverseProperty("Vehicle")]
        public virtual ICollection<ServiceOrderModel> ServiceOrderModel { get; set; }
    }
}