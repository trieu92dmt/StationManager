using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories;
using ISD.Repositories.Excel;
using ISD.Resources;
using ISD.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class CustomerRequirementController : BaseController
    {
        // GET: CustomerRequirement
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            DateTime? fromDate, toDate;
            var CommonDate = "ThisMonth";
            _unitOfWork.CommonDateRepository.GetDateBy(CommonDate, out fromDate, out toDate);
            AppointmentSearchViewModel searchViewModel = new AppointmentSearchViewModel()
            {
                //Ngày ghé thăm
                CommonDate = CommonDate,
                FromDate = fromDate,
                ToDate = toDate,

                //Ngày kết thúc
                CommonCreateDate = "Custom",
            };
            var searchModel = (AppointmentSearchViewModel)TempData[CurrentUser.AccountId + "CustomerRequirementSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "CustomerRequirementTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "CustomerRequirementModeSearch"];
            var pageId = GetPageId("/Reports/CustomerRequirement");

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
        #region ViewBag search
        private void CreateSearchViewBag(AppointmentSearchViewModel searchViewModel)
        {
            //Dropdown Company
            var companyList = _unitOfWork.CompanyRepository.GetAll(CurrentUser.isViewByStore, CurrentUser.AccountId);
            ViewBag.CompanyId = new SelectList(companyList, "CompanyId", "CompanyName", searchViewModel.CompanyId);

            //Dropdown Store
            var storeList = _unitOfWork.StoreRepository.GetAllStore(CurrentUser.isViewByStore, CurrentUser.AccountId);
            ViewBag.StoreId = new SelectList(storeList, "StoreId", "StoreName", searchViewModel.StoreId);

            //Địa điểm khách ghé
            var CustomerSourceList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CustomerSource);
            ViewBag.CustomerSourceCode = new SelectList(CustomerSourceList, "CatalogCode", "CatalogText_vi");

            //Nhân viên tiếp khách
            var _salesEmployeeRepository = new SalesEmployeeRepository(_context);
            var saleEmployeeList = _salesEmployeeRepository.GetAllForDropdownlist();
            ViewBag.SalesEmployeeCode = new SelectList(saleEmployeeList, "SalesEmployeeCode", "SalesEmployeeName", searchViewModel.SalesEmployeeCode);

            //Danh mục
            var categoryList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Appoitment_Category);
            ViewBag.CategoryCode = new SelectList(categoryList, "CatalogCode", "CatalogText_vi");

            //Phân loại khách hàng
            var customerClassList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CustomerClass);
            ViewBag.CustomerClassCode = new SelectList(customerClassList, "CatalogCode", "CatalogText_vi");

            //Get list Age (Độ tuổi)
            var ageList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Age);
            ViewBag.Age = new SelectList(ageList, "CatalogCode", "CatalogText_vi");

            //Get list CustomerType (Tiêu dùng, Doanh nghiệp || Liên hệ)
            var catalogList = _context.CatalogModel.Where(
                p => p.CatalogTypeCode == ConstCatalogType.CustomerType
                && p.Actived == true && p.CatalogCode != ConstCustomerType.Contact).OrderBy(p => p.OrderIndex).ToList();

            ViewBag.CustomerTypeCode = new SelectList(catalogList, "CatalogCode", "CatalogText_vi");

            //Trạng thái xử lý yêu cầu
            var WorkFlowId = _unitOfWork.WorkFlowRepository.FindWorkFlowIdByCode(ConstWorkFlow.GT);
            var taskStatusList = _unitOfWork.TaskStatusRepository.GetTaskStatusByWorkFlow(WorkFlowId);
            ViewBag.TaskStatusId = new SelectList(taskStatusList, "TaskStatusId", "TaskStatusName");

            //CommonDate
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", searchViewModel.CommonDate);
            ViewBag.CommonCreateDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", searchViewModel.CommonCreateDate);

            //Nhóm khách hàng
            var customerGroupList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CustomerCategory);
            ViewBag.CustomerGroupCode = new SelectList(customerGroupList, "CatalogCode", "CatalogText_vi");

            //Ngành nghề
            var customerCareerList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CustomerCareer);
            ViewBag.CustomerCareerCode = new SelectList(customerCareerList, "CatalogCode", "CatalogText_vi");

            //Bắc trung Nam
            var SaleOfficeList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.SaleOffice);
            ViewBag.SaleOfficeCode = new SelectList(SaleOfficeList, "CatalogCode", "CatalogText_vi");

            var filterList = new List<DropdownlistFilter>();
            filterList.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.SaleOfficeCode, FilterName = LanguageResource.Profile_SaleOfficeCode });
            filterList.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CustomerTypeCode, FilterName = LanguageResource.Profile_CustomerTypeCode });
            filterList.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.Age, FilterName = LanguageResource.Profile_Age });
            filterList.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.TaskStatusId, FilterName = LanguageResource.TaskStatus });
            filterList.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.Phone, FilterName = LanguageResource.Profile_Phone });
            filterList.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.TaxNo, FilterName = LanguageResource.Profile_TaxNo });
            filterList.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CustomerGroupCode, FilterName = LanguageResource.Profile_CustomerCategoryCode });
            filterList.Add(new DropdownlistFilter { FilterCode = ConstCustomerFilter.CustomerCareerCode, FilterName = LanguageResource.Profile_CustomerCareerCode });
            ViewBag.Filters = filterList;
        }
        #endregion


        #region export to excel
        public ActionResult ExportExcel(AppointmentSearchViewModel searchViewModel)
        {
            var requirementList = GetData(searchViewModel);
            return Export(requirementList);
        }

        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<RequirementReportViewModel> viewModel)
        {
            #region Master
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            columns.Add(new ExcelTemplate { ColumnName = "TaskStatusName", isAllowedToEdit = false });//1. Trạng thái
            columns.Add(new ExcelTemplate { ColumnName = "Requirement", isAllowedToEdit = false });//2. Yêu cầu
            columns.Add(new ExcelTemplate { ColumnName = "ProfileCode", isAllowedToEdit = false });//3. Mã khách
            columns.Add(new ExcelTemplate { ColumnName = "ProfileName", isAllowedToEdit = false });//4. Tên Khách
            columns.Add(new ExcelTemplate { ColumnName = "Address", isAllowedToEdit = false });//5. Địa chỉ 
            columns.Add(new ExcelTemplate { ColumnName = "Phone", isAllowedToEdit = false });//6. SĐT 
            columns.Add(new ExcelTemplate { ColumnName = "ShowroomCode", isAllowedToEdit = false });//7. Nguồn KH
            columns.Add(new ExcelTemplate { ColumnName = "StoreName", isAllowedToEdit = false });//8. Chi Nhánh
            columns.Add(new ExcelTemplate { ColumnName = "SalesSupervisorName", isAllowedToEdit = false });//9. NV Kinh doanh
            columns.Add(new ExcelTemplate { ColumnName = "VisitDate", isAllowedToEdit = false, isDateTime = true });//10. Thời gian
            #endregion Master

            //Header
            string fileheader = "BÁO CÁO DANH SÁCH YÊU CẦU CỦA KHÁCH HÀNG";
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


            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion
        private List<RequirementReportViewModel> GetData(AppointmentSearchViewModel searchViewModel)
        {

            if (searchViewModel.CommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                DateTime? fromPreviousDay;
                DateTime? toPreviousDay;


                _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.CommonDate, out fromDate, out toDate, out fromPreviousDay, out toPreviousDay);
                //Tìm kiếm kỳ hiện tại
                searchViewModel.FromDate = fromDate;
                searchViewModel.ToDate = toDate;
            }
            //Get data filter
            //Get data from server
            int resultCount = 0;
            var appointments = _unitOfWork.AppointmentRepository.QueryReportAppointment(searchViewModel, CurrentUser.CompanyCode, out resultCount);

            var requirementList = new List<RequirementReportViewModel>();
            if (appointments != null && appointments.Count > 0)
            {
                foreach (var item in appointments)
                {
                    if (!string.IsNullOrEmpty(item.Requirement))
                    {
                        var requirement = new RequirementReportViewModel();
                        var contact = _unitOfWork.AppointmentRepository.GetMainContact(item.ProfileId, CurrentUser.CompanyCode);
                        requirement.TaskStatusName = item.TaskStatusName;
                        requirement.Requirement = item.Requirement;
                        requirement.ProfileCode = item.ProfileCode;
                        requirement.ProfileName = item.ProfileName;
                        requirement.Address = item.Address;
                        requirement.Phone = item.Phone;
                        requirement.ShowroomCode = item.ShowroomName;
                        requirement.StoreName = item.StoreName;
                        requirement.SalesSupervisorName = contact.SalesSupervisorName;
                        requirement.VisitDate = item.VisitDate;
                        requirementList.Add(requirement);
                    }
                }
            }

            return requirementList;
        }
        public ActionResult ViewDetail(AppointmentSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "CustomerRequirementSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "CustomerRequirementTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "CustomerRequirementModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, AppointmentSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "CustomerRequirementSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "CustomerRequirementTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "CustomerRequirementModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult CustomerRequirementPivotGridPartial(Guid? templateId = null, AppointmentSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/CustomerRequirement");
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
                return PartialView("_RequirementPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<AppointmentSearchViewModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_RequirementPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(AppointmentSearchViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_DANH_SACH_YEU_CAU_CUA_KHACH_HANG";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}