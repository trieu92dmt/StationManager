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
    public class QualityControlInformationViewModel : QualityControlInformationModel
    {
        public string WorkCenterName { get; set; }
        public string WorkCenterCode { get; set; }
    }
}
