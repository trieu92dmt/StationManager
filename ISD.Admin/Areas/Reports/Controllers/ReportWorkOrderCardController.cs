using DevExpress.Web.Mvc;
using ISD.Constant;
using Reports.XReports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class ReportWorkOrderCardController : Controller
    {
        // GET: ReportTag
        public ActionResult Index(List<Guid> Id)
        {
            ViewBag.WordOrderId = Id.ElementAt(0);
            return View();
        }
        public ActionResult DocumentViewerPartial(List<Guid> Id)
        {
            //Tranh tinh trang load data lan dau
            WorkOderCardXtraReport report = new WorkOderCardXtraReport();
            report = CreateDataReport(Id);
            ViewData["Tag_Report"] = report;
            ViewBag.WordOrderId = Id.ElementAt(0);
            return PartialView("_DocumentViewerPartial");
        }
        public ActionResult DocumentViewerPartialExport(List<Guid> Id)
        {
            WorkOderCardXtraReport catalogueReport = CreateDataReport(Id);
            return DocumentViewerExtension.ExportTo(catalogueReport, Request);
        }
        #region Lấy dữ liệu từ store proc
        //Tạo report
        public WorkOderCardXtraReport CreateDataReport(List<Guid> Id)
        {
            //Bước 1: Lây data 
            DataSet ds = GetData(Id);
            //Bước 2: Tạo report
            WorkOderCardXtraReport report = new WorkOderCardXtraReport();
            //Bước 3: Gán data cho report
            report.DataSource = ds;
            report.DataMember = "PalletHeader";
  
            //Bước 4: Set các thông số khác cho report
            return report;
        }

        public DataSet GetData(List<Guid> Id)
        {
            try
            {
                DataSet ds = new DataSet();
                string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[MES].[GetHangTagPallet]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.SelectCommand.Parameters.AddWithValue("@Id", Id.ElementAt(0));
                            sda.Fill(ds);
                            ds.Tables[0].TableName = "PalletHeader";
                            ds.Tables[1].TableName = "PalletDetail";

                            //handle QR Code URL
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                var adminDomain = ConstDomain.Domain;
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    item["QRCode"] = string.Format("{0}{1}", adminDomain, item["QRCode"]?.ToString());
                                    //item["WorkOrderQuantity"] = string.Format("{0:0.00}", item["WorkOrderQuantity"]);
                                }
                            }
                            if (ds.Tables[1].Rows.Count < 20)
                            {
                                int currentRowCount = ds.Tables[1].Rows.Count;
                                for (int j = 1; j <= 20 - currentRowCount; j++)
                                {
                                    DataRow row = ds.Tables[1].NewRow();
                                    ds.Tables[1].Rows.Add(row);
                                }
                            }
                            //insert  5 row trống cho table detail
                            //for (int j = 0; j < 5; j++)
                            //{
                            //    DataRow row = ds.Tables[1].NewRow();
                            //    ds.Tables[1].Rows.Add(row);
                            //}
                            //int i = 1;
                        }
                    }
                }
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}