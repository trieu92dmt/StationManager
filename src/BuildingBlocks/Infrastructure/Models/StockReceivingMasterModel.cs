﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("StockReceivingMasterModel", Schema = "Warehouse")]
    public partial class StockReceivingMasterModel
    {
        public StockReceivingMasterModel()
        {
            StockReceivingDetailModel = new HashSet<StockReceivingDetailModel>();
        }

        [Key]
        public Guid StockReceivingId { get; set; }
        public int StockReceivingCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DocumentDate { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? StoreId { get; set; }
        [StringLength(50)]
        public string SalesEmployeeCode { get; set; }
        public Guid? ProfileId { get; set; }
        public string Note { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? DeletedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeletedTime { get; set; }
        public bool? isDeleted { get; set; }
        public bool? isFirst { get; set; }
        [StringLength(4000)]
        public string DeletedReason { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("StockReceivingMasterModel")]
        public virtual CompanyModel Company { get; set; }
        [ForeignKey("ProfileId")]
        [InverseProperty("StockReceivingMasterModel")]
        public virtual ProfileModel Profile { get; set; }
        [ForeignKey("SalesEmployeeCode")]
        [InverseProperty("StockReceivingMasterModel")]
        public virtual SalesEmployeeModel SalesEmployeeCodeNavigation { get; set; }
        [ForeignKey("StoreId")]
        [InverseProperty("StockReceivingMasterModel")]
        public virtual StoreModel Store { get; set; }
        [InverseProperty("StockReceiving")]
        public virtual ICollection<StockReceivingDetailModel> StockReceivingDetailModel { get; set; }
    }
}