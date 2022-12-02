using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class AppointmentWithPersonInChargeReportController : BaseController
    {
        // GET: AppointmentWithPersonInChargeReport
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (AppointmentWithPersonInChargeReportSearchViewModel)TempData[CurrentUser.AccountId + "AppointmentWithPersonInChargeSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "AppointmentWithPersonInChargeTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "AppointmentWithPersonInChargeModeSearch"];
            var pageId = GetPageId("/Reports/AppointmentWithPersonInChargeReport");
            DateTime? fromDate, toDate;
            var CommonDate = "ThisMonth";
            _unitOfWork.CommonDateRepository.GetDateBy(CommonDate, out fromDate, out toDate);
            AppointmentWithPersonInChargeReportSearchViewModel searchViewModel = new AppointmentWithPersonInChargeReportSearchViewModel()
            {
                //Ngày ghé thăm
                CommonDate = CommonDate,
                FromDate = fromDate,
                ToDate = toDate,
            };
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
            CreateSearchViewBag(searchViewModel);
            return View(searchViewModel);
        }

        #region CreateSearchViewBag
        private void CreateSearchViewBag(AppointmentWithPersonInChargeReportSearchViewModel searchViewModel)
        {
            //Nhân viên
            var saleEmployeeList = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist();
            ViewBag.SalesEmployeeCode = new SelectList(saleEmployeeList, "SalesEmployeeCode", "SalesEmployeeName");

            //CommonDate
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", searchViewModel.CommonDate);
        }
        #endregion

        #region Export to Excel
        public ActionResult ExportExcel(AppointmentWithPersonInChargeReportSearchViewModel searchViewModel)
        {
            DateTime? excelFromDate = searchViewModel.FromDate;
            DateTime? excelToDate = searchViewModel.ToDate;
            DateTime? excelRatioFromDate, excelRatioToDate;
            var data = GetData(searchViewModel, out excelRatioFromDate, out excelRatioToDate);
            return Export(data, searchViewModel.CommonDate, excelFromDate, excelToDate, excelRatioFromDate, excelRatioToDate);
        }

        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<AppointmentWithPersonInChargeReportViewModel> viewModel, string CommonDate = null, DateTime? excelFromDate = null, DateTime? excelToDate = null, DateTime? excelRatioFromDate = null, DateTime? excelRatioToDate = null)
        {
            #region Master
            List<ExcelTemplate> columns = new List<ExcelTemplate>();

            columns.Add(new ExcelTemplate { ColumnName = "SalesEmployeeName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "QtyAppointment", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Ratio", isAllowedToEdit = false });
            #endregion Master

            //Header
            string fileheader = "BÁO CÁO SỐ LƯỢNG KHÁCH GHÉ THĂM THEO NHÂN VIÊN";
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
            if (!string.IsNullOrEmpty(CommonDate) && CommonDate != "Custom")
            {
                var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate).ToList();
                var commonDateStr = commonDateList.Where(p => p.CatalogCode == CommonDate).Select(p => p.CatalogText_vi).FirstOrDefault();
                heading.Add(new ExcelHeadingTemplate()
                {
                    Content = string.Format("Tỷ lệ: {0} ({1:dd/MM/yyyy}-{2:dd/MM/yyyy} so với {3:dd/MM/yyyy}-{4:dd/MM/yyyy})", commonDateStr, excelFromDate, excelToDate, excelRatioFromDate, excelRatioToDate),
                    RowsToIgnore = 1,
                    isWarning = false,
                    isCode = true
                });
            }

            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false, IsMergeCellHeader: false);
            //File name
            //Insert => THEM_MOI
            //Edit => CAP_NHAT
            //string exportType = LanguageResource.exportType_Insert;
            //if (isEdit == true)
            //{
            //    exportType = LanguageResource.exportType_Edit;
            //}
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion

        #region GetData for Report
        private List<AppointmentWithPersonInChargeReportViewModel> GetData(AppointmentWithPersonInChargeReportSearchViewModel searchViewModel, out DateTime? excelRatioFromDate, out DateTime? excelRatioToDate)
        {
            excelRatioFromDate = null;
            excelRatioToDate = null;
            if (searchViewModel.ToDate != null)
            {
                searchViewModel.ToDate = searchViewModel.ToDate.Value.AddDays(1).AddSeconds(-1);
            }
            var data = new List<AppointmentWithPersonInChargeReportViewModel>();
            data = _unitOfWork.AppointmentRepository.GetQuantityAppointmentWithPersonInChargeReport(searchViewModel, CurrentUser.CompanyCode);
            if (data != null && data.Count > 0)
            {
                //Ngày ghé thăm
                if (searchViewModel.CommonDate != "Custom")
                {
                    DateTime? fromDate;
                    DateTime? toDate;
                    DateTime? fromPreviousDay;
                    DateTime? toPreviousDay;

                    _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.CommonDate, out fromDate, out toDate, out fromPreviousDay, out toPreviousDay);
                    //Tìm kiếm kỳ trước
                    searchViewModel.FromDate = fromPreviousDay;
                    searchViewModel.ToDate = toPreviousDay;

                    excelRatioFromDate = fromPreviousDay;
                    excelRatioToDate = toPreviousDay;
                }
                //Lấy dữ liệu kỳ trước
                var previous = _unitOfWork.AppointmentRepository.GetQuantityAppointmentWithPersonInChargeReport(searchViewModel, CurrentUser.CompanyCode);
                if (previous != null && previous.Count > 0)
                {
                    foreach (var item in data)
                    {
                        var existData = previous.Where(p => p.SalesEmployeeCode == item.SalesEmployeeCode).FirstOrDefault();
                        if (existData != null)
                        {
                            //Kỳ này
                            decimal KyNay = item.QtyAppointment;
                            //Kỳ 
                            decimal KyTruoc = existData.QtyAppointment;
                            //Tỷ lệ kỳ này so với kỳ trước
                            decimal TyLe = 100;
                            if (KyTruoc != 0)
                            {
                                TyLe = (KyNay - KyTruoc) / KyTruoc * 100;
                            }
                            else if (KyTruoc == 0 && KyNay == 0)
                            {
                                TyLe = 0;
                            }

                            item.Ratio = string.Format("{0:0.##} %", TyLe);
                        }
                        else
                        {
                            item.Ratio = "0 %";
                        }
                    }
                }
            }
            return data;
        }
        #endregion


        public ActionResult ViewDetail(AppointmentWithPersonInChargeReportSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "AppointmentWithPersonInChargeSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "AppointmentWithPersonInChargeTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "AppointmentWithPersonInChargeModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, AppointmentWithPersonInChargeReportSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "AppointmentWithPersonInChargeSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "AppointmentWithPersonInChargeTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "AppointmentWithPersonInChargeModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult AppointmentWithPersonInChargeGridPartial(Guid? templateId = null, AppointmentWithPersonInChargeReportSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/AppointmentWithPersonInChargeReport");
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
                return PartialView("_AppointmentWithPersonInChargePivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<AppointmentWithPersonInChargeReportSearchViewModel>(jsonReq);
                }
                DateTime? excelFromDate = searchViewModel.FromDate;
                DateTime? excelToDate = searchViewModel.ToDate;
                DateTime? excelRatioFromDate, excelRatioToDate;
                var model = GetData(searchViewModel,out excelRatioFromDate,out excelRatioToDate);
                ViewBag.Search = searchViewModel;
                return PartialView("_AppointmentWithPersonInChargePivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(AppointmentWithPersonInChargeReportSearchViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            DateTime? excelFromDate = searchViewModel.FromDate;
            DateTime? excelToDate = searchViewModel.ToDate;
            DateTime? excelRatioFromDate, excelRatioToDate;
            var model = GetData(searchViewModel, out excelRatioFromDate, out excelRatioToDate);
            
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_SO_LUONG_GHE_THAM_THEO_NHAN_VIEN";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}