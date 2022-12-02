using DevExpress.Web.Mvc;
using DevExpress.Web.Internal;
using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Reports.Controllers
{
    public class ProfileQuantityAppointmentWithShowRoomReportController : BaseController
    {
        // GET: ProfileQuantityAppointmentWithShowRoomReport
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            DateTime? fromDate, toDate;
            var CommonDate = "ThisMonth";
            _unitOfWork.CommonDateRepository.GetDateBy(CommonDate, out fromDate, out toDate);
            ProfileQuantityAppointmentWithShowRoomReportSearchViewModel searchViewModel = new ProfileQuantityAppointmentWithShowRoomReportSearchViewModel()
            {
                //Ngày ghé thăm
                CommonDate = CommonDate,
                FromDate = fromDate,
                ToDate = toDate,
            };
            var searchModel = (ProfileQuantityAppointmentWithShowRoomReportSearchViewModel)TempData[CurrentUser.AccountId + "ProfileQuantityAppointmentWithShowRoomSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "ProfileQuantityAppointmentWithShowRoomTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "ProfileQuantityAppointmentWithShowRoomModeSearch"];
            var pageId = GetPageId("/Reports/ProfileQuantityAppointmentWithShowRoomReport");
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
        #endregion Index

        #region CreateSearchViewBag
        private void CreateSearchViewBag(ProfileQuantityAppointmentWithShowRoomReportSearchViewModel searchViewModel)
        {
            //Showroom (Chi nhánh)
            var storeList = _unitOfWork.StoreRepository.GetAllStore(CurrentUser.isViewByStore, CurrentUser.AccountId);
            ViewBag.CreateAtSaleOrg = new SelectList(storeList, "SaleOrgCode", "StoreName", searchViewModel.CreateAtSaleOrg);

            //CommonDate
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", searchViewModel.CommonDate);
        }
        #endregion

        //Export
        #region Export to excel
        //const string controllerCode = ConstExcelController.Appointment;
        const int startIndex = 8;

        public ActionResult ExportExcel(ProfileQuantityAppointmentWithShowRoomReportSearchViewModel searchViewModel)
        {
            DateTime? excelFromDate = searchViewModel.FromDate;
            DateTime? excelToDate = searchViewModel.ToDate;
            DateTime? excelRatioFromDate, excelRatioToDate;
            var  data = GetData(searchViewModel, out excelRatioFromDate, out excelRatioToDate);
            return Export(data, searchViewModel.CommonDate, excelFromDate, excelToDate, excelRatioFromDate, excelRatioToDate);
        }

        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<ProfileQuantityAppointmentWithShowRoomReportViewModel> viewModel, string CommonDate = null, DateTime? excelFromDate = null, DateTime? excelToDate = null, DateTime? excelRatioFromDate = null, DateTime? excelRatioToDate = null)
        {
            #region Master
            List<ExcelTemplate> columns = new List<ExcelTemplate>();

            columns.Add(new ExcelTemplate { ColumnName = "ShowroomName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "StoreName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileCount", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Ratio", isAllowedToEdit = false });
            #endregion Master

            //Header
            string fileheader = "BÁO CÁO SỐ LƯỢNG KHÁCH GHÉ THĂM TỪNG SHOWROOM";
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
        #endregion Export to excel

        
        private List<ProfileQuantityAppointmentWithShowRoomReportViewModel> GetData(ProfileQuantityAppointmentWithShowRoomReportSearchViewModel searchViewModel, out DateTime? excelRatioFromDate, out DateTime? excelRatioToDate)
        {
            excelRatioFromDate = null;
            excelRatioToDate = null;
            if (searchViewModel.ToDate != null)
            {
                searchViewModel.ToDate = searchViewModel.ToDate.Value.AddDays(1).AddSeconds(-1);
            }
            var data = new List<ProfileQuantityAppointmentWithShowRoomReportViewModel>();
            data = _unitOfWork.AppointmentRepository.GetProfileQuantityAppointmentWithShowRoomReport(searchViewModel, CurrentUser.CompanyCode);
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
                var previous = _unitOfWork.AppointmentRepository.GetProfileQuantityAppointmentWithShowRoomReport(searchViewModel, CurrentUser.CompanyCode);
                if (previous != null && previous.Count > 0)
                {
                    foreach (var item in data)
                    {
                        var existData = previous.Where(p => p.ShowroomName == item.ShowroomName && p.StoreName == item.StoreName).FirstOrDefault();
                        if (existData != null)
                        {
                            //Kỳ này
                            decimal KyNay = item.ProfileCount;
                            //Kỳ 
                            decimal KyTruoc = existData.ProfileCount;
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
        public ActionResult ViewDetail(ProfileQuantityAppointmentWithShowRoomReportSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "ProfileQuantityAppointmentWithShowRoomSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "ProfileQuantityAppointmentWithShowRoomTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "ProfileQuantityAppointmentWithShowRoomModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, ProfileQuantityAppointmentWithShowRoomReportSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "ProfileQuantityAppointmentWithShowRoomSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "ProfileQuantityAppointmentWithShowRoomTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "ProfileQuantityAppointmentWithShowRoomModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult ProfileQuantityAppointmentWithShowRoomPivotGridPartial(Guid? templateId = null, ProfileQuantityAppointmentWithShowRoomReportSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/ProfileQuantityAppointmentWithShowRoomReport");
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
                return PartialView("_ProfileAppointmentShowroomPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<ProfileQuantityAppointmentWithShowRoomReportSearchViewModel>(jsonReq);
                }
                DateTime? excelFromDate = searchViewModel.FromDate;
                DateTime? excelToDate = searchViewModel.ToDate;
                DateTime? excelRatioFromDate, excelRatioToDate;
                var model = GetData(searchViewModel, out excelRatioFromDate, out excelRatioToDate);
                ViewBag.Search = searchViewModel;
                return PartialView("_ProfileAppointmentShowroomPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(ProfileQuantityAppointmentWithShowRoomReportSearchViewModel searchViewModel, Guid? templateId)
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
            string fileName = "BAO_CAO_SO_LUONG_KHACH_GHE_THAM_TUNG_SHOWROOM";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}