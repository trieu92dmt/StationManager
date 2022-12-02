using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ConstructionViewModel : BaseClassViewModel
    {
        public Nullable<System.Guid> OpportunityConstructionId { get; set; }
        public Nullable<System.Guid> ConstructionId { get; set; }
        public string ConstructionName { get; set; }
        public Nullable<System.Guid> ProfileId { get; set; }
    }
}
