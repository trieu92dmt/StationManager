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
    public class QualityControlErrorViewModel : QualityControl_Error_Mapping
    {
        public List<FileAttachmentViewModel> ErrorFileViewModel { get; set; }
        public List<HttpPostedFileBase> File { get; set; }
        public string CatalogText_vi { get; set; }
    }
}
