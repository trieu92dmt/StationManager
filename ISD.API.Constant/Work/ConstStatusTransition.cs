using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Constant
{
    public class ConstStatusTransition
    {
        //Cấu hình status transition
        //Không cấu hình
        public static string NotConfig = "NotConfig";

        //Người tạo
        public static string CreateUser = "CreateUser";

        //Người nhấn nút
        public static string SubmitUser = "SubmitUser";

        //Chọn nhóm
        public static string Roles = "Roles";

        //Xóa trống
        public static string Remove = "Remove";
        //==================================================
        //Cấu hình Value
        //ValueType
        public static string INPUT = "INPUT";
        public static string API = "API";
        //==================================================
        //Trạng thái duyệt SAP
        public static string ChuaDuyet = "L";
        public static string ChuaDuyet_Display = "Chưa duyệt";
        public static string DuyetL1 = "L1";
        public static string DuyetL1_Display = "Đã duyệt L1";
        public static string DuyetL2 = "L2";
        public static string DuyetL2_Display = "Đã duyệt L2";
    }
}
