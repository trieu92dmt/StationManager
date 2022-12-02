using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class ProfileWithPersonInChargeReportController : BaseController
    {
        // GET: ProfileWithPersonInChargeReport
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (ProfileWithPersonInChargeReportSearchViewModel)TempData[CurrentUser.AccountId + "ProfileWithPersonInChargeSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "ProfileWithPersonInChargeTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "ProfileWithPersonInChargeModeSearch"];
            var pageId = GetPageId("/Reports/ProfileWithPersonInChargeReport");
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
            CreateSearchViewBag();
            return View();
        }
        #endregion Index

        #region CreateSearchViewBag
        private void CreateSearchViewBag()
        {
            //Nhân viên tiếp khách
            var saleEmployeeList = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist();
            ViewBag.SalesEmployeeCode = new SelectList(saleEmployeeList, "SalesEmployeeCode", "SalesEmployeeName");

            #region //Get list Roles (Phòng ban)
            var rolesList = _context.RolesModel.Where(p => p.Actived == true && p.isEmployeeGroup == true).ToList();
            ViewBag.DepartmentCode = new SelectList(rolesList, "RolesCode", "RolesName");
            #endregion
        }
        #endregion

        //Export
        #region Export to excel
        //const string controllerCode = ConstExcelController.Appointment;
        const int startIndex = 8;

        public ActionResult ExportExcel(ProfileWithPersonInChargeReportSearchViewModel searchViewModel)
        {
            var data = new List<ProfileWithPersonInChargeReportViewModel>();
            data = _unitOfWork.ProfileRepository.GetProfileWithPersonInChargeReport(searchViewModel, CurrentUser.CompanyCode);
            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    //Địa chỉ
                    if (item.Address.StartsWith(","))
                    {
                        item.Address = item.Address.Remove(0, 2);
                    }

                    //Đối tượng
                    if (item.isForeignCustomer == true)
                    {
                        item.ForeignCustomer = "Nước ngoài";
                    }
                    else
                    {
                        item.ForeignCustomer = "Trong nước";
                    }

                    //Doanh thu năm hiện tại
                    //var doanhThuNamHienTai = _unitOfWork.RevenueRepository.GetProfileRevenueBy(item.ProfileId, DateTime.Now.Year.ToString());
                    //if (doanhThuNamHienTai != null && doanhThuNamHienTai.Count > 0)
                    //{
                     //   item.CurrentRevenue = doanhThuNamHienTai[0].DOANHSO;
                    //}

                    //Doanh thu năm trước đó
                    //var doanhThuNamTruocDo = _unitOfWork.RevenueRepository.GetProfileRevenueBy(item.ProfileId, DateTime.Now.AddYears(-1).Year.ToString());
                    //if (doanhThuNamTruocDo != null && doanhThuNamTruocDo.Count > 0)
                    //{
                    //    item.LastYearrevenue = doanhThuNamTruocDo[0].DOANHSO;
                    //}
                }
            }
            return Export(data);
        }
        private List<ProfileWithPersonInChargeReportViewModel> GetData(ProfileWithPersonInChargeReportSearchViewModel searchViewModel)
        {
            List<ProfileWithPersonInChargeReportViewModel> data = _unitOfWork.ProfileRepository.GetProfileWithPersonInChargeReport(searchViewModel, CurrentUser.CompanyCode);
            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    //Địa chỉ
                    if (item.Address.StartsWith(","))
                    {
                        item.Address = item.Address.Remove(0, 2);
                    }

                    //Đối tượng
                    if (item.isForeignCustomer == true)
                    {
                        item.ForeignCustomer = "Nước ngoài";
                    }
                    else
                    {
                        item.ForeignCustomer = "Trong nước";
                    }
                }
            }
            return data;
        }
        public ActionResult ViewDetail(ProfileWithPersonInChargeReportSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "ProfileWithPersonInChargeSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "ProfileWithPersonInChargeTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "ProfileWithPersonInChargeModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, ProfileWithPersonInChargeReportSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "ProfileWithPersonInChargeSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "ProfileWithPersonInChargeTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "ProfileWithPersonInChargeModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult ProfileWithPersonInChargeGridPartial(Guid? templateId = null, ProfileWithPersonInChargeReportSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/ProfileWithPersonInChargeReport");
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
                return PartialView("_ProfileWithPersonInChargePivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<ProfileWithPersonInChargeReportSearchViewModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_ProfileWithPersonInChargePivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(ProfileWithPersonInChargeReportSearchViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_KHACH_HANG_THEO_NHAN_VIEN_KINH_DOANH";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<ProfileWithPersonInChargeReportViewModel> viewModel)
        {
            #region Master
            List<ExcelTemplate> columns = new List<ExcelTemplate>();

            columns.Add(new ExcelTemplate { ColumnName = "SalesEmployeeName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "RolesName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileCode", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileForeignCode", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Address", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Phone", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ForeignCustomer", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "CustomerTypeName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "CustomerGroupName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "CustomerCareerName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "WardName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "DistrictName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProvinceName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "SaleOfficeName", isAllowedToEdit = false });
            //columns.Add(new ExcelTemplate { ColumnName = "LastYearrevenue", isAllowedToEdit = false, isCurrency = true });
            //columns.Add(new ExcelTemplate { ColumnName = "CurrentRevenue", isAllowedToEdit = false, isCurrency = true });

            #endregion Master

            //Header
            string fileheader = "BÁO CÁO KHÁCH HÀNG THEO NHÂN VIÊN KINH DOANH";
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
                Content = string.Format("Tổng số KH: {0}", viewModel.Count),
                RowsToIgnore = 1,
                isWarning = false,
                isCode = true
            });

            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false);
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
    }
}