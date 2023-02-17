using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Application.Commands.ScaleMonitor
{
    public class SearchScaleMinitorCommand
    {
        //Plant
        public string PlantFrom { get; set; }
        public string PlantTo { get; set; }
        //Đầu cân
        public string WeightHeadCodeFrom { get; set; }
        public string WeightHeadCodeTo { get; set; }
        //Loại
        //public  MyProperty { get; set; }
        //Ngày ghi nhận
        //Giờ ghi nhận
    }
}
