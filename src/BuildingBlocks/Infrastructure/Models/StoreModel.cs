﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    [Table("StoreModel", Schema = "tMasterData")]
    public partial class StoreModel
    {
        public StoreModel()
        {
            DeliveryModel = new HashSet<DeliveryModel>();
            DepartmentModel = new HashSet<DepartmentModel>();
            PromotionByStoreModel = new HashSet<PromotionByStoreModel>();
            StockReceivingMasterModel = new HashSet<StockReceivingMasterModel>();
            StockTransferRequestModel = new HashSet<StockTransferRequestModel>();
            Stock_Store_Mapping = new HashSet<Stock_Store_Mapping>();
            TransferModel = new HashSet<TransferModel>();
            Account = new HashSet<AccountModel>();
        }

        [Key]
        public Guid StoreId { get; set; }
        [StringLength(50)]
        public string SaleOrgCode { get; set; }
        public Guid? StoreTypeId { get; set; }
        [Required]
        [StringLength(100)]
        public string StoreName { get; set; }
        public Guid CompanyId { get; set; }
        [StringLength(100)]
        public string TelProduct { get; set; }
        [StringLength(100)]
        public string TelService { get; set; }
        [StringLength(500)]
        public string StoreAddress { get; set; }
        [StringLength(50)]
        public string Area { get; set; }
        public Guid? ProvinceId { get; set; }
        public Guid? DistrictId { get; set; }
        [StringLength(100)]
        public string Fax { get; set; }
        [StringLength(1000)]
        public string LogoUrl { get; set; }
        [StringLength(1000)]
        public string ImageUrl { get; set; }
        public int? OrderIndex { get; set; }
        public bool Actived { get; set; }
        [StringLength(50)]
        public string mLat { get; set; }
        [StringLength(50)]
        public string mLong { get; set; }
        [StringLength(200)]
        public string InvoiceStoreName { get; set; }
        [StringLength(50)]
        public string DefaultCustomerSource { get; set; }
        [StringLength(50)]
        public string SMSTemplateCode { get; set; }

        [ForeignKey("CompanyId")]
        [InverseProperty("StoreModel")]
        public virtual CompanyModel Company { get; set; }
        [InverseProperty("Store")]
        public virtual ICollection<DeliveryModel> DeliveryModel { get; set; }
        [InverseProperty("Store")]
        public virtual ICollection<DepartmentModel> DepartmentModel { get; set; }
        [InverseProperty("Store")]
        public virtual ICollection<PromotionByStoreModel> PromotionByStoreModel { get; set; }
        [InverseProperty("Store")]
        public virtual ICollection<StockReceivingMasterModel> StockReceivingMasterModel { get; set; }
        [InverseProperty("Store")]
        public virtual ICollection<StockTransferRequestModel> StockTransferRequestModel { get; set; }
        [InverseProperty("Store")]
        public virtual ICollection<Stock_Store_Mapping> Stock_Store_Mapping { get; set; }
        [InverseProperty("Store")]
        public virtual ICollection<TransferModel> TransferModel { get; set; }

        [ForeignKey("StoreId")]
        [InverseProperty("Store")]
        public virtual ICollection<AccountModel> Account { get; set; }
    }
}