using GoogleMaps.LocationServices;
using ISD.Constant;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories;
using ISD.Resources;
using ISD.ViewModels;
using ISD.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ISD.ViewModels.Work;
using ISD.ViewModels.MES;
using ISD.Repositories.MES.IntegrationSAP.MasterData;
using System.Threading.Tasks;
using System.IO;

namespace Work.Controllers
{
    public class TaskController : BaseController
    {
        private string GoogleMapAPIKey = WebConfigurationManager.AppSettings["GoogleMapAPIKey"].ToString();

        #region Index
        // GET: Task
        [ISDAuthorizationAttribute]
        public ActionResult Index(string Type = null, Guid? WorkFlowId = null, string Assignee = null, string TaskProcessCode_Input = null, string tab = null)
        {
            TaskSearchViewModel searchModel = new TaskSearchViewModel();
            //ViewBag.Actived = true;
            //Title
            string pageUrl = "/Work/Task";
            var parameter = "?Type=" + Type;
            //var containElemen = "?Type=" + Type;
            if (WorkFlowId != null)
            {
                parameter += "&WorkFlowId=" + WorkFlowId;
            }
            if (!string.IsNullOrEmpty(tab))
            {
                parameter += "&tab=" + tab;
            }
            var title = (from p in _context.PageModel
                         where p.PageUrl == pageUrl
                         && p.Parameter == parameter
                         select p.PageName).FirstOrDefault();
            ViewBag.Title = title;

            ViewBag.PageId = GetPageId(pageUrl, parameter);
            //Kanban
            var kanban = _context.KanbanModel.Where(p => p.KanbanCode == Type).FirstOrDefault();
            if (kanban != null)
            {
                ViewBag.KanbanId = kanban.KanbanId;
            }

            string Reporter = string.Empty;
            string TaskProcessCode = string.Empty;
            switch (Type)
            {
                //Nếu là công việc của tôi: chọn sẵn tôi là người được phân công là tôi
                case "MyWork":
                    if (string.IsNullOrEmpty(Assignee))
                    {
                        Assignee = CurrentUser.EmployeeCode;
                    }
                    break;
                //Nếu là công việc tôi đang theo dõi: chọn sẵn người giao việc là tôi
                case "MyFollow":
                    if (string.IsNullOrEmpty(Assignee))
                    {
                        Reporter = CurrentUser.EmployeeCode;
                    }
                    break;
                //Nếu góc vật liệu chọn tất cả
                case "GTB":
                    TaskProcessCode = string.Empty;
                    break;
                //Nếu là các công việc còn lại: chọn sẵn việc chưa hoàn thành
                default:
                    TaskProcessCode = ConstTaskStatus.Incomplete;
                    break;
            }
            if (!string.IsNullOrEmpty(TaskProcessCode_Input))
            {
                TaskProcessCode = TaskProcessCode_Input;
            }
            CreateViewBag(Type: Type, WorkFlowId: WorkFlowId, Reporter: Reporter, Assignee: Assignee, isEditMode: false);
            ViewBag.GoogleMapAPIKey = GoogleMapAPIKey;
            CreateSearchViewBag(Type: Type, TaskProcessCode: TaskProcessCode);
            var estimatedTitle = (from p in _context.PageModel
                                  where p.PageUrl.Contains("EstimatedCalendar")
                                  select p.PageName).FirstOrDefault();
            ViewBag.EstimatedTitle = estimatedTitle;

            var companyList = _context.CompanyModel.OrderBy(p => p.CompanyCode).Select(p => new
            {
                CompanyCode = p.CompanyCode,
                CompanyName = p.CompanyCode + " | " + p.CompanyName
            }).ToList();
            ViewBag.CompanyCode = new SelectList(companyList, "CompanyCode", "CompanyName");
            //var TaskStatusList = _context
            return View(searchModel);
        }

        public ActionResult _Search(TaskSearchViewModel searchModel)
        {
            return ExecuteSearch(() =>
            {
                var searchResult = _unitOfWork.TaskRepository.Search(searchModel);
                return PartialView(searchResult);
            });
        }

        public ActionResult _PaggingServerSide(DatatableViewModel model, TaskSearchViewModel searchViewModel, Guid? KanbanId, List<string> UsualErrorCode, List<string> ProductColorCode, string tab = null)
        {
            try
            {
                // action inside a standard controller
                int filteredResultsCount;
                int totalResultsCount = model.length;

                searchViewModel.ContactId = searchViewModel.CompanyId;
                searchViewModel.ContactName = searchViewModel.CompanyName;
                if (searchViewModel.CompanyId == null)
                {
                    searchViewModel.CompanyId = _unitOfWork.CompanyRepository.GetBy(CurrentUser.SaleOrg).CompanyId;
                }
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

                #region //Create Date
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

                List<KanbanTaskViewModel> calendarList = new List<KanbanTaskViewModel>();
                //Page Size 
                searchViewModel.PageSize = model.length;
                //Page Number
                searchViewModel.PageNumber = model.start / model.length + 1;

                List<KanbanColumnViewModel> columns = new List<KanbanColumnViewModel>();
                List<TaskViewModel> kanbanDetailList = new List<TaskViewModel>();
                bool isRenderKanban = false;
                var kanban = _context.KanbanModel.FirstOrDefault(p => p.KanbanId == KanbanId && p.Actived == true);
                if (kanban != null)
                {
                    isRenderKanban = true;
                    //Load column base on KanbanCode
                    columns = _unitOfWork.KanbanTaskRepository.GetColumnKanban(searchViewModel.KanbanId, HasUnmapped: false);
                }

                //Loại
                var workflowList = new List<Guid>();
                //Nếu filter là chọn nhiều => lấy các loại có trong filter
                if (searchViewModel.WorkFlowIdList != null && searchViewModel.WorkFlowIdList.Count > 0)
                {
                    workflowList = searchViewModel.WorkFlowIdList;
                }
                else
                {
                    //Nếu chọn loại "Tất cả" -> chỉ lấy các loại có trong list filter
                    if (searchViewModel.WorkFlowId == null || searchViewModel.WorkFlowId == Guid.Empty)
                    {
                        var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(searchViewModel.Type, CurrentUser.CompanyCode);
                        if (listWorkFlow != null && listWorkFlow.Count > 0)
                        {
                            workflowList = listWorkFlow.Select(p => p.WorkFlowId).ToList();
                        }
                    }
                }

                var res = new List<TaskViewModel>();
                //Nếu là bản đồ thì lấy tất cả 
                if (tab == "#tab-map")
                {
                    //Page Size 
                    searchViewModel.PageSize = null;
                    //Page Number
                    searchViewModel.PageNumber = null;

                    res = _unitOfWork.TaskRepository.SearchQueryTaskProc(searchViewModel, out filteredResultsCount, workflowList: workflowList, AccountId: CurrentUser.AccountId, processCodeList: processCodeList, errorList: UsualErrorCode, colorList: ProductColorCode);
                }
                else
                {
                    res = _unitOfWork.TaskRepository.SearchQueryTaskProc(searchViewModel, out filteredResultsCount, workflowList: workflowList, AccountId: CurrentUser.AccountId, processCodeList: processCodeList, errorList: UsualErrorCode, colorList: ProductColorCode);

                    if (res != null && res.Count > 0)
                    {
                        int i = model.start;
                        foreach (var item in res)
                        {
                            i++;
                            item.STT = i;

                            if (kanban != null)
                            {
                                kanbanDetailList = res;
                                //Assignee
                                var assigneeLst = (from p in _context.TaskAssignModel
                                                   join s in _context.SalesEmployeeModel on p.SalesEmployeeCode equals s.SalesEmployeeCode
                                                   where p.TaskId == item.TaskId
                                                   select new SalesEmployeeViewModel
                                                   {
                                                       SalesEmployeeCode = s.SalesEmployeeCode,
                                                       SalesEmployeeName = s.SalesEmployeeName
                                                   }).ToList();

                                foreach (var emp in assigneeLst)
                                {
                                    var href = string.Format("/Work/Task?Type=MyWork&Assignee={0}", emp.SalesEmployeeCode);
                                    item.tags += string.Format(",<a href='{0}' title='{1}' target='_blank'><img src='https://i0.wp.com/avatar-management--avatars.us-west-2.prod.public.atl-paas.net/initials/{2}-0.png?ssl=1' /></a>", href, emp.SalesEmployeeName, emp.SalesEmployeeName.GetCharacterForLogoName());
                                }
                            }
                            //Nếu loại = Đợt sản xuất => lấy thông tin các ngày dựa vào LSX SAP
                            //Ngày bắt đầu: Min
                            //Ngày kết thúc: Max
                            if (searchViewModel.Type == ConstWorkFlow.LSXD)
                            {
                                item.StartDate = _context.TaskModel.Where(p => p.ParentTaskId == item.TaskId).Min(e => e.StartDate);
                                item.EstimateEndDate = _context.TaskModel.Where(p => p.ParentTaskId == item.TaskId).Max(e => e.EstimateEndDate);
                                item.Date1 = _context.TaskModel.Where(p => p.ParentTaskId == item.TaskId).Min(e => e.Date1);
                                item.ReceiveDate = _context.TaskModel.Where(p => p.ParentTaskId == item.TaskId).Max(e => e.ReceiveDate);
                            }
                        }

                        //Estimated Calendar
                        var taskIdList = res.Select(p => p.TaskId).ToList();
                        calendarList = _unitOfWork.TaskRepository.SearchTaskEstimatedCalendar(taskIdList, searchViewModel.StartFromDate, searchViewModel.StartToDate);
                    }
                }


                //Vị trí hiện tại dựa vào cửa hàng user chọn login
                var currentLocation = (from p in _context.StoreModel
                                       join pr in _context.ProvinceModel on p.ProvinceId equals pr.ProvinceId
                                       where p.SaleOrgCode == CurrentUser.SaleOrg
                                       select pr.ProvinceName).FirstOrDefault();

                var jsonResult = Json(new
                {
                    draw = model.draw,
                    recordsTotal = totalResultsCount,
                    recordsFiltered = filteredResultsCount,
                    data = res,
                    //kanban
                    isRenderKanban = isRenderKanban,
                    columns = columns,
                    kanbanDetailList = kanbanDetailList,
                    //Maps
                    CurrentLocation = currentLocation,
                    //Calendar GTB
                    calendarList = calendarList
                }, JsonRequestBehavior.AllowGet);
                //max length
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }

        /// <summary>
        /// Render list in tab inside "Profile" edit
        /// </summary>
        /// <param name="ProfileId"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ActionResult _List(Guid ProfileId, string Type)
        {
            var lst = (from p in _context.TaskModel
                       join ts in _context.TaskStatusModel on p.TaskStatusId equals ts.TaskStatusId
                       join w in _context.WorkFlowModel on p.WorkFlowId equals w.WorkFlowId
                       join pr in _context.CatalogModel on new
                       {
                           p.PriorityCode,
                           CatalogTypeCode = ConstCatalogType.Priority
                       } equals new
                       {
                           PriorityCode = pr.CatalogCode,
                           pr.CatalogTypeCode
                       }
                       //Reporter
                       join re in _context.SalesEmployeeModel on p.Reporter equals re.SalesEmployeeCode into reg
                       from report in reg.DefaultIfEmpty()
                       where
                       //Khách hàng hoặc là đơn vị thi công
                       (p.ProfileId == ProfileId || p.ConstructionUnit == ProfileId)
                       && w.WorkflowCategoryCode == Type
                       //Khác Ghé thăm
                       && w.WorkFlowCode != ConstWorkFlow.GT
                       && p.isDeleted != true
                       select new TaskViewModel
                       {
                           TaskId = p.TaskId,
                           TaskCode = p.TaskCode,
                           Summary = p.Summary,
                           Description = p.Description,
                           CustomerReviews = p.CustomerReviews,
                           WorkFlowName = w.WorkFlowName,
                           Property5 = p.Property5,
                           TaskStatusId = ts.TaskStatusId,
                           TaskStatusCode = ts.TaskStatusCode,
                           TaskStatusName = ts.TaskStatusName,
                           ProcessCode = ts.ProcessCode,
                           PriorityText_vi = pr.CatalogText_vi,
                           ReporterName = report.SalesEmployeeName,
                           StartDate = p.StartDate,
                           EndDate = p.EndDate,
                           EstimateEndDate = p.EstimateEndDate,
                       }).ToList();
            if (lst != null && lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    //Assignee
                    var assigneeLst = (from p in _context.TaskAssignModel
                                       join s in _context.SalesEmployeeModel on p.SalesEmployeeCode equals s.SalesEmployeeCode
                                       where p.TaskId == item.TaskId
                                       select new SalesEmployeeViewModel
                                       {
                                           SalesEmployeeCode = s.SalesEmployeeCode,
                                           SalesEmployeeName = s.SalesEmployeeName
                                       }).ToList();
                    item.AssigneeName = string.Join(", ", assigneeLst.Select(p => p.SalesEmployeeName).ToArray());

                    //Color
                    if (Type == ConstWorkFlowCategory.TICKET_MLC)
                    {
                        var currentDate = DateTime.Now;

                        #region TaskStatusBackgroundColor
                        //1. Quá hạn
                        if (item.ProcessCode != ConstProcess.completed && (item.EstimateEndDate != null && item.EstimateEndDate < currentDate && item.EndDate == null))
                        {
                            item.TaskStatusBackgroundColor = "#dd4b39";
                        }
                        //2. Hoàn thành đúng hạn
                        else if (item.ProcessCode == ConstProcess.completed && (item.EndDate <= item.EstimateEndDate || item.EstimateEndDate == null || item.EndDate == null))
                        {
                            item.TaskStatusBackgroundColor = "#398439";
                        }
                        //3. Hoàn thành quá hạn
                        else if (item.ProcessCode == ConstProcess.completed && (item.EstimateEndDate != null && item.EndDate != null && item.EstimateEndDate < item.EndDate))
                        {
                            item.TaskStatusBackgroundColor = "#f39c12";
                        }
                        //4. Chờ xử lý
                        else if (item.ProcessCode == ConstProcess.todo && (item.EstimateEndDate != null && item.EndDate != null && item.EstimateEndDate < item.EndDate))
                        {
                            item.TaskStatusBackgroundColor = "#fff";
                        }
                        //5. Đang thực hiện
                        else if (item.TaskStatusCode == "PC" && item.ProcessCode == ConstProcess.processing)
                        {
                            item.TaskStatusBackgroundColor = "#F39C12";
                        }
                        else if (item.TaskStatusCode != "PC" && item.ProcessCode == ConstProcess.processing)
                        {
                            item.TaskStatusBackgroundColor = "#0052CC";
                        }
                        else
                        {
                            item.TaskStatusBackgroundColor = "#fff";
                        }
                        #endregion TaskStatusBackgroundColor

                        #region TaskStatusColor
                        if (item.ProcessCode != ConstProcess.completed && (item.EstimateEndDate != null && item.EstimateEndDate < currentDate && item.EndDate == null))
                        {
                            item.TaskStatusColor = "#fff";
                        }
                        else if (item.ProcessCode == ConstProcess.completed && (item.EndDate <= item.EstimateEndDate || item.EstimateEndDate == null || item.EndDate == null))
                        {
                            item.TaskStatusColor = "#fff";
                        }
                        else if (item.ProcessCode == ConstProcess.completed && (item.EstimateEndDate != null && item.EndDate != null && item.EstimateEndDate < item.EndDate))
                        {
                            item.TaskStatusColor = "#fff";
                        }
                        else if (item.ProcessCode == ConstProcess.todo)
                        {
                            item.TaskStatusColor = "#000";
                        }
                        else if (item.ProcessCode == ConstProcess.processing)
                        {
                            item.TaskStatusColor = "#fff";
                        }
                        else
                        {
                            item.TaskStatusColor = "#000";
                        }
                        #endregion TaskStatusColor
                    }
                }
            }
            ViewBag.Type = Type;
            //Title
            var title = (from p in _context.PageModel
                         where p.Parameter == "?Type=" + Type
                         select p.PageName).FirstOrDefault();
            ViewBag.Title = title;
            ViewBag.ProfileId = ProfileId;
            return PartialView(lst.OrderByDescending(p => p.TaskCode));
        }

        /// <summary>
        /// Search popup Profile and Contact base on Type
        /// </summary>
        /// <param name="ProfileId"></param>
        /// <param name="hasNoContact"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public ActionResult _ProfileSearch(Guid? ProfileId, bool? hasNoContact, string ProfileType)
        {
            ProfileSearchViewModel model = new ProfileSearchViewModel();
            model.ProfileId = ProfileId;
            model.hasNoContact = hasNoContact;
            model.ProfileType = ProfileType;

            var catalogList = _context.CatalogModel.Where(p => p.CatalogTypeCode == ConstCatalogType.CustomerType
                                                            && p.CatalogCode != ConstCustomerType.Contact
                                                            && p.Actived == true)
                                                   .OrderBy(p => p.OrderIndex).ToList();
            ViewBag.CustomerTypeCode = new SelectList(catalogList, "CatalogCode", "CatalogText_vi");

            return PartialView("~/Areas/Customer/Views/Profile/_ProfileSearch.cshtml", model);
        }
        #endregion

        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create(string Type = null, Guid? ProductWarrantyId = null, Guid? ProfileId = null, Guid? ParentTaskId = null, Guid? WorkFlowId = null, Guid? CopyFrom = null)
        {
            ViewBag.CopyFrom = CopyFrom;
            var saleOrgCode = CurrentUser.SaleOrg;
            var storeId = _unitOfWork.StoreRepository.GetStoreIdBySaleOrgCode(saleOrgCode);
            TaskViewModel taskVM = new TaskViewModel();
            //Nếu có ParentTaskId => tạo subtask
            if (ParentTaskId != null)
            {
                taskVM = _unitOfWork.TaskRepository.GetSubtaskInfo(ParentTaskId);
                taskVM.Type = Type;
                //Lấy parent type của type subtask
                var parentType = (from p in _context.TaskModel
                                  join w in _context.WorkFlowModel on p.WorkFlowId equals w.WorkFlowId
                                  where p.TaskId == ParentTaskId
                                  select w.WorkflowCategoryCode).FirstOrDefault();
                ViewBag.ParentType = parentType;
            }
            else
            {
                taskVM.Type = Type;
                taskVM.ReceiveDate = DateTime.Now.Date;
                if (Type == ConstWorkFlowCategory.TICKET_MLC)
                {
                    taskVM.StartDate = DateTime.Now;
                }
                taskVM.PriorityCode = ConstPriotityCode.NORMAL;
                taskVM.StoreId = storeId;
                taskVM.ProductWarrantyId = ProductWarrantyId;
                taskVM.ProfileId = ProfileId;
                taskVM.ProfileName = _unitOfWork.ProfileRepository.GetProfileNameBy(taskVM.ProfileId);
                taskVM.ContactShortName = _unitOfWork.ProfileRepository.GetProfileShortNameBy(taskVM.ProfileId);
                if (Type == ConstWorkFlowCategory.BOOKING_VISIT)
                {
                    taskVM.Assignee = CurrentUser.EmployeeCode;
                }
                else
                {
                    taskVM.Reporter = CurrentUser.EmployeeCode;
                }
                taskVM.RemindCycle = ConstRemindCycle.THANG;
            }

            #region ViewBag
            var WorkFlowName = string.Empty;
            List<string> hasRequestList = new List<string>();
            List<SelectListItem> hasRequestRadioList = new List<SelectListItem>();
            var catList = _context.CatalogModel.Where(p => p.CatalogTypeCode == ConstCatalogType.HasRequest)
                                  .OrderBy(p => p.OrderIndex).ToList();
            //Task config field
            List<WorkFlowConfigModel> configList = new List<WorkFlowConfigModel>();
            var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(taskVM.Type);
            if (listWorkFlow != null && listWorkFlow.Count > 0)
            {
                listWorkFlow = listWorkFlow.Where(p => p.WorkFlowCode != ConstWorkFlow.GT).ToList();
                if (WorkFlowId == null)
                {
                    taskVM.WorkFlowId = listWorkFlow[0].WorkFlowId;
                    taskVM.WorkFlowCode = listWorkFlow[0].WorkFlowCode;
                    WorkFlowName = listWorkFlow[0].WorkFlowName;
                }
                else
                {
                    var existWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlow((Guid)WorkFlowId);
                    if (existWorkFlow != null)
                    {
                        taskVM.WorkFlowId = existWorkFlow.WorkFlowId;
                        taskVM.WorkFlowCode = existWorkFlow.WorkFlowCode;
                        WorkFlowName = existWorkFlow.WorkFlowName;
                    }
                }

                configList = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == taskVM.WorkFlowId).ToList();
                if (configList != null && configList.Count > 0)
                {
                    foreach (var item in configList)
                    {
                        if (string.IsNullOrEmpty(item.Note))
                        {
                            item.Note = PropertyHelper.GetDisplayNameByString<TaskViewModel>(item.FieldCode);
                        }
                        //Yêu cầu cần xử lý: YC/NO_YC
                        if (item.FieldCode == ConstCatalogType.HasRequest && !string.IsNullOrEmpty(item.Parameters))
                        {
                            if (item.Parameters.Contains(","))
                            {
                                hasRequestList = item.Parameters.Split(',').ToList();
                                foreach (var para in hasRequestList)
                                {
                                    var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                    hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                                }
                            }
                            else
                            {
                                var para = hasRequestList.FirstOrDefault();
                                var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                            }
                        }
                        //Set default value
                        if (!string.IsNullOrEmpty(item.AddDefaultValue))
                        {
                            var type = taskVM.GetType().GetProperty(item.FieldCode).PropertyType;
                            taskVM.GetType().GetProperty(item.FieldCode).SetValue(taskVM, PropertyHelper.ChangeType(item.AddDefaultValue, type));
                        }
                    }
                }
            }
            var defaultValue = (catList != null && catList.Count > 0 && hasRequestList.Count > 0) ?
                                    catList.FirstOrDefault(p => p.CatalogCode == "NO_YC").CatalogCode :
                                    null;
            ViewBag.HasRequest = new SelectList(hasRequestRadioList, "Value", "Text", defaultValue);
            ViewBag.WorkFlowConfig = configList;
            ViewBag.WorkFlowConfigCode = configList.Select(p => p.FieldCode).ToList();
            ViewBag.WorkFlowHiddenField = configList.Where(p => p.HideWhenAdd == true).Select(p => p.FieldCode).ToList();

            CreateViewBag(PriorityCode: taskVM.PriorityCode, StoreId: storeId, isEditMode: false, Type: taskVM.Type, Reporter: taskVM.Reporter, Assignee: taskVM.Assignee, WorkFlowId: taskVM.WorkFlowId, ServiceTechnicalTeamCode: taskVM.ServiceTechnicalTeamCode, RemindCycle: taskVM.RemindCycle, Category: defaultValue, TaskSourceCode: taskVM.TaskSourceCode);

