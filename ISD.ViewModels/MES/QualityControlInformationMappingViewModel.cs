using ISD.EntityModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ISD.ViewModels
{
    public class QualityControlInformationMappingViewModel : QualityControl_QCInformation_Mapping
    {
        public List<FileAttachmentViewModel> CheckedFileViewModel { get; set; }
        public List<HttpPostedFileBase> File { get; set; }
        public string WorkCenterName { get; set; }
        public int? QualityControlInformationCode { get; set; }   
        public string QualityControlInformationName { get; set; }   

    }
}
