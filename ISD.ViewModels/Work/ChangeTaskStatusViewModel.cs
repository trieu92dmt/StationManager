using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.Work
{
    public class ChangeTaskStatusViewModel
    {
        public Guid? TaskId { get; set; }
        [Display(Name = "Từ trạng thái")]
        public Guid? FromTaskStatusId { get; set; }
        [Display(Name = "Đến trạng thái")]
        public Guid ToTaskStatusId { get; set; }
    }

    public class TaskStatusSelectModel
    {
        public Guid TaskStatusId { get; set; }
        public string TaskStatusName { get; set; }
        public int? OrderIndex { get; set; }
    }
}
