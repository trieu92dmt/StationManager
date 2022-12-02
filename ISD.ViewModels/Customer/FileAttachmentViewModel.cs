using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class FileAttachmentViewModel : BaseClassViewModel
    {
        public System.Guid FileAttachmentId { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resources.LanguageResource), ErrorMessageResourceName = "Required")]
        //[Display(ResourceType = typeof(Resources.LanguageResource), Name = "FileAttachmentCode")]
        public string FileAttachmentCode { get; set; }
        public string FileAttachmentTypeName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Drawing_FileAttachmentName")]
        public string FileAttachmentName { get; set; }
        public string FileExtention { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Drawing_FileUrl")]
        public string FileUrl { get; set; }

        public Guid? ObjectId { get; set; }
        public Guid? ProfileId { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FileAttachmentCode")]
        public string FileType { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "FileAttachmentCode")]
        public string FileTypeName { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DrawingCode")]
        public string DrawingCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DrawingMoldCode")]
        public string DrawingMoldCode { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "DrawingNote")]
        public string DrawingNote { get; set; }
    }
}
