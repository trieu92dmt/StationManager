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
    public class XuatKhuonController : Controller
    {
        // GET: XuatKhuon
        public ActionResult Index(int Id)
        {
            ViewBag.CatalogueNo = Id;
            return View();
        }
        public ActionResult DocumentViewerPartial(int Id)
        {
            //Tranh tinh trang load data lan dau
            XuatKhuonInToanPhatXtraReport report = new XuatKhuonInToanPhatXtraReport();
            report = CreateDataReport(Id);
            ViewData["Tag_Report"] = report;
            ViewBag.CatalogueNo = Id;
            return PartialView("_DocumentViewerPartial");
        }
        public ActionResult DocumentViewerPartialExport(int Id)
        {
            XuatKhuonInToanPhatXtraReport report = CreateDataReport(Id);
            return DocumentViewerExtension.ExportTo(report, Request);
        }
        public XuatKhuonInToanPhatXtraReport CreateDataReport(int Id)
        {
            //Bước 1: Lây data 
            DataSet ds = GetData(Id);
            //Bước 2: Tạo report
            XuatKhuonInToanPhatXtraReport report = new XuatKhuonInToanPhatXtraReport();
            //Bước 3: Gán data cho report
            report.DataSource = ds;

            //Bước 4: Set các thông số khác cho report
            report.DataMember = "HeaderInformation";
            return report;
        }

        public DataSet GetData(int Id)
        {
            try
            {
                DataSet ds = new DataSet();
                string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[Warehouse].[Transfer_Catalogue_intoanphat]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.SelectCommand.Parameters.AddWithValue("@TransferCode", Id);
                            sda.SelectCommand.Parameters.AddWithValue("@Name", System.Web.HttpContext.Current.User.Identity.Name);
                            sda.Fill(ds);
                            ds.Tables[0].TableName = "HeaderInformation";
                            ds.Tables[1].TableName = "Detail";

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
    }
}