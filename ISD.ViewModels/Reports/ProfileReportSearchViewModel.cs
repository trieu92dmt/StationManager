using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.ViewModels
{
    public class ProfileReportSearchViewModel
    {
        /// 1. Ngày nhập (từ … đến …)
        /// 
        //Từ ngày
        public string FromDate { get; set; }
        //Đến ngày
        public string ToDate { get; set; }
        /// 2. User
        public Guid? UserName { get; set; }
        /// 3. Nguồn KH
        public string CustomerSourceCode { get; set; }
        /// 4. Phân loại KH(B/C)
        public string CustomerTypeCode { get; set; }
        /// 5. Nhóm KH
        public string CustomerGroupCode { get; set; }
        /// 6. Ngành nghề KH
        public string CustomerCareerCode { get; set; }
        /// 7. Trạng thái xử lý yêu cầu KH
        public Guid? TaskStatusId { get; set; }

        #region //Additional Field by login user
        //Công ty
        public string CurrentCompanyCode { get; set; }
        //Chi nhánh
        public string CurrentSaleOrg { get; set; }
        //Tài khoản đang đăng nhập
        public string CurrentUserName { get; set; }
        //Có phải là đại lý ủy quyền hay không
        public bool? isAgency { get; set; }
        #endregion
    }
}
