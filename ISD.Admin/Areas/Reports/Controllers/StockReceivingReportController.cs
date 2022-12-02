using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.ViewModels;
using Newtonsoft.Json;
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
    public class StockReceivingReportController : BaseController
    {
        // GET: StockReceivingReport
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (StockReceivingSearchViewModel)TempData[CurrentUser.AccountId + "StockReceivingSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "StockReceivingTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "StockReceivingModeSearch"];
            var pageId = GetPageId("/Reports/StockReceivingReport");

            if (modeSearch == null || modeSearch.ToString() == "Default")
            {
                ViewBag.ModeSearch = "Default";
            }
            else
            {
                ViewBag.ModeSearch = "Recently";
            }
            Guid templateId = Guid.Empty;
            if (tempalteIdString != null)
            {
                templateId = Guid.Parse(tempalteIdString.ToString());
            }

            if (searchModel == null || searchModel.IsView != true)
            {
                ViewBag.Search = null;
            }
            else
            {
                ViewBag.Search = searchModel;
            }
            var listSystemTemplate = _unitOfWork.PivotGridTemplateRepository.GetSystemTemplate(pageId);
            var listUserTemplate = _unitOfWork.PivotGridTemplateRepository.GetUserTemplate(pageId, CurrentUser.AccountId.Value);
            List<FieldSettingModel> pivotSetting = new List<FieldSettingModel>();
            if (templateId != Guid.Empty && templateId != null)
            {

                pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId);
                ViewBag.PivotSetting = pivotSetting;
                ViewBag.TemplateId = templateId;
            }
            else
            {
                var userDefaultTemplate = listUserTemplate.FirstOrDefault(s => s.IsDefault == true);
                if (userDefaultTemplate != null)
                {
                    pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(userDefaultTemplate.SearchResultTemplateId);
                    ViewBag.PivotSetting = pivotSetting;
                    ViewBag.TemplateId = userDefaultTemplate.SearchResultTemplateId;
                }
                else
                {
                    var sysDefaultTemplate = listSystemTemplate.FirstOrDefault(s => s.IsDefault == true);
                    if (sysDefaultTemplate != null)
                    {
                        pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(sysDefaultTemplate.SearchResultTemplateId);
                        ViewBag.PivotSetting = pivotSetting;
                        ViewBag.TemplateId = sysDefaultTemplate.SearchResultTemplateId;
                    }
                    else
                    {
                        ViewBag.PivotSetting = null;
                        ViewBag.TemplateId = templateId;
                    }
                }
            }
            ViewBag.PageId = pageId;
            ViewBag.SystemTemplate = listSystemTemplate;
            ViewBag.UserTemplate = listUserTemplate;
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

            //Dropdown Nhân viên
            var saleEmployeeList = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist();
            ViewBag.SearchSalesEmployeeCode = new SelectList(saleEmployeeList, "SalesEmployeeCode", "SalesEmployeeName");

            //Dropdown Stock
            var listStock = _unitOfWork.StockRepository.GetAllForDropdown();
            ViewBag.SearchStockId = new SelectList(listStock, "StockId", "StockName");

            //Dropdown Department
            var listDepartment = _context.DepartmentModel.Where(s => s.Actived == true);
            ViewBag.DepartmentId = new SelectList(listDepartment, "DepartmentId", "DepartmentName");

            //Dropdown Nhóm khách hàng
            var listProfileGroupCode = _unitOfWork.CatalogRepository.GetCustomerCategory(CurrentUser.CompanyCode);
            ViewBag.SearchProfileGroupCode = new SelectList(listProfileGroupCode, "CatalogCode", "CatalogText_vi");
        }

        #region export to excel
        public ActionResult ExportExcel(StockReceivingSearchViewModel searchModel)
        {
            var data = GetData(searchModel);
            //Tổng số lượng catalog đã xuất
            //int TotalStockDelivery = Convert.ToInt32(result.Sum(p => p.Quantity));
            return Export(data);
        }

        private List<StockReceivingReportViewModel> GetData(StockReceivingSearchViewModel searchModel)
        {
            List<StockReceivingReportViewModel> result = new List<StockReceivingReportViewModel>();
            int FilteredResultsCount = 0;
            DataSet ds = new DataSet();
            string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("[Report].[usp_StockReceivingReport]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        #region Parameters
                        sda.SelectCommand.Parameters.AddWithValue("@CompanyId", searchModel.SearchCompanyId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StoreId", searchModel.SearchStoreId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StockId", searchModel.SearchStockId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@SalesEmployeeCode", searchModel.SearchSalesEmployeeCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StockReceivingCode", searchModel.SearchStockReceivingCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProductId", searchModel.SearchProductId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@FromDate", searchModel.SearchFromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ToDate", searchModel.SearchToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@isDeleted", searchModel.isDelete ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CurrentCompanyCode", CurrentUser.CompanyCode ?? (object)DBNull.Value);
                        //sda.SelectCommand.Parameters.AddWithValue("@ProfileGroupCode", searchModel.SearchProfileGroupCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@DepartmentId", searchModel.DepartmentId ?? (object)DBNull.Value);
                        var output = sda.SelectCommand.Parameters.AddWithValue("@FilteredResultsCount", FilteredResultsCount);
                        output.Direction = ParameterDirection.Output;
                        #endregion

                        sda.Fill(ds);
                        var dt = ds.Tables[0];
                        var filteredResultsCount = output.Value;
                        if (filteredResultsCount != null)
                        {
                            FilteredResultsCount = Convert.ToInt32(filteredResultsCount);
                        }

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow item in dt.Rows)
                            {
                                #region Convert to list
                                StockReceivingReportViewModel model = new StockReceivingReportViewModel();
                                model.ERPProductCode = item["ERPProductCode"].ToString();
                                model.ProductName = item["ProductName"].ToString();
                                model.CategoryName = item["CategoryName"].ToString();
                                if (!string.IsNullOrEmpty(item["Quantity"].ToString()))
                                {
                                    model.Quantity = Convert.ToDecimal(item["Quantity"].ToString());
                                }
                                model.SalesEmployeeName = item["SalesEmployeeName"].ToString();
                                model.DepartmentName = item["DepartmentName"].ToString();
                                model.StockCode = item["StockCode"].ToString();
                                model.StockName = item["StockName"].ToString();
                                if (!string.IsNullOrEmpty(item["DocumentDate"].ToString()))
                                {
                                    model.DocumentDate = Convert.ToDateTime(item["DocumentDate"].ToString());
                                }
                                model.StockReceivingCode = item["StockReceivingCode"].ToString();
                                model.Note = item["Note"].ToString();

                                result.Add(model);
                                #endregion
                            }
                        }
                    }
                }
            }
            return result;
        }
        public ActionResult Export(List<StockReceivingReportViewModel> viewModel)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;
            fileheader = "BÁO CÁO GIAO DỊCH NHẬP KHO CATALOGUE";

            #region Master
            columns.Add(new ExcelTemplate { ColumnName = "DocumentDate", isAllowedToEdit = false, isDateTime = true }); //1. Ngày chứng từ            
            columns.Add(new ExcelTemplate { ColumnName = "StockReceivingCode", isAllowedToEdit = false });//2. Mã phiếu nhập
            columns.Add(new ExcelTemplate { ColumnName = "ERPProductCode", isAllowedToEdit = false });  //3. Mã Catalogue            
            columns.Add(new ExcelTemplate { ColumnName = "ProductName", isAllowedToEdit = false });//4. Tên Catalogue            
            columns.Add(new ExcelTemplate { ColumnName = "CategoryName", isAllowedToEdit = false });//5. Nhóm Vật Tư            
            columns.Add(new ExcelTemplate { ColumnName = "Quantity", isAllowedToEdit = false });//6. Số lượng xuất                  
            columns.Add(new ExcelTemplate { ColumnName = "StockCode", isAllowedToEdit = false });//9. Mã Kho Nhận            
            columns.Add(new ExcelTemplate { ColumnName = "StockName", isAllowedToEdit = false });//10. Tên Kho nhận            
            columns.Add(new ExcelTemplate { ColumnName = "SalesEmployeeName", isAllowedToEdit = false });//11. Nhân viên 
            columns.Add(new ExcelTemplate { ColumnName = "DepartmentName", isAllowedToEdit = false });//12. Phòng ban            
            columns.Add(new ExcelTemplate { ColumnName = "Note", isAllowedToEdit = false });//12. Ghi chú
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
        #endregion
        public ActionResult ViewDetail(StockReceivingSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "StockReceivingSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "StockReceivingTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "StockReceivingModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, StockReceivingSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "StockReceivingSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "StockReceivingTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "StockReceivingModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult StockReceivingPivotGridPartial(Guid? templateId = null, StockReceivingSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/StockReceivingReport");
            var listSystemTemplate = _unitOfWork.PivotGridTemplateRepository.GetSystemTemplate(pageId);
            var listUserTemplate = _unitOfWork.PivotGridTemplateRepository.GetUserTemplate(pageId, CurrentUser.AccountId.Value);
            List<FieldSettingModel> pivotSetting = new List<FieldSettingModel>();
            if (templateId != Guid.Empty && templateId != null)
            {

                pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value);
                ViewBag.PivotSetting = pivotSetting;
                ViewBag.TemplateId = templateId;
            }
            else
            {
                var userDefaultTemplate = listUserTemplate.FirstOrDefault(s => s.IsDefault == true);
                if (userDefaultTemplate != null)
                {
                    pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(userDefaultTemplate.SearchResultTemplateId);
                    ViewBag.PivotSetting = pivotSetting;
                    ViewBag.TemplateId = userDefaultTemplate.SearchResultTemplateId;
                }
                else
                {
                    var sysDefaultTemplate = listSystemTemplate.FirstOrDefault(s => s.IsDefault == true);
                    if (sysDefaultTemplate != null)
                    {
                        pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(sysDefaultTemplate.SearchResultTemplateId);
                        ViewBag.PivotSetting = pivotSetting;
                        ViewBag.TemplateId = sysDefaultTemplate.SearchResultTemplateId;
                    }
                    else
                    {
                        ViewBag.PivotSetting = null;
                        ViewBag.TemplateId = templateId;
                    }
                }
            }

            if ((string.IsNullOrEmpty(jsonReq) || jsonReq == "null") && (searchViewModel == null || searchViewModel.IsView != true))
            {
                ViewBag.Search = null;
                return PartialView("_StockReceivingPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<StockReceivingSearchViewModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_StockReceivingPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(StockReceivingSearchViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_GIAO_DICH_NHAP_KHO_CATALOGUE";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}