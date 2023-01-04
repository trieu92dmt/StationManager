﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.Infrastructure.Models
{
    [Table("WorkOrder_Mold_Mapping", Schema = "MESP2")]
    public partial class WorkOrder_Mold_Mapping
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? WorkOrderId { get; set; }
        [StringLength(50)]
        public string MoldCode { get; set; }
        [StringLength(50)]
        public string StepCode { get; set; }
        public Guid? ProductId { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("WorkOrder_Mold_Mapping")]
        public virtual ProductModel1 Product { get; set; }
        [ForeignKey("WorkOrderId")]
        [InverseProperty("WorkOrder_Mold_Mapping")]
        public virtual WorkOrderModel WorkOrder { get; set; }
    }
}