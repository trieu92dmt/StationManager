using System.ComponentModel.DataAnnotations;

namespace ISD.ViewModels.Marketing
{
    public class TargetGroupCreateViewModel
    {
        [Required]
        [MaxLength(50)]
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "TargetGroupName")]
        public string TargetGroupName { get; set; }
    }
}
