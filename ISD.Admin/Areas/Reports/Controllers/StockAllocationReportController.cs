using ISD.Core;
using ISD.Repositories.Excel;
using ISD.ViewModels;
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
    public class StockAllocationReportController : BaseController
    {
        // GET: StockAllocationReport
        public ActionResult Index()
        {
            ViewBag.PageId = GetPageId("/Reports/StockAllocationReport");
            CreateViewBagForSearch();
            return View();
        }
        private void CreateViewBagForSearch()
        {
            //Dropdown Company
            var companyList = _unitOfWork.CompanyRepository.GetAll();
            ViewBag.SearchCompanyId = new SelectList(companyList, "CompanyId", "CompanyName", CurrentUser.CompanyId);

            //Dropdown Store
            var storeList = _unitOfWork.StoreRepository.GetAllStore();
            ViewBag.SearchStoreId = new SelectList(storeList, "StoreId", "StoreName");        
        }
        public ActionResult ExportExcel(StockAllocationSearchViewModel searchModel)
        {
            List<StockAllocationReportViewModel> result = new List<StockAllocationReportViewModel>();
            if(!searchModel.SearchToDate.HasValue)
            {
                searchModel.SearchToDate = DateTime.Now;
            }

           DataSet ds = new DataSet();
            string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("[Report].[usp_StockAllocationReport]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        #region Parameters
                        sda.SelectCommand.Parameters.AddWithValue("@CompanyId", searchModel.SearchCompanyId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StoreId", searchModel.SearchStoreId ?? (object)DBNull.Value);                      
                        sda.SelectCommand.Parameters.AddWithValue("@ToDate", searchModel.SearchToDate.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@FromDate", searchModel.SearchFromDate ?? (object)DBNull.Value);
                        #endregion

                        sda.Fill(ds);
                        var dt = ds.Tables[0];
                        //var filteredResultsCount = output.Value;
                        //if (filteredResultsCount != null)
                        //{
                        //    FilteredResultsCount = Convert.ToInt32(filteredResultsCount);
                        //}

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow item in dt.Rows)
                            {
                                #region Convert to list
                                StockAllocationReportViewModel model = new StockAllocationReportViewModel();
                                model.CompanyName = item["CompanyName"].ToString();
                                model.StoreName = item["StoreName"].ToString();
                                model.CategoryName = item["CategoryName"].ToString();
                                switch (model.CategoryName)
                                {
                                    case "Brochure":
                                        if (!string.IsNullOrEmpty(item["ReceiveQuantity"].ToString()))
                                        {
                                            model.ReceiveQuantity = Convert.ToDecimal(item["ReceiveQuantity"].ToString());
                                            model.ExpectedQuantity = searchModel.BrochureEpectedQuantity;
                                            model.Ratio = (((decimal)model.ReceiveQuantity / (decimal)model.ExpectedQuantity) * 100).ToString("#.##") + "%";
                                        }
                                        break;
                                    case "Catalogue":
                                        if (!string.IsNullOrEmpty(item["ReceiveQuantity"].ToString()))
                                        {
                                            model.ReceiveQuantity = Convert.ToDecimal(item["ReceiveQuantity"].ToString());
                                            model.ExpectedQuantity = searchModel.CatalogueExpectedQuantity;
                                            model.Ratio = (((decimal)model.ReceiveQuantity / (decimal)model.ExpectedQuantity) * 100).ToString("#.##") + "%";
                                        }
                                        break;
                                    case "Kệ và Vật Tư đi kèm":
                                        if (!string.IsNullOrEmpty(item["ReceiveQuantity"].ToString()))
                                        {
                                            model.ReceiveQuantity = Convert.ToDecimal(item["ReceiveQuantity"].ToString());
                                            model.ExpectedQuantity = searchModel.KEExpectedQuantity;
                                            model.Ratio = (((decimal)model.ReceiveQuantity / (decimal)model.ExpectedQuantity) * 100).ToString("#.##") + "%";
                                        }
                                        break;
                                    case "Bao bì":
                                        if (!string.IsNullOrEmpty(item["ReceiveQuantity"].ToString()))
                                        {
                                            model.ReceiveQuantity = Convert.ToDecimal(item["ReceiveQuantity"].ToString());
                                            model.ExpectedQuantity = searchModel.PackagingExpectedQuantity;
                                            model.Ratio = (((decimal)model.ReceiveQuantity / (decimal)model.ExpectedQuantity) * 100).ToString("#.##") + "%";
                                        }
                                        break;
                                    case "Mẫu":
                                        if (!string.IsNullOrEmpty(item["ReceiveQuantity"].ToString()))
                                        {
                                            model.ReceiveQuantity = Convert.ToDecimal(item["ReceiveQuantity"].ToString());
                                            model.ExpectedQuantity = searchModel.SampleExpectedQuantity;
                                            model.Ratio = (((decimal)model.ReceiveQuantity / (decimal)model.ExpectedQuantity) * 100).ToString("#.##") + "%";
                                        }
                                        break;
                                    default:
                                        break;
                                }                               
                                result.Add(model);
                                #endregion
                            }
                        }
                    }
                }
            }
            //return Export(result, FilteredResultsCount);
            //Tổng số lượng catalog đã xuất
          //  int TotalStockDelivery = Convert.ToInt32(result.Sum(p => p.Quantity));
            return Export(result, searchModel.SearchToDate.Value);
        }


        public ActionResult Export(List<StockAllocationReportViewModel> viewModel,DateTime toDate)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;
            fileheader = "BÁO CÁO SỐ LƯỢNG CATALOGUE PHÂN BỔ THỰC TẾ ĐẾN " + toDate.Date.ToString("dd/MM/yyyy");

            #region Master
            columns.Add(new ExcelTemplate { ColumnName = "CompanyName", isAllowedToEdit = false, isDateTime = true }); //1. Công ty   
            columns.Add(new ExcelTemplate { ColumnName = "StoreName", isAllowedToEdit = false, isDateTime = true }); //1. Chi nhánh            
            columns.Add(new ExcelTemplate { ColumnName = "CategoryName", isAllowedToEdit = false });//2. Nhóm vật tư
            columns.Add(new ExcelTemplate { ColumnName = "ReceiveQuantity", isAllowedToEdit = false });  //3. Số lượng đã nhập           
            columns.Add(new ExcelTemplate { ColumnName = "ExpectedQuantity", isAllowedToEdit = false });//4. Số lượng dự kiến            
            columns.Add(new ExcelTemplate { ColumnName = "Ratio", isAllowedToEdit = false });//5. Tỉ lệ       
            #endregion

            //List<ExcelHeadingTemplate> heading initialize in BaseController
            //Default:
            //          1. heading[0] is controller code
            //          2. heading[1] is file name
            //          3. headinf[2] is warning (edit)
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = "",//controllerCode,
                RowsToIgnore = 1,
                isWarning = false,
                isCode = true
            });
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = fileheader.ToUpper(),
                RowsToIgnore = 1,
                isWarning = false,
                isCode = false
            });
            //heading.Add(new ExcelHeadingTemplate()
            //{
            //    Content = "Tổng số lượng xuất: " + TotalStockDelivery,
            //    RowsToIgnore = 1,
            //    isWarning = false,
            //    isCode = true
            //});

            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
    }
}