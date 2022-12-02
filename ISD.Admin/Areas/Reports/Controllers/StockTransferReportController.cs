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
    public class StockTransferReportController : BaseController
    {
        // GET: StockTransferReport
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (TransferSearchViewModel)TempData[CurrentUser.AccountId + "StockTransferSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "StockTransferTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "StockTransferModeSearch"];
            var pageId = GetPageId("/Reports/StockTransferReport");

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

        #region ViewBag
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
            ViewBag.SearchFromStockId = new SelectList(listStock, "StockId", "StockName");
            ViewBag.SearchToStockId = new SelectList(listStock, "StockId", "StockName");

            //Dropdown Department
            var listDepartment = _context.DepartmentModel.Where(s => s.Actived == true);
            ViewBag.SearchDepartmentId = new SelectList(listDepartment, "DepartmentId", "DepartmentName");
        }
        #endregion

        #region Export to excel
        public ActionResult ExportExcel(TransferSearchViewModel searchModel)
        {
            var data = GetData(searchModel);
            //return Export(result, FilteredResultsCount);
            //Tổng số lượng catalog đã xuất
           // int TotalStockDelivery = Convert.ToInt32(result.Sum(p => p.Quantity));
            return Export(data);
        }


        public ActionResult Export(List<StockTransferReportViewModel> viewModel)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;
            fileheader = "BÁO CÁO GIAO DỊCH CHUYỂN KHO CATALOGUE";

            #region Master
            columns.Add(new ExcelTemplate { ColumnName = "DocumentDate", isAllowedToEdit = false, isDateTime = true }); //1. Ngày chứng từ            
            columns.Add(new ExcelTemplate { ColumnName = "TransferCode", isAllowedToEdit = false });//2. Mã phiếu chuyển          
            columns.Add(new ExcelTemplate { ColumnName = "ERPProductCode", isAllowedToEdit = false });  //3. Mã Catalogue            
            columns.Add(new ExcelTemplate { ColumnName = "ProductName", isAllowedToEdit = false });//4. Tên Catalogue            
            columns.Add(new ExcelTemplate { ColumnName = "CategoryName", isAllowedToEdit = false });//5. Nhóm Vật Tư            
            columns.Add(new ExcelTemplate { ColumnName = "Quantity", isAllowedToEdit = false });//6. Số lượng xuất           
            columns.Add(new ExcelTemplate { ColumnName = "FromStockCode", isAllowedToEdit = false });//7. Mã Kho xuất            
            columns.Add(new ExcelTemplate { ColumnName = "FromStockName", isAllowedToEdit = false });//8. Tên Kho xuất            
            columns.Add(new ExcelTemplate { ColumnName = "ToStockCode", isAllowedToEdit = false });//9. Mã Kho Nhận            
            columns.Add(new ExcelTemplate { ColumnName = "ToStockName", isAllowedToEdit = false });//10. Tên Kho nhận            
            columns.Add(new ExcelTemplate { ColumnName = "SalesEmployeeName", isAllowedToEdit = false });//11. Nhân viên     
            columns.Add(new ExcelTemplate { ColumnName = "DepartmentName", isAllowedToEdit = false });//Phòng ban     
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

        private List<StockTransferReportViewModel> GetData(TransferSearchViewModel searchModel)
        {
            List<StockTransferReportViewModel> result = new List<StockTransferReportViewModel>();
            int FilteredResultsCount = 0;
            DataSet ds = new DataSet();
            string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("[Report].[usp_StockTransferReport]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        #region Parameters
                        sda.SelectCommand.Parameters.AddWithValue("@CompanyId", searchModel.SearchCompanyId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StoreId", searchModel.SearchStoreId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@FromStockId", searchModel.SearchFromStockId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ToStockId", searchModel.SearchToStockId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@SalesEmployeeCode", searchModel.SearchSalesEmployeeCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@TransferCode", searchModel.SearchTransferCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProductId", searchModel.SearchProductId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@FromDate", searchModel.SearchFromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ToDate", searchModel.SearchToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@isDeleted", searchModel.isDelete ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CurrentCompanyCode", CurrentUser.CompanyCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@DepartmentId", searchModel.SearchDepartmentId ?? (object)DBNull.Value);
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
                                StockTransferReportViewModel model = new StockTransferReportViewModel();
                                model.ERPProductCode = item["ERPProductCode"].ToString();
                                model.ProductName = item["ProductName"].ToString();
                                model.CategoryName = item["CategoryName"].ToString();
                                if (!string.IsNullOrEmpty(item["Quantity"].ToString()))
                                {
                                    model.Quantity = Convert.ToDecimal(item["Quantity"].ToString());
                                }
                                model.SalesEmployeeName = item["SalesEmployeeName"].ToString();
                                model.FromStockCode = item["FromStockCode"].ToString();
                                model.FromStockName = item["FromStockName"].ToString();
                                model.ToStockCode = item["ToStockCode"].ToString();
                                model.ToStockName = item["ToStockName"].ToString();
                                model.DepartmentName = item["DepartmentName"].ToString();
                                if (!string.IsNullOrEmpty(item["DocumentDate"].ToString()))
                                {
                                    model.DocumentDate = Convert.ToDateTime(item["DocumentDate"].ToString());
                                }
                                model.TransferCode = item["TransferCode"].ToString();
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
        public ActionResult ViewDetail(TransferSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "StockTransferSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "StockTransferTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "StockTransferModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, TransferSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "StockTransferSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "StockTransferTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "StockTransferModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult StockTransferPivotGridPartial(Guid? templateId = null, TransferSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/StockTransferReport");
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
                return PartialView("_StockTransferPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<TransferSearchViewModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_StockTransferPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(TransferSearchViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_GIAO_DICH_CHUYEN_KHO_CATALOGUE";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}