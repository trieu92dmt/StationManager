using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MES
{
   public class RoutingOrderViewModel
    {
        public Guid StepId { get; set; }
        public int? OrderIndex { get; set; }
    }
}
