using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Constant
{
    public class ConstantFunctionName
    {
        #region AC iCRM
        //1. Customer
        public const string YAC_FM_CRM_GET_DATALIST = "YAC_FM_CRM_GET_DATALIST";

        //2. Credit Limit
        public const string YAC_FM_CRM_GET_LEVEL_DEBIT = "YAC_FM_CRM_GET_LEVEL_DEBIT";

        //3. Material
        public const string YAC_FM_CRM_GET_MATERIAL = "YAC_FM_CRM_GET_MATERIAL";

        //4. Revenue
        public const string YAC_FM_WEB_GET_DOANHSO = "YAC_FM_WEB_GET_DOANHSO";

        //5.Check serial status
        public const string YAC_FM_CRM_CHECK_SERIAL_BH = "YAC_FM_CRM_CHECK_SERIAL_BH";

        //6. Check OD information
        public const string YAC_FM_CRM_GET_OD_BH = "YAC_FM_CRM_GET_OD_BH";

        //Giải mã password
        public const string YAC_FM_GET_GIAIMA = "YAC_FM_GET_GIAIMA";

        //Trả về danh sách SO-Đơn hàng theo Customer
        public const string YAC_FM_CRM_GET_SO_CUST = "YAC_FM_CRM_GET_SO_CUST";

        //Trả về doanh số theo từng tháng của Customer
        public const string YAC_FM_CRM_GET_DOANHSO = "YAC_FM_CRM_GET_DOANHSO";
        #endregion
        #region TTF iMES
        //2021-05-09
        //TTF- Đồng bộ dữ liệu Material Master 
        public const string ZMES_MATERIAL = "ZMES_MATERIAL";
        public const string ZMES_RESET_MASTERDATA = "ZMES_RESET_MASTERDATA";
        public const string ZMES_PRODUCTION_VER = "ZMES_PRODUCTION_VER";
        public const string ZMES_FM_RESERVATION = "ZMES_FM_RESERVATION";
        public const string ZMES_FM_BOM = "ZMES_FM_BOM";
        public const string ZMES_FM_EQUIPMENT = "ZMES_FM_EQUIPMENT";
        public const string ZMES_FM_ROUTING = "ZMES_FM_ROUTING";

        public const string ZMES_TB_ROUTING = "ZMES_TB_ROUTING";

        public const string ZMES_SALE_ORDER = "ZMES_SALE_ORDER";
        public const string ZMES_PRODUCTION_ORDER = "ZMES_PRODUCTION_ORDER";
        public const string ZMES_FM_PRPO_MIGO = "ZMES_FM_PRPO_MIGO";
        public const string ZMES_FM_PRPO_RETURN = "ZMES_FM_PRPO_RETURN";
        public const string ZMES_FM_MIGO = "ZMES_FM_MIGO";
        public const string ZMES_FM_MATLTYPE_GROUP = "ZMES_FM_MATLTYPE_GROUP";
        public const string ZMES_FM_BOM_SALE = "ZMES_FM_BOM_SALE";

        //Transfer data from MES To SAP
        public const string ZMES_FM_TRANSACTION = "ZMES_FM_TRANSACTION";
        //SOTEXT_PR
        public const string ZMES_SOTEXT_PR = "ZMES_SOTEXT_PR";
        //POTEXT_PR_SO
        public const string ZMES_PO_POTEXTPRSO = "ZMES_PO_POTEXTPRSO";

        //Báo cáo mặt bằng xưởng
        public const string ZMES_REPORT_MBX = "ZMES_REPORT_MBX";

        //Báo cáo tiền dự kiến routing
        public const string ZMES_REPORT_MBX_CP = "ZMES_REPORT_MBX_CP"; 
        #endregion
    }
}