            //Title
            var categoryName = _unitOfWork.WorkFlowRepository.GetWorkFlow(taskVM.WorkFlowId).WorkFlowName;
            ViewBag.Title = string.Format("{0}{1}\"{2}\"", LanguageResource.Btn_Create, ParentTaskId == null ? " " : " subtask ", categoryName.ToLower() == "subtask" ? "" : categoryName.FirstCharToUpper(false));
            #endregion

            //Summary
            var IsDisabledSummary = _context.WorkFlowModel.Where(p => p.WorkFlowId == taskVM.WorkFlowId)
                                            .Select(p => p.IsDisabledSummary).FirstOrDefault();
            if (IsDisabledSummary == true && Type != ConstWorkFlow.TTLSX)
            {
                taskVM.Summary = "Tự động cập nhật sau khi lưu";
            }
            return View(taskVM);
        }
        [HttpPost]
        [ValidateInput(false)] //need when using ckeditor, do not delete
        public JsonResult Create(TaskViewModel taskViewModel, List<HttpPostedFileBase> FileUrl, List<TaskAssignViewModel> taskAssignList, List<TaskAssignViewModel> taskAssignGroupList, List<TaskAssignViewModel> taskAssignPersonGroupList, List<TaskReporterViewModel> taskReporterList, string Type, bool? IsCreateNewTaskGroup, string GroupName)
        {
            return ExecuteContainer(() =>
            {
                var result = _unitOfWork.TaskRepository.CreateTask(taskViewModel, FileUrl, taskAssignList, taskAssignGroupList, taskAssignPersonGroupList, taskReporterList, Type, IsCreateNewTaskGroup, GroupName, CurrentUser, GoogleMapAPIKey);
                if (result.Id == null && result.redirectUrl == null)
                {
                    if (result.LData == null)
                    {
                        return Json(new
                        {
                            Code = result.Code,
                            Success = result.Success,
                            Data = result.Data,

                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Code = result.Code,
                            Success = result.Success,
                            Data = result.LData,

                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Code = result.Code,
                        Success = result.Success,
                        Data = result.Data,
                        RedirectUrl = result.redirectUrl,
                        Id = result.Id
                    });
                }
            });
        }
        #endregion

        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var task = _unitOfWork.TaskRepository.GetTaskInfo(id);
            if (task == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Information.ToLower()) });
            }

            //ý kiến khách hàng
            task.CustomerRatings = task.Property5;
            if (task.Property5 == "none")
            {
                task.Property5 = null;
            }
            else if (task.Property5 == "rating")
            {
                //task.Property5 = null;
                task.Ticket_CustomerReviews_Product = _context.RatingModel.Where(p => p.ReferenceId == task.TaskId && p.RatingTypeCode == ConstCatalogType.Ticket_CustomerReviews_Product).Select(p => p.Ratings).FirstOrDefault();
                task.Ticket_CustomerReviews_Service = _context.RatingModel.Where(p => p.ReferenceId == task.TaskId && p.RatingTypeCode == ConstCatalogType.Ticket_CustomerReviews_Service).Select(p => p.Ratings).FirstOrDefault();
                task.Property5 = _context.RatingModel.Where(p => p.ReferenceId == task.TaskId && p.RatingTypeCode == null).Select(p => p.Reviews).FirstOrDefault();
            }
            //else if (task.Property5 == "other")
            //{
            //    task.Property5 = _context.RatingModel.Where(p => p.ReferenceId == task.TaskId && p.RatingTypeCode == null).Select(p => p.Reviews).FirstOrDefault();
            //}

            ViewBag.TodoSubtask = task.TodoSubtask;
            ViewBag.ProcessingSubtask = task.ProcessingSubtask;
            ViewBag.CompletedSubtask = task.CompletedSubtask;

            //GenerateQRCode
            //var qrText = "<T1>" + task.TaskId + "</T1><T2>" + task.Summary + "</T2><T3>" + task.ParentTaskId + "</T3>";
            string qrFile = "";
            //Lấy tên hình ảnh QR Code
            var fileName = task.TaskCode + "/Task_" + task.TaskId;
            //Kiểm tra  đường dẫn lưu QR Code đã tồn tại chưa => Nếu chưa thì tạo đường dẫnn
            if (!Directory.Exists(Server.MapPath("/Upload/QRCode/" + task.TaskCode + "/")))
            {
                Directory.CreateDirectory(Server.MapPath("/Upload/QRCode/" + task.TaskCode + "/"));
            }
            //Kiểm tra file QR Code có tồn tại trong hệ thống chưa => Nếu có thì lấy hình ảnh gán vào biến qrFile để hiển thị.
            //if (System.IO.File.Exists(Server.MapPath("/Upload/QRCode/" + fileName + ".png")))
            //{
            //    //Gán Url file QR Code vào biến qrFile
            //    qrFile = "/Upload/QRCode/" + fileName + ".png";
            //}
            //else if (task.Barcode.HasValue)
            //{
            //    //Trong trường hợp chưa có QR Code mà đã có Barcode thì sẽ tạo QR Code tương ứng với Barcode
            //    qrFile = GenerateQRCode(fileName, Convert.ToString(task.Barcode.Value));
            //}
            ViewBag.QRCodeFileName = qrFile;

            #region ViewBag
            //Task Processing
            task.taskProcessingList = _unitOfWork.TaskRepository.GetTaskProcessingList(task.TaskId);
            if (task.taskProcessingList != null && task.taskProcessingList.Count > 0)
            {
                var maxOrderIndex = task.taskProcessingList.Where(p => p.LastEditTime != null)
                                        .OrderByDescending(p => p.OrderIndex).FirstOrDefault();
                if (maxOrderIndex == null)
                {
                    maxOrderIndex = new TaskProcessingViewModel() { OrderIndex = 10 };
                }
                ViewBag.MaxOrderIndex = maxOrderIndex.OrderIndex;
            }
            //Task config field
            List<string> hasRequestList = new List<string>();
            List<WorkFlowConfigModel> configList = new List<WorkFlowConfigModel>();
            List<SelectListItem> hasRequestRadioList = new List<SelectListItem>();
            var catList = _context.CatalogModel.Where(p => p.CatalogTypeCode == ConstCatalogType.HasRequest)
                                  .OrderBy(p => p.OrderIndex).ToList();
            var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(task.Type);
            if (listWorkFlow != null && listWorkFlow.Count > 0)
            {
                configList = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == task.WorkFlowId).ToList();
                if (configList != null && configList.Count > 0)
                {
                    foreach (var item in configList)
                    {
                        if (string.IsNullOrEmpty(item.Note))
                        {
                            item.Note = PropertyHelper.GetDisplayNameByString<TaskViewModel>(item.FieldCode);
                        }
                        //Yêu cầu cần xử lý: YC/NO_YC
                        if (item.FieldCode == ConstCatalogType.HasRequest && !string.IsNullOrEmpty(item.Parameters))
                        {
                            if (item.Parameters.Contains(","))
                            {
                                hasRequestList = item.Parameters.Split(',').ToList();
                                foreach (var para in hasRequestList)
                                {
                                    var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                    hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                                }
                            }
                            else
                            {
                                var para = hasRequestList.FirstOrDefault();
                                var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                            }
                        }
                        //Set default value
                        if (!string.IsNullOrEmpty(item.EditDefaultValue))
                        {
                            var value = task.GetType().GetProperty(item.FieldCode).GetValue(task, null);
                            if (value == null)
                            {
                                var type = task.GetType().GetProperty(item.FieldCode).PropertyType;
                                task.GetType().GetProperty(item.FieldCode).SetValue(task, PropertyHelper.ChangeType(item.EditDefaultValue, type));
                            }
                        }
                    }
                }
            }
            var defaultValue = _unitOfWork.TaskStatusRepository.GetCategoryByTaskStatus(task.TaskStatusId);
            ViewBag.HasRequest = new SelectList(hasRequestRadioList, "Value", "Text", defaultValue);
            ViewBag.WorkFlowConfig = configList;
            ViewBag.WorkFlowConfigCode = configList.Select(p => p.FieldCode).ToList();
            ViewBag.WorkFlowHiddenField = configList.Where(p => p.HideWhenEdit == true).Select(p => p.FieldCode).ToList();
            ViewBag.WorkFlowField = _context.WorkFlowFieldModel.ToList();

            //Title
            //var categoryName = _unitOfWork.WorkFlowRepository.GetWorkFlowCategory(task.Type);
            var categoryName = _unitOfWork.WorkFlowRepository.GetWorkFlow(task.WorkFlowId).WorkFlowName;
            ViewBag.Title = LanguageResource.Update + " " + categoryName.ToLower();

            //return type list of subtask based on WorkFlowId
            var typeList = _unitOfWork.WorkFlowRepository.GetTypeByParentWorkFlow(task.WorkFlowId);
            ViewBag.SubtaskType = typeList;

            ViewBag.SubtaskParentTaskId = task.TaskId;

            if (task.Property3 != null)
            {
                var numberProperty3 = task.Property3.FormatCurrency().Replace(",", ".");
                ViewBag.NumberProperty3 = numberProperty3;
            }
            //Nếu không là Ghé thăm
            if (task.WorkFlowCode != ConstWorkFlow.GT)
            {
                //Hài lòng khách hàng
                var customerSatisfaction = _context.RatingModel.Where(p => p.ReferenceId == task.TaskId && p.RatingTypeCode == ConstCatalogType.CustomerSatisfaction)
                                                               .FirstOrDefault();
                if (customerSatisfaction != null)
                {
                    task.CustomerSatisfactionCode = customerSatisfaction.Ratings;
                    task.CustomerSatisfactionReviews = customerSatisfaction.Reviews;
                }
                //NV kinh doanh của đơn vị thi công
                var SaleSupervisorList = (from p in _context.PersonInChargeModel
                                          join s in _context.SalesEmployeeModel on p.SalesEmployeeCode equals s.SalesEmployeeCode
                                          join acc in _context.AccountModel on s.SalesEmployeeCode equals acc.EmployeeCode
                                          from r in acc.RolesModel
                                          where p.ProfileId == task.ConstructionUnit
                                          && p.CompanyCode == CurrentUser.CompanyCode
                                          select new
                                          {
                                              SalesEmployeeCode = p.SalesEmployeeCode,
                                              SalesEmployeeName = s.SalesEmployeeName,
                                              DepartmentName = r.RolesName,
                                              isEmployeeGroup = r.isEmployeeGroup
                                          }).ToList();

                if (SaleSupervisorList != null && SaleSupervisorList.Count > 0)
                {
                    var SaleSupervisor = SaleSupervisorList.Where(p => p.isEmployeeGroup == true).FirstOrDefault();
                    if (SaleSupervisor == null)
                    {
                        SaleSupervisor = SaleSupervisorList.FirstOrDefault();
                    }
                    task.Construction_SalesSupervisorName = SaleSupervisor.SalesEmployeeName;
                }

                CreateViewBag(PriorityCode: task.PriorityCode,
                              WorkFlowId: task.WorkFlowId,
                              CommonMistakeCode: task.CommonMistakeCode,
                              StoreId: task.StoreId,
                              TaskStatusId: task.TaskStatusId,
                              Reporter: task.Reporter,
                              ProfileId: task.ProfileId,
                              ContactId: task.ContactId,
                              isEditMode: true,
                              Type: task.Type,
                              ProcessCode: task.ProcessCode,
                              WorkFlowName: task.WorkFlowName,
                              TaskId: task.TaskId,
                              TaskCode: task.TaskCode,
                              ServiceTechnicalTeamCode: task.ServiceTechnicalTeamCode,
                              ErrorTypeCode: task.ErrorTypeCode,
                              ErrorCode: task.ErrorCode,
                              VisitTypeCode: task.VisitTypeCode,
                              VisitSaleOfficeCode: task.VisitSaleOfficeCode,
                              RemindCycle: task.RemindCycle,
                              TaskSourceCode: task.Property4,
                              Category: defaultValue,
                              CustomerSatisfactionCode: task.CustomerSatisfactionCode,
                              SubtaskCode: task.SubtaskCode,
                              ConstructionUnit: task.ConstructionUnit,
                              ConstructionUnitContact: task.ConstructionUnitContact,
                              CustomerRatings: task.CustomerRatings,
                              Ticket_CustomerReviews_Product: task.Ticket_CustomerReviews_Product,
                              Ticket_CustomerReviews_Service: task.Ticket_CustomerReviews_Service,
                              VisitPlace: task.VisitPlace);
            }
            //Nếu là Ghé thăm
            else if (task.WorkFlowCode == ConstWorkFlow.GT)
            {
                CreateViewBag(
                    StoreId: task.StoreId,
                    ShowroomCode: task.ShowroomCode,
                    PriorityCode: task.PriorityCode,
                    WorkFlowId: task.WorkFlowId,
                    TaskStatusId: task.TaskStatusId,
                    Reporter: task.Reporter,
                    CustomerClassCode: task.CustomerClassCode,
                    CategoryCode: task.CategoryCode,
                    ChannelCode: task.ChannelCode,
                    SaleEmployeeCode: task.SaleEmployeeCode,
                    ProfileId: task.ProfileId,
                    ContactId: task.ContactId,
                    isEditMode: true,
                    Type: task.Type,
                    TaskId: task.TaskId,
                    ProcessCode: task.ProcessCode,
                    WorkFlowName: task.WorkFlowName,
                    TaskCode: task.TaskCode,
                    Ratings: task.Ratings);
            }
            //Permission to edit IsAssignGroup
            if (task.taskAssignList.Count == 0)
            {
                task.IsChangeAssignGroup = true;
            }
            ViewBag.IsChangeAssignGroup = task.IsChangeAssignGroup;

            //More phone
            ViewBag.MorePhone = _context.ProfilePhoneModel.Where(p => p.ProfileId == task.ProfileId).ToList();
            ViewBag.Id = id;
            if (task.Type == ConstWorkFlowCategory.BOOKING_VISIT || task.Type == ConstWorkFlowCategory.SUBTASK_BOOKINGVISIT)
            {
                ViewBag.Mode = "simple";
            }
            #endregion

            return View(task);
        }

        //Cập nhật loại xuất catalogue
        public ActionResult EditDeliveryType(Guid? DeliveryId, string DeliveryType)
        {
            return ExecuteContainer(() =>
            {
                var delivery = _context.DeliveryModel.Where(p => p.DeliveryId == DeliveryId).FirstOrDefault();
                if (delivery != null)
                {
                    delivery.DeliveryType = DeliveryType;
                    _context.Entry(delivery).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, "loại xuất catalogue"),
                });
            });
        }
        #endregion

        #region ViewBag
        private void CreateViewBag(string PriorityCode = "", Guid? WorkFlowId = null, string CommonMistakeCode = null, Guid? StoreId = null, string Reporter = null, Guid? ProfileId = null, Guid? TaskStatusId = null, string ShowroomCode = "", string CustomerClassCode = "", string CategoryCode = "", string ChannelCode = "", string SaleEmployeeCode = "", Guid? ContactId = null, bool? isEditMode = false, string Type = null, string Assignee = null, string ProcessCode = null, string WorkFlowName = null, int? TaskCode = null, string ServiceTechnicalTeamCode = null, string ErrorTypeCode = null, string ErrorCode = null, string VisitTypeCode = null, Guid? TaskId = null, string RemindCycle = null, string ProductLevelCode = null, string ProductColorCode = null, string UsualErrorCode = null, string ProductCategoryCode = null, bool? IsEditProduct = null, string TaskSourceCode = null, string Category = null, bool? isInPopup = null, string VisitSaleOfficeCode = null, string Unit = null, string AccErrorTypeCode = null, string Ratings = null, string CustomerSatisfactionCode = null, string SubtaskCode = null, Guid? ConstructionUnit = null, Guid? ConstructionUnitContact = null, List<string> UsualErrorCodeList = null, string CustomerRatings = null, string Ticket_CustomerReviews_Product = null, string Ticket_CustomerReviews_Service = null, string VisitPlace = null)
        {
            //Type: Loại (WorkflowCategoryCode)
            ViewBag.Type = Type;
            //GoogleMapAPIKey
            ViewBag.GoogleMapAPIKey = GoogleMapAPIKey;

            string WorkFlowCode = string.Empty;
            string NewWorkFlowName = string.Empty;
            bool? IsDisabledSummary = false;
            if (WorkFlowId == null)
            {
                WorkFlowId = Guid.Empty;
            }
            else
            {
                var workflow = _context.WorkFlowModel.Where(p => p.WorkFlowId == WorkFlowId).FirstOrDefault();
                if (workflow != null)
                {
                    WorkFlowCode = workflow.WorkFlowCode;
                    NewWorkFlowName = workflow.WorkFlowName;
                    IsDisabledSummary = workflow.IsDisabledSummary;
                }
            }
            ViewBag.IsDisabledSummary = IsDisabledSummary;

            #region Task
            //Priority
            var listPriority = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Priority);
            ViewBag.PriorityCode = new SelectList(listPriority, "CatalogCode", "CatalogText_vi", PriorityCode);

            //WorkFlow
            //Harcode: Nếu không phải là Activities hoặc Task Lấy tất cả all
            if (Type == ConstWorkFlowCategory.MyWork || Type == ConstWorkFlowCategory.MyFollow)
            {
                Type = ConstWorkFlowCategory.ALL;
            }
            var listWorkFlow = new List<WorkFlowViewModel>();
            //Nếu là thêm mới hoặc tìm kiếm => không load theo công ty
            if (isEditMode != true)
            {
                listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(Type);
                listWorkFlow = listWorkFlow.Where(p => p.WorkFlowCode != ConstWorkFlow.GT).ToList();
            }
            //Nếu là cập nhật => load theo công ty đang login
            else if (isEditMode == true)
            {
                listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(Type, CurrentUser.CompanyCode);
                if (listWorkFlow == null || listWorkFlow.Count == 0)
                {
                    listWorkFlow = new List<WorkFlowViewModel>();
                }
                //Nếu list workflow không có workflow hiện tại của task => add thêm
                var existWorkflowIdList = listWorkFlow.Select(p => p.WorkFlowId).ToList();
                if (WorkFlowId != null && WorkFlowId != Guid.Empty && !existWorkflowIdList.Contains((Guid)WorkFlowId))
                {
                    listWorkFlow.Add(new WorkFlowViewModel()
                    {
                        WorkFlowId = (Guid)WorkFlowId,
                        WorkFlowCode = WorkFlowCode,
                        WorkFlowName = NewWorkFlowName
                    });
                }
            }
            ViewBag.WorkFlowIdList = new SelectList(listWorkFlow, "WorkFlowId", "WorkFlowName", WorkFlowId);

            //TaskStatusId
            if (WorkFlowId == null || WorkFlowId == Guid.Empty)
            {
                if (listWorkFlow != null && listWorkFlow.Count > 0)
                {
                    WorkFlowId = listWorkFlow[0].WorkFlowId;
                }
            }
            //danh sách trạng thái
            var lst = _unitOfWork.TaskStatusRepository.GetTaskStatusByWorkFlow((Guid)WorkFlowId, Category);
            var TaskStatusId_GT = (from p in _context.TaskStatusModel
                                   join w in _context.WorkFlowModel on p.WorkFlowId equals w.WorkFlowId
                                   where w.WorkFlowCode == ConstWorkFlow.GT
                                   && p.TaskStatusCode == ConstWorkFlow.GT
                                   select p.TaskStatusId).FirstOrDefault();

            //cập nhật
            if (isEditMode == true && TaskStatusId != TaskStatusId_GT)
            {
                var taskProcessingList = _unitOfWork.TaskRepository.GetTaskProcessingList((Guid)TaskId);
                var existWorkFlowId = _context.TaskModel.Where(p => p.TaskId == TaskId).Select(p => p.WorkFlowId).FirstOrDefault();
                if (taskProcessingList != null && taskProcessingList.Count > 0 && existWorkFlowId == WorkFlowId)
                {
                    var taskStatusList = taskProcessingList.Select(p => p.TaskStatusId).ToList();
                    lst = lst.Where(p => taskStatusList.Contains(p.TaskStatusId)).ToList();
                }
            }
            //thêm mới
            else
            {
                if (TaskStatusId == null && WorkFlowId != null && WorkFlowId != Guid.Empty)
                {
                    var configTaskStatusId = _unitOfWork.TaskStatusRepository.GetDefaultValue((Guid)WorkFlowId, "TaskStatusId");
                    if (!string.IsNullOrEmpty(configTaskStatusId))
                    {
                        TaskStatusId = new Guid(configTaskStatusId);
                    }
                }

                ViewBag.DefaultTaskStatusId = TaskStatusId;
            }
            ViewBag.TaskStatusIdList = new SelectList(lst, "TaskStatusId", "TaskStatusName", TaskStatusId);
            ViewBag.TaskStatusId = new SelectList(lst, "TaskStatusId", "TaskStatusName", TaskStatusId);


            //CommonMistakeCode
            var commonMistakeList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonMistake);
            ViewBag.CommonMistakeCode = new SelectList(commonMistakeList, "CatalogCode", "CatalogText_vi", CommonMistakeCode);

            //StoreId: get all store
            var storeList = _unitOfWork.StoreRepository.GetAllStore();
            ViewBag.StoreIdList = new SelectList(storeList, "StoreId", "StoreName", StoreId);

            //CustomerType
            var customerTypeList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CustomerType);
            customerTypeList = customerTypeList.Where(p => p.CatalogCode != ConstCustomerType.Contact).ToList();
            ViewBag.CustomerTypeCode = new SelectList(customerTypeList, "CatalogCode", "CatalogText_vi");

            //Employee
            var empLst = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist();
            ViewBag.EmployeeList = empLst;

            if (isEditMode == false)
            {
                var currentUser = _unitOfWork.SalesEmployeeRepository.GetSaleEmployeeCodeBy(CurrentUser.AccountId);
                ViewBag.EmployeeCode = currentUser;
            }
            //Role: Task Assign Type
            ViewBag.RoleList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.TaskAssignType);

            //Task Roles List (Phân công cho nhóm/phòng ban)
            var TaskRolesList = _unitOfWork.AccountRepository.GetRolesList(isEmployeeGroup: true);
            //Nếu có tạo nhóm ngoài hệ thống thì lấy thêm thông tin
            //Lấy tất cả thông tin nhóm được tạo từ CurrentUser
            //var group = _context.TaskGroupModel.Where(p => p.CreatedAccountId == CurrentUser.AccountId)
            //                                    .Select(p => new RolesViewModel()
            //                                    {
            //                                        RolesCode = p.GroupId.ToString(),
            //                                        RolesName = p.GroupName,
            //                                    }).ToList();
            //if (group != null && group.Count > 0)
            //{
            //    int groupIndex = 0;
            //    foreach (var item in group)
            //    {
            //        TaskRolesList.Insert(groupIndex, item);
            //        groupIndex++;
            //    }
            //}
            //ViewBag.TaskRolesList = TaskRolesList;
            //Reporter
            ViewBag.Reporter = Reporter;
            ViewBag.ReporterList = new SelectList(empLst, "SalesEmployeeCode", "SalesEmployeeName", Reporter);

            //Assignee
            ViewBag.Assignee = Assignee;
            ViewBag.AssigneeList = new SelectList(empLst, "SalesEmployeeCode", "SalesEmployeeName", Assignee);

            //CreateBy
            ViewBag.CreateBy = new SelectList(empLst, "SalesEmployeeCode", "SalesEmployeeName");

            //Profile
            //var profiles = _unitOfWork.ProfileRepository.GetProfiles();
            //ViewBag.ProfileId = new SelectList(profiles, "ProfileId", "ProfileName");

            //Contact
            if (ProfileId != null)
            {
                var contacts = _unitOfWork.ProfileRepository.GetContactListOfProfile(ProfileId);
                ViewBag.ContactId = new SelectList(contacts, "ProfileContactId", "ProfileContactName", ContactId);
            }
            else
            {
                var contacts = new List<ProfileContactViewModel>();
                ViewBag.ContactId = new SelectList(contacts, "ProfileId", "ProfileName");
            }
            //Construction unit
            if (ConstructionUnit != null)
            {
                var contacts = (from p in _context.ProfileContactAttributeModel
                                join pro in _context.ProfileModel on p.ProfileId equals pro.ProfileId
                                where p.CompanyId == ConstructionUnit
                                select new ConstructionUnitContactViewModel
                                {
                                    ContactId = pro.ProfileId,
                                    ContactName = pro.ProfileShortName != null ? pro.ProfileShortName : pro.ProfileName,
                                    IsMain = p.IsMain
                                }).OrderByDescending(p => p.IsMain == true).ToList();
                ViewBag.ConstructionUnitContact = new SelectList(contacts, "ContactId", "ContactName", ConstructionUnitContact);
            }
            else
            {
                var contacts = new List<ConstructionUnitContactViewModel>();
                ViewBag.ConstructionUnitContact = new SelectList(contacts, "ContactId", "ContactName");
            }

            //Task Status Color
            string TaskStatusBackgroundColor = "";
            string TaskStatusColor = "";
            //Todo
            if (ProcessCode == ConstProcess.todo)
            {
                TaskStatusBackgroundColor = "#fff";
                TaskStatusColor = "#000";
            }
            //Processing
            else if (ProcessCode == ConstProcess.processing)
            {
                TaskStatusBackgroundColor = "#0052CC";
                TaskStatusColor = "#fff";
            }
            //Completed
            else if (ProcessCode == ConstProcess.completed)
            {
                TaskStatusBackgroundColor = "#398439";
                TaskStatusColor = "#fff";
            }
            ViewBag.TaskStatusBackgroundColor = TaskStatusBackgroundColor;
            ViewBag.TaskStatusColor = TaskStatusColor;

            //Title on modal popup
            if (!string.IsNullOrEmpty(WorkFlowName) && TaskCode != null)
            {
                string editUrl = string.Empty;
                if (isInPopup == true)
                {
                    editUrl = string.Format("<a href='/Work/Task/Edit/{0}'><i class='fa fa-eye mr-5'></i></a> ", TaskId);
                }
                string CommentTitle = editUrl;
                if (ProfileId != null)
                {
                    var profileName = _unitOfWork.TaskRepository.GetProfileByTask(TaskCode).ProfileName;
                    if (!string.IsNullOrEmpty(profileName))
                    {
                        CommentTitle += profileName + "-";
                    }
                }
                CommentTitle += string.Format("{0}-{1}", WorkFlowName.Trim(), !string.IsNullOrEmpty(SubtaskCode) ? SubtaskCode : TaskCode.ToString());
                ViewBag.CommentTitle = CommentTitle;
            }

            //Name on logo
            var logoName = (from p in _context.SalesEmployeeModel
                            where p.SalesEmployeeCode == CurrentUser.EmployeeCode
                            select p.SalesEmployeeName).FirstOrDefault();
            logoName = logoName.GetCharacterForLogoName();
            ViewBag.LogoName = logoName;

            //ServiceTechnicalTeamCode
            var serviceLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ServiceTechnicalTeam);
            ViewBag.ServiceTechnicalTeamCode = new SelectList(serviceLst, "CatalogCode", "CatalogText_vi", ServiceTechnicalTeamCode);

            //ErrorTypeCode
            var errorTypeLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ErrorType);
            ViewBag.ErrorTypeCode = new SelectList(errorTypeLst, "CatalogCode", "CatalogText_vi", ErrorTypeCode);

            //ErrorCode
            var errorLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Error);
            ViewBag.ErrorCode = new SelectList(errorLst, "CatalogCode", "CatalogText_vi", ErrorCode);

            //ErrorCode
            //var accErrorTypeLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ErrorType);
            // ViewBag.AccErrorTypeCode = new SelectList(errorTypeLst, "CatalogCode", "CatalogText_vi", AccErrorTypeCode);

            //VisitTypeCode
            var visitTypeLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.VisitType);
            ViewBag.VisitTypeCode = new SelectList(visitTypeLst, "CatalogCode", "CatalogText_vi", VisitTypeCode);

            //VisitSaleOfficeCode
            var visitSaleOfficeLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.SaleOffice);
            ViewBag.VisitSaleOfficeCode = new SelectList(visitSaleOfficeLst, "CatalogCode", "CatalogText_vi", VisitSaleOfficeCode);

            //RemindCycle
            var remindCycleLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.RemindCycle);
            ViewBag.RemindCycle = new SelectList(remindCycleLst, "CatalogCode", "CatalogText_vi", RemindCycle);

            //Mã màu SP
            var productColorLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ProductColor);
            ViewBag.ProductColorCode = new SelectList(productColorLst, "CatalogCode", "CatalogCode", ProductColorCode);

            var config = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == WorkFlowId && p.FieldCode == "ProductCategoryCode")
                                     .Select(p => p.Parameters).FirstOrDefault();
            //Phân loại vật tư
            var PhanLoaiVatTu = _context.CategoryModel.Where(p => p.CategoryCode == ConstCategoryCode.PHANLOAIVATTU).FirstOrDefault();
            if (PhanLoaiVatTu != null)
            {
                var parentCategoryList = _context.CategoryModel.Where(p => p.ParentCategoryId == PhanLoaiVatTu.CategoryId && p.Actived == true)
                                                        .Select(p => new
                                                        {
                                                            CategoryId = p.CategoryId,
                                                            CategoryCode = p.CategoryCode,
                                                            CategoryName = p.CategoryCode + " | " + p.CategoryName,
                                                            OrderIndex = p.OrderIndex
                                                        })
                                                        .OrderBy(p => p.CategoryCode).ToList();
                ViewBag.ParentCategoryId = new SelectList(parentCategoryList, "CategoryId", "CategoryName");
                ViewBag.SearchParentCategoryId = new SelectList(parentCategoryList, "CategoryId", "CategoryName");

                //Nhóm vật tư
                var NhomVatTu = _context.CategoryModel.Where(p => p.CategoryCode == ConstCategoryCode.NHOMVATTU).FirstOrDefault();
                var categoryList = _context.CategoryModel.Where(p => p.ParentCategoryId == NhomVatTu.CategoryId && p.Actived == true)
                                                             .Select(p => new
                                                             {
                                                                 CategoryId = p.CategoryId,
                                                                 CategoryCode = p.CategoryCode,
                                                                 CategoryName = p.CategoryCode + " | " + p.CategoryName,
                                                                 OrderIndex = p.OrderIndex
                                                             })
                                                             .OrderBy(p => p.CategoryCode).ToList();

                ViewBag.CategoryId = new SelectList(categoryList, "CategoryId", "CategoryName");
                ViewBag.SearchCategoryId = new SelectList(categoryList, "CategoryId", "CategoryName");
            }
            
         
            var productSearchCategoryLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ProductCategory);
            if (IsEditProduct == true && !string.IsNullOrEmpty(config))
            {
                var productCategoryLst = productSearchCategoryLst.Where(p => p.CatalogText_en.Contains(config)).ToList();
                ViewBag.ProductCategoryCode = new SelectList(productCategoryLst, "CatalogCode", "CatalogText_vi", ProductCategoryCode);
            }
            else
            {
                ViewBag.ProductCategoryCode = new SelectList(productSearchCategoryLst, "CatalogCode", "CatalogText_vi", ProductCategoryCode);
            }

            //Các lỗi bảo hành thuờng gặp
            if (IsEditProduct == true && !string.IsNullOrEmpty(ProductCategoryCode))
            {
                var usualErrorLst = _unitOfWork.TaskRepository.GetUsualErrorByProductCategory(ProductCategoryCode, config);
                //ViewBag.UsualErrorCode = new SelectList(usualErrorLst, "CatalogCode", "CatalogText_vi", UsualErrorCodeList);
                List<SelectListItem> usualErrorSelectList = new List<SelectListItem>();
                foreach (var item in usualErrorLst)
                {
                    var i = new SelectListItem()
                    {
                        Text = item.CatalogText_vi,
                        Value = item.CatalogCode.ToString(),

                    };
                    if (UsualErrorCodeList != null)
                    {
                        i.Selected = UsualErrorCodeList.Contains(item.CatalogCode);
                    }
                    usualErrorSelectList.Add(i);
                }
                ViewBag.UsualErrorCode = usualErrorSelectList;
                ViewBag.IsEditProduct = IsEditProduct;
            }
            else
            {
                var usualErrorLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.UsualError);
                ViewBag.UsualErrorCode = new SelectList(usualErrorLst, "CatalogCode", "CatalogText_vi");
            }
            //Nguồn tiếp nhận (TaskSourceCode)
            var taskSourceList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.TaskSource)
                                                                .Select(p => new SelectListItem()
                                                                {
                                                                    Text = p.CatalogText_vi,
                                                                    Value = p.CatalogCode,
                                                                });
            ViewBag.TaskSourceCode = new SelectList(taskSourceList, "Value", "Text", TaskSourceCode);

            //Hài lòng khách hàng
            var customerSatisfactionLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CustomerSatisfaction)
                                                                .Select(p => new SelectListItem()
                                                                {
                                                                    Text = p.CatalogText_vi,
                                                                    Value = p.CatalogCode,
                                                                });
            ViewBag.CustomerSatisfactionCode = new SelectList(customerSatisfactionLst, "Value", "Text", CustomerSatisfactionCode);

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
            //customerRatings.Add(new SelectListItem()
            //{
            //    Value = "other",
            //    Text = "Khác"
            //});
            ViewBag.CustomerRatings = new SelectList(customerRatings, "Value", "Text", CustomerRatings);
            //Đánh giá theo sao
            //1. Về sản phẩm
            var ticket_CustomerReviews_Product = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Ticket_CustomerReviews_Product)
                                                                            .Select(p => new SelectListItem()
                                                                            {
                                                                                Value = p.CatalogCode,
                                                                                Text = p.CatalogText_vi,
                                                                            });
            ViewBag.Ticket_CustomerReviews_Product = new SelectList(ticket_CustomerReviews_Product, "Value", "Text", Ticket_CustomerReviews_Product);
            //2. Về dịch vụ
            var ticket_CustomerReviews_Service = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Ticket_CustomerReviews_Service)
                                                                          .Select(p => new SelectListItem()
                                                                          {
                                                                              Value = p.CatalogCode,
                                                                              Text = p.CatalogText_vi,
                                                                          });
            ViewBag.Ticket_CustomerReviews_Service = new SelectList(ticket_CustomerReviews_Service, "Value", "Text", Ticket_CustomerReviews_Service);

            //Nơi tham quan
            var visitPlaceList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.VisitPlace);
            ViewBag.VisitPlace = new SelectList(visitPlaceList, "CatalogCode", "CatalogText_vi", VisitPlace);
            #endregion

            #region Appointment
            if (isEditMode == true && !string.IsNullOrEmpty(WorkFlowCode) && WorkFlowCode == ConstWorkFlow.GT)
            {
                //ShowRoom
                var showroomList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CustomerSource);
                ViewBag.ShowroomCode = new SelectList(showroomList, "CatalogCode", "CatalogText_vi", ShowroomCode);

                //Phân loại khách hàng
                var customerClassList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CustomerClass);
                ViewBag.CustomerClassCode = new SelectList(customerClassList, "CatalogCode", "CatalogText_vi", CustomerClassCode);
                ViewBag.CustomerClassCodeList = new SelectList(customerClassList, "CatalogCode", "CatalogText_vi", CustomerClassCode);

                //Danh mục
                var appointmentCategoryList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Appoitment_Category);
                ViewBag.CategoryCode = new SelectList(appointmentCategoryList, "CatalogCode", "CatalogText_vi", CategoryCode);
                ViewBag.CategoryCodeList = new SelectList(appointmentCategoryList, "CatalogCode", "CatalogText_vi", CategoryCode);

                //Khách biết đến ACN qua
                var channelList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Appoitment_Channel);
                ViewBag.ChannelCode = new SelectList(channelList, "CatalogCode", "CatalogText_vi", ChannelCode);

                //Người tiếp khách
                var _salesEmployeeRepository = new SalesEmployeeRepository(_context);
                var saleEmployeeList = _salesEmployeeRepository.GetAllForDropdownlist();
                ViewBag.SaleEmployeeCode = new SelectList(saleEmployeeList, "SalesEmployeeCode", "SalesEmployeeName", SaleEmployeeCode);

                //Ý kiến khách hàng
                var ratingLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CustomerReviews);
                ViewBag.Ratings = new SelectList(ratingLst, "CatalogCode", "CatalogText_vi", Ratings);
            }
            #endregion

            #region Product
            if (isEditMode == true)
            {
                //Sản phẩm
                var ProductList = _context.ProductModel.Where(p => p.Actived == true).Select(p => new
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    OrderIndex = p.OrderIndex
                }).OrderBy(p => p.OrderIndex);
                ViewBag.ProductId = new SelectList(ProductList, "ProductId", "ProductName");



                //Loại phụ kiện
                var productAccessoryTypeCode = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ProductAccessoryType);
                //ViewBag.ProductAccessoryTypeCode = new SelectList(productAccessoryTypeCode, "CatalogCode", "CatalogText_vi");
                ViewBag.ProductAccessoryTypeCode = productAccessoryTypeCode;

                //Hình thức BH PK
                ViewBag.AccErrorTypeCode = errorTypeLst;

                //Phân cấp SP
                var productLevelLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ProductLevel);
                ViewBag.ProductLevelCode = new SelectList(productLevelLst, "CatalogCode", "CatalogText_vi", ProductLevelCode);

                //ĐVT
                var unitLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Unit);
                ViewBag.Unit = new SelectList(unitLst, "CatalogCode", "CatalogText_vi", Unit);
            }
            #endregion        
            //isCreatePrivateTask
            ViewBag.isCreatePrivateTask = _context.AccountModel.Where(p => p.AccountId == CurrentUser.AccountId).Select(p => p.isCreatePrivateTask).FirstOrDefault();

        }

        private void CreateSearchViewBag(string Type, string TaskProcessCode = null)
        {
            #region CommonDate
            var SelectedCommonDate = "Custom";
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);

            //Common Date 2
            var commonDateList2 = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate2);
            ViewBag.CommonDate2 = new SelectList(commonDateList2, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            #endregion

            #region //TaskStatusCode - Trạng thái
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

            #region //Kanban
            var kanban = _context.KanbanModel.Where(p => p.KanbanCode == Type && p.Actived == true).FirstOrDefault();
            if (kanban != null)
            {
                ViewBag.KanbanId = kanban.KanbanId;
            }
            #endregion

            #region //TaskProcessCode 
            var statusLst = _unitOfWork.TaskStatusRepository.GetTaskStatusList();
            ViewBag.TaskProcessCode = new SelectList(statusLst, "StatusCode", "StatusName", TaskProcessCode);
            #endregion

            #region //Get list CustomerGroup (Nhóm khách hàng doanh nghiệp)
            var customerGroupList = _unitOfWork.CatalogRepository.GetCustomerCategory(CurrentUser.CompanyCode);
            ViewBag.ProfileGroupCode = new SelectList(customerGroupList, "CatalogCode", "CatalogText_vi");
            #endregion

            #region //Get list SalesSupervisor (NV kinh doanh)
            var empList = _unitOfWork.PersonInChargeRepository.GetListEmployee();
            ViewBag.SalesSupervisorCode = new SelectList(empList, "SalesEmployeeCode", "SalesEmployeeName");
            ViewBag.CompletedEmployee = new SelectList(empList, "SalesEmployeeCode", "SalesEmployeeName");
            #endregion

            #region //Get list Roles (Phòng ban)
            var rolesList = _context.RolesModel.Where(p => p.Actived == true && p.isEmployeeGroup == true).ToList();
            ViewBag.DepartmentCode = new SelectList(rolesList, "RolesCode", "RolesName");
            #endregion

            #region //Actived (Trạng thái hoạt động)
            var activedList = new List<ISDSelectBoolItem>();
            activedList.Add(new ISDSelectBoolItem()
            {
                id = null,
                name = "Tất cả",
            }); activedList.Add(new ISDSelectBoolItem()
            {
                id = true,
                name = "Hoạt động",
            });
            activedList.Add(new ISDSelectBoolItem()
            {
                id = false,
                name = "Đã hủy",
            });
            ViewBag.Actived = new SelectList(activedList, "id", "name");
            #endregion

            #region //Get list ServiceTechnicalTeamCode (Trung tâm bảo hành)
            var serviceTechnicalTeamCodeList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ServiceTechnicalTeam);
            ViewBag.ServiceTechnicalTeamCode = new SelectList(serviceTechnicalTeamCodeList, "CatalogCode", "CatalogText_vi");
            #endregion

            #region //Get list Province (Tỉnh/Thành phố)
            var _provinceRepository = new ProvinceRepository(_context);
            var provinceList = _provinceRepository.GetAll();
            ViewBag.ProvinceId = new SelectList(provinceList, "ProvinceId", "ProvinceName");
            ViewBag.ProvinceIdSearchList = new SelectList(provinceList, "ProvinceId", "ProvinceName");
            #endregion

            //VisitTypeCode
            var visitTypeLst = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.VisitType);
            ViewBag.VisitTypeCode = new SelectList(visitTypeLst, "CatalogCode", "CatalogText_vi");

            #region //Filters
            var filterLst = new List<DropdownlistFilter>();
            /*
            //Khách hàng
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.WorkFlowId, FilterName = LanguageResource.Type });
            //Liên hệ
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ContactId, FilterName = LanguageResource.Profile_Contact });
            //Người tạo
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.CreateBy, FilterName = LanguageResource.CreateBy });
            //Mức độ
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.PriorityCode, FilterName = LanguageResource.Task_PriorityCode });
            //Ngày tạo
            if (Type == ConstWorkFlowCategory.TICKET_MLC)
            {
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.CreateDate, FilterName = LanguageResource.CreatedDate });
            }
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
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ProductCategoryCode, FilterName = LanguageResource.Sale_Category });
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
            //Trạng thái hoạt động
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.Actived, FilterName = "Trạng thái hoạt động" });
            //Trung tâm bảo hành
            if (Type == ConstWorkFlowCategory.TICKET_MLC)
            {
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ServiceTechnicalTeamCode, FilterName = LanguageResource.Task_ServiceTechnicalTeamCode });
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.CompletedEmployee, FilterName = LanguageResource.CompletedEmployee });
            }
            //Phân loại chuyến thăm
            if (Type == ConstWorkFlowCategory.THKH)
            {
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.VisitTypeCode, FilterName = LanguageResource.Task_VisitTypeCode });
            }
            //ĐTB
            if (Type == ConstWorkFlowCategory.GTB)
            {
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.ProvinceId, FilterName = LanguageResource.Profile_ProvinceId });
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.DistrictId, FilterName = LanguageResource.Profile_DistrictId });
                filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.WardId, FilterName = LanguageResource.WardId });
            }
            */

            //Ngày bắt đầu
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.StartDate, FilterName = LanguageResource.Task_StartDate });
            //Lệnh sản xuất SAP
            filterLst.Add(new DropdownlistFilter { FilterCode = ConstFilter.LSXC, FilterName = LanguageResource.LSXSAP });
            ViewBag.Filters = filterLst;
            #endregion
        }
        #endregion

        #region Helper
        /// <summary>
        /// Lấy danh sách nhân viên được tạo từ nhóm ngoài hệ thống
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        public ActionResult GetExternalGroup(Guid? GroupId)
        {
            var lst = (from tg in _context.TaskGroupDetailModel
                       join acc in _context.AccountModel on tg.AccountId equals acc.AccountId
                       join emp in _context.SalesEmployeeModel on acc.EmployeeCode equals emp.SalesEmployeeCode
                       where tg.GroupId == GroupId
                       select new ISDSelectStringItem()
                       {
                           id = acc.EmployeeCode,
                           name = emp.SalesEmployeeName,
                       }).ToList();
            return Json(lst);
        }

        /// <summary>
        /// Lấy danh sách liên hệ theo khách hàng
        /// </summary>
        /// <param name="ProfileId"></param>
        /// <returns></returns>
        public ActionResult GetContactBy(Guid? ProfileId)
        {
            var lst = _unitOfWork.ProfileRepository.GetContactListOfProfile(ProfileId);
            return Json(lst);
        }

        /// <summary>
        /// Lấy thông tin NV kinh doanh, liên hệ, SĐT liên hệ, email
        /// </summary>
        /// <param name="ConstructionUnit"></param>
        /// <returns></returns>
        public ActionResult GetConstructionUnitInfo(Guid? ConstructionUnit)
        {
            ConstructionUnitInfoViewModel result = new ConstructionUnitInfoViewModel();
            //1. Lấy thông tin NV kinh doanh
            var SaleSupervisorList = (from p in _context.PersonInChargeModel
                                      join s in _context.SalesEmployeeModel on p.SalesEmployeeCode equals s.SalesEmployeeCode
                                      join acc in _context.AccountModel on s.SalesEmployeeCode equals acc.EmployeeCode
                                      from r in acc.RolesModel
                                      where p.ProfileId == ConstructionUnit
                                      && p.CompanyCode == CurrentUser.CompanyCode
                                      select new
                                      {
                                          SalesEmployeeCode = p.SalesEmployeeCode,
                                          SalesEmployeeName = s.SalesEmployeeName,
                                          DepartmentName = r.RolesName,
                                          isEmployeeGroup = r.isEmployeeGroup
                                      }).ToList();

            if (SaleSupervisorList != null && SaleSupervisorList.Count > 0)
            {
                var SaleSupervisor = SaleSupervisorList.Where(p => p.isEmployeeGroup == true).FirstOrDefault();
                if (SaleSupervisor == null)
                {
                    SaleSupervisor = SaleSupervisorList.FirstOrDefault();
                }
                result.SalesSupervisorCode = SaleSupervisor.SalesEmployeeCode;
                result.SalesSupervisorName = SaleSupervisor.SalesEmployeeName;
            }

            //2. Lấy danh sách liên hệ 
            var contactList = (from p in _context.ProfileContactAttributeModel
                               join pro in _context.ProfileModel on p.ProfileId equals pro.ProfileId
                               where p.CompanyId == ConstructionUnit
                               select new ConstructionUnitContactViewModel
                               {
                                   ContactId = pro.ProfileId,
                                   ContactName = pro.ProfileShortName != null ? pro.ProfileShortName : pro.ProfileName,
                                   IsMain = p.IsMain
                               }).OrderByDescending(p => p.IsMain == true).ToList();

            if (contactList != null && contactList.Count > 0)
            {
                result.ContactList = new List<ConstructionUnitContactViewModel>();
                result.ContactList = contactList;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContactInfoBy(Guid? ContactId)
        {
            var result = (from p in _context.ProfileContactAttributeModel
                          join pro in _context.ProfileModel on p.ProfileId equals pro.ProfileId
                          where pro.ProfileId == ContactId
                          select new
                          {
                              ContactPhone = pro.Phone,
                              ContactEmail = pro.Email,
                          }).FirstOrDefault();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Lấy thông tin Địa chỉ, SĐT, Email khi chọn khách hàng/liên hệ
        /// Hàm sử dụng chung cho lấy thông tin khách hàng + liên hệ
        /// Nếu là Khách hàng: lấy địa chỉ, SĐT liên hệ, Email và NV kinh doanh theo khách hàng (Account)
        /// Nếu là Liên hệ: lấy địa chỉ và NV kinh doanh theo khách hàng (Account), SĐT liên hệ, Email theo liên hệ (Contact)
        /// Nếu là Đơn vị thi công: lấy NV kinh doanh theo khách hàng (Account)
        /// </summary>
        /// <param name="ContactId"></param>
        /// <param name="ProfileId"></param>
        /// <returns></returns>
        public ActionResult GetAddressByContact(Guid? ContactId, Guid? TaskId = null)
        {
            TaskContactViewModel task = new TaskContactViewModel();
            var contact = (from p in _context.ProfileModel
                               //Province
                           join pr in _context.ProvinceModel on p.ProvinceId equals pr.ProvinceId into prG
                           from province in prG.DefaultIfEmpty()
                               //District
                           join d in _context.DistrictModel on p.DistrictId equals d.DistrictId into dG
                           from district in dG.DefaultIfEmpty()
                               //Ward
                           join w in _context.WardModel on p.WardId equals w.WardId into wG
                           from ward in wG.DefaultIfEmpty()
                           where p.ProfileId == ContactId
                           select new
                           {
                               ContactId = p.ProfileId,
                               CustomerTypeCode = p.CustomerTypeCode,
                               ProfileAddress = p.Address,
                               ProvinceName = province == null ? "" : ", " + province.ProvinceName,
                               DistrictName = district == null ? "" : ", " + district.Appellation + " " + district.DistrictName,
                               WardName = ward == null ? "" : ", " + ward.Appellation + " " + ward.WardName,
                               //Phone = p.SAPPhone ?? p.Phone,
                               Phone = p.Phone ?? p.SAPPhone,
                               ContactShortName = p.ProfileCode + " | " + (p.ProfileShortName != null ? p.ProfileShortName : p.ProfileName),
                               Email = p.Email,
                           }).FirstOrDefault();

            List<string> AddressList = new List<string>();
            if (contact != null)
            {
                //Nếu ContactId đang là khách hàng => lấy địa chỉ, SĐT liên hệ, Email và NV kinh doanh theo khách hàng (Account)
                //Nếu ContactId đang là liên hệ => lấy địa chỉ và NV kinh doanh theo khách hàng(Account) của liên hệ đó (Contact)
                var profileInTask = contact.ContactId;
                var taskInDb = _context.TaskModel.Where(p => p.TaskId == TaskId).FirstOrDefault();
                if (taskInDb != null)
                {
                    if (taskInDb.ProfileId != ContactId && taskInDb.ProfileId != null && contact.CustomerTypeCode == ConstProfileType.Contact)
                    {
                        profileInTask = (Guid)taskInDb.ProfileId;
                    }
                }
                //Address list
                var addressList = _unitOfWork.AddressBookRepository.GetAll(profileInTask);
                if (addressList != null && addressList.Count > 0)
                {
                    foreach (var item in addressList)
                    {
                        item.Address += item.WardName + item.DistrictName + item.ProvinceName;
                        AddressList.Add(item.Address);
                    }
                }
                var SaleSupervisorList = (from p in _context.PersonInChargeModel
                                          join s in _context.SalesEmployeeModel on p.SalesEmployeeCode equals s.SalesEmployeeCode
                                          join acc in _context.AccountModel on s.SalesEmployeeCode equals acc.EmployeeCode
                                          from r in acc.RolesModel
                                          where p.ProfileId == profileInTask
                                          && p.CompanyCode == CurrentUser.CompanyCode
                                          select new
                                          {
                                              SalesEmployeeCode = p.SalesEmployeeCode,
                                              SalesEmployeeName = s.SalesEmployeeName,
                                              DepartmentName = r.RolesName,
                                              isEmployeeGroup = r.isEmployeeGroup
                                          }).ToList();
                if (SaleSupervisorList != null && SaleSupervisorList.Count > 0)
                {
                    var SaleSupervisor = SaleSupervisorList.Where(p => p.isEmployeeGroup == true).FirstOrDefault();
                    if (SaleSupervisor == null)
                    {
                        SaleSupervisor = SaleSupervisorList.FirstOrDefault();
                    }
                    task.SalesSupervisorCode = SaleSupervisor.SalesEmployeeCode;
                    task.SalesSupervisorName = SaleSupervisor.SalesEmployeeName;
                    task.DepartmentName = SaleSupervisor.isEmployeeGroup == true ? SaleSupervisor.DepartmentName : "";
                }
                //Khách hàng của liên hệ
                var profileId = _unitOfWork.ProfileRepository.GetProfileByContact(contact.ContactId);
                task.ProfileId = profileId;
                task.ContactShortName = contact.ContactShortName;
                task.ContactAddress = string.Format("{0}{1}{2}{3}", contact.ProfileAddress, contact.WardName, contact.DistrictName, contact.ProvinceName);
                task.ContactPhone = contact.Phone;
                task.ContactEmail = contact.Email;
                task.AddressList = AddressList;

                //Thông tin người liên hệ chính (nếu có)
                var mainContactList = (from p in _context.ProfileContactAttributeModel
                                       join pro in _context.ProfileModel on p.ProfileId equals pro.ProfileId
                                       where p.CompanyId == ContactId
                                       select new
                                       {
                                           MainContactName = pro.ProfileShortName != null ? pro.ProfileShortName : pro.ProfileName,
                                           MainContactPhone = pro.Phone,
                                           MainContactEmail = pro.Email,
                                           IsMain = p.IsMain
                                       }).ToList();
                if (mainContactList != null && mainContactList.Count > 0)
                {
                    var mainContact = mainContactList.Where(p => p.IsMain == true).FirstOrDefault();
                    if (mainContact == null)
                    {
                        mainContact = mainContactList.FirstOrDefault();
                    }
                    task.MainContactName = mainContact.MainContactName;
                    task.MainContactPhone = mainContact.MainContactPhone != null ? mainContact.MainContactPhone : task.ContactPhone;
                    task.MainContactEmail = mainContact.MainContactEmail != null ? mainContact.MainContactEmail : task.ContactEmail;
                }
                else
                {
                    task.MainContactPhone = task.ContactPhone;
                    task.MainContactEmail = task.ContactEmail;
                }

                var taskAddress = _context.TaskModel.Where(p => p.TaskId == TaskId).Select(p => p.ProfileAddress).FirstOrDefault();
                if (!string.IsNullOrEmpty(taskAddress))
                {
                    task.ExistProfileAddress = taskAddress;
                }
            }
            return Json(task, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hiển thị hình ảnh workflow đi kèm ở dropdownlist Loại (WorkFlowId)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetWorkflowImage(Guid id)
        {
            string image = string.Empty;
            var workflow = _unitOfWork.WorkFlowRepository.GetWorkFlow(id);
            if (workflow != null)
            {
                image = workflow.ImageUrl;
            }
            return Json(image, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lưu file đính kèm
        /// </summary>
        /// <param name="ObjectId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private FileAttachmentModel SaveFileAttachment(Guid ObjectId, HttpPostedFileBase item)
        {
            //FileAttachmentModel
            var fileNew = new FileAttachmentModel();
            //1. GUID
            fileNew.FileAttachmentId = Guid.NewGuid();
            //2. Mã Profile
            fileNew.ObjectId = ObjectId;
            //3. Tên file
            fileNew.FileAttachmentName = item.FileName;
            //4. Đường dẫn
            fileNew.FileUrl = UploadDocumentFile(item, "Document");
            //5. Đuôi file
            fileNew.FileExtention = _unitOfWork.UtilitiesRepository.FileExtension(item.FileName);
            //7. Loại file
            fileNew.FileAttachmentCode = _unitOfWork.UtilitiesRepository.GetFileTypeByExtension(fileNew.FileExtention);
            //7. Người tạo
            fileNew.CreateBy = CurrentUser.AccountId;
            //8. Thời gian tạo
            fileNew.CreateTime = DateTime.Now;
            _context.Entry(fileNew).State = EntityState.Added;
            return fileNew;
        }

        /// <summary>
        /// Load lại partial khi cập nhật comment/đính kèm
        /// </summary>
        /// <param name="TaskId"></param>
        /// <param name="isOnTab">Nếu cập nhật file đính kèm ở Tab "Đính kèm" sẽ load lại partial ở tab đó</param>
        /// <returns></returns>
        public ActionResult GetTaskCommentList(Guid TaskId, bool? isOnTab)
        {
            var task = _unitOfWork.TaskRepository.GetTaskInfo(TaskId);
            if (task == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Information.ToLower()) });
            }
            ViewBag.TodoSubtask = task.TodoSubtask;
            ViewBag.ProcessingSubtask = task.ProcessingSubtask;
            ViewBag.CompletedSubtask = task.CompletedSubtask;
            //Nếu không là Ghé thăm
            if (task.WorkFlowCode != ConstWorkFlow.GT)
            {
                CreateViewBag(PriorityCode: task.PriorityCode, WorkFlowId: task.WorkFlowId, CommonMistakeCode: task.CommonMistakeCode, StoreId: task.StoreId, TaskStatusId: task.TaskStatusId, Reporter: task.Reporter, ProfileId: task.ProfileId, ContactId: task.ContactId, isEditMode: true, Type: task.Type, ProcessCode: task.ProcessCode, WorkFlowName: task.WorkFlowName, TaskCode: task.TaskCode, ServiceTechnicalTeamCode: task.ServiceTechnicalTeamCode, ErrorTypeCode: task.ErrorTypeCode, ErrorCode: task.ErrorCode, VisitTypeCode: task.VisitTypeCode, VisitSaleOfficeCode: task.VisitSaleOfficeCode, RemindCycle: task.RemindCycle, TaskSourceCode: task.Property4, TaskId: task.TaskId);
            }
            //Nếu là Ghé thăm
            else if (task.WorkFlowCode == ConstWorkFlow.GT)
            {
                CreateViewBag(StoreId: task.StoreId, ShowroomCode: task.ShowroomCode, PriorityCode: task.PriorityCode, WorkFlowId: task.WorkFlowId, TaskStatusId: task.TaskStatusId, Reporter: task.Reporter, CustomerClassCode: task.CustomerClassCode, CategoryCode: task.CategoryCode, ChannelCode: task.ChannelCode, SaleEmployeeCode: task.SaleEmployeeCode, ProfileId: task.ProfileId, ContactId: task.ContactId, isEditMode: true, Type: task.Type, ProcessCode: task.ProcessCode, WorkFlowName: task.WorkFlowName, TaskCode: task.TaskCode, TaskId: task.TaskId);
            }
            if (isOnTab == true)
            {
                return PartialView("_ListImage", task.taskFileList);
            }
            return PartialView("_ListComment", task);
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

            if (IsTakeAll == true)
            {
                errorList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.UsualError);
            }
            else
            {
                var config = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == WorkFlowId).ToList();
                var fieldCodeList = config.Select(p => p.FieldCode).ToList();
                if (fieldCodeList.Contains(PropertyHelper.GetPropertyName<TaskProductViewModel, string>(p => p.UsualErrorCode)))
                {
                    if (string.IsNullOrEmpty(ProductCategoryCode))
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
            }
            return Json(errorList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách theo trạng thái theo nhiệm vụ và danh mục
        /// </summary>
        /// <param name="WorkFlowId"></param>
        /// <param name="Category"></param>
        /// <returns></returns>
        public ActionResult GetTaskStatusList(Guid? WorkFlowId, string Category)
        {
            var lst = _unitOfWork.TaskStatusRepository.GetTaskStatusByWorkFlow((Guid)WorkFlowId, Category);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy đơn vị tính theo sản phẩm
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public ActionResult GetProductUnitById(Guid? ProductId)
        {
            var Unit = (from p in _context.ProductAttributeModel
                        where p.ProductId == ProductId
                        select p.Unit).FirstOrDefault();
            return Json(Unit, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTaskById(Guid id)
        {
            var result = _unitOfWork.TaskRepository.GetTaskById(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy khu vực theo địa chỉ khách hàng
        /// </summary>
        /// <param name="ProfileId"></param>
        /// <returns></returns>
        public ActionResult GetSaleOfficeByAddress(Guid? ProfileId)
        {
            var area = _context.ProfileModel.Where(p => p.ProfileId == ProfileId).Select(p => p.SaleOfficeCode).FirstOrDefault();
            return Json(area, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Push notification
        /// </summary>
        /// <param name="AccountId"></param>
        /// <param name="TaskId"></param>
        /// <param name="notificationMessage"></param>
        /// <param name="task"></param>
        private void PushNotification(Guid AccountId, Guid TaskId, string notificationMessage, TaskModel task)
        {
            //get device list except current user
            string[] deviceLst = new string[] { };
            string taskCode = string.Empty;
            List<Guid> accountLst = new List<Guid>();
            var deviceIdList = new List<string>();
            var workFlow = _context.WorkFlowModel.Where(p => p.WorkFlowId == task.WorkFlowId).FirstOrDefault();
            if (workFlow != null)
            {
                taskCode = workFlow.WorkFlowCode + "." + task.TaskCode;
            }

            var accountDeviceLst = _context.Account_Device_Mapping.Where(p => p.AccountId != AccountId).ToList();

            //assignee
            var assigneeList = _context.TaskAssignModel.Where(p => p.TaskId == task.TaskId).Select(p => new TaskAssignViewModel()
            {
                SalesEmployeeCode = p.SalesEmployeeCode
            }).ToList();

            if ((assigneeList == null || assigneeList.Count == 0) && task.IsAssignGroup == true)
            {
                assigneeList = (from a in _context.TaskModel
                                join b in _context.TaskAssignModel on a.TaskId equals b.TaskId
                                where a.ParentTaskId == TaskId
                                select new TaskAssignViewModel()
                                {
                                    SalesEmployeeCode = b.SalesEmployeeCode
                                }).ToList();
            }
            if (assigneeList != null && assigneeList.Count > 0)
            {
                ////Nếu phân công cho nhóm thì gửi thông báo đến tất cả nhân viên thuộc nhóm đó
                //if (task.IsAssignGroup == true)
                //{
                //    var rolesCodeLst = assigneeList.Select(p => p.RolesCode).ToList();
                //    var accountList = (from a in _context.AccountModel
                //                       from r in a.RolesModel
                //                       where rolesCodeLst.Contains(r.RolesCode)
                //                       select a.AccountId
                //                      ).ToList();
                //    if (accountList != null && accountList.Count > 0)
                //    {
                //        var deviceAccountLst = accountDeviceLst.Where(p => accountList.Contains(p.AccountId)).Select(p => p.DeviceId).Distinct().ToList();
                //        if (deviceAccountLst != null && deviceAccountLst.Count > 0)
                //        {
                //            deviceIdList.AddRange(deviceAccountLst);
                //        }

                //        accountLst.AddRange(accountList);
                //    }
                //}
                //else
                //{
                //    foreach (var taskAssign in assigneeList)
                //    {
                //        var account = _context.AccountModel.Where(p => p.EmployeeCode == taskAssign.SalesEmployeeCode).FirstOrDefault();
                //        if (account != null)
                //        {
                //            var deviceAccountLst = accountDeviceLst.Where(p => p.AccountId == account.AccountId).Select(p => p.DeviceId).ToList();
                //            if (deviceAccountLst != null && deviceAccountLst.Count > 0)
                //            {
                //                deviceIdList.AddRange(deviceAccountLst);
                //                accountLst.Add(account.AccountId);
                //            }
                //        }
                //    }
                //}
                //Cập nhật => nếu phân công cho nhóm => chọn nhân viên phân công => chỉ cần gửi cho danh sách nhân viên được phân công
                foreach (var taskAssign in assigneeList)
                {
                    var account = _context.AccountModel.Where(p => p.EmployeeCode == taskAssign.SalesEmployeeCode).FirstOrDefault();
                    if (account != null)
                    {
                        var deviceAccountLst = accountDeviceLst.Where(p => p.AccountId == account.AccountId).Select(p => p.DeviceId).ToList();
                        if (deviceAccountLst != null && deviceAccountLst.Count > 0)
                        {
                            deviceIdList.AddRange(deviceAccountLst);
                            accountLst.Add(account.AccountId);
                        }
                    }
                }
            }

            //reporter
            var accountReporter = _context.AccountModel.Where(p => p.EmployeeCode == task.Reporter && p.AccountId != AccountId).FirstOrDefault();
            if (accountReporter != null)
            {
                var deviceAccountLst = accountDeviceLst.Where(p => p.AccountId == accountReporter.AccountId).Select(p => p.DeviceId).ToList();
                if (deviceAccountLst != null && deviceAccountLst.Count > 0)
                {
                    deviceIdList.AddRange(deviceAccountLst);
                    accountLst.Add(accountReporter.AccountId);
                }
            }

            //reporter list
            var taskReporterLst = _context.TaskReporterModel.Where(p => p.TaskId == TaskId).ToList();
            if (taskReporterLst != null && taskReporterLst.Count > 0)
            {
                foreach (var taskReporter in taskReporterLst)
                {
                    var account = _context.AccountModel.Where(p => p.EmployeeCode == taskReporter.SalesEmployeeCode).FirstOrDefault();
                    if (account != null)
                    {
                        var deviceAccountLst = accountDeviceLst.Where(p => p.AccountId == account.AccountId).Select(p => p.DeviceId).ToList();
                        if (deviceAccountLst != null && deviceAccountLst.Count > 0)
                        {
                            deviceIdList.AddRange(deviceAccountLst);
                            accountLst.Add(account.AccountId);
                        }
                    }
                }
            }

            //salessupervisorcode (NV kinh doanh)
            if (!string.IsNullOrEmpty(task.SalesSupervisorCode))
            {
                var accountSalesSupervisorCode = _context.AccountModel.Where(p => p.EmployeeCode == task.SalesSupervisorCode && p.AccountId != AccountId).FirstOrDefault();
                if (accountSalesSupervisorCode != null)
                {
                    var deviceAccountLst = accountDeviceLst.Where(p => p.AccountId == accountSalesSupervisorCode.AccountId).Select(p => p.DeviceId).ToList();
                    if (deviceAccountLst != null && deviceAccountLst.Count > 0)
                    {
                        deviceIdList.AddRange(deviceAccountLst);
                        accountLst.Add(accountSalesSupervisorCode.AccountId);
                    }
                }
            }

            if (deviceIdList != null && deviceIdList.Count > 0)
            {
                deviceLst = deviceIdList.Distinct().ToArray();
            }

            //push notification
            string summary = task.Summary;
            _unitOfWork.TaskRepository.PushNotification(TaskId, notificationMessage, deviceLst, summary, accountLst.Distinct().ToList());
            _context.SaveChanges();
        }

        /// <summary>
        /// Lấy thông tin khách hàng theo Id
        /// </summary>
        /// <param name="ProfileId"></param>
        /// <returns></returns>
        public ActionResult GetProfileInfo(Guid? ProfileId)
        {
            var profile = (from p in _context.ProfileModel
                               //Contact
                           join contact in _context.ProfileContactAttributeModel on p.ProfileId equals contact.CompanyId into cg
                           from c in cg.DefaultIfEmpty()
                               //Contact
                           join profiles in _context.ProfileModel on c.ProfileId equals profiles.ProfileId into pg
                           from cont in pg.DefaultIfEmpty()
                               //Province
                           join pr in _context.ProvinceModel on p.ProvinceId equals pr.ProvinceId into prG
                           from province in prG.DefaultIfEmpty()
                               //District
                           join d in _context.DistrictModel on p.DistrictId equals d.DistrictId into dG
                           from district in dG.DefaultIfEmpty()
                               //ProfileType
                           join type in _context.ProfileTypeModel on new { ProfileId = p.ProfileId, CompanyCode = CurrentUser.CompanyCode } equals new { ProfileId = (Guid)type.ProfileId, CompanyCode = type.CompanyCode } into tg
                           from pt in tg.DefaultIfEmpty()
                           where p.ProfileId == ProfileId
                           select new ProfileViewModel
                           {
                               ProfileId = p.ProfileId,
                               ProfileCode = p.ProfileCode,
                               ProfileForeignCode = p.ProfileForeignCode,
                               ProfileName = p.ProfileName,
                               ContactName = cont.ProfileName,
                               CustomerTypeCode = pt.CustomerTypeCode,
                               Phone = p.Phone,
                               Email = p.Email,
                               Address = p.Address,
                               ProvinceName = province == null ? "" : ", " + province.ProvinceName,
                               DistrictName = district == null ? "" : ", " + district.Appellation + " " + district.DistrictName
                           })
                           .FirstOrDefault();
            if (profile != null)
            {
                profile.Address += profile.DistrictName + profile.ProvinceName;
            }
            return Json(profile, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy thông tin 3 lần thăm hỏi gần nhất của khách hàng
        /// </summary>
        /// <param name="ProfileId"></param>
        /// <returns></returns>
        public ActionResult GetTHKHBy(Guid? ProfileId)
        {
            //Lấy loại thăm hỏi khách hàng
            var workflowIdLst = _context.WorkFlowModel.Where(p => p.WorkflowCategoryCode == ConstWorkFlowCategory.THKH).Select(p => p.WorkFlowId).ToList();
            //Lấy danh sách 3 lần thăm hỏi gần nhất của khách hàng
            var taskSummary = _context.TaskModel.Where(p => workflowIdLst.Contains(p.WorkFlowId) && p.ProfileId == ProfileId)
                                                .OrderByDescending(p => p.EndDate)
                                                .Select(p => new
                                                {
                                                    p.TaskId,
                                                    p.Summary
                                                })
                                                .Take(3).ToList();
            return Json(taskSummary, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Load partial when change WorkFlow
        /// <summary>
        /// Load lại partial Thêm mới khi thay đổi Loại (WorkFlowId)
        /// </summary>
        /// <param name="taskVM"></param>
        /// <param name="taskAssignList"></param>
        /// <returns></returns>
        [ValidateInput(false)] //need when using ckeditor, do not delete
        public ActionResult LoadFormByWorkFlow_Create(TaskViewModel taskVM, List<TaskAssignViewModel> taskAssignList)
        {
            var saleOrgCode = CurrentUser.SaleOrg;
            var storeId = _unitOfWork.StoreRepository.GetStoreIdBySaleOrgCode(saleOrgCode);

            var workflow = _context.WorkFlowModel.FirstOrDefault(p => p.WorkFlowId == taskVM.WorkFlowId);
            taskVM.WorkFlowCode = workflow.WorkFlowCode;
            //Nếu là loại "Thăm hỏi khách hàng" => mặc định có yêu cầu checkin
            if (taskVM.Type == ConstWorkFlowCategory.THKH)
            {
                taskVM.isRequiredCheckin = true;
            }

            #region ParentType/ Lấy parent type của type subtask
            var parentType = (from p in _context.TaskModel
                              join w in _context.WorkFlowModel on p.WorkFlowId equals w.WorkFlowId
                              where p.TaskId == taskVM.ParentTaskId
                              select w.WorkflowCategoryCode).FirstOrDefault();
            ViewBag.ParentType = parentType;
            #endregion

            #region Summary/ Tiêu đề
            var IsDisabledSummary = _context.WorkFlowModel.Where(p => p.WorkFlowId == taskVM.WorkFlowId)
                                            .Select(p => p.IsDisabledSummary).FirstOrDefault();
            if (IsDisabledSummary == true)
            {
                taskVM.Summary = "Tự động cập nhật sau khi lưu";
            }
            else
            {
                if (taskVM.Summary == "Tự động cập nhật sau khi lưu")
                {
                    taskVM.Summary = string.Empty;
                }
            }
            #endregion

            #region SupervisorCode/ NV kinh doanh, ProfileAddress/ Địa chỉ KH
            if (taskVM.ProfileId != null)
            {
                //SupervisorCode
                var SaleSupervisor = (from pic in _context.PersonInChargeModel
                                      join s in _context.SalesEmployeeModel on pic.SalesEmployeeCode equals s.SalesEmployeeCode
                                      join acc in _context.AccountModel on s.SalesEmployeeCode equals acc.EmployeeCode
                                      from r in acc.RolesModel
                                      where pic.ProfileId == taskVM.ProfileId
                                      && pic.CompanyCode == CurrentUser.CompanyCode
                                      select new SalesSupervisorViewModel()
                                      {
                                          SalesSupervisorCode = pic.SalesEmployeeCode,
                                          SalesSupervisorName = s.SalesEmployeeName,
                                          DepartmentName = r.isEmployeeGroup == true ? r.RolesName : ""
                                      }).FirstOrDefault();
                if (SaleSupervisor != null)
                {
                    taskVM.SalesSupervisorCode = SaleSupervisor.SalesSupervisorCode;
                    taskVM.SalesSupervisorName = SaleSupervisor.SalesSupervisorName;
                    taskVM.DepartmentName = SaleSupervisor.DepartmentName;
                }

                //ProfileAddress
                List<string> AddressList = new List<string>();
                var addressList = _unitOfWork.AddressBookRepository.GetAll(taskVM.ProfileId);
                if (addressList != null && addressList.Count > 0)
                {
                    foreach (var item in addressList)
                    {
                        if (item != null)
                        {
                            item.Address += item.WardName + item.DistrictName + item.ProvinceName;
                            AddressList.Add(item.Address);
                        }
                    }
                }
                ViewBag.AddressList = new SelectList(AddressList, taskVM.ProfileAddress);
            }
            #endregion

            #region Assignee/ NV được phân công
            taskVM.taskAssignList = taskAssignList;
            if (taskVM.taskAssignList != null && taskVM.taskAssignList.Count > 0)
            {
                foreach (var item in taskVM.taskAssignList)
                {
                    var role = (from acc in _context.AccountModel
                                from r in acc.RolesModel
                                where acc.EmployeeCode == item.SalesEmployeeCode
                                && r.isEmployeeGroup == true
                                select r.RolesName).FirstOrDefault();
                    item.RoleName = role != null ? role : "";
                }
            }
            #endregion

            #region ViewBag
            //Task config field
            List<string> hasRequestList = new List<string>();
            List<SelectListItem> hasRequestRadioList = new List<SelectListItem>();
            var catList = _context.CatalogModel.Where(p => p.CatalogTypeCode == ConstCatalogType.HasRequest)
                                  .OrderBy(p => p.OrderIndex).ToList();

            var configList = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == taskVM.WorkFlowId).ToList();
            if (configList != null && configList.Count > 0)
            {
                foreach (var item in configList)
                {
                    if (string.IsNullOrEmpty(item.Note))
                    {
                        item.Note = PropertyHelper.GetDisplayNameByString<TaskViewModel>(item.FieldCode);
                    }
                    //Yêu cầu cần xử lý: YC/NO_YC
                    if (item.FieldCode == ConstCatalogType.HasRequest && !string.IsNullOrEmpty(item.Parameters))
                    {
                        if (item.Parameters.Contains(","))
                        {
                            hasRequestList = item.Parameters.Split(',').ToList();
                            foreach (var para in hasRequestList)
                            {
                                var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                            }
                        }
                        else
                        {
                            var para = hasRequestList.FirstOrDefault();
                            var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                            hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                        }
                    }
                    //Set default value
                    if (!string.IsNullOrEmpty(item.AddDefaultValue))
                    {
                        var type = taskVM.GetType().GetProperty(item.FieldCode).PropertyType;
                        taskVM.GetType().GetProperty(item.FieldCode).SetValue(taskVM, PropertyHelper.ChangeType(item.AddDefaultValue, type));
                    }
                }
            }
            var defaultValue = (catList != null && catList.Count > 0 && hasRequestList.Count > 0) ?
                                catList.FirstOrDefault(p => p.CatalogCode == "NO_YC").CatalogCode :
                                null;
            ViewBag.HasRequest = new SelectList(hasRequestRadioList, "Value", "Text", defaultValue);
            ViewBag.WorkFlowConfig = configList;
            ViewBag.WorkFlowConfigCode = configList.Select(p => p.FieldCode).ToList();
            ViewBag.WorkFlowHiddenField = configList.Where(p => p.HideWhenAdd == true).Select(p => p.FieldCode).ToList();

            CreateViewBag(PriorityCode: taskVM.PriorityCode, StoreId: storeId, isEditMode: false, Type: taskVM.Type, Reporter: taskVM.Reporter, WorkFlowId: taskVM.WorkFlowId, RemindCycle: taskVM.RemindCycle, Category: defaultValue);

            if (taskVM.Property3 != null)
            {
                var numberProperty3 = taskVM.Property3.FormatCurrency().Replace(",", ".");
                ViewBag.NumberProperty3 = numberProperty3;
            }
            #endregion

            return PartialView("_CreateTaskInfo", taskVM);
        }

        /// <summary>
        /// Load lại partial Cập nhật khi thay đổi Loại (WorkFlowId)
        /// </summary>
        /// <param name="taskViewModel"></param>
        /// <param name="taskAssignList"></param>
        [ValidateInput(false)] //need when using ckeditor, do not delete
        public ActionResult LoadFormByWorkFlow_Edit(TaskViewModel taskViewModel, List<TaskAssignViewModel> taskAssignList)
        {
            var workflow = _unitOfWork.WorkFlowRepository.GetWorkFlow(taskViewModel.WorkFlowId);
            taskViewModel.WorkFlowCode = workflow.WorkFlowCode;

            #region TaskComment/ Bình luận
            var commentList = _unitOfWork.TaskRepository.GetTaskCommentList(taskViewModel.TaskId);
            taskViewModel.taskCommentList = commentList;
            taskViewModel.NumberOfComments = commentList.Count;
            #endregion

            #region Assignee/ NV được phân công
            taskViewModel.taskAssignList = taskAssignList;
            if (taskViewModel.taskAssignList != null && taskViewModel.taskAssignList.Count > 0)
            {
                foreach (var item in taskViewModel.taskAssignList)
                {
                    var role = (from acc in _context.AccountModel
                                from r in acc.RolesModel
                                where acc.EmployeeCode == item.SalesEmployeeCode
                                && r.isEmployeeGroup == true
                                select r.RolesName).FirstOrDefault();
                    item.RoleName = role != null ? role : "";
                }
            }
            #endregion

            #region Summary/ Tiêu đề
            var IsDisabledSummary = _context.WorkFlowModel.Where(p => p.WorkFlowId == taskViewModel.WorkFlowId)
                                            .Select(p => p.IsDisabledSummary).FirstOrDefault();
            if (IsDisabledSummary == true)
            {
                taskViewModel.Summary = _unitOfWork.TaskRepository.GetSummary(taskViewModel.WorkFlowId, taskViewModel.VisitTypeCode, taskViewModel.ReceiveDate, taskViewModel.StartDate, taskViewModel.ProfileId, taskViewModel.taskAssignList);
            }
            ViewBag.IsDisabledSummary = IsDisabledSummary;
            #endregion

            #region SupervisorCode/ NV kinh doanh, ProfileAddress/ Địa chỉ KH
            if (taskViewModel.ProfileId != null)
            {
                //SupervisorCode
                var SaleSupervisor = (from pic in _context.PersonInChargeModel
                                      join s in _context.SalesEmployeeModel on pic.SalesEmployeeCode equals s.SalesEmployeeCode
                                      join acc in _context.AccountModel on s.SalesEmployeeCode equals acc.EmployeeCode
                                      from r in acc.RolesModel
                                      where pic.ProfileId == taskViewModel.ProfileId
                                      && pic.CompanyCode == CurrentUser.CompanyCode
                                      select new SalesSupervisorViewModel()
                                      {
                                          SalesSupervisorCode = pic.SalesEmployeeCode,
                                          SalesSupervisorName = s.SalesEmployeeName,
                                          DepartmentName = r.isEmployeeGroup == true ? r.RolesName : ""
                                      }).FirstOrDefault();
                if (SaleSupervisor != null)
                {
                    taskViewModel.SalesSupervisorCode = SaleSupervisor.SalesSupervisorCode;
                    taskViewModel.SalesSupervisorName = SaleSupervisor.SalesSupervisorName;
                    taskViewModel.DepartmentName = SaleSupervisor.DepartmentName;
                }

                //ProfileAddress
                List<string> AddressList = new List<string>();
                var addressList = _unitOfWork.AddressBookRepository.GetAll(taskViewModel.ProfileId);
                if (addressList != null && addressList.Count > 0)
                {
                    foreach (var item in addressList)
                    {
                        if (item != null)
                        {
                            item.Address += item.WardName + item.DistrictName + item.ProvinceName;
                            AddressList.Add(item.Address);
                        }
                    }
                }
                ViewBag.AddressList = new SelectList(AddressList, taskViewModel.ProfileAddress);
            }
            #endregion

            #region Subtask
            taskViewModel.subtaskList = _unitOfWork.TaskRepository.GetSubtaskList(taskViewModel.TaskId);
            if (taskViewModel.subtaskList != null && taskViewModel.subtaskList.Count > 0)
            {
                var processCodeList = (from p in taskViewModel.subtaskList
                                       join ts in _context.TaskStatusModel on p.TaskStatusId equals ts.TaskStatusId
                                       select ts.ProcessCode.ToLower()).ToList();

                taskViewModel.TodoSubtask = processCodeList.Where(p => p == ConstProcess.todo).Count();
                taskViewModel.ProcessingSubtask = processCodeList.Where(p => p == ConstProcess.processing).Count();
                taskViewModel.CompletedSubtask = processCodeList.Where(p => p == ConstProcess.completed).Count();
            }
            #endregion

            #region ViewBag
            //Subtask
            ViewBag.TodoSubtask = taskViewModel.TodoSubtask;
            ViewBag.ProcessingSubtask = taskViewModel.ProcessingSubtask;
            ViewBag.CompletedSubtask = taskViewModel.CompletedSubtask;

            //Task config field
            List<string> hasRequestList = new List<string>();
            List<WorkFlowConfigModel> configList = new List<WorkFlowConfigModel>();
            List<SelectListItem> hasRequestRadioList = new List<SelectListItem>();
            var catList = _context.CatalogModel.Where(p => p.CatalogTypeCode == ConstCatalogType.HasRequest)
                                  .OrderBy(p => p.OrderIndex).ToList();
            var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(taskViewModel.Type);
            if (listWorkFlow != null && listWorkFlow.Count > 0)
            {
                configList = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == taskViewModel.WorkFlowId).ToList();
                if (configList != null && configList.Count > 0)
                {
                    foreach (var item in configList)
                    {
                        if (string.IsNullOrEmpty(item.Note))
                        {
                            item.Note = PropertyHelper.GetDisplayNameByString<TaskViewModel>(item.FieldCode);
                        }
                        //Yêu cầu cần xử lý: YC/NO_YC
                        if (item.FieldCode == ConstCatalogType.HasRequest && !string.IsNullOrEmpty(item.Parameters))
                        {
                            if (item.Parameters.Contains(","))
                            {
                                hasRequestList = item.Parameters.Split(',').ToList();
                                foreach (var para in hasRequestList)
                                {
                                    var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                    hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                                }
                            }
                            else
                            {
                                var para = hasRequestList.FirstOrDefault();
                                var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                            }
                        }
                        //Set default value
                        if (!string.IsNullOrEmpty(item.EditDefaultValue))
                        {
                            var value = taskViewModel.GetType().GetProperty(item.FieldCode).GetValue(taskViewModel, null);
                            if (value == null)
                            {
                                var type = taskViewModel.GetType().GetProperty(item.FieldCode).PropertyType;
                                taskViewModel.GetType().GetProperty(item.FieldCode).SetValue(taskViewModel, PropertyHelper.ChangeType(item.EditDefaultValue, type));
                            }
                        }
                    }
                }
            }
            var defaultValue = _unitOfWork.TaskStatusRepository.GetCategoryByTaskStatus(taskViewModel.TaskStatusId);
            ViewBag.HasRequest = new SelectList(hasRequestRadioList, "Value", "Text", defaultValue);
            ViewBag.WorkFlowConfig = configList;
            ViewBag.WorkFlowConfigCode = configList.Select(p => p.FieldCode).ToList();
            ViewBag.WorkFlowHiddenField = configList.Where(p => p.HideWhenEdit == true).Select(p => p.FieldCode).ToList();

            //return type list of subtask based on WorkFlowId
            var typeList = _unitOfWork.WorkFlowRepository.GetTypeByParentWorkFlow(taskViewModel.WorkFlowId);
            ViewBag.SubtaskType = typeList;
            ViewBag.SubtaskParentTaskId = taskViewModel.TaskId;
            if (taskViewModel.Property3 != null)
            {
                var numberProperty3 = taskViewModel.Property3.FormatCurrency().Replace(",", ".");
                ViewBag.NumberProperty3 = numberProperty3;
            }
            //Nếu không là Ghé thăm
            if (taskViewModel.WorkFlowCode != ConstWorkFlow.GT)
            {
                CreateViewBag(PriorityCode: taskViewModel.PriorityCode,
                              WorkFlowId: taskViewModel.WorkFlowId,
                              CommonMistakeCode: taskViewModel.CommonMistakeCode,
                              StoreId: taskViewModel.StoreId,
                              TaskStatusId: taskViewModel.TaskStatusId,
                              Reporter: taskViewModel.Reporter,
                              ProfileId: taskViewModel.ProfileId,
                              ContactId: taskViewModel.ContactId,
                              isEditMode: true,
                              Type: taskViewModel.Type,
                              TaskId: taskViewModel.TaskId,
                              ProcessCode: taskViewModel.ProcessCode,
                              WorkFlowName: taskViewModel.WorkFlowName,
                              TaskCode: taskViewModel.TaskCode,
                              ServiceTechnicalTeamCode: taskViewModel.ServiceTechnicalTeamCode,
                              ErrorTypeCode: taskViewModel.ErrorTypeCode,
                              ErrorCode: taskViewModel.ErrorCode,
                              VisitTypeCode: taskViewModel.VisitTypeCode,
                              VisitSaleOfficeCode: taskViewModel.VisitSaleOfficeCode,
                              RemindCycle: taskViewModel.RemindCycle,
                              TaskSourceCode: taskViewModel.Property4,
                              Category: defaultValue);
            }
            //Nếu là Ghé thăm
            else if (taskViewModel.WorkFlowCode == ConstWorkFlow.GT)
            {
                CreateViewBag(
                    StoreId: taskViewModel.StoreId,
                    ShowroomCode: taskViewModel.ShowroomCode,
                    PriorityCode: taskViewModel.PriorityCode,
                    WorkFlowId: taskViewModel.WorkFlowId,
                    TaskStatusId: taskViewModel.TaskStatusId,
                    Reporter: taskViewModel.Reporter,
                    CustomerClassCode: taskViewModel.CustomerClassCode,
                    CategoryCode: taskViewModel.CategoryCode,
                    ChannelCode: taskViewModel.ChannelCode,
                    SaleEmployeeCode: taskViewModel.SaleEmployeeCode,
                    ProfileId: taskViewModel.ProfileId,
                    ContactId: taskViewModel.ContactId,
                    isEditMode: true,
                    Type: taskViewModel.Type,
                    TaskId: taskViewModel.TaskId,
                    ProcessCode: taskViewModel.ProcessCode,
                    WorkFlowName: taskViewModel.WorkFlowName,
                    TaskCode: taskViewModel.TaskCode);
            }
            //Permission to edit IsAssignGroup
            var existTaskAssign = _context.TaskAssignModel.Where(p => p.TaskId == taskViewModel.TaskId).ToList();
            if (existTaskAssign.Count == 0)
            {
                taskViewModel.IsChangeAssignGroup = true;
            }
            ViewBag.IsChangeAssignGroup = taskViewModel.IsChangeAssignGroup;
            #endregion

            return PartialView("_TaskInfo2", taskViewModel);
        }
        #endregion

        #region Show task detail on modal
        public ActionResult _Edit(Guid TaskId, Guid? KanbanId, string NextColumnName, string PartialName = null)
        {
            //Type
            var type = _unitOfWork.TaskRepository.GetWorkflowCategoryCode(TaskId);

            TaskViewModel task = new TaskViewModel();

            task = _unitOfWork.TaskRepository.GetTaskById(TaskId);
            var workFlowCode = _unitOfWork.WorkFlowRepository.GetWorkFlow(task.WorkFlowId).WorkFlowCode;
            task.Type = workFlowCode;
            if (task != null)
            {
                //Task Status list
                #region Task Status
                //List task status theo workflow
                var statusLst = _context.TaskStatusModel
                                        .Where(p => p.WorkFlowId == task.WorkFlowId)
                                        .Select(p => p.TaskStatusId)
                                        .ToList();

                //Lấy các column kanban được cấu hình theo workflow
                var columns = (from p in _context.Kanban_TaskStatus_Mapping
                               join d in _context.KanbanDetailModel on p.KanbanDetailId equals d.KanbanDetailId
                               where statusLst.Contains(p.TaskStatusId)
                               && d.KanbanId == KanbanId
                               select d.ColumnName).Distinct().ToList();

                if (!string.IsNullOrEmpty(NextColumnName))
                {
                    var CurrentColumnName = (from p in _context.TaskModel
                                             join t in _context.TaskStatusModel on p.TaskStatusId equals t.TaskStatusId
                                             join m in _context.Kanban_TaskStatus_Mapping on t.TaskStatusId equals m.TaskStatusId
                                             join d in _context.KanbanDetailModel on m.KanbanDetailId equals d.KanbanDetailId
                                             where p.TaskId == TaskId
                                             && d.KanbanId == KanbanId
                                             select d.ColumnName).FirstOrDefault();

                    //Nếu kéo task chỉ ở column hiện tại, không kéo sang column khác thì vẫn hiển thị thông tin task hiện tại
                    //Nếu kéo sang bước khác => Lưu task status mới
                    if (CurrentColumnName != NextColumnName)
                    {
                        if (!columns.Contains(NextColumnName))
                        {
                            var mess = type == ConstWorkFlowCategory.TICKET ? LanguageResource.UncomfortableColumn_Ticket : LanguageResource.UncomfortableColumn_Activities;
                            return Json(new
                            {
                                Code = HttpStatusCode.NotModified,
                                Success = false,
                                Data = mess
                            });
                        }
                        else
                        {
                            task.TaskStatusId = (from p in _context.Kanban_TaskStatus_Mapping
                                                 join d in _context.KanbanDetailModel on p.KanbanDetailId equals d.KanbanDetailId
                                                 join st in _context.TaskStatusModel on p.TaskStatusId equals st.TaskStatusId
                                                 where d.ColumnName == NextColumnName
                                                 && d.KanbanId == KanbanId
                                                 && st.WorkFlowId == task.WorkFlowId
                                                 select st.TaskStatusId).FirstOrDefault();
                        }
                        //Nếu move task sang column đích => Hiển thị dropdown Status tiếp theo
                        var KanbanDetailId = (from t in _context.TaskStatusModel
                                              join m in _context.Kanban_TaskStatus_Mapping on t.TaskStatusId equals m.TaskStatusId
                                              join d in _context.KanbanDetailModel on m.KanbanDetailId equals d.KanbanDetailId
                                              where d.KanbanId == KanbanId
                                              && t.TaskStatusId == task.TaskStatusId
                                              select d.KanbanDetailId).FirstOrDefault();

                        var taskStatusList = _unitOfWork.TaskStatusRepository.GetNextTaskStatus(task.WorkFlowId, task.TaskStatusId, KanbanDetailId);
                        ViewBag.ToStatus = new SelectList(taskStatusList, "TaskStatusId", "TaskStatusName");

                        //Nếu khi di chuyển task qua bước tiếp theo => Lưu task status mới
                        task = _unitOfWork.TaskStatusRepository.SaveTaskStatus(task, taskStatusList, (Guid)KanbanId, NextColumnName, CurrentUser.AccountId);
                    }
                    else
                    {
                        //Nếu click vào task => Hiển thị Status hiện tại
                        var lst = _unitOfWork.TaskStatusRepository.GetTaskStatusByWorkFlow(task.WorkFlowId);
                        ViewBag.ToStatus = new SelectList(lst, "TaskStatusId", "TaskStatusName", task.TaskStatusId);
                    }
                }
                else
                {
                    //Nếu click vào task => Hiển thị Status hiện tại
                    var lst = _unitOfWork.TaskStatusRepository.GetTaskStatusByWorkFlow(task.WorkFlowId);
                    ViewBag.ToStatus = new SelectList(lst, "TaskStatusId", "TaskStatusName", task.TaskStatusId);
                }
                #endregion

                //Load comments of task
                #region Comments
                var commentList = _unitOfWork.TaskRepository.GetTaskCommentList(TaskId);
                task.taskCommentList = commentList;
                task.NumberOfComments = commentList.Count;
                #endregion

                //Load files attachment upload
                #region File Attachment
                var taskFileList = _unitOfWork.TaskRepository.GetTaskFileList(commentList, TaskId);
                task.taskFileList = taskFileList;
                task.NumberOfFiles = taskFileList.Count;
                #endregion

                //Load history
                #region History
                var historyList = _unitOfWork.TaskRepository.GetTaskHistoryList(TaskId);
                task.taskHistoryList = historyList;
                #endregion

                //return type list of subtask based on WorkFlowId
                var typeList = _unitOfWork.WorkFlowRepository.GetTypeByParentWorkFlow(task.WorkFlowId);
                ViewBag.SubtaskType = typeList;

                ViewBag.SubtaskParentTaskId = task.TaskId;
                ViewBag.TodoSubtask = task.TodoSubtask;
                ViewBag.ProcessingSubtask = task.ProcessingSubtask;
                ViewBag.CompletedSubtask = task.CompletedSubtask;

                //đợt
                var LSXSAPList = _context.TaskModel.Where(p => p.ParentTaskId == task.TaskId);
                task.Dot_StartDate = LSXSAPList.Min(e => e.StartDate);
                task.Dot_EstimateEndDate = LSXSAPList.Max(e => e.EstimateEndDate);
                var SONumberList = LSXSAPList.Where(p => p.Property1 != null && p.Property1 != string.Empty).GroupBy(p => p.Property1).Select(grp => grp.Key).ToList();
                if (SONumberList != null && SONumberList.Count > 0)
                {
                    task.SONumber = string.Join(",", SONumberList);
                }
            }
            #region ViewBag
            CreateViewBag(PriorityCode: task.PriorityCode, WorkFlowId: task.WorkFlowId, CommonMistakeCode: task.CommonMistakeCode, StoreId: task.StoreId, TaskStatusId: task.TaskStatusId, Reporter: task.Reporter, ProfileId: task.ProfileId, ContactId: task.ContactId, isEditMode: true, Type: type, ProcessCode: task.ProcessCode, WorkFlowName: task.WorkFlowName, TaskCode: task.TaskCode, TaskId: task.TaskId, RemindCycle: task.RemindCycle, VisitTypeCode: task.VisitTypeCode, VisitSaleOfficeCode: task.VisitSaleOfficeCode, isInPopup: true, SubtaskCode: task.SubtaskCode);
            ViewBag.NextColumnName = NextColumnName;

            //Task config field
            List<string> hasRequestList = new List<string>();
            List<WorkFlowConfigModel> configList = new List<WorkFlowConfigModel>();
            List<SelectListItem> hasRequestRadioList = new List<SelectListItem>();
            var catList = _context.CatalogModel.Where(p => p.CatalogTypeCode == ConstCatalogType.HasRequest)
                                  .OrderBy(p => p.OrderIndex).ToList();
            var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(task.Type);
            if (listWorkFlow != null && listWorkFlow.Count > 0)
            {
                configList = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == task.WorkFlowId).ToList();
                if (configList != null && configList.Count > 0)
                {
                    foreach (var item in configList)
                    {
                        if (string.IsNullOrEmpty(item.Note))
                        {
                            item.Note = PropertyHelper.GetDisplayNameByString<TaskViewModel>(item.FieldCode);
                        }
                        //Yêu cầu cần xử lý: YC/NO_YC
                        if (item.FieldCode == ConstCatalogType.HasRequest && !string.IsNullOrEmpty(item.Parameters))
                        {
                            if (item.Parameters.Contains(","))
                            {
                                hasRequestList = item.Parameters.Split(',').ToList();
                                foreach (var para in hasRequestList)
                                {
                                    var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                    hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                                }
                            }
                            else
                            {
                                var para = hasRequestList.FirstOrDefault();
                                var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                            }
                        }
                        //Set default value
                        if (!string.IsNullOrEmpty(item.EditDefaultValue))
                        {
                            var value = task.GetType().GetProperty(item.FieldCode).GetValue(task, null);
                            if (value == null)
                            {
                                var fieldType = task.GetType().GetProperty(item.FieldCode).PropertyType;
                                task.GetType().GetProperty(item.FieldCode).SetValue(task, PropertyHelper.ChangeType(item.EditDefaultValue, fieldType));
                            }
                        }
                    }
                }
            }
            var defaultValue = _unitOfWork.TaskStatusRepository.GetCategoryByTaskStatus(task.TaskStatusId);
            ViewBag.HasRequest = new SelectList(hasRequestRadioList, "Value", "Text", defaultValue);
            ViewBag.WorkFlowConfig = configList;
            ViewBag.WorkFlowConfigCode = configList.Select(p => p.FieldCode).ToList();
            ViewBag.WorkFlowHiddenField = configList.Where(p => p.HideWhenEdit == true).Select(p => p.FieldCode).ToList();
            ViewBag.WorkFlowField = _context.WorkFlowFieldModel.ToList();

            if (task.Property3 != null)
            {
                var numberProperty3 = task.Property3.FormatCurrency().Replace(",", ".");
                ViewBag.NumberProperty3 = numberProperty3;
            }
            #endregion
            if (string.IsNullOrEmpty(PartialName))
            {
                return PartialView("_FormUpdateTask", task);
            }
            else
            {
                return PartialView(PartialName, task);
            }
        }
        #endregion

        #region Save task update
        [HttpPost]
        [ValidateInput(false)] //need when using ckeditor, do not delete
        public ActionResult SaveComment(TaskViewModel taskViewModel, List<HttpPostedFileBase> CommentFileUrl, List<TaskAssignViewModel> taskAssignList, List<TaskReporterViewModel> taskReporterList, string Type, bool? isInPopup, bool? isSaveAll, List<HttpPostedFileBase> FileUrl)
        {
            return ExecuteContainer(() =>
                {
                    //isSaveAll = true: Lưu chi tiết task trong Edit
                    //isInPopup = true: Lưu chi tiết task trong popup
                    //Nếu isInPopup = false && isSaveAll = false => Chỉ lưu comment

                    //Push Notification
                    string notificationMessage = string.Empty;
                    string currentAccountName = string.Empty;
                    string currentEmployeeCode = string.Empty;
                    string taskCode = string.Empty;

                    //Nguồn tiếp nhận
                    taskViewModel.Property4 = taskViewModel.TaskSourceCode;

                    var task = _context.TaskModel.FirstOrDefault(p => p.TaskId == taskViewModel.TaskId);
                    if (task != null)
                    {
                        taskCode = taskViewModel.WorkFlowCode + "." + task.TaskCode;
                        currentAccountName = _unitOfWork.SalesEmployeeRepository.GetSaleEmployeeNameBy(CurrentUser.EmployeeCode);

                        #region Assignee
                        //Lưu trong Edit và popup
                        if (isInPopup == true || isSaveAll == true)
                        {
                            if (taskViewModel.IsAssignGroup == true)
                            {
                                _unitOfWork.TaskRepository.SaveTaskAssignGroup(taskAssignList, task.TaskId, CurrentUser.AccountId);
                            }
                            else
                            {
                                _unitOfWork.TaskRepository.SaveTaskAssignee(taskAssignList, task.TaskId, CurrentUser.AccountId);
                            }
                        }
                        #endregion

                        #region Reporter
                        //Lưu trong Edit và popup
                        if (isInPopup == true || isSaveAll == true)
                        {
                            _unitOfWork.TaskRepository.SaveTaskReporter(taskReporterList, task.TaskId, CurrentUser.AccountId);
                        }
                        #endregion

                        #region Comment
                        //Lưu trong popup
                        //Lưu ở edit không lưu comment
                        if (isSaveAll != true)
                        {
                            //Comment
                            TaskCommentModel comment = new TaskCommentModel();
                            comment.TaskCommentId = Guid.NewGuid();
                            comment.TaskId = task.TaskId;
                            comment.Comment = taskViewModel.Comment;
                            comment.FromStatusId = task.TaskStatusId;
                            comment.ToStatusId = taskViewModel.ToStatus;
                            comment.CreateBy = CurrentUser.AccountId;
                            comment.CreateTime = DateTime.Now;

                            notificationMessage = string.Format(LanguageResource.TaskCommentNotificationMessage, currentAccountName);

                            //CommentFileUrl
                            if (CommentFileUrl != null && CommentFileUrl.Count > 0)
                            {
                                //Save task comment
                                _context.Entry(comment).State = EntityState.Added;
                                foreach (var item in CommentFileUrl)
                                {
                                    FileAttachmentModel fileNew = SaveFileAttachment(comment.TaskCommentId, item);

                                    //Comment File mapping
                                    Comment_File_Mapping mapping = new Comment_File_Mapping();
                                    mapping.FileAttachmentId = fileNew.FileAttachmentId;
                                    mapping.TaskCommentId = comment.TaskCommentId;
                                    _context.Entry(mapping).State = EntityState.Added;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(taskViewModel.Comment))
                                {
                                    //Save task comment
                                    _context.Entry(comment).State = EntityState.Added;
                                }
                            }
                        }
                        #endregion

                        #region Update task
                        //Lưu trong Edit và popup
                        if (isInPopup == true || isSaveAll == true)
                        {
                            #region Push Notification (Summary, Reporter, TaskStatusId)
                            //Push Notification Summary
                            //2020-11-10: Nếu là thăm hỏi KH thì lấy ưu tiên ngày TH thực tế, nếu không có thì lấy qua ngày dự kiến
                            DateTime? startDate = taskViewModel.StartDate;
                            if (Type == ConstWorkFlowCategory.THKH && taskViewModel.EndDate.HasValue)
                            {
                                startDate = taskViewModel.EndDate;
                            }
                            var summary = _unitOfWork.TaskRepository.GetSummary(taskViewModel.WorkFlowId, taskViewModel.VisitTypeCode, taskViewModel.ReceiveDate, startDate, taskViewModel.ProfileId, taskAssignList);
                            if (!string.IsNullOrEmpty(summary))
                            {
                                taskViewModel.Summary = summary;
                            }
                            task.Summary = taskViewModel.Summary;
                            task.ProductId = taskViewModel.ProductId;
                            task.Unit = taskViewModel.Unit;
                            task.Qty = taskViewModel.Qty;

                            task.Reporter = taskViewModel.Reporter;

                            task.TaskStatusId = taskViewModel.TaskStatusId;
                            #endregion

                            task.CommonMistakeCode = taskViewModel.CommonMistakeCode;
                            task.Description = taskViewModel.Description;
                            if (string.IsNullOrEmpty(taskViewModel.PriorityCode) && string.IsNullOrEmpty(task.PriorityCode))
                            {
                                var listPriority = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.Priority);
                                if (listPriority != null && listPriority.Count > 0)
                                {
                                    taskViewModel.PriorityCode = listPriority[0].CatalogCode;
                                }
                            }
                            if (string.IsNullOrEmpty(taskViewModel.PriorityCode) && !string.IsNullOrEmpty(task.PriorityCode))
                            {
                                taskViewModel.PriorityCode = task.PriorityCode;
                            }
                            task.PriorityCode = taskViewModel.PriorityCode;

                            task.ReceiveDate = taskViewModel.ReceiveDate;
                            task.StartDate = taskViewModel.StartDate;
                            task.EstimateEndDate = taskViewModel.EstimateEndDate;
                            //Get task process code
                            var processCode = _unitOfWork.TaskStatusRepository.GetProcessCodeByTaskStatus(task.WorkFlowId, taskViewModel.TaskStatusId);
                            if (taskViewModel.EndDate != task.EndDate && taskViewModel.EndDate != null && processCode != ConstProcess.completed)
                            {
                                return Json(new
                                {
                                    Code = HttpStatusCode.NotModified,
                                    Success = false,
                                    Data = "Vui lòng chuyển trạng thái sang \"Hoàn thành\" trước khi cập nhật \"Ngày kết thúc\"!"
                                });
                            }
                            task.EndDate = taskViewModel.EndDate;
                            //Riêng tư
                            if (taskViewModel.isPrivate.HasValue)
                            {
                                task.isPrivate = taskViewModel.isPrivate;
                            }
                            //Nhắc nhở
                            task.isRemind = taskViewModel.isRemind;
                            if (task.isRemind == true)
                            {
                                if (taskViewModel.RemindTime == null || taskViewModel.RemindCycle == null || taskViewModel.RemindStartDate == null)
                                {
                                    return Json(new
                                    {
                                        Code = HttpStatusCode.NotModified,
                                        Success = false,
                                        Data = "Vui lòng nhập thông tin \"Thời gian\" và \"Ngày bắt đầu nhắc nhở\"!"
                                    });
                                }
                                task.RemindTime = taskViewModel.RemindTime;
                                task.RemindCycle = taskViewModel.RemindCycle;
                                task.isRemindForReporter = taskViewModel.isRemindForReporter;
                                task.isRemindForAssignee = taskViewModel.isRemindForAssignee;
                                task.RemindStartDate = taskViewModel.RemindStartDate;
                            }
                            //Update remind task
                            _unitOfWork.TaskRepository.UpdateRemindTask(taskViewModel, CurrentUser.AccountId);
                            //Yêu cầu
                            if (!string.IsNullOrEmpty(taskViewModel.Requirement))
                            {
                                var appointment = _context.AppointmentModel.FirstOrDefault(p => p.AppointmentId == task.TaskId);
                                if (appointment != null)
                                {
                                    appointment.Requirement = taskViewModel.Requirement;
                                }
                            }
                            //Thông tin thi công
                            task.ConstructionUnit = taskViewModel.ConstructionUnit;
                            task.ConstructionUnitContact = taskViewModel.ConstructionUnitContact;
                            task.Property3 = taskViewModel.Property3;
                            task.Property4 = taskViewModel.Property4;
                            task.Date1 = taskViewModel.Date1;
                            //Phân công cho nhóm
                            task.IsAssignGroup = taskViewModel.IsAssignGroup;

                            task.LastEditBy = CurrentUser.AccountId;
                            task.LastEditTime = DateTime.Now;

                            //Nơi tham quan
                            task.VisitPlace = taskViewModel.VisitPlace;

                            //isSaveAll = true: Lưu chi tiết task trong Edit
                            #region isSaveAll
                            if (isSaveAll == true)
                            {
                                task.WorkFlowId = taskViewModel.WorkFlowId;
                                if (taskViewModel.StoreId != Guid.Empty)
                                {
                                    task.StoreId = taskViewModel.StoreId;
                                }
                                task.ProfileId = taskViewModel.ProfileId;
                                task.ProfileAddress = taskViewModel.ProfileAddress;
                                task.ServiceTechnicalTeamCode = taskViewModel.ServiceTechnicalTeamCode;
                                task.CustomerReviews = taskViewModel.CustomerReviews;
                                task.ErrorTypeCode = taskViewModel.ErrorTypeCode;
                                task.ErrorCode = taskViewModel.ErrorCode;

                                #region VisitAddress
                                //Required VisitAddress
                                var IsHasVisitAddress = _context.WorkFlowConfigModel
                                                                .Where(p => p.WorkFlowId == taskViewModel.WorkFlowId
                                                                    && p.FieldCode == "VisitAddress"
                                                                    && p.IsRequired == true).FirstOrDefault();
                                if (IsHasVisitAddress != null && string.IsNullOrEmpty(taskViewModel.VisitAddress))
                                {
                                    return Json(new
                                    {
                                        Code = HttpStatusCode.NotModified,
                                        Success = false,
                                        Data = string.Format(LanguageResource.Required, IsHasVisitAddress.Note ?? LanguageResource.Task_VisitAddress),
                                    });
                                }
                                task.VisitAddress = taskViewModel.VisitAddress;
                                //Get lat lng of the VisitAddress
                                if (!string.IsNullOrEmpty(taskViewModel.VisitAddress))
                                {
                                    //Nếu chưa lấy được lat lng ở client => Lấy lat lng theo VisitAddress bằng GoogleLocationService
                                    if (string.IsNullOrEmpty(taskViewModel.lat) || string.IsNullOrEmpty(taskViewModel.lng))
                                    {
                                        //Lấy lat long theo google api => nếu lỗi khỏi set
                                        try
                                        {
                                            var locationService = new GoogleLocationService(GoogleMapAPIKey);
                                            var point = locationService.GetLatLongFromAddress(taskViewModel.VisitAddress);
                                            if (point != null)
                                            {
                                                task.lat = point.Latitude.ToString();
                                                task.lng = point.Longitude.ToString();
                                            }
                                        }
                                        catch //(Exception ex)
                                        {

                                        }

                                    }
                                    else
                                    {
                                        task.lat = taskViewModel.lat;
                                        task.lng = taskViewModel.lng;
                                    }
                                }
                                #endregion

                                task.isRequiredCheckin = taskViewModel.isRequiredCheckin;
                                task.VisitTypeCode = taskViewModel.VisitTypeCode;
                                task.VisitSaleOfficeCode = taskViewModel.VisitSaleOfficeCode;
                                //Bảo hành
                                task.Property1 = taskViewModel.Property1;
                                task.Property2 = taskViewModel.Property2;
                                task.Property5 = taskViewModel.Property5;
                                //Ngày
                                task.Date2 = taskViewModel.Date2;
                                task.Date3 = taskViewModel.Date3;
                                task.Date4 = taskViewModel.Date4;
                                task.Date5 = taskViewModel.Date5;
                                task.ParentTaskId = taskViewModel.ParentTaskId;
                                //NV kinh doanh
                                task.SalesSupervisorCode = taskViewModel.SalesSupervisorCode;
                                //Giá trị ĐTB
                                task.Property6 = taskViewModel.Property6;

                                //Hài lòng khách hàng
                                if (!string.IsNullOrEmpty(taskViewModel.CustomerSatisfactionReviews) || !string.IsNullOrEmpty(taskViewModel.CustomerSatisfactionCode))
                                {
                                    var customerSatisfaction = _context.RatingModel.Where(p => p.ReferenceId == task.TaskId && p.RatingTypeCode == ConstCatalogType.CustomerSatisfaction).FirstOrDefault();
                                    if (customerSatisfaction != null)
                                    {
                                        customerSatisfaction.Reviews = taskViewModel.CustomerSatisfactionReviews;
                                        customerSatisfaction.Ratings = taskViewModel.CustomerSatisfactionCode;
                                        _context.Entry(customerSatisfaction).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        RatingModel newCustomerSatisfaction = new RatingModel();
                                        newCustomerSatisfaction.RatingId = Guid.NewGuid();
                                        newCustomerSatisfaction.RatingTypeCode = ConstCatalogType.CustomerSatisfaction;
                                        newCustomerSatisfaction.ReferenceId = task.TaskId;
                                        newCustomerSatisfaction.Ratings = taskViewModel.CustomerSatisfactionCode;
                                        newCustomerSatisfaction.Reviews = taskViewModel.CustomerSatisfactionReviews;
                                        _context.Entry(newCustomerSatisfaction).State = EntityState.Added;
                                    }
                                }
                                //Ý kiến khách hàng + đánh giá
                                if (!string.IsNullOrEmpty(taskViewModel.CustomerRatings))
                                {
                                    task.Property5 = taskViewModel.CustomerRatings;
                                    //Nếu dữ liệu cũ có rating thì xóa đi thêm lại
                                    var existRatingProduct = _context.RatingModel.Where(p => p.ReferenceId == task.TaskId && p.RatingTypeCode == ConstCatalogType.Ticket_CustomerReviews_Product).FirstOrDefault();
                                    if (existRatingProduct != null)
                                    {
                                        _context.Entry(existRatingProduct).State = EntityState.Deleted;
                                    }
                                    var existRatingService = _context.RatingModel.Where(p => p.ReferenceId == task.TaskId && p.RatingTypeCode == ConstCatalogType.Ticket_CustomerReviews_Service).FirstOrDefault();
                                    if (existRatingService != null)
                                    {
                                        _context.Entry(existRatingService).State = EntityState.Deleted;
                                    }
                                    var existsCustomerReviews = _context.RatingModel.Where(p => p.ReferenceId == task.TaskId && p.RatingTypeCode == null && p.Reviews != null).FirstOrDefault();
                                    if (existsCustomerReviews != null)
                                    {
                                        _context.Entry(existsCustomerReviews).State = EntityState.Deleted;
                                    }
                                    //1. Không ý kiến => Property5 = none
                                    if (taskViewModel.CustomerRatings == "none")
                                    {

                                    }
                                    //2. Đánh giá theo sao => lưu thông tin vào RatingModel
                                    else if (taskViewModel.CustomerRatings == "rating")
                                    {
                                        //2.1. Về sản phẩm
                                        if (!string.IsNullOrEmpty(taskViewModel.Ticket_CustomerReviews_Product))
                                        {
                                            RatingModel ratingProduct = new RatingModel();
                                            ratingProduct.RatingId = Guid.NewGuid();
                                            ratingProduct.RatingTypeCode = ConstCatalogType.Ticket_CustomerReviews_Product;
                                            ratingProduct.ReferenceId = task.TaskId;
                                            ratingProduct.Ratings = taskViewModel.Ticket_CustomerReviews_Product;
                                            _context.Entry(ratingProduct).State = EntityState.Added;
                                        }

                                        //2.2. Về dịch vụ
                                        if (!string.IsNullOrEmpty(taskViewModel.Ticket_CustomerReviews_Service))
                                        {
                                            RatingModel ratingService = new RatingModel();
                                            ratingService.RatingId = Guid.NewGuid();
                                            ratingService.RatingTypeCode = ConstCatalogType.Ticket_CustomerReviews_Service;
                                            ratingService.ReferenceId = task.TaskId;
                                            ratingService.Ratings = taskViewModel.Ticket_CustomerReviews_Service;
                                            _context.Entry(ratingService).State = EntityState.Added;
                                        }

                                        //2.3. Ý kiến khác => nhập nội dung => lưu thông tin vào RatingModel (chỉ lưu reviews)
                                        if (!string.IsNullOrEmpty(taskViewModel.Property5))
                                        {
                                            RatingModel customerReviews = new RatingModel();
                                            customerReviews.RatingId = Guid.NewGuid();
                                            customerReviews.ReferenceId = task.TaskId;
                                            customerReviews.Reviews = taskViewModel.Property5;
                                            _context.Entry(customerReviews).State = EntityState.Added;
                                        }
                                    }
                                    ////3. Khác => nhập nội dung => lưu thông tin vào RatingModel (chỉ lưu reviews)
                                    //else if (taskViewModel.CustomerRatings == "other")
                                    //{
                                    //    if (!string.IsNullOrEmpty(taskViewModel.Property5))
                                    //    {
                                    //        RatingModel customerReviews = new RatingModel();
                                    //        customerReviews.RatingId = Guid.NewGuid();
                                    //        customerReviews.ReferenceId = task.TaskId;
                                    //        customerReviews.Reviews = taskViewModel.Property5;
                                    //        _context.Entry(customerReviews).State = EntityState.Added;
                                    //    }
                                    //}
                                }

                                if (FileUrl != null)
                                {
                                    foreach (var item in FileUrl)
                                    {
                                        FileAttachmentModel fileNew = SaveFileAttachment(taskViewModel.TaskId, item);

                                        //Task File mapping
                                        Task_File_Mapping mapping = new Task_File_Mapping();
                                        mapping.FileAttachmentId = fileNew.FileAttachmentId;
                                        mapping.TaskId = taskViewModel.TaskId;
                                        _context.Entry(mapping).State = EntityState.Added;
                                    }
                                }

                                #region Update task contact
                                var existContact = (from p in _context.TaskContactModel
                                                    where p.TaskId == task.TaskId
                                                    select p).FirstOrDefault();
                                if (taskViewModel.ContactId != null)
                                {
                                    if (existContact != null)
                                    {
                                        existContact.ContactId = taskViewModel.ContactId;
                                    }
                                    else
                                    {
                                        TaskContactModel contact = new TaskContactModel();
                                        contact.TaskContactId = Guid.NewGuid();
                                        contact.TaskId = task.TaskId;
                                        contact.ContactId = taskViewModel.ContactId;
                                        contact.isMain = true;
                                        contact.CreateBy = CurrentUser.AccountId;
                                        contact.CreateTime = DateTime.Now;
                                        _context.Entry(contact).State = EntityState.Added;
                                    }
                                }
                                else if (taskViewModel.ContactId == null && existContact != null)
                                {
                                    _context.Entry(existContact).State = EntityState.Deleted;
                                }
                                #endregion

                                #region Appointment
                                var appointment = _context.AppointmentModel.FirstOrDefault(p => p.AppointmentId == task.TaskId);
                                if (appointment != null)
                                {
                                    appointment.PrimaryContactId = taskViewModel.ContactId;
                                    appointment.ShowroomCode = taskViewModel.ShowroomCode;
                                    appointment.SaleEmployeeCode = taskViewModel.SaleEmployeeCode;
                                    appointment.CategoryCode = taskViewModel.CategoryCode;
                                    appointment.CustomerClassCode = taskViewModel.CustomerClassCode;
                                    appointment.VisitDate = taskViewModel.VisitDate;
                                    appointment.ChannelCode = taskViewModel.ChannelCode;
                                    appointment.Requirement = taskViewModel.Requirement;
                                    //Ghé thăm cabinet pro
                                    appointment.isVisitCabinetPro = taskViewModel.isVisitCabinetPro;
                                    if (appointment.isVisitCabinetPro == true)
                                    {
                                        appointment.VisitCabinetProRequest = taskViewModel.VisitCabinetProRequest;
                                    }
                                    else
                                    {
                                        appointment.VisitCabinetProRequest = null;
                                    }

                                    _context.Entry(appointment).State = EntityState.Modified;

                                    //Rating
                                    var rating = _context.RatingModel.Where(p => p.ReferenceId == task.TaskId && p.RatingTypeCode == ConstCatalogType.CustomerReviews).FirstOrDefault();
                                    if (rating != null)
                                    {
                                        rating.Reviews = taskViewModel.Reviews;
                                        rating.Ratings = taskViewModel.Ratings;
                                        _context.Entry(rating).State = EntityState.Modified;
                                    }
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    _context.SaveChanges();

                    #region Push notification
                    taskCode = taskViewModel.WorkFlowCode + "." + (!string.IsNullOrEmpty(task.SubtaskCode) ? task.SubtaskCode : task.TaskCode.ToString());
                    notificationMessage = !string.IsNullOrEmpty(notificationMessage) ? notificationMessage : string.Format("{0} vừa được cập nhật bởi {1}", taskCode, currentAccountName);

                    //PushNotification(CurrentUser.AccountId.Value, taskViewModel.TaskId, notificationMessage, task);
                    #endregion Push notification

                    //Tạo subtask cho nhân viên nếu phân theo nhóm
                    if (taskViewModel.IsAssignGroup == true)
                    {
                        _unitOfWork.TaskRepository.UpdateGroupAssignTask(taskViewModel.TaskId);
                    }

                    //Lấy parent type của type subtask
                    var parentType = (from p in _context.TaskModel
                                      join w in _context.WorkFlowModel on p.WorkFlowId equals w.WorkFlowId
                                      where p.TaskId == taskViewModel.ParentTaskId
                                      select w.WorkflowCategoryCode).FirstOrDefault();
                    if (!string.IsNullOrEmpty(parentType))
                    {
                        Type = parentType;
                    }
                    //Nếu là Ghé thăm => redirect về trang Ghé thăm
                    string redirectUrl = taskViewModel.WorkFlowCode == ConstWorkFlow.GT ? "/Customer/Appointment" : string.Format("/Work/Task?Type={0}", Type);
                    var typeName = _unitOfWork.WorkFlowRepository.GetWorkFlowCategory(Type);
                    return Json(new
                    {
                        Code = HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Edit_Success, typeName),
                        RedirectUrl = redirectUrl
                    });
                });
        }
        #endregion

        #region Comment on modal popup
        [HttpPost]
        [ValidateInput(false)] //need when using ckeditor, do not delete
        public ActionResult EditComment(Guid? TaskCommentId, string EditComment, string Type)
        {
            return ExecuteContainer(() =>
            {
                var comment = _context.TaskCommentModel.FirstOrDefault(p => p.TaskCommentId == TaskCommentId);
                if (comment != null)
                {
                    comment.Comment = EditComment;
                    comment.LastEditBy = CurrentUser.AccountId;
                    comment.LastEditTime = DateTime.Now;
                    _context.SaveChanges();
                }

                string redirectUrl = string.Format("/Work/Task?Type={0}&tab=tab=tab-kanban", Type);
                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Comment.ToLower()),
                    RedirectUrl = redirectUrl
                });
            });
        }
        [HttpPost]
        public ActionResult DeleteComment(Guid? TaskCommentId, string Type)
        {
            return ExecuteDelete(() =>
            {
                var comment = _context.TaskCommentModel.FirstOrDefault(p => p.TaskCommentId == TaskCommentId);
                if (comment != null)
                {
                    _context.Entry(comment).State = EntityState.Deleted;
                    _context.SaveChanges();
                }

                string redirectUrl = string.Format("/Work/Task?Type={0}&tab=tab=tab-kanban", Type);
                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Comment.ToLower()),
                    RedirectUrl = redirectUrl
                });
            });
        }
        #endregion

        #region File attachment on modal
        [HttpPost]
        public ActionResult SaveFileAttachments(Guid TaskId, List<HttpPostedFileBase> MainCommentFileUrl)
        {
            return ExecuteContainer(() =>
            {
                var task = _context.TaskModel.FirstOrDefault(p => p.TaskId == TaskId);
                if (task != null)
                {
                    if (MainCommentFileUrl != null && MainCommentFileUrl.Count > 0)
                    {
                        foreach (var item in MainCommentFileUrl)
                        {
                            FileAttachmentModel fileNew = SaveFileAttachment(task.TaskId, item);

                            //Task File mapping
                            Task_File_Mapping mapping = new Task_File_Mapping();
                            mapping.FileAttachmentId = fileNew.FileAttachmentId;
                            mapping.TaskId = task.TaskId;
                            _context.Entry(mapping).State = EntityState.Added;
                        }
                        _context.SaveChanges();
                    }
                }
                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Task_FileUrl.ToLower())
                });
            });
        }
        [HttpPost]
        public ActionResult DeleteFileAttachment(Guid FileAttachmentId)
        {
            return ExecuteDelete(() =>
            {
                var file = _context.FileAttachmentModel.FirstOrDefault(p => p.FileAttachmentId == FileAttachmentId);
                if (file != null)
                {
                    //Delete in Task_File_Mapping
                    var task_mapp = _context.Task_File_Mapping.FirstOrDefault(p => p.FileAttachmentId == FileAttachmentId);
                    if (task_mapp != null)
                    {
                        _context.Entry(task_mapp).State = EntityState.Deleted;
                    }
                    //Delete in Comment_File_Mapping
                    var comment_mapp = _context.Comment_File_Mapping.FirstOrDefault(p => p.FileAttachmentId == FileAttachmentId);
                    if (comment_mapp != null)
                    {
                        _context.Entry(comment_mapp).State = EntityState.Deleted;
                    }
                    //Delete in FileAttachmentModel
                    _context.Entry(file).State = EntityState.Deleted;
                    _context.SaveChanges();
                }

                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Task_FileUrl.ToLower())
                });
            });
        }
        #endregion

        #region Save Task Product
        public ActionResult GetTaskProduct(Guid? TaskId, Guid? TaskProductId, string Type, Guid? WorkFlowId)
        {
            TaskProductViewModel taskProduct = new TaskProductViewModel();
            if (TaskProductId != null)
            {
                taskProduct = _unitOfWork.TaskRepository.GetTaskProduct(TaskProductId);
            }
            #region ViewBag
            CreateViewBag(WorkFlowId: WorkFlowId, ErrorCode: taskProduct.ErrorCode, ErrorTypeCode: taskProduct.ErrorTypeCode, isEditMode: true, ProductLevelCode: taskProduct.ProductLevelCode, ProductColorCode: taskProduct.ProductColorCode, UsualErrorCode: taskProduct.UsualErrorCode, ProductCategoryCode: taskProduct.ProductCategoryCode, IsEditProduct: true, TaskId: TaskId, Unit: taskProduct.Unit, UsualErrorCodeList: taskProduct.usualErrorList);

            List<string> hasRequestList = new List<string>();
            List<WorkFlowConfigModel> configList = new List<WorkFlowConfigModel>();
            List<SelectListItem> hasRequestRadioList = new List<SelectListItem>();
            var catList = _context.CatalogModel.Where(p => p.CatalogTypeCode == ConstCatalogType.HasRequest)
                                  .OrderBy(p => p.OrderIndex).ToList();
            var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(Type);
            if (listWorkFlow != null && listWorkFlow.Count > 0)
            {
                listWorkFlow = listWorkFlow.Where(p => p.WorkFlowCode != ConstWorkFlow.GT).ToList();

                configList = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == WorkFlowId).ToList();
                if (configList != null && configList.Count > 0)
                {
                    foreach (var item in configList)
                    {
                        if (string.IsNullOrEmpty(item.Note))
                        {
                            item.Note = PropertyHelper.GetDisplayNameByString<TaskViewModel>(item.FieldCode);
                        }
                        //Yêu cầu cần xử lý: YC/NO_YC
                        if (item.FieldCode == ConstCatalogType.HasRequest && !string.IsNullOrEmpty(item.Parameters))
                        {
                            if (item.Parameters.Contains(","))
                            {
                                hasRequestList = item.Parameters.Split(',').ToList();
                                foreach (var para in hasRequestList)
                                {
                                    var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                    hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                                }
                            }
                            else
                            {
                                var para = hasRequestList.FirstOrDefault();
                                var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                            }
                        }
                    }
                }
            }
            var defaultValue = (catList != null && catList.Count > 0 && hasRequestList.Count > 0) ?
                                    catList.FirstOrDefault(p => p.CatalogCode == "NO_YC").CatalogCode :
                                    null;
            ViewBag.HasRequest = new SelectList(hasRequestRadioList, "Value", "Text", defaultValue);
            ViewBag.WorkFlowConfig = configList;
            ViewBag.WorkFlowConfigCode = configList.Select(p => p.FieldCode).ToList();
            ViewBag.WorkFlowHiddenField = configList.Where(p => p.HideWhenAdd == true).Select(p => p.FieldCode).ToList();
            ViewBag.WorkFlowField = _context.WorkFlowFieldModel.ToList();
            ViewBag.Type = Type;
            #endregion

            return PartialView("_FormAddProduct", taskProduct);
        }

        [HttpPost]
        public ActionResult SaveTaskProduct(TaskProductViewModel taskProductVM, List<TaskProductViewModel> accessoryList, List<string> UsualErrorCode)
        {
            return ExecuteSearch(() =>
            {
                Guid taskProductId = Guid.Empty;
                //Nếu không có TaskProductId => thêm mới
                //Ngược lại => cập nhật
                if (taskProductVM.TaskProductId != null)
                {
                    var taskProduct = _context.TaskProductModel.FirstOrDefault(p => p.TaskProductId == taskProductVM.TaskProductId);
                    if (taskProduct != null)
                    {
                        taskProductId = taskProduct.TaskProductId;
                        taskProduct.ProductId = taskProductVM.ProductId;
                        taskProduct.Qty = taskProductVM.Qty;
                        //if (taskProduct.ProductId != null)
                        //{
                        //    taskProduct.Unit = taskProductVM.Unit;
                        //}
                        taskProduct.Unit = taskProductVM.Unit;
                        taskProduct.LastEditBy = CurrentUser.AccountId;
                        taskProduct.LastEditTime = DateTime.Now;
                        taskProduct.ErrorTypeCode = taskProductVM.ErrorTypeCode;
                        taskProduct.ErrorCode = taskProductVM.ErrorCode;
                        taskProduct.ProductLevelCode = taskProductVM.ProductLevelCode;
                        taskProduct.ProductColorCode = taskProductVM.ProductColorCode;
                        //taskProduct.UsualErrorCode = taskProductVM.UsualErrorCode;
                        //Các lỗi bảo hành thường gặp => Cập nhật: Cho chọn được nhiều lỗi
                        #region Các lỗi bảo hành thường gặp
                        //Xóa dữ liệu lỗi cũ
                        var existsTaskProductUsualError = _context.TaskProductUsualErrorModel.Where(p => p.TaskProductId == taskProductVM.TaskProductId).ToList();
                        if (existsTaskProductUsualError != null && existsTaskProductUsualError.Count > 0)
                        {
                            _context.TaskProductUsualErrorModel.RemoveRange(existsTaskProductUsualError);
                        }

                        if (UsualErrorCode != null && UsualErrorCode.Count > 0)
                        {
                            foreach (var usualError in UsualErrorCode)
                            {
                                TaskProductUsualErrorModel taskProductUsualError = new TaskProductUsualErrorModel();
                                taskProductUsualError.TaskProductUsualErrorId = Guid.NewGuid();
                                taskProductUsualError.TaskProductId = taskProduct.TaskProductId;
                                taskProductUsualError.UsualErrorCode = usualError;

                                _context.Entry(taskProductUsualError).State = EntityState.Added;
                            }
                        }
                        #endregion

                        taskProduct.ProductCategoryCode = taskProductVM.ProductCategoryCode;
                        taskProduct.WarrantyValue = taskProductVM.WarrantyValue;

                        //Xóa phụ kiện thay thế cũ
                        var oldTaskAccessory = _context.TaskProductAccessoryModel.Where(p => p.TaskProductId == taskProduct.TaskProductId).ToList();
                        if (oldTaskAccessory != null && oldTaskAccessory.Count > 0)
                        {
                            _context.TaskProductAccessoryModel.RemoveRange(oldTaskAccessory);
                        }
                    }
                }
                else
                {
                    //Phụ kiện thay thế
                    var accList = new List<TaskProductViewModel>();
                    if (accessoryList != null)
                    {
                        accList = accessoryList.Where(p => p.ProductId != null).ToList();
                    }
                    //Phải có ít nhất 1 trong các field có value mới thêm mới
                    if (taskProductVM.ProductId != null || taskProductVM.Qty != null || taskProductVM.Unit != null || taskProductVM.ErrorTypeCode != null || taskProductVM.ErrorCode != null || taskProductVM.ProductLevelCode != null || taskProductVM.ProductColorCode != null || taskProductVM.UsualErrorCode != null || taskProductVM.ProductCategoryCode != null || taskProductVM.WarrantyValue != null || (accList != null && accList.Count > 0))
                    {
                        TaskProductModel model = new TaskProductModel();
                        model.TaskProductId = Guid.NewGuid();
                        model.TaskId = taskProductVM.TaskId;
                        model.ProductId = taskProductVM.ProductId;
                        model.Qty = taskProductVM.Qty;
                        //if (model.ProductId != null)
                        //{
                        //    model.Unit = taskProductVM.Unit;
                        //}
                        model.Unit = taskProductVM.Unit;
                        model.CreateBy = CurrentUser.AccountId;
                        model.CreateTime = DateTime.Now;
                        model.ErrorTypeCode = taskProductVM.ErrorTypeCode;
                        model.ErrorCode = taskProductVM.ErrorCode;
                        model.ProductLevelCode = taskProductVM.ProductLevelCode;
                        model.ProductColorCode = taskProductVM.ProductColorCode;
                        //model.UsualErrorCode = taskProductVM.UsualErrorCode;

                        //Các lỗi bảo hành thường gặp => Cập nhật: Cho chọn được nhiều lỗi
                        #region Các lỗi bảo hành thường gặp
                        if (UsualErrorCode != null && UsualErrorCode.Count > 0)
                        {
                            foreach (var usualError in UsualErrorCode)
                            {
                                TaskProductUsualErrorModel taskProductUsualError = new TaskProductUsualErrorModel();
                                taskProductUsualError.TaskProductUsualErrorId = Guid.NewGuid();
                                taskProductUsualError.TaskProductId = model.TaskProductId;
                                taskProductUsualError.UsualErrorCode = usualError;

                                _context.Entry(taskProductUsualError).State = EntityState.Added;
                            }
                        }
                        #endregion

                        model.ProductCategoryCode = taskProductVM.ProductCategoryCode;
                        model.WarrantyValue = taskProductVM.WarrantyValue;
                        _context.Entry(model).State = EntityState.Added;

                        taskProductId = model.TaskProductId;
                    }
                }

                //Phụ kiện thay thế
                if (accessoryList != null)
                {
                    var lst = accessoryList.Where(p => p.ProductId != null).ToList();
                    if (lst != null && lst.Count > 0 && taskProductId != Guid.Empty)
                    {
                        foreach (var item in lst)
                        {
                            TaskProductAccessoryModel accessory = new TaskProductAccessoryModel();
                            accessory.TaskProductAccessoryId = Guid.NewGuid();
                            accessory.TaskProductId = taskProductId;
                            accessory.AccessoryId = item.ProductId;
                            accessory.Qty = item.Qty;
                            accessory.ProductAccessoryTypeCode = item.ProductAccessoryTypeCode;
                            accessory.AccErrorTypeCode = item.AccErrorTypeCode;

                            _context.Entry(accessory).State = EntityState.Added;
                        }
                    }
                }
                _context.SaveChanges();

                var productList = _unitOfWork.TaskRepository.GetTaskProductList(taskProductVM.TaskId.Value);

                #region ViewBag
                var task = _context.TaskModel.FirstOrDefault(p => p.TaskId == taskProductVM.TaskId);

                //Task config field
                List<string> hasRequestList = new List<string>();
                List<WorkFlowConfigModel> configList = new List<WorkFlowConfigModel>();
                List<SelectListItem> hasRequestRadioList = new List<SelectListItem>();
                var catList = _context.CatalogModel.Where(p => p.CatalogTypeCode == ConstCatalogType.HasRequest)
                                      .OrderBy(p => p.OrderIndex).ToList();

                var type = _unitOfWork.TaskRepository.GetWorkflowCategoryCode(taskProductVM.TaskId);
                var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(type);
                if (listWorkFlow != null && listWorkFlow.Count > 0)
                {
                    configList = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == task.WorkFlowId).ToList();
                    if (configList != null && configList.Count > 0)
                    {
                        foreach (var item in configList)
                        {
                            if (string.IsNullOrEmpty(item.Note))
                            {
                                item.Note = PropertyHelper.GetDisplayNameByString<TaskViewModel>(item.FieldCode);
                            }
                            //Yêu cầu cần xử lý: YC/NO_YC
                            if (item.FieldCode == ConstCatalogType.HasRequest && !string.IsNullOrEmpty(item.Parameters))
                            {
                                if (item.Parameters.Contains(","))
                                {
                                    hasRequestList = item.Parameters.Split(',').ToList();
                                    foreach (var para in hasRequestList)
                                    {
                                        var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                        hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                                    }
                                }
                                else
                                {
                                    var para = hasRequestList.FirstOrDefault();
                                    var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                    hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                                }
                            }
                        }
                    }
                }
                ViewBag.WorkFlowConfig = configList;
                ViewBag.WorkFlowConfigCode = configList.Select(p => p.FieldCode).ToList();
                ViewBag.WorkFlowHiddenField = configList.Where(p => p.HideWhenEdit == true).Select(p => p.FieldCode).ToList();
                ViewBag.WorkFlowField = _context.WorkFlowFieldModel.ToList();
                #endregion
                return PartialView("_ListProduct", productList);
            });
        }

        [HttpPost]
        public ActionResult DeleteTaskProduct(Guid TaskProductId, Guid TaskId)
        {
            return ExecuteSearch(() =>
            {
                var taskProduct = _context.TaskProductModel.FirstOrDefault(p => p.TaskProductId == TaskProductId);
                if (taskProduct != null)
                {
                    _context.Entry(taskProduct).State = EntityState.Deleted;
                }
                _context.SaveChanges();

                var productList = _unitOfWork.TaskRepository.GetTaskProductList(TaskId);

                #region ViewBag
                var task = _context.TaskModel.FirstOrDefault(p => p.TaskId == TaskId);

                //Task config field
                List<string> hasRequestList = new List<string>();
                List<WorkFlowConfigModel> configList = new List<WorkFlowConfigModel>();
                List<SelectListItem> hasRequestRadioList = new List<SelectListItem>();
                var catList = _context.CatalogModel.Where(p => p.CatalogTypeCode == ConstCatalogType.HasRequest)
                                      .OrderBy(p => p.OrderIndex).ToList();

                var type = _unitOfWork.TaskRepository.GetWorkflowCategoryCode(TaskId);
                var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(type);
                if (listWorkFlow != null && listWorkFlow.Count > 0)
                {
                    configList = _context.WorkFlowConfigModel.Where(p => p.WorkFlowId == task.WorkFlowId).ToList();
                    if (configList != null && configList.Count > 0)
                    {
                        foreach (var item in configList)
                        {
                            if (string.IsNullOrEmpty(item.Note))
                            {
                                item.Note = PropertyHelper.GetDisplayNameByString<TaskViewModel>(item.FieldCode);
                            }
                            //Yêu cầu cần xử lý: YC/NO_YC
                            if (item.FieldCode == ConstCatalogType.HasRequest && !string.IsNullOrEmpty(item.Parameters))
                            {
                                if (item.Parameters.Contains(","))
                                {
                                    hasRequestList = item.Parameters.Split(',').ToList();
                                    foreach (var para in hasRequestList)
                                    {
                                        var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                        hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                                    }
                                }
                                else
                                {
                                    var para = hasRequestList.FirstOrDefault();
                                    var cat = catList.Where(p => p.CatalogCode == para).Select(p => p.CatalogText_vi).FirstOrDefault();
                                    hasRequestRadioList.Add(new SelectListItem { Text = cat, Value = para });
                                }
                            }
                        }
                    }
                }
                ViewBag.WorkFlowConfig = configList;
                ViewBag.WorkFlowConfigCode = configList.Select(p => p.FieldCode).ToList();
                ViewBag.WorkFlowHiddenField = configList.Where(p => p.HideWhenEdit == true).Select(p => p.FieldCode).ToList();
                ViewBag.WorkFlowField = _context.WorkFlowFieldModel.ToList();
                #endregion
                return PartialView("_ListProduct", productList);
            });
        }
        #endregion

        #region TaskGroup
        public ActionResult AddTaskGroup()
        {
            TaskGroupViewModel taskGroup = new TaskGroupViewModel();

            //var empLst = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist();
            //ViewBag.EmployeeList = empLst;

            //Get department
            var deparmentList = _context.DepartmentModel
                .Where(p => p.Actived == true)
                .OrderBy(p => p.OrderIndex)
                .ToList();
            var departmentNull = new DepartmentModel
            {
                DepartmentId = Guid.Empty,
                DepartmentName = LanguageResource.Department_NotSet
            };
            deparmentList.Add(departmentNull);

            ViewBag.DepartmentId = new SelectList(deparmentList, "DepartmentId", "DepartmentName");
            return PartialView("_FormAddTaskGroup", taskGroup);
        }

        public ActionResult SaveTaskGroup(string GroupName, List<string> accountInGroup)
        {
            if (string.IsNullOrEmpty(GroupName))
            {
                return Json(new
                {
                    Success = false,
                    Data = "Vui lòng nhập thông tin \"Tên nhóm\"",
                });
            }
            else
            {
                if (accountInGroup != null && accountInGroup.Count > 0)
                {
                    //Master
                    TaskGroupModel taskGroup = new TaskGroupModel();
                    taskGroup.GroupId = Guid.NewGuid();
                    taskGroup.GroupName = GroupName;
                    taskGroup.CreatedAccountId = CurrentUser.AccountId;

                    _context.Entry(taskGroup).State = EntityState.Added;

                    //Detail
                    foreach (var employeeCode in accountInGroup)
                    {
                        var account = _context.AccountModel.Where(p => p.EmployeeCode == employeeCode).FirstOrDefault();
                        if (account != null)
                        {
                            TaskGroupDetailModel taskGroupDetail = new TaskGroupDetailModel();
                            taskGroupDetail.GroupId = taskGroup.GroupId;
                            taskGroupDetail.AccountId = account.AccountId;
                            taskGroupDetail.Note = string.Format("{0} được tạo bởi {1}", taskGroup.GroupName, CurrentUser.UserName);

                            _context.Entry(taskGroupDetail).State = EntityState.Added;
                        }
                    }

                    _context.SaveChanges();
                    return Json(new
                    {
                        Success = true,
                        Data = new ISDSelectStringItem()
                        {
                            id = taskGroup.GroupId.ToString(),
                            name = taskGroup.GroupName,
                        },
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        Data = "Vui lòng chọn ít nhất 1 nhân viên trong nhóm",
                    });
                }

            }
        }

        public ActionResult GetAccountInGroup(string RolesCode, List<string> accountInGroup)
        {
            Guid GroupId = Guid.Empty;
            bool isValid = Guid.TryParse(RolesCode, out GroupId);
            List<SalesEmployeeViewModel> taskAssignList = new List<SalesEmployeeViewModel>();
            if (isValid == true)
            {
                taskAssignList = (from p in _context.TaskGroupDetailModel
                                  join acc in _context.AccountModel on p.AccountId equals acc.AccountId
                                  join se in _context.SalesEmployeeModel.Include(s => s.DepartmentModel) on acc.EmployeeCode equals se.SalesEmployeeCode
                                  where p.GroupId == GroupId
                                  orderby acc.EmployeeCode
                                  select new SalesEmployeeViewModel()
                                  {
                                      SalesEmployeeCode = acc.EmployeeCode,
                                      SalesEmployeeName = se.SalesEmployeeName,
                                      DepartmentName = se.DepartmentModel.DepartmentName,
                                  }).ToList();
            }
            else
            {
                taskAssignList = (from p in _context.RolesModel
                                  from r in p.AccountModel
                                  join acc in _context.AccountModel on r.AccountId equals acc.AccountId
                                  join se in _context.SalesEmployeeModel.Include(s => s.DepartmentModel) on acc.EmployeeCode equals se.SalesEmployeeCode
                                  orderby acc.EmployeeCode
                                  where p.RolesCode == RolesCode
                                  select new SalesEmployeeViewModel()
                                  {
                                      SalesEmployeeCode = acc.EmployeeCode,
                                      SalesEmployeeName = se.SalesEmployeeName,
                                      DepartmentName = se.DepartmentModel.DepartmentName,
                                  }).ToList();
            }

            ////Employee
            //var empLst = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist();
            //empLst.Insert(0, new SalesEmployeeViewModel
            //{
            //    SalesEmployeeCode = string.Empty,
            //    SalesEmployeeName = "-- Chọn người thực hiện --",
            //});
            //return Json(new
            //{
            //    Success = true,
            //    Data = taskAssignList,
            //    EmployeeList = empLst,
            //}, JsonRequestBehavior.AllowGet);
            ViewBag.accountInGroup = accountInGroup;
            return PartialView("_SearchEmployee", taskAssignList);
        }

        public ActionResult _SearchEmployee(string SalesEmployeeCode = "", string SalesEmployeeName = "", Guid? DepartmentId = null)
        {
            return ExecuteSearch(() =>
            {
                List<SalesEmployeeViewModel> laborList = new List<SalesEmployeeViewModel>();
                //not set department
                if (DepartmentId == Guid.Empty)
                {
                    laborList = (from p in _context.SalesEmployeeModel.Include(s => s.DepartmentModel)
                                 where
                                 //search by SalesEmployeeName
                                 (SalesEmployeeName == "" || p.SalesEmployeeName.Contains(SalesEmployeeName))
                                 //search by SalesEmployeeCode
                                 && (SalesEmployeeCode == "" || p.SalesEmployeeCode.Contains(SalesEmployeeCode))
                                 && p.DepartmentId == null
                                 && p.Actived == true
                                 select new SalesEmployeeViewModel()
                                 {
                                     SalesEmployeeCode = p.SalesEmployeeCode,
                                     SalesEmployeeName = p.SalesEmployeeName,
                                     Phone = p.Phone,
                                     Email = p.Email,
                                     Actived = p.Actived,
                                     DepartmentName = p.DepartmentModel.DepartmentName
                                 }).ToList();
                }
                else
                {
                    // all department
                    if (DepartmentId == null)
                    {
                        laborList = (from p in _context.SalesEmployeeModel.Include(s => s.DepartmentModel)
                                     where
                                     //search by SalesEmployeeName
                                     (SalesEmployeeName == "" || p.SalesEmployeeName.Contains(SalesEmployeeName))
                                     //search by SalesEmployeeCode
                                     && (SalesEmployeeCode == "" || p.SalesEmployeeCode.Contains(SalesEmployeeCode))
                                     select new SalesEmployeeViewModel()
                                     {
                                         SalesEmployeeCode = p.SalesEmployeeCode,
                                         SalesEmployeeName = p.SalesEmployeeName,
                                         Phone = p.Phone,
                                         Email = p.Email,
                                         Actived = p.Actived,
                                         DepartmentName = p.DepartmentModel.DepartmentName
                                     }).ToList();
                    }
                    else
                    {
                        //by departmentid 
                        laborList = (from p in _context.SalesEmployeeModel.Include(s => s.DepartmentModel)
                                     where
                                     //search by SalesEmployeeName
                                     (SalesEmployeeName == "" || p.SalesEmployeeName.Contains(SalesEmployeeName))
                                     //search by SalesEmployeeCode
                                     && (SalesEmployeeCode == "" || p.SalesEmployeeCode.Contains(SalesEmployeeCode))
                                     && p.DepartmentId == DepartmentId
                                     select new SalesEmployeeViewModel()
                                     {
                                         SalesEmployeeCode = p.SalesEmployeeCode,
                                         SalesEmployeeName = p.SalesEmployeeName,
                                         Phone = p.Phone,
                                         Email = p.Email,
                                         Actived = p.Actived,
                                         DepartmentName = p.DepartmentModel.DepartmentName
                                     }).ToList();
                    }
                }


                return PartialView(laborList);
            });
        }

        //xóa nhóm phân công
        public ActionResult DeleteExternalTaskGroup(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var taskGroup = _context.TaskGroupModel.FirstOrDefault(p => p.GroupId == id);
                if (taskGroup != null)
                {
                    //TaskGroupDetail
                    var TaskGroupDetail = _context.TaskGroupDetailModel.Where(p => p.GroupId == id).ToList();
                    if (TaskGroupDetail != null && TaskGroupDetail.Count > 0)
                    {
                        _context.TaskGroupDetailModel.RemoveRange(TaskGroupDetail);
                    }

                    //Xóa task
                    _context.Entry(taskGroup).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.AssigneeGroup.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.AssigneeGroup.ToLower())
                    });
                }
            });
        }
        #endregion

        #region Move item to another kanban
        public ActionResult _TaskPopup(Guid TaskId, Guid KanbanId, string NextColumnName)
        {
            if (!string.IsNullOrEmpty(NextColumnName))
            {
                var CurrentColumnName = (from p in _context.TaskModel
                                         join t in _context.TaskStatusModel on p.TaskStatusId equals t.TaskStatusId
                                         join m in _context.Kanban_TaskStatus_Mapping on t.TaskStatusId equals m.TaskStatusId
                                         join d in _context.KanbanDetailModel on m.KanbanDetailId equals d.KanbanDetailId
                                         where p.TaskId == TaskId
                                         && d.KanbanId == KanbanId
                                         select d.ColumnName).FirstOrDefault();

                if (CurrentColumnName != NextColumnName)
                {
                    var task = _unitOfWork.TaskRepository.GetTaskById(TaskId);

                    TaskPopupViewModel model = new TaskPopupViewModel();
                    model.TaskId = TaskId;
                    model.TaskStatusId = (from p in _context.Kanban_TaskStatus_Mapping
                                          join d in _context.KanbanDetailModel on p.KanbanDetailId equals d.KanbanDetailId
                                          join st in _context.TaskStatusModel on p.TaskStatusId equals st.TaskStatusId
                                          where d.ColumnName == NextColumnName
                                          && d.KanbanId == KanbanId
                                          && st.WorkFlowId == task.WorkFlowId
                                          select st.TaskStatusId).FirstOrDefault();

                    //ViewBag
                    var KanbanDetailId = (from t in _context.TaskStatusModel
                                          join m in _context.Kanban_TaskStatus_Mapping on t.TaskStatusId equals m.TaskStatusId
                                          join d in _context.KanbanDetailModel on m.KanbanDetailId equals d.KanbanDetailId
                                          where d.KanbanId == KanbanId
                                          && t.TaskStatusId == model.TaskStatusId
                                          select d.KanbanDetailId).FirstOrDefault();

                    var taskStatusList = _unitOfWork.TaskStatusRepository.GetNextTaskStatus(task.WorkFlowId, model.TaskStatusId, KanbanDetailId);
                    ViewBag.TaskStatusId = new SelectList(taskStatusList, "TaskStatusId", "TaskStatusName");

                    if (!string.IsNullOrEmpty(task.WorkFlowName))
                    {
                        model.Title = string.Format("({0}){1}", task.WorkFlowName.Trim(), task.TaskCode);
                    }
                    return PartialView(model);
                }
            }
            return RedirectToAction("_Edit", new { TaskId, KanbanId, NextColumnName });
        }
        [HttpPost]
        [ValidateInput(false)] //need when using ckeditor, do not delete
        public ActionResult SaveTaskStatus(TaskPopupViewModel viewModel, List<HttpPostedFileBase> FileUrl)
        {
            return ExecuteContainer(() =>
            {
                var task = _context.TaskModel.FirstOrDefault(p => p.TaskId == viewModel.TaskId);
                if (task != null)
                {
                    TaskCommentModel comment = new TaskCommentModel();
                    comment.TaskCommentId = Guid.NewGuid();
                    comment.TaskId = viewModel.TaskId;
                    comment.FromStatusId = task.TaskStatusId;
                    comment.ToStatusId = viewModel.TaskStatusId;
                    comment.Comment = viewModel.Comment;
                    comment.CreateBy = CurrentUser.AccountId;
                    comment.CreateTime = DateTime.Now;
                    _context.Entry(comment).State = EntityState.Added;

                    if (FileUrl != null && FileUrl.Count > 0)
                    {
                        foreach (var item in FileUrl)
                        {
                            FileAttachmentModel fileNew = SaveFileAttachment(comment.TaskCommentId, item);

                            //Comment File mapping
                            Comment_File_Mapping mapping = new Comment_File_Mapping();
                            mapping.FileAttachmentId = fileNew.FileAttachmentId;
                            mapping.TaskCommentId = comment.TaskCommentId;
                            _context.Entry(mapping).State = EntityState.Added;
                        }
                    }

                    task.TaskStatusId = viewModel.TaskStatusId;
                    task.LastEditBy = CurrentUser.AccountId;
                    task.LastEditTime = DateTime.Now;
                    _context.SaveChanges();
                }

                var Type = _unitOfWork.TaskRepository.GetWorkflowCategoryCode(viewModel.TaskId);
                //Lấy parent type của type subtask
                var parentType = (from p in _context.TaskModel
                                  join w in _context.WorkFlowModel on p.WorkFlowId equals w.WorkFlowId
                                  where p.TaskId == task.ParentTaskId
                                  select w.WorkflowCategoryCode).FirstOrDefault();
                if (!string.IsNullOrEmpty(parentType))
                {
                    Type = parentType;
                }
                string redirectUrl = string.Format("/Work/Task?Type={0}", Type);
                var typeName = _unitOfWork.WorkFlowRepository.GetWorkFlowCategory(Type);
                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, typeName),
                    RedirectUrl = redirectUrl
                });
            });
        }
        #endregion

        #region Delete Task
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var task = _context.TaskModel.FirstOrDefault(p => p.TaskId == id);
                if (task != null)
                {
                    //Xóa những bảng liên quan đến task trước
                    //1. AppointmentModel
                    var appointment = _context.AppointmentModel.Where(p => p.AppointmentId == id).FirstOrDefault();
                    if (appointment != null)
                    {
                        _context.Entry(appointment).State = EntityState.Deleted;
                    }
                    //2. CheckInOutModel
                    var checkInOut = _context.CheckInOutModel.Where(p => p.CheckInOutId == id).FirstOrDefault();
                    if (checkInOut != null)
                    {
                        _context.Entry(checkInOut).State = EntityState.Deleted;
                    }
                    //3. RemindTaskModel (ParentTaskId)
                    var parentRemindTask = _context.RemindTaskModel.Where(p => p.ParentTaskId == id).ToList();
                    if (parentRemindTask != null && parentRemindTask.Count > 0)
                    {
                        _context.RemindTaskModel.RemoveRange(parentRemindTask);
                    }
                    //4. FileAttachmentModel
                    var fileAttachment = _context.FileAttachmentModel.Where(p => p.ObjectId == id).ToList();
                    if (fileAttachment != null && fileAttachment.Count > 0)
                    {
                        _context.FileAttachmentModel.RemoveRange(fileAttachment);
                    }
                    //4.1. Task_File_Mapping
                    var taskFileMapping = _context.Task_File_Mapping.Where(p => p.TaskId == id).ToList();
                    if (taskFileMapping != null && taskFileMapping.Count > 0)
                    {
                        _context.Task_File_Mapping.RemoveRange(taskFileMapping);
                    }

                    //5. TaskCommentModel
                    var taskComment = _context.TaskCommentModel.Where(p => p.TaskId == id).ToList();
                    if (taskComment != null && taskComment.Count > 0)
                    {
                        var taskCommentId = taskComment.Select(p => p.TaskCommentId).ToList();
                        //5.1. Comment_File_Mapping
                        if (taskCommentId != null && taskCommentId.Count > 0)
                        {
                            var commentFile = _context.Comment_File_Mapping.Where(p => taskCommentId.Contains(p.TaskCommentId)).ToList();
                            if (commentFile != null && commentFile.Count > 0)
                            {
                                _context.Comment_File_Mapping.RemoveRange(commentFile);
                            }
                        }
                        _context.TaskCommentModel.RemoveRange(taskComment);
                    }


                    //6. TaskAssignModel
                    var taskAssign = _context.TaskAssignModel.Where(p => p.TaskId == id).ToList();
                    if (taskAssign != null && taskAssign.Count > 0)
                    {
                        _context.TaskAssignModel.RemoveRange(taskAssign);
                    }

                    //7. TaskContactModel
                    var taskContact = _context.TaskContactModel.Where(p => p.TaskId == id).ToList();
                    if (taskContact != null && taskContact.Count > 0)
                    {
                        _context.TaskContactModel.RemoveRange(taskContact);
                    }

                    //8. TaskProductModel
                    var taskProduct = _context.TaskProductModel.Where(p => p.TaskId == id).ToList();
                    if (taskProduct != null && taskProduct.Count > 0)
                    {
                        var taskProductId = taskProduct.Select(p => p.TaskProductId).ToList();
                        //8.1. TaskProductAccessoryModel
                        if (taskProductId != null && taskProductId.Count > 0)
                        {
                            var taskProductAccessory = _context.TaskProductAccessoryModel.Where(p => taskProductId.Contains(p.TaskProductId.Value)).ToList();
                            if (taskProductAccessory != null && taskProductAccessory.Count > 0)
                            {
                                _context.TaskProductAccessoryModel.RemoveRange(taskProductAccessory);
                            }
                        }
                        _context.TaskProductModel.RemoveRange(taskProduct);
                    }

                    //9. TaskReferenceModel
                    var taskReference = _context.TaskReferenceModel.Where(p => p.TaskId == id).ToList();
                    if (taskReference != null && taskReference.Count > 0)
                    {
                        _context.TaskReferenceModel.RemoveRange(taskReference);
                    }

                    //10. TaskRoleInChargeModel
                    var taskRoleInCharge = _context.TaskRoleInChargeModel.Where(p => p.TaskId == id).ToList();
                    if (taskRoleInCharge != null && taskRoleInCharge.Count > 0)
                    {
                        _context.TaskRoleInChargeModel.RemoveRange(taskRoleInCharge);
                    }

                    //Xóa task
                    _context.Entry(task).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Work_Task.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, LanguageResource.Work_Task.ToLower())
                    });
                }
            });
        }
        #endregion Delete Task

        #region Cancel Task
        [HttpPost]
        public ActionResult Cancel(Guid id, string CancelReason)
        {
            return ExecuteContainer(() =>
            {
                if (!string.IsNullOrEmpty(CancelReason))
                {
                    var task = _context.TaskModel.FirstOrDefault(p => p.TaskId == id);
                    if (task != null)
                    {
                        //Hủy task
                        task.Actived = false;
                        task.CancelReason = CancelReason;
                        _context.Entry(task).State = EntityState.Modified;
                        _context.SaveChanges();
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = true,
                            Data = string.Format(LanguageResource.Alert_Cancel_Success, LanguageResource.Work_Task.ToLower())
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.NotModified,
                            Success = false,
                            Data = string.Format(LanguageResource.Alert_NotExist_Cancel, LanguageResource.Work_Task.ToLower())
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format(LanguageResource.Required, LanguageResource.DeletedReason.ToLower())
                    });
                }
            });
        }
        #endregion Cancel Task

        #region Change Task Status by QRCode
        public ActionResult ChangeStatusByQRCode(Guid TaskId)
        {
            var FromTaskStatus = (from t in _context.TaskModel
                                  join st in _context.TaskStatusModel on t.TaskStatusId equals st.TaskStatusId
                                  where t.TaskId == TaskId
                                  select new TaskStatusSelectModel
                                  {
                                      TaskStatusId = st.TaskStatusId,
                                      TaskStatusName = st.TaskStatusName,
                                      OrderIndex = st.OrderIndex
                                  }).FirstOrDefault();
            var FromStausLst = new List<TaskStatusSelectModel>();
            FromStausLst.Add(FromTaskStatus);
            ViewBag.FromTaskStatusId = new SelectList(FromStausLst, "TaskStatusId", "TaskStatusName");

            //Danh sách trạng thái task được sắp xếp và không có trạng thái hiện tại của task
            var ToTaskStatus = (from st in _context.TaskStatusModel
                                join t in _context.TaskModel on st.WorkFlowId equals t.WorkFlowId
                                where t.TaskId == TaskId && st.TaskStatusId != FromTaskStatus.TaskStatusId && st.Actived == true
                                select new TaskStatusSelectModel
                                {
                                    TaskStatusId = st.TaskStatusId,
                                    TaskStatusName = st.TaskStatusName,
                                    OrderIndex = st.OrderIndex
                                }).OrderBy(p => p.OrderIndex).ToList();

            var NextStatusId = ToTaskStatus.Where(p => p.OrderIndex > FromTaskStatus.OrderIndex).Select(p => p.TaskStatusId).FirstOrDefault();

            ViewBag.ToTaskStatusId = new SelectList(ToTaskStatus, "TaskStatusId", "TaskStatusName", NextStatusId);//Chọn sẵn trạng thái tiếp theo
            ViewBag.TaskId = TaskId;

            return PartialView("_ChangeStatusByQRCode");
        }

        public JsonResult UpdateStauts(ChangeTaskStatusViewModel changeTaskStatus)
        {
            return ExecuteContainer(() =>
            {

                var task = _context.TaskModel.Where(p => p.TaskId == changeTaskStatus.TaskId).FirstOrDefault();
                task.TaskStatusId = changeTaskStatus.ToTaskStatusId;
                _context.Entry(task).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    Code = HttpStatusCode.OK,
                    Success = true,
                    Message = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Status.ToLower())
                });
            });
        }
        #endregion

        public async Task<JsonResult> SyncSaleOrder(string CompanyCode, string SaleOrderCode, string SOType)
        {
            bool Success = true;
            string Message = string.Empty;
            try
            {
                var paramRequest = new ParamRequestSyncSapModel
                {
                    CompanyCode = CompanyCode,
                    VBELN = SaleOrderCode,
                    BEDAE = SOType
                };
                var _soReposiroty = new SaleOrderRepository(_context);
                //var message = await _soReposiroty.SyncSaleOrder100(paramRequest, true);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        Message = ex.InnerException.InnerException.Message;
                    }
                }
            }
            return Json(new { Success, Message }, JsonRequestBehavior.AllowGet);
        }
    }
}