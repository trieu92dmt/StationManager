﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("CompanyModel", Schema = "tMasterData")]
    public partial class CompanyModel
    {
        public CompanyModel()
        {
            DeliveryModel = new HashSet<DeliveryModel>();
            StockReceivingMasterModel = new HashSet<StockReceivingMasterModel>();
            StockTransferRequestModel = new HashSet<StockTransferRequestModel>();
            StoreModel = new HashSet<StoreModel>();
            TransferModel = new HashSet<TransferModel>();
        }

        [Key]
        public Guid CompanyId { get; set; }
        [Required]
        [StringLength(50)]
        public string CompanyCode { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; }
        [StringLength(100)]
        public string TelProduct { get; set; }
        [StringLength(100)]
        public string TelService { get; set; }
        [StringLength(500)]
        public string CompanyAddress { get; set; }
        [StringLength(100)]
        public string Logo { get; set; }
        public int? OrderIndex { get; set; }
        public bool Actived { get; set; }
        [StringLength(50)]
        public string Fax { get; set; }
        [StringLength(50)]
        public string TaxCode { get; set; }
        [StringLength(100)]
        public string CompanyShortName { get; set; }
        [StringLength(50)]
        public string SMSTemplateCode { get; set; }

        [InverseProperty("Company")]
        public virtual ICollection<DeliveryModel> DeliveryModel { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<StockReceivingMasterModel> StockReceivingMasterModel { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<StockTransferRequestModel> StockTransferRequestModel { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<StoreModel> StoreModel { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<TransferModel> TransferModel { get; set; }
    }
}