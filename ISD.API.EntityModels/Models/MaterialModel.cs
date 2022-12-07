﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISD.API.EntityModels.Models
{
    [Table("MaterialModel", Schema = "ghMasterData")]
    public partial class MaterialModel
    {
        public MaterialModel()
        {
            SaleOrderMasterModel = new HashSet<SaleOrderMasterModel>();
        }

        [Key]
        [StringLength(50)]
        public string MaterialCode { get; set; }
        [StringLength(400)]
        public string MaterialName { get; set; }
        [StringLength(50)]
        public string MaterialUnit { get; set; }
        [StringLength(50)]
        public string MaterialUnitName { get; set; }
        [StringLength(50)]
        public string MaterialGroupCode { get; set; }
        [StringLength(50)]
        public string ProfitCenterCode { get; set; }
        [StringLength(50)]
        public string ProductHierarchyCode { get; set; }
        [StringLength(50)]
        public string LaborCode { get; set; }
        [StringLength(50)]
        public string MaterialFreightGroupCode { get; set; }
        [StringLength(50)]
        public string ExternalMaterialGroupCode { get; set; }
        [StringLength(50)]
        public string TemperatureConditionCode { get; set; }
        [StringLength(50)]
        public string ContainerRequirementCode { get; set; }
        [StringLength(155)]
        public string InternalComment { get; set; }
        [StringLength(155)]
        public string SalesText { get; set; }
        [StringLength(155)]
        public string BasicDataText { get; set; }
        [StringLength(50)]
        public string OldMaterial { get; set; }
        [StringLength(100)]
        public string Dimension { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Capacity { get; set; }
        [StringLength(10)]
        public string CapacityUnit { get; set; }
        [StringLength(400)]
        public string ImageUrl { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? BalanceDueMax { get; set; }
        public bool? Actived { get; set; }

        [ForeignKey("ContainerRequirementCode")]
        [InverseProperty("MaterialModel")]
        public virtual ContainerRequirementModel ContainerRequirementCodeNavigation { get; set; }
        [ForeignKey("LaborCode")]
        [InverseProperty("MaterialModel")]
        public virtual LaborModel LaborCodeNavigation { get; set; }
        [ForeignKey("ProductHierarchyCode")]
        [InverseProperty("MaterialModel")]
        public virtual ProductHierarchyModel ProductHierarchyCodeNavigation { get; set; }
        [ForeignKey("ProfitCenterCode")]
        [InverseProperty("MaterialModel")]
        public virtual ProfitCenterModel ProfitCenterCodeNavigation { get; set; }
        [ForeignKey("TemperatureConditionCode")]
        [InverseProperty("MaterialModel")]
        public virtual TemperatureConditionModel TemperatureConditionCodeNavigation { get; set; }
        [InverseProperty("MaterialCodeNavigation")]
        public virtual ICollection<SaleOrderMasterModel> SaleOrderMasterModel { get; set; }
    }
}