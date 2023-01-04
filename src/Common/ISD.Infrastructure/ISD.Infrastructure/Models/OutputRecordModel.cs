﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("OutputRecordModel", Schema = "MESP2")]
    public partial class OutputRecordModel
    {
        public OutputRecordModel()
        {
            EmployeeRecordModel = new HashSet<EmployeeRecordModel>();
            EquimenRecordModel = new HashSet<EquimenRecordModel>();
            MoldRecordModel = new HashSet<MoldRecordModel>();
        }

        [Key]
        public Guid OutputRecordId { get; set; }
        public Guid? WorkOrderCardId { get; set; }
        public Guid? WorkOrderId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? DepartmentId { get; set; }
        [StringLength(50)]
        public string StepFinish { get; set; }
        public Guid? StockId { get; set; }
        public Guid? StaffRecord { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Quantity { get; set; }
        [StringLength(50)]
        public string OutputRecordType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RecordFromTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RecordToTime { get; set; }
        public int? DateKey { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? ProductOnSheet { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SheetOnStep { get; set; }
        [StringLength(50)]
        public string MovementType { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateTime { get; set; }
        public Guid? LastEditBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastEditTime { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("DepartmentId")]
        [InverseProperty("OutputRecordModel")]
        public virtual DepartmentModel Department { get; set; }
        [ForeignKey("ProductId")]
        [InverseProperty("OutputRecordModel")]
        public virtual ProductModel1 Product { get; set; }
        [ForeignKey("StockId")]
        [InverseProperty("OutputRecordModel")]
        public virtual StockModel Stock { get; set; }
        [ForeignKey("WorkOrderId")]
        [InverseProperty("OutputRecordModel")]
        public virtual WorkOrderModel WorkOrder { get; set; }
        [ForeignKey("WorkOrderCardId")]
        [InverseProperty("OutputRecordModel")]
        public virtual WorkOrderCardModel WorkOrderCard { get; set; }
        [InverseProperty("OutputRecord")]
        public virtual ICollection<EmployeeRecordModel> EmployeeRecordModel { get; set; }
        [InverseProperty("OutputRecord")]
        public virtual ICollection<EquimenRecordModel> EquimenRecordModel { get; set; }
        [InverseProperty("OutputRecord")]
        public virtual ICollection<MoldRecordModel> MoldRecordModel { get; set; }
    }
}