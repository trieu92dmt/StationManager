using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class PrintMoldViewModel
    {
        public int STT { get; set; }
        public System.Guid PrintMoldId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PrintMoldIntId")]
        public int PrintMoldIntId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PrintMoldCode")]
        public string PrintMoldCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PrintMoldName")]
        public string PrintMoldName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PrintMoldType")]
        public string PrintMoldType { get; set; }
        public System.Guid ProfileId { get; set; }
        public System.Guid ProductId { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications_Length")]
        public Nullable<decimal> Specifications_Length { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications_Width")]
        public Nullable<decimal> Specifications_Width { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications_Height")]
        public Nullable<decimal> Specifications_Height { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications_Overalls")]
        public Nullable<decimal> Specifications_Overalls { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Specifications_Side")]
        public Nullable<decimal> Specifications_Side { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductPerMold")]
        public Nullable<int> ProductPerMold { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "NoteLocation")]
        public string LocationNote { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PrintMoldFilm")]
        public string PrintMoldFilm { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "PrintMoldDate")]
        public Nullable<System.DateTime> PrintMoldDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "LastMaintenanceDate")]
        public Nullable<System.DateTime> LastMaintenanceDate { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "MaintenanceAlert")]
        public Nullable<int> MaintenanceAlert { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StampQuantity")]
        public Nullable<int> StampQuantity { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "CurrentStampeQuantity")]
        public Nullable<int> CurrentStampeQuantity { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Description")]
        public string Description { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Serial")]
        public string Serial { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Bin")]
        public string Bin { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Status")]
        public string Status { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "StampQuantityAlert")]
        public Nullable<int> StampQuantityAlert { get; set; }
        [Display(Name = "Tên khách hàng")]
        public string ProfileName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "ProductName")]
        public string ProductName { get; set; }

    }
}
