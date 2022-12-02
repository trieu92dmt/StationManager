using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class QualityControlWorkCenterViewModel : QualityControlInformationModel
    {
        public List<WorkCenterModel> WorkCenterList { get; set; }

        public List<WorkCenterModel> ActivedWorkCenterList { get; set; }
        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "Routing_WorkCenter")]
        public string WorkCenterCode { get; set; }
    }
}
