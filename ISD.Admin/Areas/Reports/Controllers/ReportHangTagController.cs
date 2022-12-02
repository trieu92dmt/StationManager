using DevExpress.Web.Mvc;
using ISD.Constant;
using ISD.Core;
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
    public class ReportHangTagController : BaseController
    {
        // GET: ReportHangTag
        public ActionResult Index(string CustomerReference = null, int? BatchPrinting = null)
        {
            //Check số lượng kế hoạch cần in
            //Nếu số lượng <= 50 thì in mẫu dành cho cụm
            //Ngược lại thì in mẫu dành cho chi tiết
            bool isCum = false;
            if (!string.IsNullOrEmpty(CustomerReference))
            {
                var TaskId = Guid.Parse(CustomerReference);
                var LSXSAP = _unitOfWork.HangtagRepository.GetLSXSAPInfo(TaskId);
                if (LSXSAP != null)
                {
                    //Tranh tinh trang load data lan dau
                    if (LSXSAP.Number2 <= 50)
                    {
                        isCum = true;
                    }
                    else
                    {
                        isCum = false;
                    }
                }
            }
            ViewBag.CustomerReference = CustomerReference;
            ViewBag.BatchPrinting = BatchPrinting;
            ViewBag.IsCum = isCum;
            return View();
        }

        public ActionResult DocumentViewerPartial(Guid? CustomerReference = null, int? BatchPrinting = null, bool isFist = true, bool? IsCum = null)
        {
            if (IsCum == true)
            {
                HangTagCumXtraReport report = new HangTagCumXtraReport();
                if (isFist == false)
                {
                    report = CreateDataCumReport(CustomerReference, BatchPrinting);
                }
                ViewData["HangTag_Report"] = report;
            }
            else
            {
                HangTagXtraReport report = new HangTagXtraReport();
                if (isFist == false)
                {
                    report = CreateDataReport(CustomerReference, BatchPrinting);
                }
                ViewData["HangTag_Report"] = report;
            }

            ViewBag.CustomerReference = CustomerReference;
            ViewBag.BatchPrinting = BatchPrinting;
            ViewBag.IsCum = IsCum;
            return PartialView("_DocumentViewerPartial");
        }
        public ActionResult DocumentViewerPartialExport(Guid? CustomerReference = null, int? BatchPrinting = null, bool? IsCum = null)
        {
            if (IsCum == true)
            {
                HangTagCumXtraReport catalogueReport = CreateDataCumReport(CustomerReference, BatchPrinting);
                return DocumentViewerExtension.ExportTo(catalogueReport, Request);
            }
            else
            {
                HangTagXtraReport catalogueReport = CreateDataReport(CustomerReference, BatchPrinting);
                return DocumentViewerExtension.ExportTo(catalogueReport, Request);
            }
        }

        #region Lấy dữ liệu từ store proc
        //Tạo report mẫu chi tiết
        public HangTagXtraReport CreateDataReport(Guid? CustomerReference = null, int? BatchPrinting = null)
        {
            //Bước 1: Lây data 
            DataSet ds = GetData(CustomerReference, BatchPrinting);
            //Bước 2: Tạo report
            HangTagXtraReport report = new HangTagXtraReport();
            //Bước 3: Gán data cho report
            report.DataSource = ds;

            //Bước 4: Set các thông số khác cho report
            report.DataMember = "HangTag";
            report.Name = "Thẻ treo Pallet";
            return report;
        }
        //Tạo report mẫu cụm
        public HangTagCumXtraReport CreateDataCumReport(Guid? CustomerReference = null, int? BatchPrinting = null)
        {
            //Bước 1: Lây data 
            DataSet ds = GetData(CustomerReference, BatchPrinting, isCum: true);
            //Bước 2: Tạo report
            HangTagCumXtraReport report = new HangTagCumXtraReport();
            //Bước 3: Gán data cho report
            report.DataSource = ds;

            //Bước 4: Set các thông số khác cho report
            report.DataMember = "HangTag";
            report.Name = "Thẻ treo Pallet";
            return report;
        }

        public DataSet GetData(Guid? CustomerReference, int? BatchPrinting = null, bool? isCum = null)
        {
            try
            {
                DataSet ds = new DataSet();
                string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("[MES].[GetHangTag]", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.SelectCommand.Parameters.AddWithValue("@CustomerReference", CustomerReference ?? (object)DBNull.Value);
                            sda.SelectCommand.Parameters.AddWithValue("@BatchPrinting", BatchPrinting ?? (object)DBNull.Value);
                            sda.SelectCommand.Parameters.AddWithValue("@IsCum", isCum ?? (object)DBNull.Value);
                            sda.Fill(ds);
                            ds.Tables[0].TableName = "HangTag";
                            ds.Tables[1].TableName = "HangTagDetail";

                            ds.Relations.Add(
                              "HangTag_HangTagDetail",
                              ds.Tables["HangTag"].Columns["HangTagId"],
                              ds.Tables["HangTagDetail"].Columns["HangTagId"]
                            );
                            //handle QR Code URL
                            if (ds != null && ds.Tables[0].Rows.Count > 0 )
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
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}