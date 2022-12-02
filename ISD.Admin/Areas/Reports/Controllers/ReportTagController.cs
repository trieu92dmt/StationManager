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
    public class ReportTagController : Controller
    {
        // GET: ReportTag
        public ActionResult Index(Guid Id)
        {
            ViewBag.WordOrderId = Id;
            return View();
        }
        public ActionResult DocumentViewerPartial(Guid Id)
        {
            //Tranh tinh trang load data lan dau
            HangTagXtraReport report = new HangTagXtraReport();
            report = CreateDataReport(Id);
            ViewData["Tag_Report"] = report;
            ViewBag.WordOrderId = Id;
            return PartialView("_DocumentViewerPartial");
        }
        public ActionResult DocumentViewerPartialExport(Guid Id)
        {
            HangTagXtraReport catalogueReport = CreateDataReport(Id);
            return DocumentViewerExtension.ExportTo(catalogueReport, Request);
        }
        #region Lấy dữ liệu từ store proc
        //Tạo report
        public HangTagXtraReport CreateDataReport(Guid Id)
        {
            //Bước 1: Lây data 
            DataSet ds = GetData(Id);
            //Bước 2: Tạo report
            HangTagXtraReport report = new HangTagXtraReport();
            //Bước 3: Gán data cho report
            report.DataSource = ds;

            //Bước 4: Set các thông số khác cho report
            return report;
        }

        public DataSet GetData(Guid Id)
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
                            sda.SelectCommand.Parameters.AddWithValue("@Id", Id);
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
                                }
                            }
                            //insert  5 row trống cho table detail
                            for (int j = 0; j < 5 ; j++)
                            {
                                DataRow row = ds.Tables[1].NewRow();
                                ds.Tables[1].Rows.Add(row);
                            }
                            int i = 1;
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