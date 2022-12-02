using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class AllotmentViewModel
    {
        public DateTime? DocumentDate { get; set; }
        public string SelectedStepCode { get; set; }
        public List<DataAllotmentViewModel> dataAllotmentList { get; set; }
    }
}
