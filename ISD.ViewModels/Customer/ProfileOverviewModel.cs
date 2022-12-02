using ISD.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ProfileOverviewModel : ProfileModel
    {
        public string ProvinceName { get; set; }
        public string DistrictName { get; set; }
        public string WardName { get; set; }
        public string isForeignName { get; set; }
        public string CustomerTypeName { get; set; }
        public string SaleOfficeName { get; set; }
        public string CustomerSourceName { get; set; }
        public string CreateAtSaleOrgName { get; set; }
        public string ActivedName { get; set; }
        public string CustomerCareerName { get; set; }
        public string AddressTypeName { get; set; }
        public List<ProfileGroupCreateViewModel> profileGroupList { get; set; }
    }
}
