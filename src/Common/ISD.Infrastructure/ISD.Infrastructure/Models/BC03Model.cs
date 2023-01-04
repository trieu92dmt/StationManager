﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("BC03Model", Schema = "Report")]
    public partial class BC03Model
    {
        [Key]
        public Guid BC03Id { get; set; }
        public int? STT { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        [StringLength(1000)]
        public string LSX { get; set; }
        [StringLength(1000)]
        public string LSXDT { get; set; }
        public Guid? DSXId { get; set; }
        [StringLength(1000)]
        public string DSX { get; set; }
        [StringLength(50)]
        public string VBELN { get; set; }
        [StringLength(1000)]
        public string PYCSXDT { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeliveryDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FinishDateChange { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? NK { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Remaining { get; set; }
        [StringLength(1000)]
        public string AssignResponsibility { get; set; }
        [StringLength(4000)]
        public string Status { get; set; }
        [StringLength(4000)]
        public string WorkShop { get; set; }
        [StringLength(1000)]
        public string Stock { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? CompletedPercent { get; set; }
        [StringLength(500)]
        public string WorkShopData { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? RemainingData { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FinishDateChangeData { get; set; }
        [StringLength(500)]
        public string TinhTrangNVL { get; set; }
        [StringLength(500)]
        public string TinhTrangLSX { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? NKTrongThang { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? NKLuyKeThangTruoc { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? CompletedPercentGo { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? CompletedPercentVan { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? CompletedPercentGiaCong { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? CompletedPercentVatTu { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? CompletedPercentHoaChat { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? CompletedPercentBBPL { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? KLTGo { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXP_SoContKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXP_SoContHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXP_SoContConLai { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PXP_NgayYCHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXP_TinhTrang { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXM_SoContKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXM_SoContHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXM_SoContConLai { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PXM_NgayYCHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXM_TinhTrang { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX4_SoContKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX4_SoContHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX4_SoContConLai { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PX4_NgayYCHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX4_TinhTrang { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX5_SoContKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX5_SoContHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX5_SoContConLai { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PX5_NgayYCHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX5_TinhTrang { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX3_SoContKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX3_SoContHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX3_SoContConLai { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PX3_NgayYCHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX3_TinhTrang { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX6_SoContKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX6_SoContHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX6_SoContConLai { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PX6_NgayYCHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX6_TinhTrang { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXC_SoContKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXC_SoContHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXC_SoContConLai { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PXC_NgayYCHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PXC_TinhTrang { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX1_SoContKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX1_SoContHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX1_SoContConLai { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PX1_NgayYCHT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? PX1_TinhTrang { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
    }
}