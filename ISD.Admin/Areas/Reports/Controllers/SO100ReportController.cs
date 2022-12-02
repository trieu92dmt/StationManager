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
    public class SO100ReportController : BaseController
    {
        // GET: SO100Report
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (SO100ReportViewModel)TempData[CurrentUser.AccountId + "SO100ReportSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "SO100ReportTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "SO100ReportModeSearch"];
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
            var pageId = GetPageId("/Reports/SO100Report");
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
            #region //Get List SO100
            //var SO100 = new SaleOrderHeader100ViewModel();
            //SO100.Plant = CurrentUser.SaleOrg;
            //var SO100List = _unitOfWork.SaleOrderHeader100Repository.Search(SO100).Select(x => new { x.VBELN, view = x.VBELN }).ToList();
            //ViewBag.VBELNList = new SelectList(SO100List, "VBELN", "view");
            #endregion
            #region LSXSAP
            var LSXSAPList = _unitOfWork.TaskRepository.GetSummaryLSXSAP().Select(x=>new { x.Summary, view = x.Summary }).OrderByDescending(x=>x.Summary).ToList();
            ViewBag.LSXSAPList = new SelectList(LSXSAPList, "Summary", "view");
            #endregion
            #region Common Date
            //Get list commonDate
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi");
            ViewBag.CommonDate2 = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi");
            #endregion
            return View();

        }

        [HttpPost]
        public ActionResult ExportPivot(SO100ReportViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            if (string.IsNullOrEmpty(searchViewModel.SaleOrg))
            {
                searchViewModel.SaleOrg = CurrentUser.SaleOrg;
            }
            searchViewModel.WERKS = CurrentUser.CompanyCode;
            var model = _unitOfWork.SaleOrderHeader100Repository.GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            var FullFileName = _unitOfWork.PivotGridTemplateRepository.GetTemplateNameBy(templateId.Value);
            string fileName = "NGUYEN_VAT_LIEU_DU_KIEN_DON_HANG_100_" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model, FullFileName);
        }

        public ActionResult ViewDetail(SO100ReportViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "SO100ReportSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "SO100ReportTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "SO100ReportModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, SO100ReportViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "SO100ReportSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "SO100ReportTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "SO100ReportModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult SO100ReportPivotGridPartial(Guid? templateId = null, SO100ReportViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/SO100Report");
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
                return PartialView("_SO100PivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<SO100ReportViewModel>(jsonReq);
                }
                if (string.IsNullOrEmpty(searchViewModel.SaleOrg))
                {
                    searchViewModel.SaleOrg = CurrentUser.SaleOrg;
                }
                searchViewModel.WERKS = CurrentUser.CompanyCode;
                var model = _unitOfWork.SaleOrderHeader100Repository.GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_SO100PivotGridPartial", model);
            }
        }

        #region Helper
        //Tìm kiếm LSX ĐT theo ngày làm việc
        public ActionResult SearchLSXDT(string SearchText)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = _unitOfWork.SaleOrderHeader100Repository.SearchLSXDT(SearchText);

                return _APISuccess(result);
            });
        }
        //Tìm kiếm đợt SX theo LSX ĐT
        public ActionResult SearchLSXDByLSXDT(string LSXDT, string SearchText)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = _unitOfWork.SaleOrderHeader100Repository.GetLSXDByLSXDT(LSXDT, SearchText);

                return _APISuccess(result);
            });
        }
        //Tìm kiếm LSX SAP theo Đợt SX
        public ActionResult SearchLSXCByLSXD(Guid? LSXD, string SearchText)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = _unitOfWork.SaleOrderHeader100Repository.GetLSXCByLSXD(LSXD, SearchText);

                return _APISuccess(result);
            });
        }
        #endregion
    }
}