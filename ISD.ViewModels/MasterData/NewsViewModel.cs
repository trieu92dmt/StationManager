using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels.MasterData
{
    public class NewsViewModel : NewsModel
    {
        public string CreateByName { get; set; }

        [Display(ResourceType = typeof(Resources.LanguageResource), Name = "News_NewsCategoryId")]
        public string NewsCategoryName { get; set; }
    }

    public class ListCompanyViewModel
    {
        public bool? isCheckComp { get; set; }
        public System.Guid CompanyId { get; set; }
    }
}
