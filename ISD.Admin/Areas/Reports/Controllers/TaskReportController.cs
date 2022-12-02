using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.Resources;
using ISD.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class TaskReportController : BaseController
    {
        // GET: TaskReport
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index(string Type = null, Guid? WorkFlowId = null)
        {
            TaskSearchViewModel searchViewModel = new TaskSearchViewModel();
            ViewBag.Actived = true;
            string pageUrl = "/Reports/TaskReport";
            var parameter = "?Type=" + Type;
            //Title
            var containElement = "?Type=" + Type;
            if (WorkFlowId != null)
            {
                containElement += "&WorkFlowId=" + WorkFlowId;
                parameter += "&WorkFlowId=" + WorkFlowId;
            }
            var title = (from p in _context.PageModel
                         where p.PageUrl == pageUrl
                         && p.Parameter == containElement
                         select p.PageName).FirstOrDefault();
            ViewBag.Title = (LanguageResource.Report + " " + title).ToUpper();

            var searchModel = (TaskSearchViewModel)TempData[CurrentUser.AccountId + "TaskReportSearchData" + Type];
            var tempalteIdString = TempData[CurrentUser.AccountId + "TaskReportTemplateId" + Type];
            var modeSearch = TempData[CurrentUser.AccountId + "TaskReportModeSearch" + Type];
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
            var pageId = GetPageId(pageUrl, parameter);
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


            //ViewBag.PageId = GetPageId(pageUrl,parameter);
            string TaskProcessCode = string.Empty;
            switch (Type)
            {
                //Nếu góc vật liệu chọn tất cả
                case "GTB":
                    TaskProcessCode = string.Empty;
                    break;
                //Nếu là các công việc còn lại: chọn sẵn việc chưa hoàn thành
                default:
                    TaskProcessCode = ConstTaskStatus.Incomplete;
                    break;
            }
            
            CreateViewBag(Type: Type, DefaultWorkFlowId: WorkFlowId);
            return View(searchViewModel);
        }
        #endregion

        #region CreateViewBag, Helper
        public void CreateViewBag(string Type = null, Guid? DefaultWorkFlowId = null)
        {
            ViewBag.Type = Type;

            Guid? WorkFlowId = null;
            if (DefaultWorkFlowId != null)
            {
                ViewBag.DefaultWorkFlowId = DefaultWorkFlowId;
            }
            #region CommonDate
            var SelectedCommonDate = "Custom";
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);

            //Common Date 2
            var commonDateList2 = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate2);
            ViewBag.CommonDate2 = new SelectList(commonDateList2, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            #endregion

            #region //TaskStatusCode (Trạng thái)
            var isShowTaskStatusCode = true;
            if (Type == ConstWorkFlowCategory.MyFollow || Type == ConstWorkFlowCategory.MyWork)
            {
                isShowTaskStatusCode = false;
            }
            else
            {
                var result = (from wf in _context.WorkFlowModel
                              join ts in _context.TaskStatusModel on wf.WorkFlowId equals ts.WorkFlowId
                              where wf.WorkflowCategoryCode == Type
                              select new
                              {
                                  ts.OrderIndex,
                                  ts.TaskStatusCode,
                                  ts.TaskStatusName
                              }).Distinct().OrderBy(p => p.OrderIndex).ToList();
                if (result != null && result.Count > 0)
                {
                    ViewBag.TaskStatusCode = new SelectList(result, "TaskStatusCode", "TaskStatusName");
                }
                else
                {
                    isShowTaskStatusCode = false;
                }
            }
            ViewBag.isShowTaskStatusCode = isShowTaskStatusCode;
            #endregion

            #region //TaskProcessCode 
            var statusLst = _unitOfWork.TaskStatusRepository.GetTaskStatusList();
            ViewBag.TaskProcessCode = new SelectList(statusLst, "StatusCode", "StatusName");
            #endregion

            #region //Get list CustomerGroup (Nhóm khách hàng doanh nghiệp)
            var customerGroupList = _unitOfWork.CatalogRepository.GetCustomerCategory(CurrentUser.CompanyCode);
            ViewBag.ProfileGroupCode = new SelectList(customerGroupList, "CatalogCode", "CatalogText_vi");
            #endregion

            #region //Get list SalesSupervisor (NV kinh doanh)
            var empList = _unitOfWork.PersonInChargeRepository.GetListEmployee();
            ViewBag.SalesSupervisorCode = new SelectList(empList, "SalesEmployeeCode", "SalesEmployeeName");
            #endregion

            #region //Get list Roles (Phòng ban)
            var rolesList = _context.RolesModel.Where(p => p.Actived == true && p.isEmployeeGroup == true).ToList();
            ViewBag.DepartmentCode = new SelectList(rolesList, "RolesCode", "RolesName");
            #endregion

            #region //WorkFlowId (Loại)
            var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(Type);
            listWorkFlow = listWorkFlow.Where(p => p.WorkFlowCode != ConstWorkFlow.GT).ToList();
            ViewBag.WorkFlowIdList = new SelectList(listWorkFlow, "WorkFlowId", "WorkFlowName", WorkFlowId);
            #endregion

            #region //TaskStatusId (Trạng thái)
            if (listWorkFlow != null && listWorkFlow.Count > 0)
            {
                WorkFlowId = listWorkFlow[0].WorkFlowId;
            }
            var lst = _unitOfWork.TaskStatusRepository.GetTaskStatusByWorkFlow((Guid)WorkFlowId);
            var TaskStatusId_GT = (from p in _context.TaskStatusModel
                                   join w in _context.WorkFlowModel on p.WorkFlowId equals w.WorkFlowId
                                   where w.WorkFlowCode == ConstWorkFlow.GT
                                   && p.TaskStatusCode == ConstWorkFlow.GT
                                   select p.TaskStatusId).FirstOrDefault();
            ViewBag.TaskStatusIdList = new SelectList(lst, "TaskStatusId", "TaskStatusName");
            #endregion

            //Priority
            var listPriority = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Priority);
            ViewBag.PriorityCode = new SelectList(listPriority, "CatalogCode", "CatalogText_vi");
            //Ý kiến khách hàng
            var customerRatings = new List<SelectListItem>();
            customerRatings.Add(new SelectListItem()
            {
                Value = "none",
                Text = "Không ý kiến"
            });
            customerRatings.Add(new SelectListItem()
            {
                Value = "rating",
                Text = "Đánh giá theo sao & ý kiến khác"
            });
            ViewBag.Property5 = new SelectList(customerRatings, "Value", "Text");
            //CommonMistakeCode
            var commonMistakeList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonMistake);
            ViewBag.CommonMistakeCode = new SelectList(commonMistakeList, "CatalogCode", "CatalogText_vi");

            //StoreId: get all store
            var storeList = _unitOfWork.StoreRepository.GetAllStore();
            ViewBag.StoreIdList = new SelectList(storeList, "StoreId", "StoreName");

            //CustomerType
            var customerTypeList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CustomerType);
            customerTypeList = customerTypeList.Where(p => p.CatalogCode != ConstCustomerType.Contact).ToList();
            ViewBag.CustomerTypeCode = new SelectList(customerTypeList, "CatalogCode", "CatalogText_vi");

            //Employee
            var empLst = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist();
            ViewBag.EmployeeList = empLst;

            //Role: Task Assign Type
            ViewBag.RoleList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.TaskAssignType);

            //Task Roles List (Phân công cho nhóm/phòng ban)
            ViewBag.TaskRolesList = _unitOfWork.AccountRepository.GetRolesList(isEmployeeGroup: true);

            //Reporter
            ViewBag.Reporter = new SelectList(empLst, "SalesEmployeeCode", "SalesEmployeeName");

            //Assignee
            //ViewBag.AssigneeList = new SelectList(empLst, "SalesEmployeeCode", "SalesEmployeeName");
            ViewBag.Assignee = new SelectList(empLst, "SalesEmployeeCode", "SalesEmployeeName");

            //CreateBy
            var accounts = _unitOfWork.AccountRepository.GetAll();
            ViewBag.CreateBy = new SelectList(accounts, "AccountId", "UserName");

            //ServiceTechnicalTeamCode
            var serviceLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ServiceTechnicalTeam);
            ViewBag.ServiceTechnicalTeamCode = new SelectList(serviceLst, "CatalogCode", "CatalogText_vi");

            //ErrorTypeCode
            var errorTypeLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ErrorType);
            ViewBag.ErrorTypeCode = new SelectList(errorTypeLst, "CatalogCode", "CatalogText_vi");

            //ErrorCode
            var errorLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Error);
            ViewBag.ErrorCode = new SelectList(errorLst, "CatalogCode", "CatalogText_vi");

            //VisitTypeCode
            var visitTypeLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.VisitType);
            ViewBag.VisitTypeCode = new SelectList(visitTypeLst, "CatalogCode", "CatalogText_vi");

            //RemindCycle
            var remindCycleLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.RemindCycle);
            ViewBag.RemindCycle = new SelectList(remindCycleLst, "CatalogCode", "CatalogText_vi");

            //Mã màu SP
            var productColorLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ProductColor);
            ViewBag.ProductColorCode = new SelectList(productColorLst, "CatalogCode", "CatalogCode");

            //Nhóm vật tư
            var productSearchCategoryLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ProductCategory);
            ViewBag.ProductCategoryCode = new SelectList(productSearchCategoryLst, "CatalogCode", "CatalogText_vi");

            //Các lỗi bảo hành thuờng gặp
            var usualErrorLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.UsualError);
            ViewBag.UsualErrorCode = new SelectList(usualErrorLst, "CatalogCode", "CatalogText_vi");

            //Nguồn tiếp nhận (TaskSourceCode)
            var taskSourceList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.TaskSource)
                                                                .Select(p => new SelectListItem()
                                                                {
                                                                    Text = p.CatalogText_vi,
                                                                    Value = p.CatalogCode,
                                                                });

            ViewBag.TaskSourceCode = new SelectList(taskSourceList, "Value", "Text");
            //Loại phụ kiện
            var accessoryTypeCode = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ProductAccessoryType);
            ViewBag.AccessoryTypeCode = new SelectList(accessoryTypeCode, "CatalogCode", "CatalogText_vi");
            //ContactId
            var contacts = new List<ProfileContactViewModel>();
            ViewBag.ContactId = new SelectList(contacts, "ProfileId", "ProfileName");

            #region Loại catalogue (Nhóm VT)
            var catalogCategoryList = _context.View_Catalog_Category.OrderBy(p => p.OrderIndex).Select(p => new ISDSelectGuidItem()
            {
                id = p.CategoryId,
                name = p.CategoryName,
            }).ToList();

            ViewBag.CategoryId = new SelectList(catalogCategoryList, "id", "name");
            #endregion

            #region //Filters
            var filterLst = new List<DropdownlistFilter>();
            //Khách hàng
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.WorkFlowId, FilterName = LanguageResource.Type });
            //Khách hàng
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.Property5, FilterName = LanguageResource.CustomerResult });
            //Liên hệ
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ContactId, FilterName = LanguageResource.Profile_Contact });
            //Người tạo
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.CreateBy, FilterName = LanguageResource.CreateBy });
            //Mức độ
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.PriorityCode, FilterName = LanguageResource.Task_PriorityCode });
            //Ngày tiếp nhận
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ReceiveDate, FilterName = LanguageResource.Task_ReceiveDate });
            //Ngày bắt đầu
            //filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.StartDate, FilterName = LanguageResource.Task_StartDate });
            //Ngày đến hạn
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.EstimateEndDate, FilterName = LanguageResource.Task_EstimateEndDate });
            //Ngày kết thúc
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.EndDate, FilterName = LanguageResource.Task_EndDate });
            //filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ConstructionUnit, FilterName = LanguageResource.ConstructionUnit });
            //filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.CommonMistakeCode, FilterName = LanguageResource.Task_CommonMistakeCode });

            //Hình thức bảo hành
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ErrorTypeCode, FilterName = LanguageResource.Task_ErrorTypeCode2 });
            //Phương thức xử lý
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ErrorCode, FilterName = LanguageResource.Task_ErrorCode2 });
            //Nhóm vật tư
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ProductCategoryCode, FilterName = LanguageResource.Task_ProductCategory });
            //Các lỗi BH thường gặp
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.UsualErrorCode, FilterName = LanguageResource.UsualErrorCode });
            //Mã màu
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ProductColorCode, FilterName = LanguageResource.ProductColorCode });
            //Nhóm KH
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.CustomerGroupCode, FilterName = LanguageResource.Profile_General_CustomerGroup });
            //NV kinh doanh
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.SalesSupervisorCode, FilterName = LanguageResource.PersonInCharge });
            //Phòng ban
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.DepartmentCode, FilterName = LanguageResource.Profile_Department });
            //TT bảo hành
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ServiceTechnicalTeamCode, FilterName = LanguageResource.Task_ServiceTechnicalTeamCode });
            //Mã SAP sp
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ERPProductCode, FilterName = LanguageResource.Task_ERPProductCode });
            //Mã SAP phụ kiện
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ERPAccessoryCode, FilterName = LanguageResource.Task_ERPAccessoryCode });
            //Loại phụ kiện
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.AccessoryTypeCode, FilterName = LanguageResource.Task_AccessoryTypeCode });
            //THKH
            //Phân loại chuyến thăm
            if (Type == ConstWorkFlowCategory.THKH)
            {
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.VisitTypeCode, FilterName = LanguageResource.Task_VisitTypeCode });
            }
            //Nhóm VT
            if (Type == ConstWorkFlowCategory.GTB)
            {
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.CategoryId, FilterName = "Nhóm vật tư" });
            }
            ViewBag.Filters = filterLst;
            #endregion
        }

        /// <summary>
        /// Lấy các lỗi thường gặp theo Nhóm vât tư
        /// Nếu cấu hình có hiển thị Các lỗi BH thường gặp mới lấy list
        /// </summary>
        /// <param name="ProductCategoryCode"></param>
        /// <param name="IsTakeAll">Nếu take all thì lấy hết các lỗi (dành cho filter tìm kiếm)</param>
        /// <returns></returns>
        public ActionResult GetUsualErrorByProductCategory(string ProductCategoryCode, bool? IsTakeAll = null, Guid? WorkFlowId = null)
        {
            var errorList = new List<CatalogViewModel>();

            var config = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == WorkFlowId).ToList();
            var fieldCodeList = config.Select(p => p.FieldCode).ToList();
            if (fieldCodeList.Contains(PropertyHelper.GetPropertyName<TaskProductViewModel, string>(p => p.UsualErrorCode)))
            {
                if (IsTakeAll == true && string.IsNullOrEmpty(ProductCategoryCode))
                {
                    errorList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.UsualError);
                }
                else
                {
                    var CompanyCode = config.Where(p => p.WorkFlowId == WorkFlowId && p.FieldCode == "ProductCategoryCode")
                                            .Select(p => p.Parameters).FirstOrDefault();
                    errorList = _unitOfWork.TaskRepository.GetUsualErrorByProductCategory(ProductCategoryCode, CompanyCode);
                }
            }
            return Json(errorList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Export Excel
        const int startIndex = 8;

        public ActionResult ExportExcel( TaskSearchViewModel searchViewModel)
        {
            var data = GetData(searchViewModel);
            return Export(data, searchViewModel.Type, searchViewModel.DefaultWorkFlowId);
        }
        private List<TaskExcelViewModel> GetData(TaskSearchViewModel searchViewModel)
        {
            searchViewModel.ContactId = searchViewModel.CompanyId;
            searchViewModel.ContactName = searchViewModel.CompanyName;

            List<string> processCodeList = new List<string>();
            if (searchViewModel.TaskProcessCode == null)
            {
                processCodeList.Add(ConstTaskStatus.Todo);
                processCodeList.Add(ConstTaskStatus.Processing);
                processCodeList.Add(ConstTaskStatus.Incomplete);
                processCodeList.Add(ConstTaskStatus.CompletedOnTime);
                processCodeList.Add(ConstTaskStatus.CompletedExpire);
                processCodeList.Add(ConstTaskStatus.Expired);
            }

            #region //Created Date
            if (searchViewModel.CreatedCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.CreatedCommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchViewModel.CreatedFromDate = fromDate;
                searchViewModel.CreatedToDate = toDate;
            }
            #endregion

            #region //Receive Date
            if (searchViewModel.ReceiveCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.ReceiveCommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchViewModel.ReceiveFromDate = fromDate;
                searchViewModel.ReceiveToDate = toDate;
            }
            #endregion

            #region //Start Date
            if (searchViewModel.StartCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.StartCommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchViewModel.StartFromDate = fromDate;
                searchViewModel.StartToDate = toDate;
            }
            #endregion

            #region //Estimate End Date
            if (searchViewModel.EstimateEndCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.EstimateEndCommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchViewModel.EstimateEndFromDate = fromDate;
                searchViewModel.EstimateEndToDate = toDate;
            }
            #endregion

            #region //End Date
            if (searchViewModel.EndCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.EndCommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchViewModel.EndFromDate = fromDate;
                searchViewModel.EndToDate = toDate;
            }
            #endregion

            //Nếu chọn loại "Tất cả" -> chỉ lấy các loại có trong list filter
            var workflowList = new List<Guid>();
            if (searchViewModel.WorkFlowIdList != null && searchViewModel.WorkFlowIdList.Count > 0)
            {
                foreach (var item in searchViewModel.WorkFlowIdList)
                {
                    workflowList.Add((Guid)item);
                }
            }
            else
            {
                var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(searchViewModel.Type, CurrentUser.CompanyCode);
                if (listWorkFlow != null && listWorkFlow.Count > 0)
                {
                    workflowList = listWorkFlow.Select(p => p.WorkFlowId).ToList();
                }
            }

            int filteredResultsCount = 0;
            var res = _unitOfWork.TaskRepository.SearchQueryTaskExcelProc(searchViewModel, out filteredResultsCount, workflowList: workflowList, AccountId: CurrentUser.AccountId, processCodeList: processCodeList, errorList: searchViewModel.UsualErrorCode, colorList: searchViewModel.ProfileGroupCodeList, CurrentCompanyCode: CurrentUser.CompanyCode, taskStatusCodeList: searchViewModel.TaskStatusCodeList, assigneeList: searchViewModel.AssigneeList, workFlowId: searchViewModel.WorkFlowIdList, productCategoryCodeList: searchViewModel.ProductCategoryCodeList, profileGroupCodeList: searchViewModel.ProfileGroupCodeList, departmentCodeList: searchViewModel.DepartmentCodeList);

            //Get doanh số cho từng khách hàng nếu Type = GTB
            if (searchViewModel.Type == ConstWorkFlowCategory.GTB)
            {
                foreach (var item in res)
                {
                    //Doanh thu năm hiện tại
                    var doanhThuNamHienTai = _unitOfWork.RevenueRepository.GetProfileRevenueBy(item.ProfileId, DateTime.Now.Year.ToString(), CurrentUser.CompanyCode);
                    if (doanhThuNamHienTai != null && doanhThuNamHienTai.Count > 0)
                    {
                        item.GTB_CurrentRevenue = doanhThuNamHienTai[0].DOANHSO;
                    }

                    //Doanh thu năm trước đó
                    var doanhThuNamTruocDo = _unitOfWork.RevenueRepository.GetProfileRevenueBy(item.ProfileId, DateTime.Now.AddYears(-1).Year.ToString(), CurrentUser.CompanyCode);
                    if (doanhThuNamTruocDo != null && doanhThuNamTruocDo.Count > 0)
                    {
                        item.GTB_LastYearrevenue = doanhThuNamTruocDo[0].DOANHSO;
                    }
                }
            }
            //Get list sđt
            foreach (var item in res)
            {
                var phoneList = _context.ProfilePhoneModel.Where(s => s.ProfileId == item.ProfileId).ToList();
                if (phoneList.Count != 0)
                {
                    foreach (var phone in phoneList)
                    {
                        item.Phone = item.Phone + ", " + phone.PhoneNumber;
                    }
                }
            }
            return res;
        }

        public FileContentResult Export(List<TaskExcelViewModel> viewModel, string Type, Guid? WorkFlowId = null)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;
            //Title
            var containElement = "?Type=" + Type;
            if (WorkFlowId != null)
            {
                containElement += "&WorkFlowId=" + WorkFlowId;
            }
            var title = (from p in _context.PageModel
                         where p.PageUrl == "/Reports/TaskReport"
                         && p.Parameter == containElement
                         select p.PageName).FirstOrDefault();
            fileheader = (LanguageResource.Report + " " + title).ToUpper();

            #region Columns
            //Thăm hỏi khách hàng
            if (Type == ConstWorkFlowCategory.THKH)
            {
                columns.Add(new ExcelTemplate { ColumnName = "THKH_EndDate", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "Summary", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TaskStatusName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ProfileCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ProfileName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "PersonInCharge", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "RoleInCharge", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Address", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Phone", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Email", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "VisitAddress", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "SaleOfficeName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "AssigneeName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Description", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "CustomerReviews", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "CreateTime", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "THKH_StartDate", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "CheckInTime", isAllowedToEdit = false, isDateTime = true });
            }
            //Bảo hành (Bảo hành MLC)
            else if (Type == ConstWorkFlowCategory.TICKET_MLC)
            {
                columns.Add(new ExcelTemplate { ColumnName = "StartDate", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "EndDate", isAllowedToEdit = false, isDateTime = true });
               // columns.Add(new ExcelTemplate { ColumnName = "Summary", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ServiceTechnicalTeam", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TaskStatusName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "WorkFlowName", isAllowedToEdit = false });
               // columns.Add(new ExcelTemplate { ColumnName = "ProfileCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ProfileName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Address", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Phone", isAllowedToEdit = false });
              //  columns.Add(new ExcelTemplate { ColumnName = "SaleOfficeName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "AssigneeName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Description", isAllowedToEdit = false, isWraptext=true });
             //columns.Add(new ExcelTemplate { ColumnName = "ConstructionDate", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "CustomerReviews", isAllowedToEdit = false, isWraptext = true });
               // columns.Add(new ExcelTemplate { ColumnName = "OrderCode", isAllowedToEdit = false });
                //columns.Add(new ExcelTemplate { ColumnName = "WarrantyValue", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ERPProductCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ProductName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Qty", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ProductCategoryName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ERPAccessoryCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "AccessoryName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "AccessoryQty", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "AccessoryCategoryName", isAllowedToEdit = false });
                //columns.Add(new ExcelTemplate { ColumnName = "ProductColorCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ErrorName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ErrorTypeName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ReceiveDate", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "ServiceRating", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ProductRating", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "Review", isAllowedToEdit = false });
            }
            //Xử lý yêu cầu (Bảo hành ACC)
            else if (Type == ConstWorkFlowCategory.TICKET)
            {
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_ReceiveDate", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_ProfileCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_ProfileName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_Address", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_Phone", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_Description", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_StartDate", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_EndDate", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_CustomerReviews", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_UsualErrorName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_ErrorTypeName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_TaskSourceName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_SaleOfficeName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_CreateByName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_ConstructionUnit", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_ConstructionUnitName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_PersonInCharge", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_ConstructionDate", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_OrderCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_AssigneeName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_ProductCategoryName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_ProductColorCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_Qty", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_WarrantyValue", isAllowedToEdit = false, isNumber = true });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_TaskStatusName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "TICKET_WorkFlowName", isAllowedToEdit = false });
            }
            else if (Type == ConstWorkFlowCategory.GTB)
            {
                if (WorkFlowId != ConstWorkFlow.GVLID)
                {
                    columns.Add(new ExcelTemplate { ColumnName = "WorkFlowName", isAllowedToEdit = false });
                }
                columns.Add(new ExcelTemplate { ColumnName = "GTB_ProfileCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_ProfileForeignCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_ProfileShortName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_CustomerGroupName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_PersonInCharge", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_RoleInCharge", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_TaskStatusName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_StartDate", isAllowedToEdit = false, isDateTime = true });
                if (WorkFlowId == ConstWorkFlow.GVLID)
                {
                    columns.Add(new ExcelTemplate { ColumnName = "GTB_VisitAddress", isAllowedToEdit = false });
                }
                else
                {
                    columns.Add(new ExcelTemplate { ColumnName = "DTB_VisitAddress", isAllowedToEdit = false });
                }

                columns.Add(new ExcelTemplate { ColumnName = "GTB_DistrictName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_ProvinceName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_SaleOfficeName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_ContactName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_ContactPhone", isAllowedToEdit = false });
                if (WorkFlowId == ConstWorkFlow.GVLID)
                {
                    columns.Add(new ExcelTemplate { ColumnName = "GTB_ValueOfShowroom", isAllowedToEdit = false, isCurrency = true });
                }
                else
                {
                    columns.Add(new ExcelTemplate { ColumnName = "DTB_ValueOfShowroom", isAllowedToEdit = false, isCurrency = true });
                }
                columns.Add(new ExcelTemplate { ColumnName = "GTB_ValueOfShowroom", isAllowedToEdit = false, isCurrency = true });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_NearestDate_THKH", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_AssigneeName_THKH", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_Description_THKH", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_RemindDate_THKH", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_LastYearrevenue", isAllowedToEdit = false, isCurrency = true });
                columns.Add(new ExcelTemplate { ColumnName = "GTB_CurrentRevenue", isAllowedToEdit = false, isCurrency = true });
            }
            else if (Type == ConstWorkFlowCategory.ACTIVITIES)
            {
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_StartDate", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_Summary", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_TaskStatusName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_WorkFlowName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_Description", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_PersonInCharge", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_RoleInCharge", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_ProfileCode", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_ProfileName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_Address", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_Phone", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_Email", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_ReceiveDate", isAllowedToEdit = false, isDateTime = true });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_ReporterName", isAllowedToEdit = false });
                columns.Add(new ExcelTemplate { ColumnName = "ACTI_AssigneeName", isAllowedToEdit = false });
            }
            else if (Type == ConstWorkFlowCategory.MISSION)
            {
                columns.Add(new ExcelTemplate { ColumnName = "MIS_TaskCode", isAllowedToEdit = false });//1. Mã yêu cầu
                columns.Add(new ExcelTemplate { ColumnName = "MIS_Summary", isAllowedToEdit = false });//2. Tiêu Đề
                columns.Add(new ExcelTemplate { ColumnName = "MIS_TaskStatusName", isAllowedToEdit = false });//3. Trạng Thái
                columns.Add(new ExcelTemplate { ColumnName = "MIS_PriorityName", isAllowedToEdit = false });//4. Mức Độ
                columns.Add(new ExcelTemplate { ColumnName = "MIS_Description", isAllowedToEdit = false });//5. Mô tả
                columns.Add(new ExcelTemplate { ColumnName = "MIS_CreateByName", isAllowedToEdit = false });//6. NV Giao việc
                columns.Add(new ExcelTemplate { ColumnName = "MIS_ReporterName", isAllowedToEdit = false });//7. NV theo dõi/giám sát
                columns.Add(new ExcelTemplate { ColumnName = "MIS_AssigneeName", isAllowedToEdit = false });//8. NV được phân công
                columns.Add(new ExcelTemplate { ColumnName = "MIS_StartDate", isAllowedToEdit = false, isDateTime = true });//9. Ngày bắt đầu
                columns.Add(new ExcelTemplate { ColumnName = "MIS_EstimateEndDate", isAllowedToEdit = false, isDateTime = true });//10. Ngày đến hạn
                columns.Add(new ExcelTemplate { ColumnName = "MIS_EndDate", isAllowedToEdit = false, isDateTime = true });//11. Ngày kết thúc
               

            }

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
            if (Type == ConstWorkFlowCategory.ACTIVITIES)
            {
                heading.Add(new ExcelHeadingTemplate()
                {
                    Content = string.Format("Tổng số HĐ: {0}", viewModel.Count),
                    RowsToIgnore = 1,
                    isWarning = false,
                    isCode = true
                });
            }
            if (Type == ConstWorkFlowCategory.MISSION)
            {
                heading.Add(new ExcelHeadingTemplate()
                {
                    Content = string.Format("Tổng số giao việc: {0}", viewModel.Count),
                    RowsToIgnore = 1,
                    isWarning = false,
                    isCode = true
                });
            }

            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion

        public ActionResult ViewDetail(TaskSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "TaskReportSearchData" + searchModel.Type] = searchModel;
            TempData[CurrentUser.AccountId + "TaskReportTemplateId" + searchModel.Type] = pivotTemplate;
            TempData[CurrentUser.AccountId + "TaskReportModeSearch" + searchModel.Type] = modeSearch;
            return RedirectToAction("Index", new {Type = searchModel.Type, WorkFlowId =searchModel.DefaultWorkFlowId});
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, TaskSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "TaskReportSearchData" + searchModel.Type] = searchModel;
            TempData[CurrentUser.AccountId + "TaskReportTemplateId" + searchModel.Type] = pivotTemplate;
            TempData[CurrentUser.AccountId + "TaskReportModeSearch" + searchModel.Type] = modeSearch;
            return RedirectToAction("Index", new { Type = searchModel.Type, WorkFlowId = searchModel.DefaultWorkFlowId });
        }


        [ValidateInput(false)]
        public ActionResult TaskPivotGridPartial(string Type, Guid? WorkFlowId, Guid? templateId = null, TaskSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            List<FieldSettingModel> pivotSetting = new List<FieldSettingModel>();
            if (templateId != Guid.Empty && templateId != null)
            {

                pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value);
                ViewBag.PivotSetting = pivotSetting;
                ViewBag.TemplateId = templateId;
            }
            
            if ((string.IsNullOrEmpty(jsonReq) || jsonReq == "null") && (searchViewModel == null || searchViewModel.IsView != true))
            {
                ViewBag.Search = null;
                if (Type == ConstWorkFlowCategory.ACTIVITIES)
                {
                    return PartialView("_TaskActivePivotGridPartial", null);
                }
                else
                {
                    if (Type == ConstWorkFlowCategory.THKH)
                    {
                        return PartialView("_TaskTHKHPivotGridPartial", null);
                    }
                    else
                    {
                        if (Type == ConstWorkFlowCategory.TICKET_MLC)
                        {
                            return PartialView("_TaskTICKETMLCPivotGridPartial", null);
                        }
                        else
                        {
                            if (Type == ConstWorkFlowCategory.TICKET)
                            {
                                return PartialView("_TaskTICKETPivotGridPartial", null);
                            }
                            else
                            {
                                if (Type == ConstWorkFlowCategory.GTB)
                                {
                                    if (WorkFlowId == ConstWorkFlow.GVLID)
                                    {
                                        return PartialView("_TaskGVLPivotGridPartial", null);
                                    }
                                    return PartialView("_TaskGTBPivotGridPartial", null);
                                }
                                else
                                {
                                    return PartialView("_TaskMISSIONPivotGridPartial", null);
                                }
                            }
                        }
                        
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<TaskSearchViewModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                if(searchViewModel.Type == ConstWorkFlowCategory.ACTIVITIES)
                {
                    return PartialView("_TaskActivePivotGridPartial", model);
                }
                else
                {
                    if(searchViewModel.Type == ConstWorkFlowCategory.THKH)
                    {
                        return PartialView("_TaskTHKHPivotGridPartial", model);
                    }
                    else
                    {
                        if (searchViewModel.Type == ConstWorkFlowCategory.TICKET_MLC)
                        {
                            return PartialView("_TaskTICKETMLCPivotGridPartial", model);
                        }
                        else
                        {
                            if (searchViewModel.Type == ConstWorkFlowCategory.TICKET)
                            {
                                return PartialView("_TaskTICKETPivotGridPartial", model);
                            }
                            else
                            {
                                if (searchViewModel.Type == ConstWorkFlowCategory.GTB)
                                {
                                    if (searchViewModel.DefaultWorkFlowId == ConstWorkFlow.GVLID)
                                    {
                                        return PartialView("_TaskGVLPivotGridPartial", model);
                                    }
                                    return PartialView("_TaskGTBPivotGridPartial", model);
                                }
                                else
                                {
                                    return PartialView("_TaskMISSIONPivotGridPartial", model);
                                }
                            }
                        }
                    }
                }
                
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(TaskSearchViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "Task";
            if (searchViewModel.Type == ConstWorkFlowCategory.THKH)
            {
                fileName = "BAO_CAO_TONG_HOP_THAM_HOI_KHACH_HANG";
            }  
            else
            {
                if (searchViewModel.Type == ConstWorkFlowCategory.ACTIVITIES)
                {
                    fileName = "BAO_CAO_TONG_HOP_NHIEM_VU";
                }
                else
                {
                    if (searchViewModel.Type == ConstWorkFlowCategory.TICKET_MLC)
                    {
                        fileName = "BAO_CAO_TONG_HOP_BAO_HANH";
                    }
                    else
                    {
                        if (searchViewModel.Type == ConstWorkFlowCategory.TICKET)
                        {
                            fileName = "BAO_CAO_TONG_HOP_XU_LY_KHIEU_NAI_KH";
                        }
                        else
                        {
                            if (searchViewModel.Type == ConstWorkFlowCategory.GTB)
                            {
                                if(searchViewModel.DefaultWorkFlowId == ConstWorkFlow.GVLID)
                                {
                                    fileName = "BAO_CAO_DANH_SACH_GOC_VAT_LIEU";
                                }
                                else
                                {
                                    fileName = "BAO_CAO_DANH_SACH_DIEM_TRUNG_BAY";
                                }                           
                            }
                            else
                            {
                                fileName = "BAO_CAO_GIAO_VIEC";
                            }
                        }
                    }
                }
            }
            
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}