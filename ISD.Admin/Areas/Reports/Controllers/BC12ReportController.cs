using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class BC12ReportController : BaseController
    {
        // GET: SO100Report
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (BC12ReportViewModel)TempData[CurrentUser.AccountId + "BC12ReportSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "BC12ReportTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "BC12ReportModeSearch"];
            //mode search
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
            var pageId = GetPageId("/Reports/BC12Report");
            // search data
            if (searchModel == null || searchModel.IsView != true)
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
            #region CommonDate
            var SelectedCommonDate = "Custom";
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);

            #endregion
            var saleOrg = CurrentUser.SaleOrg;
            if (searchModel != null && !string.IsNullOrEmpty(searchModel.SaleOrgCode))
            {
                saleOrg = searchModel.SaleOrgCode;
            }
            #region //Nhà máy
            var StoreList = _unitOfWork.StoreRepository.GetStoreByPermission(CurrentUser.AccountId);
            ViewBag.SaleOrgCode = new SelectList(StoreList, "SaleOrgCode", "StoreName", saleOrg);
            #endregion
            #region //Phân xưởng
            var workShopList = _unitOfWork.WorkShopRepository.GetWorkShopByStore(saleOrg);
            Guid? WorkShopId = null;
            if (searchModel != null && searchModel.WorkShopId.HasValue)
            {
                WorkShopId = searchModel.WorkShopId;
            }
            ViewBag.WorkShopId = new SelectList(workShopList, "WorkShopId", "WorkShopName", WorkShopId);

            #endregion

            #region //Công đoạn lớn
            var workCenterList = _context.WorkCenterModel.Where(p => p.SaleOrgCode == saleOrg).OrderBy(x => x.OrderIndex).ToList();
            string WorkCenterCode = null;
            if (searchModel != null && !string.IsNullOrEmpty(searchModel.WorkCenterCode))
            {
                WorkCenterCode = searchModel.WorkCenterCode;
            }
            ViewBag.WorkCenterCode = new SelectList(workCenterList, "WorkCenterCode", "WorkCenterName", WorkCenterCode);
            #endregion

            #region //Tổ
            //Lấy thông tin WorkShop
            var data = _context.DepartmentModel.Where(x => x.WorkShopId == WorkShopId).Select(x => new { x.DepartmentCode, display = x.DepartmentCode + " | " + x.DepartmentName });
            ViewBag.DepartmentCode = new SelectList(data, "DepartmentCode", "display", searchModel != null ? searchModel.DepartmentCode : null);
            #endregion
            //ViewBag.DepartmentCode = searchModel != null ? searchModel.DepartmentCode : null;

            return View();

        }
        [HttpGet]
        public PartialViewResult _LoadWorkShopBy(string SaleOrgCode)
        {
            var workShopList = _unitOfWork.WorkShopRepository.GetWorkShopByStore(SaleOrgCode);
            ViewBag.WorkShopId = new SelectList(workShopList, "WorkShopId", "WorkShopName");

            return PartialView();
        }
        [HttpGet]
        public PartialViewResult _LoadWorkCenterBy(string SaleOrgCode)
        {
            var workCenterList = _context.WorkCenterModel.Where(p => p.SaleOrgCode == SaleOrgCode).OrderBy(x => x.OrderIndex).ToList();
            ViewBag.WorkCenterCode = new SelectList(workCenterList, "WorkCenterCode", "WorkCenterName");

            return PartialView();
        }

        public ActionResult ViewDetail(BC12ReportViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "BC12ReportSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "BC12ReportTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "BC12ReportModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, BC12ReportViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "BC12ReportSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "BC12ReportTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "BC12ReportModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult BC12ReportPivotGridPartial(Guid? templateId = null, BC12ReportViewModel searchViewModel = null, string jsonReq = null)
        {
            #region commonDate

            if (searchViewModel != null && searchViewModel.CommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.CommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchViewModel.FromTime = fromDate;
                searchViewModel.ToTime = toDate;
            }
            #endregion
            var pageId = GetPageId("/Reports/BC12Report");
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
                return PartialView("_BC12PivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<BC12ReportViewModel>(jsonReq);
                  

                }
                var model = _unitOfWork.BC12ReportRepository.GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_BC12PivotGridPartial", model);
            }
        }

        [HttpPost]
        public ActionResult ExportPivot(BC12ReportViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = _unitOfWork.BC12ReportRepository.GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            var FullFileName = _unitOfWork.PivotGridTemplateRepository.GetTemplateNameBy(templateId.Value);
            string fileName = _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(FullFileName.ToUpper()).Replace(" ", "_").Replace("/", "_");
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model, FullFileName);
        }
        [HttpGet]
        public ActionResult GetDepartmentBy(Guid WorkShopId)
        {
            //Lấy thông tin WorkShop
            var data = _context.DepartmentModel.Where(x => x.WorkShopId == WorkShopId).Select(x => new { x.DepartmentCode, display = x.DepartmentCode + " | " + x.DepartmentName });
            ViewBag.DepartmentCode = new SelectList(data, "DepartmentCode", "display");
            return PartialView("_LoadDepartmentBy");
        }
    }
}