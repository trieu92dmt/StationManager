using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class PlantRoutingConfigViewModel
    {
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_PlantRoutingCode")]
        public int? PlantRoutingCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_PlantRoutingName")]
        [Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        public string PlantRoutingName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_PlantRoutingGroup")]
        public string PlantRoutingGroup { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_FromData")]
        public string FromData { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute1")]
        public string Attribute1 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute2")]
        public string Attribute2 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute3")]
        public string Attribute3 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute4")]
        public string Attribute4 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute5")]
        public string Attribute5 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute6")]
        public string Attribute6 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute7")]
        public string Attribute7 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute8")]
        public string Attribute8 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute9")]
        public string Attribute9 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Attribute10")]
        public string Attribute10 { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_LeadTimeType")]
        public Nullable<bool> LeadTimeType { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_LeadTime")]
        public Nullable<int> LeadTime { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_FromDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public string FromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_ToDate")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public string ToDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_Condition")]
        public string Condition { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "OrderIndex")]
        public Nullable<int> OrderIndex { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Actived")]
        public Nullable<bool> Actived { get; set; }
        public string CreatedUser { get; set; }
        public Nullable<System.DateTime> CreatedTime { get; set; }
        public string LastModifiedUser { get; set; }
        public Nullable<System.DateTime> LastModifiedTime { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "IsPrimaryStep")]
        public bool? IsPrimaryStep { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
        public string DescriptionFromDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
        public string DescriptionToDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
        public string DescriptionCondition { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PlantRouting_LeadTime")]
        public string LeadTimeFormula { get; set; }

        public string LeadTimeDisplay
        {
            get
            {
                if (LeadTime.HasValue)
                {
                    return LeadTime.ToString();
                }
                else if (!string.IsNullOrEmpty(LeadTimeFormula))
                {
                    return LeadTimeFormula;
                }
                return string.Empty;
            }
        }
    }
}
