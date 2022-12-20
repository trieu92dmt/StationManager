﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("PurchaseOrderMasterModel", Schema = "DataCollection")]
    public partial class PurchaseOrderMasterModel
    {
        public PurchaseOrderMasterModel()
        {
            GoodsReceiptModel = new HashSet<GoodsReceiptModel>();
            PurchaseOrderDetailModel = new HashSet<PurchaseOrderDetailModel>();
        }

        [Key]
        public Guid PurchaseOrderId { get; set; }
        [StringLength(50)]
        public string PurchaseOrderCode { get; set; }
        [StringLength(50)]
        public string POType { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        [StringLength(50)]
        public string PurchasingOrg { get; set; }
        [StringLength(50)]
        public string PurchasingGroup { get; set; }
        [StringLength(50)]
        public string VendorCode { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DocumentDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }

        [InverseProperty("PurchaseOrder")]
        public virtual ICollection<GoodsReceiptModel> GoodsReceiptModel { get; set; }
        [InverseProperty("PurchaseOrder")]
        public virtual ICollection<PurchaseOrderDetailModel> PurchaseOrderDetailModel { get; set; }
    }
}