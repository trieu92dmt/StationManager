using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc;
using ISD.Constant;
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
using System.Security.Permissions;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Reports.Controllers
{
    public class StockDeliveryReportController : BaseController
    {
        
        // GET: StockDeliveryReport
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (DeliverySearchViewModel)TempData[CurrentUser.AccountId + "DeliverySearch"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "DeliveryTempalteId"];
            var modeSearch = TempData[CurrentUser.AccountId + "DeliveryModeSearch"];
            //mode search
            if(modeSearch == null || modeSearch.ToString() == "Default")
            {
                ViewBag.ModeSearch = "Default";
            }
            else
            {
                ViewBag.ModeSearch = "Recently";
            }
            Guid templateId = Guid.Empty;
            if (tempalteIdString!=null)
            {
                templateId = Guid.Parse(tempalteIdString.ToString());
            }    
            var pageId = GetPageId("/Reports/StockDeliveryReport");
            // search data
            if(searchModel == null ||searchModel.IsView != true)
            {
                ViewBag.Search = null;
            }
            else
            {
                ViewBag.Search = searchModel;
            }
            //get list template
            var listSystemTemplate = _unitOfWork.PivotGridTemplateRepository.GetSystemTemplate(pageId);
            var listUserTemplate = _unitOfWork.PivotGridTemplateRepository.GetUserTemplate(pageId, CurrentUser.AccountId.Value);
            //get pivot setting
            List<FieldSettingModel> pivotSetting = new List<FieldSettingModel>();
            //nếu đang có template đang xem
            if (templateId != Guid.Empty && templateId != null)
            {

                pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId);
                ViewBag.PivotSetting = pivotSetting;
                ViewBag.TemplateId = templateId;
            }
            else
            {
                var userDefaultTemplate = listUserTemplate.FirstOrDefault(s => s.IsDefault == true);
                //nếu ko có template đang xem thì lấy default của user
                if (userDefaultTemplate != null)
                {
                    pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(userDefaultTemplate.SearchResultTemplateId);
                    ViewBag.PivotSetting = pivotSetting;
                    ViewBag.TemplateId = userDefaultTemplate.SearchResultTemplateId;
                }
                else 
                {
                    var sysDefaultTemplate = listSystemTemplate.FirstOrDefault(s => s.IsDefault == true);
                    //nếu user không có template thì lấy default của hệ thống
                    if (sysDefaultTemplate != null)
                    {
                        pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(sysDefaultTemplate.SearchResultTemplateId);
                        ViewBag.PivotSetting = pivotSetting;
                        ViewBag.TemplateId = sysDefaultTemplate.SearchResultTemplateId;
                    }
                    else // nếu tất cả đều không có thì render default partial view
                    {
                        ViewBag.PivotSetting = null;
                        ViewBag.TemplateId = templateId;
                    }
                }
            }
            ViewBag.PageId = pageId;
            ViewBag.SystemTemplate = listSystemTemplate;
            ViewBag.UserTemplate = listUserTemplate;
            CreateViewBag();          
            return View();
        }
        public void CreateViewBag()
        {
            //Dropdown Company
            var companyList = _unitOfWork.CompanyRepository.GetAll();
            ViewBag.SearchCompanyId = new SelectList(companyList, "CompanyId", "CompanyName");

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
            //Dropdown Loại catalogue
           // var catalogue = _context.CategoryModel.FirstOrDefault(s => s.CategoryCode == "CTL");
            var listCatalogueCategory = _context.CategoryModel.Where(s => s.Actived == true && s.CategoryCode.Contains("CTL_")).OrderBy(s=>s.OrderIndex);
            ViewBag.SearchCategoryId = new SelectList(listCatalogueCategory, "CategoryId", "CategoryName");
            //Dropdown Nhóm khách hàng
            var listProfileGroupCode = _unitOfWork.CatalogRepository.GetCustomerCategory(CurrentUser.CompanyCode);
            ViewBag.SearchProfileGroupCode = new SelectList(listProfileGroupCode, "CatalogCode", "CatalogText_vi");
        }
        #endregion

        #region Export Excel
        public ActionResult ExportExcel(DeliverySearchViewModel searchModel)
        {
            List<StockDeliveryReportViewModel> result = new List<StockDeliveryReportViewModel>();
            int FilteredResultsCount = 0;
            DataSet ds = new DataSet();
            string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
            DataTable dtListProductId = new DataTable();
            dtListProductId.Columns.Add("Id", typeof(Guid));
            if (searchModel.ListSearchProductId != null)
            {
                foreach (var productId in searchModel.ListSearchProductId)
                {
                    if (productId != null && productId != Guid.Empty)
                    {
                        dtListProductId.Rows.Add(productId);
                    }
                }
            }
            var listProductIdParameter = new SqlParameter("@ListProductIdSearch", SqlDbType.Structured);
            listProductIdParameter.TypeName = "GuidList";
            listProductIdParameter.Value = dtListProductId;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("[Report].[usp_StockDeliveryReport]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        #region Parameters
                        sda.SelectCommand.Parameters.AddWithValue("@CompanyId", searchModel.SearchCompanyId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StoreId", searchModel.SearchStoreId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StockId", searchModel.SearchStockId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@DepartmentId", searchModel.DepartmentId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@SalesEmployeeCode", searchModel.SearchSalesEmployeeCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@DeliveryCode", searchModel.SearchDeliveryCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProfileId", searchModel.SearchProfileId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProfileGroupCode", searchModel.SearchProfileGroupCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@FromDate", searchModel.SearchFromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CategoryId", searchModel.SearchCategoryId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.Add(listProductIdParameter);
                        sda.SelectCommand.Parameters.AddWithValue("@ToDate", searchModel.SearchToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CurrentCompanyCode", CurrentUser.CompanyCode ?? (object)DBNull.Value);
                        #endregion


                        var output = sda.SelectCommand.Parameters.AddWithValue("@FilteredResultsCount", FilteredResultsCount);
                        output.Direction = ParameterDirection.Output;
                        sda.Fill(ds);
                        var filteredResultsCount = output.Value;
                        if (filteredResultsCount != null)
                        {
                            FilteredResultsCount = Convert.ToInt32(filteredResultsCount);
                        }


                    }
                }
            }
            var dt = ds.Tables[0];

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    #region Convert to list
                    StockDeliveryReportViewModel model = new StockDeliveryReportViewModel();
                    model.ERPProductCode = item["ERPProductCode"].ToString();
                    model.ProductName = item["ProductName"].ToString();
                    model.CategoryName = item["CategoryName"].ToString();
                    if (!string.IsNullOrEmpty(item["Quantity"].ToString()))
                    {
                        model.Quantity = Convert.ToDecimal(item["Quantity"].ToString());
                    }
                    model.ProfileCode = item["ProfileCode"].ToString();
                    model.ProfileName = item["ProfileName"].ToString();
                    model.ProfileGroup = item["ProfileGroup"].ToString();
                    model.PersonInCharge = item["PersonInCharge"].ToString();
                    model.SalesEmployeeName = item["SalesEmployeeName"].ToString();
                    model.RolesName = item["RolesName"].ToString();
                    model.DepartmentName = item["DepartmentName"].ToString();
                    model.RecipientName = item["RecipientName"].ToString();
                    model.RecipientAddress = item["RecipientAddress"].ToString();
                    model.RecipientPhone = item["RecipientPhone"].ToString();
                    model.DistrictName = item["DistrictName"].ToString();
                    model.StockCode = item["StockCode"].ToString();
                    model.StockName = item["StockName"].ToString();
                    if (!string.IsNullOrEmpty(item["DocumentDate"].ToString()))
                    {
                        model.DocumentDate = Convert.ToDateTime(item["DocumentDate"].ToString());
                    }
                    model.DeliveryCode = item["DeliveryCode"].ToString();
                    model.Note = item["Note"].ToString();
                    if (!string.IsNullOrEmpty(item["Price"].ToString()))
                    {
                        model.Price = Convert.ToDecimal(item["Price"].ToString());
                    }
                    if (!string.IsNullOrEmpty(item["TotalPrice"].ToString()))
                    {
                        model.TotalPrice = Convert.ToDecimal(item["TotalPrice"].ToString());
                    }
                    result.Add(model);
                    #endregion
                }
            }
            //return Export(result, FilteredResultsCount);
            //Tổng số lượng catalog đã xuất
            int TotalStockDelivery = Convert.ToInt32(result.Sum(p => p.Quantity));
            return Export(result, TotalStockDelivery);
        }
        public ActionResult Export(List<StockDeliveryReportViewModel> viewModel, int TotalStockDelivery)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;
            fileheader = "BÁO CÁO SỐ LƯỢNG CATALOGUE ĐÃ XUẤT";
            columns.Add(new ExcelTemplate { ColumnName = "ERPProductCode", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProductName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "CategoryName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Quantity", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileCode", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileGroup", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "PersonInCharge", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "SalesEmployeeName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "DepartmentName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "RecipientName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "RecipientAddress", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "RecipientPhone", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "DistrictName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "StockCode", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "StockName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "DocumentDate", isAllowedToEdit = false, isDateTime = true });
            columns.Add(new ExcelTemplate { ColumnName = "DeliveryCode", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Note", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Price", isAllowedToEdit = false, isCurrency = true });
            columns.Add(new ExcelTemplate { ColumnName = "TotalPrice", isAllowedToEdit = false, isCurrency = true });

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
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = "Tổng số lượng xuất: " + TotalStockDelivery,
                RowsToIgnore = 1,
                isWarning = false,
                isCode = true
            });

            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion

        public ActionResult ViewDetail(DeliverySearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "DeliverySearch"] = searchModel;
            TempData[CurrentUser.AccountId + "DeliveryTempalteId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "DeliveryModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, DeliverySearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "DeliverySearch"] = searchModel;
            TempData[CurrentUser.AccountId + "DeliveryTempalteId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "DeliveryModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult StockDeliveryPivotGridPartial(Guid? templateId = null , DeliverySearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/StockDeliveryReport");
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
                return PartialView("_StockDeliveryPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<DeliverySearchViewModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_StockDeliveryPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot( DeliverySearchViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var  pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_SO_LUONG_CATALOGUE_DA_XUAT";
            return PivotGridExportExcel.GetExportActionResult(fileName,options, pivotSetting, model);
        }

        #region GetData
        private List<StockDeliveryReportViewModel> GetData(DeliverySearchViewModel searchModel)
        {
            List<StockDeliveryReportViewModel> result = new List<StockDeliveryReportViewModel>();
            int FilteredResultsCount = 0;
            DataSet ds = new DataSet();
            string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
            DataTable dtListProductId = new DataTable();
            dtListProductId.Columns.Add("Id", typeof(Guid));
            if(searchModel.ListSearchProductId!=null)
            {
                foreach (var productId in searchModel.ListSearchProductId)
                {
                    if (productId != null && productId != Guid.Empty)
                    {
                        dtListProductId.Rows.Add(productId);
                    }
                }
            }         
            var listProductIdParameter = new SqlParameter("@ListProductIdSearch", SqlDbType.Structured);
            listProductIdParameter.TypeName = "GuidList";
            listProductIdParameter.Value = dtListProductId;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("[Report].[usp_StockDeliveryReport]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        #region Parameters
                        sda.SelectCommand.Parameters.AddWithValue("@CompanyId", searchModel.SearchCompanyId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StoreId", searchModel.SearchStoreId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StockId", searchModel.SearchStockId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@DepartmentId", searchModel.DepartmentId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@SalesEmployeeCode", searchModel.SearchSalesEmployeeCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@DeliveryCode", searchModel.SearchDeliveryCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProfileId", searchModel.SearchProfileId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProfileGroupCode", searchModel.SearchProfileGroupCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@FromDate", searchModel.SearchFromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CategoryId", searchModel.SearchCategoryId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.Add(listProductIdParameter);
                        sda.SelectCommand.Parameters.AddWithValue("@ToDate", searchModel.SearchToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CurrentCompanyCode", CurrentUser.CompanyCode ?? (object)DBNull.Value);
                        #endregion


                        var output = sda.SelectCommand.Parameters.AddWithValue("@FilteredResultsCount", FilteredResultsCount);
                        output.Direction = ParameterDirection.Output;
                        sda.Fill(ds);
                        var filteredResultsCount = output.Value;
                        if (filteredResultsCount != null)
                        {
                            FilteredResultsCount = Convert.ToInt32(filteredResultsCount);
                        }


                    }
                }
            }
            var dt = ds.Tables[0];

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    #region Convert to list
                    StockDeliveryReportViewModel model = new StockDeliveryReportViewModel();
                    model.ERPProductCode = item["ERPProductCode"].ToString();
                    model.ProductName = item["ProductName"].ToString();
                    model.CategoryName = item["CategoryName"].ToString();
                    if (!string.IsNullOrEmpty(item["Quantity"].ToString()))
                    {
                        model.Quantity = Convert.ToDecimal(item["Quantity"].ToString());
                    }
                    model.ProfileCode = item["ProfileCode"].ToString();
                    model.ProfileName = item["ProfileName"].ToString();
                    model.ProfileGroup = item["ProfileGroup"].ToString();
                    model.PersonInCharge = item["PersonInCharge"].ToString();
                    model.SalesEmployeeName = item["SalesEmployeeName"].ToString();
                    model.RolesName = item["RolesName"].ToString();
                    model.DepartmentName = item["DepartmentName"].ToString();
                    model.RecipientName = item["RecipientName"].ToString();
                    model.RecipientAddress = item["RecipientAddress"].ToString();
                    model.RecipientPhone = item["RecipientPhone"].ToString();
                    model.DistrictName = item["DistrictName"].ToString();
                    model.StockCode = item["StockCode"].ToString();
                    model.StockName = item["StockName"].ToString();
                    if (!string.IsNullOrEmpty(item["DocumentDate"].ToString()))
                    {
                        model.DocumentDate = Convert.ToDateTime(item["DocumentDate"].ToString());
                    }
                    model.DeliveryCode = item["DeliveryCode"].ToString();
                    model.Note = item["Note"].ToString();
                    if (!string.IsNullOrEmpty(item["Price"].ToString()))
                    {
                        model.Price = Convert.ToDecimal(item["Price"].ToString());
                    }
                    if (!string.IsNullOrEmpty(item["TotalPrice"].ToString()))
                    {
                        model.TotalPrice = Convert.ToDecimal(item["TotalPrice"].ToString());
                    }
                    result.Add(model);
                    #endregion
                }
            }
            return result;
        }
        #endregion
    }
}