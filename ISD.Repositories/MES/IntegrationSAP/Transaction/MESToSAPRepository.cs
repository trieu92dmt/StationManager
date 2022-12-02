using ISD.Constant;
using ISD.EntityModels;
using ISD.Repositories.MES;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class MESToSAPRepository
    {
        EntityDataContext _context;
        //SAPRepository _sap;
        LoggerRepository _loggerRepository;
        public MESToSAPRepository(EntityDataContext entityDataContext)
        {
            _context = entityDataContext;
            // 1. SET AutoDetectChangesEnabled = false
            _context.Configuration.AutoDetectChangesEnabled = false;
            //_sap = new SAPRepository();
            _loggerRepository = new LoggerRepository();
        }

        public void SendMESToSAP(MES_SAP_ConfirmCongDoanViewModel param)
        {
            string errorMessage = string.Empty;
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_TRANSACTION);
                //var table = function.GetTable("IM_TRANSACTION");

                //// Send Data With RFC Table  
                //var dataTbl = function.GetTable("IM_TRANSACTION");

                //dataTbl.Append();
                ////ID 
                //string LINE_ID = param.LINE_ID.ToString();
                //dataTbl.SetValue("LINE_ID", LINE_ID);
                ////Plant
                //string WERKS = param.WERKS;
                //dataTbl.SetValue("WERKS", WERKS);
                ////LSX Đại trà 
                //string AUFNR_DT = param.AUFNR_DT;
                //dataTbl.SetValue("AUFNR_DT", AUFNR_DT);
                //// Số sale order 
                //string KDAUF = param.KDAUF;
                //dataTbl.SetValue("KDAUF", KDAUF);
                //// Số SO line
                //string KDPOS = param.KDPOS;
                //dataTbl.SetValue("KDPOS", KDPOS);
                ////Đợt sản xuất tách từ MES 
                //string STAGE_MES = param.STAGE_MES;
                //dataTbl.SetValue("STAGE_MES", STAGE_MES);
                ////LSX SAP 
                //string AUFNR = param.AUFNR;
                //dataTbl.SetValue("AUFNR", AUFNR);
                ////Material Number 
                //string MATNR = param.MATNR;
                //dataTbl.SetValue("MATNR", MATNR);
                ////Tên thành phẩm 
                //string MAKTX = param.MAKTX;
                //dataTbl.SetValue("MAKTX", MAKTX);
                ////Số lượng Theo LSX SAP 
                //int? LFIMG_LSX = param.LFIMG_LSX;
                //dataTbl.SetValue("LFIMG_LSX", LFIMG_LSX);
                ////Đơn vị tính 
                //string MEINS = param.MEINS;
                //dataTbl.SetValue("MEINS", MEINS);
                ////Số lượng điều chỉnh 
                //decimal? LFIMG_DC = param.LFIMG_DC;
                //dataTbl.SetValue("LFIMG_DC", LFIMG_DC);
                ////Mã chi tiết ( nếu là "-" là sản phẩm) 
                //string PRODUCT_ATT = param.PRODUCT_ATT;
                //dataTbl.SetValue("PRODUCT_ATT", PRODUCT_ATT);
                //// Component text 
                //string KTEXT = param.KTEXT;
                //dataTbl.SetValue("KTEXT", KTEXT);
                ////Công đoạn con
                //string STOCK_CODE = param.STOCK_CODE;
                //dataTbl.SetValue("STOCK_CODE", STOCK_CODE);
                ////Số lượng 
                //decimal? LFIMG = param.LFIMG;
                //dataTbl.SetValue("LFIMG", LFIMG);
                ////Mã phiếu ghi nhận (NULL là chuyển nên không có) 
                //string CUST_REF = param.CUST_REF.ToString();
                //dataTbl.SetValue("CUST_REF", CUST_REF);
                ////Loại: Đạt/Không Đạt 
                //string STOCK_TYPE = param.STOCK_TYPE;
                //dataTbl.SetValue("STOCK_TYPE", STOCK_TYPE);
                ////Ngày ghi nhận 
                //DateTime? DATE_KEY = DateTime.ParseExact(param.DATE_KEY.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
                //dataTbl.SetValue("DATE_KEY", DATE_KEY);
                ////Thời gian bắt đầu 
                //string FROM_TIME = param.FROM_TIME.Value.TimeOfDay.ToString();
                //dataTbl.SetValue("FROM_TIME", FROM_TIME);
                ////Thời gian kết thúc 
                //string TO_TIME = param.TO_TIME.Value.TimeOfDay.ToString();
                //dataTbl.SetValue("TO_TIME", TO_TIME);
                ////Lần qua công đoạn 
                //string PHASE = param.PHASE.ToString();
                //dataTbl.SetValue("PHASE", PHASE);
                ////MovementType: ADD: Ghi Nhận ,TRANSER: Đã chuyển 
                //string MVT_TYPE = param.MVT_TYPE;
                //dataTbl.SetValue("MVT_TYPE", MVT_TYPE);
                ////ID Tổ 
                //string DEPT_ID = param.DEPT_ID.ToString();
                //dataTbl.SetValue("DEPT_ID", DEPT_ID);
                ////Mã tổ 
                //string DEPT_CODE = param.DEPT_CODE;
                //dataTbl.SetValue("DEPT_CODE", DEPT_CODE);
                ////Tên tổ
                //string DEPT_NAME = param.DEPT_NAME;
                //dataTbl.SetValue("DEPT_NAME", DEPT_NAME);
                ////Trạng thái hủy
                //string IS_DELETED = null;
                //if (param.IS_DELETED.HasValue && param.IS_DELETED == true)
                //{
                //    IS_DELETED = "1";
                //}
                //dataTbl.SetValue("IS_DELETED", IS_DELETED);
                ////Thời gian ghi nhận 
                //string ERZET = param.ERDAT.Value.TimeOfDay.ToString();
                //dataTbl.SetValue("ERZET", ERZET);
                //DateTime? ERDAT = param.ERDAT;
                //dataTbl.SetValue("ERDAT", ERDAT);
                ////Được ghi nhận bởi USER 
                //string ERNAM = param.ERNAM;
                //dataTbl.SetValue("ERNAM", ERNAM);
                ////Confirm công đoạn lớn
                //string IS_WORKC_COMP = string.Empty;
                //if (param.IS_WORKC_COMP.HasValue && param.IS_WORKC_COMP == true)
                //{
                //    IS_WORKC_COMP = "1";
                //}
                //dataTbl.SetValue("IS_WORKC_COMP", IS_WORKC_COMP);
                ////Mã Công đoạn lớn confirm SC,TC,LRHT,HTĐG 
                //string CF_WORKC = param.CF_WORKC;
                //dataTbl.SetValue("CF_WORKC", CF_WORKC);
                ////Ngày confirm công đoạn lớn 
                //if (param.CF_WORKC_DATE.HasValue && param.CF_WORKC_DATE > 0)
                //{
                //    DateTime? CF_WORKC_DATE = DateTime.ParseExact(param.CF_WORKC_DATE.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
                //    dataTbl.SetValue("CF_WORKC_DATE", CF_WORKC_DATE);
                //    string CF_WORKC_TIME = CF_WORKC_DATE.Value.TimeOfDay.ToString();
                //    dataTbl.SetValue("CF_WORKC_TIME", CF_WORKC_TIME);
                //}
                ////User name người ghi nhận 
                //string CF_ERNAM = param.CF_ERNAM;
                //dataTbl.SetValue("CF_ERNAM", CF_ERNAM);

                //function.Invoke(destination);

                //var output = function.GetTable("EX_TRANSACTION").ToDataTable("EX_TRANSACTION");
                //if (output != null && output.Rows.Count > 0)
                //{
                //    foreach (DataRow item in output.Rows)
                //    {
                //        var MESSAGE = item["MESSAGE"].ToString();
                //        //đẩy dữ liệu xong update 3 thông tin sau
                //        //[isSendToSAP]
                //        //[SendToSAPTime]
                //        //[SendToSAPError]
                //        var updateTransaction = _context.StockReceivingDetailModel.Where(p => p.StockReceivingDetailId == param.LINE_ID).FirstOrDefault();
                //        if (updateTransaction != null)
                //        {
                //            updateTransaction.isSendToSAP = true;
                //            updateTransaction.SendToSAPTime = DateTime.Now;
                //            if (!string.IsNullOrEmpty(MESSAGE) && MESSAGE != "Import Successfully")
                //            {
                //                updateTransaction.SendToSAPError = MESSAGE;
                //            }
                //            _context.Entry(updateTransaction).State = System.Data.Entity.EntityState.Modified;
                //            _context.SaveChanges();
                //        }
                //    }
                //}
                
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        errorMessage = ex.InnerException.InnerException.Message;
                    }
                }
            }
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _loggerRepository.Logging(errorMessage, "ZMES_FM_TRANSACTION");
            }
        }
    }
}
