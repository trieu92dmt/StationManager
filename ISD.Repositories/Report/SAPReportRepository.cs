using ISD.Constant;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.Report
{
    public class SAPReportRepository
    {
        //SAPRepository _sap;
        //RfcDestination destination;

        //public SAPReportRepository()
        //{
        //    _sap = new SAPRepository();
        //    destination = _sap.GetRfcWithConfig();
        //}

        public List<CustomerSaleOrderViewModel> GetSaleOrderList(string ProfileForeignCode, string CompanyCode)
        {
            var listResult = new List<CustomerSaleOrderViewModel>();

            //var _sap = new SAPRepository();
            //var destination = _sap.GetRfcWithConfig();

            ////Định nghĩa hàm cần gọi
            //var function = destination.Repository.CreateFunction(ConstantFunctionName.YAC_FM_CRM_GET_SO_CUST);
            ////Truyền parameters
            //function.SetValue("IM_WERKS", CompanyCode); //Mã công ty
            //function.SetValue("IM_KUNNR", ProfileForeignCode); //Mã SAP Khách hàng

            //function.Invoke(destination);

            //var datatable = function.GetTable("SALEORDER_T").ToDataTable("SALEORDER_T");

            //if (datatable != null && datatable.Rows.Count > 0)
            //{
            //    foreach (DataRow item in datatable.Rows)
            //    {

            //        listResult.Add(new CustomerSaleOrderViewModel()
            //        {
            //            SONumber = item["VBELN"].ToString(),//Mã đơn hàng
            //            OrderNumber = item["VBELN_OD"].ToString(),//Mã lệnh
            //            ProductCode = item["MATNR"].ToString().TrimStart(new Char[] { '0' }),//Mã sản phẩm
            //            ProductName = item["MAKTX"].ToString(),//Tên sản phẩm
            //            ProductQuantity = (decimal?)item["KWMENG"],//Số lượng
            //        });
            //    }
            //}

            return listResult;
        }
    }
}
