using DevExpress.Web.Mvc;
using ISD.Constant;
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
    public class ReportDepartmentController : Controller
    {
        // GET: ReportHangTag
        public ActionResult Index(Guid Id)
        {
            ViewBag.DepartmentId = Id;
            return View();
        }

        public ActionResult DocumentViewerPartial(Guid Id)
        {
            //Tranh tinh trang load data lan dau
            DepartmentXtraReport report = new DepartmentXtraReport();
            report = CreateDataReport(Id);
            ViewData["Department_Report"] = report;
            ViewBag.DepartmentId = Id;
            return PartialView("_DocumentViewerPartial");
        }
        public ActionResult DocumentViewerPartialExport(Guid Id)
        {
            DepartmentXtraReport catalogueReport = CreateDataReport(Id);
            return DocumentViewerExtension.ExportTo(catalogueReport, Request);
        }

        #region Lấy dữ liệu từ store proc
        //Tạo report
        public DepartmentXtraReport CreateDataReport(Guid Id)
        {
            //Bước 1: Lây data 
            DataSet ds = GetData(Id);
            //Bước 2: Tạo report
            DepartmentXtraReport report = new DepartmentXtraReport();
            //Bước 3: Gán data cho report
            report.DataSource = ds;

            //Bước 4: Set các thông số khác cho report
            report.DataMember = "Department";
            report.Name = "Thông tin tổ - phân xưởng";
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
                    using (SqlCommand cmd = new SqlCommand("[MES].[GetDepartment]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.SelectCommand.Parameters.AddWithValue("@Id", Id);
                            sda.Fill(ds);
                            ds.Tables[0].TableName = "DepartmentDetail";

                            //handle QR Code URL
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                var adminDomain = ConstDomain.Domain;
                                foreach (DataRow item in ds.Tables[0].Rows)
                                {
                                    item["QRCode"] = string.Format("{0}{1}", adminDomain, item["QRCode"]?.ToString());
                                }
                            }
                        }
                    }
                }
                return ds;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}