using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class EquimentSearchModel
    {
        public System.Guid EquipmentId { get; set; }
        public string EQART { get; set; }
        public string EARTX { get; set; }

        public int EquipmentIntId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MachineChainCode")]
        public string EquipmentCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MachineChainName")]
        public string EquipmentName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MachineChainTypeCode")]
        public string EquipmentGroupCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MachineChainType")]
        public string EquipmentTypeCode { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "WorkShopId")]
        public Nullable<System.Guid> WorkShopId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MachineChainProduction")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? EquipmentProduction { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EquipmentStatus")]
        public string EquipmentStatus { get; set; }
        public Nullable<System.Guid> CreateBy { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.Guid> LastEditBy { get; set; }
        public Nullable<System.DateTime> LastEditTime { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public Nullable<bool> Actived { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "EquipmentUnit")]
        public string Unit { get; set; }

        public string EquipmentGroupName { get;set; }
        public string grpCatalogCode { get; set; }
        public string grpCatalogText_vi { get; set; }
        public string typeCatalogCode { get; set; }
        public string typeCatalogText_vi { get; set; }
        public string statusCatalogCode { get; set; }
        public string statusCatalogText_vi { get; set; }
        public string WorkShopName { get; set; }
    }
}
