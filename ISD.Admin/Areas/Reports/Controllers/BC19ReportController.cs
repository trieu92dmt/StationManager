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
using Reports.Options;

namespace Reports.Controllers
{
    public class BC19ReportController : BaseController
    {
        // GET: SO100Report
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (BC19ReportViewModel)TempData[CurrentUser.AccountId + "BC19ReportSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "BC19ReportTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "BC19ReportModeSearch"];
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
            var pageId = GetPageId("/Reports/BC19Report");
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
            //var SelectedCommonDate = "Custom";
            var SelectedCommonDate = "ThisWeek";
            if (searchModel != null && !string.IsNullOrEmpty(searchModel.CommonDate))
            {
                SelectedCommonDate = searchModel.CommonDate;
            }
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            #endregion

            #region Cảnh báo
            var warningList = new List<ISDSelectStringItem>();
            warningList.Add(new ISDSelectStringItem()
            {
                id = "XANH",
                name = "XANH",
            });
            warningList.Add(new ISDSelectStringItem()
            {
                id = "VÀNG",
                name = "VÀNG",
            });
            warningList.Add(new ISDSelectStringItem()
            {
                id = "ĐỎ",
                name = "ĐỎ",
            });
            ViewBag.WarningSearch = new SelectList(warningList, "id", "name");
            #endregion

            #region //Plant
            var plantList = _unitOfWork.StoreRepository.GetStoreByPermission(CurrentUser.AccountId);
            ViewBag.Plant = new SelectList(plantList, "SaleOrgCode", "StoreName", CurrentUser.SaleOrg);
            #endregion

            return View(searchModel);

        }
        [HttpGet]
        public PartialViewResult _LoadWorkShopBy(string SaleOrgCode)
        {
            var workShopList = _unitOfWork.WorkShopRepository.GetWorkShopByStore(SaleOrgCode);
            ViewBag.WorkShopId = new SelectList(workShopList, "WorkShopId", "WorkShopName");

            return PartialView();
        }
        
        public ActionResult ViewDetail(BC19ReportViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "BC19ReportSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "BC19ReportTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "BC19ReportModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, BC19ReportViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "BC19ReportSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "BC19ReportTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "BC19ReportModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult BC19ReportPivotGridPartial(Guid? templateId = null, BC19ReportViewModel searchViewModel = null, string jsonReq = null)
        {
            #region commonDate

            if (searchViewModel != null && searchViewModel.CommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.CommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchViewModel.EndFromDate = fromDate;
                searchViewModel.EndToDate = toDate;
            }
            #endregion
            var pageId = GetPageId("/Reports/BC19Report");
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
                return PartialView("_BC19PivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<BC19ReportViewModel>(jsonReq);
                  

                }
                var model = _unitOfWork.BC19ReportRepository.GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_BC19PivotGridPartial", model);
            }
        }

        public ActionResult BC19ReportPiePartial(Guid? templateId = null, BC19ReportViewModel searchViewModel = null, string jsonReq = null)
        {
            #region commonDate

            if (searchViewModel != null && searchViewModel.CommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.CommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchViewModel.EndFromDate = fromDate;
                searchViewModel.EndToDate = toDate;
            }
            #endregion
            var pageId = GetPageId("/Reports/BC19Report");
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
                return PartialView("_BC19PiePartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<BC19ReportViewModel>(jsonReq);


                }
                var model = _unitOfWork.BC19ReportRepository.GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                //get thông tin cho pie chart: lấy thông tin cảnh báo
                //var warningList = model.Select(p => p.Warning).ToList();
                var warningGroup = model.GroupBy(p => p.Warning)
                                            .Select(group => new BC19DataPieChartViewModel()
                                            {
                                                WarningName = group.Key,
                                                WarningCount = group.Count(),
                                            }).ToList();
                var warningList = new List<BC19DataPieChartViewModel>();
                warningList.Add(new BC19DataPieChartViewModel {
                    WarningName = "XANH",
                    WarningColor = "#008000",
                    WarningCount = 0
                });
                warningList.Add(new BC19DataPieChartViewModel
                {
                    WarningName = "VÀNG",
                    WarningColor = "#FFFF00",
                    WarningCount = 0
                });
                warningList.Add(new BC19DataPieChartViewModel
                {
                    WarningName = "ĐỎ",
                    WarningColor = "#FF0000",
                    WarningCount = 0
                });
                if (warningList != null && warningList.Count > 0)
                {
                    foreach (var item in warningList)
                    {
                        var currentWarning = warningGroup.Where(p => p.WarningName == item.WarningName).FirstOrDefault();
                        if (currentWarning != null)
                        {
                            item.WarningCount = currentWarning.WarningCount;
                        }
                    }
                }

                ChartPieDoughnutDemoOptions options = new ChartPieDoughnutDemoOptions() { Data = warningList, ShowLabels = true };
                return PartialView("_BC19PiePartial", options);
            }
        }

        public ActionResult _BC19PiePartial(ChartPieDoughnutDemoOptions options)
        {
            return PartialView("_BC19PiePartial", options);
        }

        [HttpPost]
        public ActionResult ExportPivot(BC19ReportViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = _unitOfWork.BC19ReportRepository.GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            var FullFileName = _unitOfWork.PivotGridTemplateRepository.GetTemplateNameBy(templateId.Value);
            string fileName = _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(FullFileName.ToUpper()).Replace(" ", "_").Replace("/", "_");
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model, FullFileName);
        }
    }
}