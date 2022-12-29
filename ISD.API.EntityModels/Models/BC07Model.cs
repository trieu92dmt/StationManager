﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("BC07Model", Schema = "Report")]
    public partial class BC07Model
    {
        [Key]
        public Guid BC07Id { get; set; }
        public int? STT { get; set; }
        [StringLength(50)]
        public string Plant { get; set; }
        [StringLength(500)]
        public string AssignResponsibility { get; set; }
        [StringLength(50)]
        public string Customer { get; set; }
        [StringLength(500)]
        public string PYCSXDT { get; set; }
        [StringLength(500)]
        public string LSXDT { get; set; }
        [StringLength(500)]
        public string DSX { get; set; }
        [StringLength(50)]
        public string VBELN { get; set; }
        [StringLength(500)]
        public string PONoiBo { get; set; }
        [StringLength(500)]
        public string SoPOKhach { get; set; }
        [StringLength(500)]
        public string MaSKU { get; set; }
        [StringLength(1000)]
        public string ProductName { get; set; }
        [StringLength(50)]
        public string ProductCode { get; set; }
        [StringLength(50)]
        public string MType { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TheTich { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLCARTON { get; set; }
        public Guid? LSXSAPId { get; set; }
        [StringLength(50)]
        public string LSXSAP { get; set; }
        [StringLength(500)]
        public string WBS { get; set; }
        [StringLength(50)]
        public string TinhTrangMT { get; set; }
        [StringLength(1000)]
        public string NguyenLieu { get; set; }
        [StringLength(1000)]
        public string HoanThien { get; set; }
        [StringLength(1000)]
        public string HinhAnh { get; set; }
        [StringLength(50)]
        public string DVT { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLDaNK { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLChuaNK { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLDaXuat { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLChuaXuat { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLTonKho { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SoContKH { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SoContDaNK { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SoContChuaNK { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SoContDaXuat { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SoContChuaXuat { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SoContTonKho { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NgayKTDKTheoLSXSAP { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NgayKTDKTheoDSX { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SLSP { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? CBM { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? TongCBMChuaXuat { get; set; }
        [StringLength(50)]
        public string CangDen { get; set; }
        [StringLength(50)]
        public string LoaiCont { get; set; }
        [StringLength(50)]
        public string NgayTauChay { get; set; }
        [StringLength(50)]
        public string StartSWDate { get; set; }
        [StringLength(50)]
        public string EndSWDate { get; set; }
        [StringLength(50)]
        public string AustinDirect { get; set; }
        [StringLength(50)]
        public string TuanXuatHang { get; set; }
        [StringLength(50)]
        public string ThangXuatHang { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NgayYeuCauXuatHangSO { get; set; }
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
        public DateTime? FinishDateChange { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeliveryDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
    }
}