﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("DimDateModel", Schema = "Warehouse")]
    public partial class DimDateModel
    {
        public DimDateModel()
        {
            DeliveryDetailModel = new HashSet<DeliveryDetailModel>();
            StockReceivingDetailModel = new HashSet<StockReceivingDetailModel>();
            TransferDetailModel = new HashSet<TransferDetailModel>();
        }

        [Key]
        public int DateKey { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string FullDateUK { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string FullDateUSA { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string DayOfMonth { get; set; }
        [StringLength(4)]
        [Unicode(false)]
        public string DaySuffix { get; set; }
        [StringLength(9)]
        [Unicode(false)]
        public string DayName { get; set; }
        [StringLength(1)]
        [Unicode(false)]
        public string DayOfWeekUSA { get; set; }
        [StringLength(1)]
        [Unicode(false)]
        public string DayOfWeekUK { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string DayOfWeekInMonth { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string DayOfWeekInYear { get; set; }
        [StringLength(3)]
        [Unicode(false)]
        public string DayOfQuarter { get; set; }
        [StringLength(3)]
        [Unicode(false)]
        public string DayOfYear { get; set; }
        [StringLength(1)]
        [Unicode(false)]
        public string WeekOfMonth { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string WeekOfQuarter { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string WeekOfYear { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string Month { get; set; }
        [StringLength(9)]
        [Unicode(false)]
        public string MonthName { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string MonthOfQuarter { get; set; }
        [StringLength(1)]
        [Unicode(false)]
        public string Quarter { get; set; }
        [StringLength(9)]
        [Unicode(false)]
        public string QuarterName { get; set; }
        [StringLength(4)]
        [Unicode(false)]
        public string Year { get; set; }
        [StringLength(7)]
        [Unicode(false)]
        public string YearName { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string MonthYear { get; set; }
        [StringLength(6)]
        [Unicode(false)]
        public string MMYYYY { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FirstDayOfMonth { get; set; }
        [Column(TypeName = "date")]
        public DateTime? LastDayOfMonth { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FirstDayOfQuarter { get; set; }
        [Column(TypeName = "date")]
        public DateTime? LastDayOfQuarter { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FirstDayOfYear { get; set; }
        [Column(TypeName = "date")]
        public DateTime? LastDayOfYear { get; set; }
        public bool? IsHolidayUSA { get; set; }
        public bool? IsWeekday { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string HolidayUSA { get; set; }
        public bool? IsHolidayUK { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string HolidayUK { get; set; }
        [StringLength(3)]
        [Unicode(false)]
        public string FiscalDayOfYear { get; set; }
        [StringLength(3)]
        [Unicode(false)]
        public string FiscalWeekOfYear { get; set; }
        [StringLength(2)]
        [Unicode(false)]
        public string FiscalMonth { get; set; }
        [StringLength(1)]
        [Unicode(false)]
        public string FiscalQuarter { get; set; }
        [StringLength(9)]
        [Unicode(false)]
        public string FiscalQuarterName { get; set; }
        [StringLength(4)]
        [Unicode(false)]
        public string FiscalYear { get; set; }
        [StringLength(7)]
        [Unicode(false)]
        public string FiscalYearName { get; set; }
        [StringLength(10)]
        [Unicode(false)]
        public string FiscalMonthYear { get; set; }
        [StringLength(6)]
        [Unicode(false)]
        public string FiscalMMYYYY { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FiscalFirstDayOfMonth { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FiscalLastDayOfMonth { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FiscalFirstDayOfQuarter { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FiscalLastDayOfQuarter { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FiscalFirstDayOfYear { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FiscalLastDayOfYear { get; set; }

        [InverseProperty("DateKeyNavigation")]
        public virtual ICollection<DeliveryDetailModel> DeliveryDetailModel { get; set; }
        [InverseProperty("DateKeyNavigation")]
        public virtual ICollection<StockReceivingDetailModel> StockReceivingDetailModel { get; set; }
        [InverseProperty("DateKeyNavigation")]
        public virtual ICollection<TransferDetailModel> TransferDetailModel { get; set; }
    }
}