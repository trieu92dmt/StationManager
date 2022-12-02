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
using System.Web;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class TaskTicketMLCReportController : BaseController
    {
        // GET: TaskTicketMLCReport
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            #region ViewBag
            //Employee
            var empLst = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist();
            //Assignee
            ViewBag.AssigneeList = new SelectList(empLst, "SalesEmployeeCode", "SalesEmployeeName");
            //CreateBy
            ViewBag.CreateBy = new SelectList(empLst, "SalesEmployeeCode", "SalesEmployeeName");


            var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(ConstWorkFlowCategory.TICKET_MLC);
            listWorkFlow = listWorkFlow.Where(p => p.WorkFlowCode != ConstWorkFlow.GT).ToList();

            ViewBag.WorkFlowIdList = new SelectList(listWorkFlow, "WorkFlowId", "WorkFlowName");

            #region //Get list ServiceTechnicalTeamCode (Trung tâm bảo hành)
            var serviceTechnicalTeamCodeList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ServiceTechnicalTeam);
            ViewBag.ServiceTechnicalTeamCode = new SelectList(serviceTechnicalTeamCodeList, "CatalogCode", "CatalogText_vi");
            #endregion

            #region //TaskProcessCode 
            var statusLst = _unitOfWork.TaskStatusRepository.GetTaskStatusList();
            ViewBag.TaskProcessCode = new SelectList(statusLst, "StatusCode", "StatusName");
            #endregion

            #region //TaskStatusCode - Trạng thái

            var result = (from wf in _context.WorkFlowModel
                          join ts in _context.TaskStatusModel on wf.WorkFlowId equals ts.WorkFlowId
                          where wf.WorkflowCategoryCode == ConstWorkFlowCategory.TICKET_MLC
                          select new
                          {
                              ts.OrderIndex,
                              ts.TaskStatusCode,
                              ts.TaskStatusName
                          }).Distinct().OrderBy(p => p.OrderIndex).ToList();

            ViewBag.TaskStatusCode = new SelectList(result, "TaskStatusCode", "TaskStatusName");

            #endregion

            #region //Get list Roles (Phòng ban)
            var rolesList = _context.DepartmentModel.Where(p => p.Actived == true).ToList();
            ViewBag.DepartmentCode = new SelectList(rolesList, "DepartmentCode", "DepartmentName");
            #endregion

            #region CommonDate
            var SelectedCommonDate = "Custom";
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);

            //Common Date 2
            var commonDateList2 = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate2);
            ViewBag.CommonDate2 = new SelectList(commonDateList2, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            #endregion
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
            ViewBag.Property5 = new SelectList(customerRatings, "Value", "Text");
            #endregion


            var searchModel = (TaskTicketMLCReportSearchModel)TempData[CurrentUser.AccountId + "TaskTicketMLCSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "TaskTicketMLCTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "TaskTicketMLCModeSearch"];
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
            var pageId = GetPageId("/Reports/TaskTicketMLCReport");
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

            return View();
        }

        public ActionResult ExportExcel(TaskTicketMLCReportSearchModel searchModel)
        {
            var data = GetData(searchModel);

            return Export(data);
        }

        private FileContentResult Export(List<TaskTicketMLCReportExcelModel> viewModel)
        {
            #region Column
            List<ExcelTemplate> columns = new List<ExcelTemplate>();

            columns.Add(new ExcelTemplate { ColumnName = "ReceiveDate", isAllowedToEdit = false, isDateTime = true });
            columns.Add(new ExcelTemplate { ColumnName = "StartDate", isAllowedToEdit = false, isDateTime = true });
            columns.Add(new ExcelTemplate { ColumnName = "EndDate", isAllowedToEdit = false, isDateTime = true });
            columns.Add(new ExcelTemplate { ColumnName = "CreateByName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProfileAddress", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Phone", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "WorkFlowName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "TaskStatusName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ServiceTechnicalName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "TaskAssigneeName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Description", isAllowedToEdit = false, isWraptext = true });
            columns.Add(new ExcelTemplate { ColumnName = "CustomerReviews", isAllowedToEdit = false, isWraptext = true });
            columns.Add(new ExcelTemplate { ColumnName = "ServiceRating", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProductRating", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Property5", isAllowedToEdit = false });

            #endregion
            //Header
            string fileheader = "Báo cáo Kết lịch hằng ngày DVKT";

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

        private List<TaskTicketMLCReportExcelModel> GetData(TaskTicketMLCReportSearchModel searchModel)
        {
            List<string> processCodeList = new List<string>();
            if (searchModel.TaskProcessCode == null)
            {
                processCodeList.Add(ConstTaskStatus.Todo);
                processCodeList.Add(ConstTaskStatus.Processing);
                processCodeList.Add(ConstTaskStatus.Incomplete);
                processCodeList.Add(ConstTaskStatus.CompletedOnTime);
                processCodeList.Add(ConstTaskStatus.CompletedExpire);
                processCodeList.Add(ConstTaskStatus.Expired);
            }

            #region //Receive Date
            if (searchModel.ReceiveCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchModel.ReceiveCommonDate, out fromDate, out toDate);
                searchModel.ReceiveFromDate = fromDate;
                searchModel.ReceiveToDate = toDate;
            }
            #endregion

            #region //Start Date
            if (searchModel.StartCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchModel.StartCommonDate, out fromDate, out toDate);
                searchModel.StartFromDate = fromDate;
                searchModel.StartToDate = toDate;
            }
            #endregion

            #region //End Date
            if (searchModel.EndCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchModel.EndCommonDate, out fromDate, out toDate);
                searchModel.EndFromDate = fromDate;
                searchModel.EndToDate = toDate;
            }
            #endregion
            #region Task Process
            var TaskProcessCode_Todo = false;
            var TaskProcessCode_Processing = false;
            var TaskProcessCode_Incomplete = false;
            var TaskProcessCode_CompletedOnTime = false;
            var TaskProcessCode_CompletedExpire = false;
            var TaskProcessCode_Expired = false;

            if (processCodeList != null && processCodeList.Count > 0)
            {
                foreach (var item in processCodeList)
                {
                    //Việc cần làm
                    if (item == ConstTaskStatus.Todo)
                    {
                        TaskProcessCode_Todo = true;
                    }
                    //Đang thực hiện
                    if (item == ConstTaskStatus.Processing)
                    {
                        TaskProcessCode_Processing = true;
                    }
                    //Chưa hoàn thành
                    if (item == ConstTaskStatus.Incomplete)
                    {
                        TaskProcessCode_Incomplete = true;
                    }
                    //Hoàn thành đúng hạn
                    if (item == ConstTaskStatus.CompletedOnTime)
                    {
                        TaskProcessCode_CompletedOnTime = true;
                    }
                    //Hoàn thành quá hạn
                    if (item == ConstTaskStatus.CompletedExpire)
                    {
                        TaskProcessCode_CompletedExpire = true;
                    }
                    //Quá hạn
                    if (item == ConstTaskStatus.Expired)
                    {
                        TaskProcessCode_Expired = true;
                    }
                }
            }
            else
            {
                //Việc cần làm
                if (searchModel.TaskProcessCode == ConstTaskStatus.Todo)
                {
                    TaskProcessCode_Todo = true;
                }
                //Đang thực hiện
                if (searchModel.TaskProcessCode == ConstTaskStatus.Processing)
                {
                    TaskProcessCode_Processing = true;
                }
                //Chưa hoàn thành
                if (searchModel.TaskProcessCode == ConstTaskStatus.Incomplete)
                {
                    TaskProcessCode_Incomplete = true;
                }
                //Hoàn thành đúng hạn
                if (searchModel.TaskProcessCode == ConstTaskStatus.CompletedOnTime)
                {
                    TaskProcessCode_CompletedOnTime = true;
                }
                //Hoàn thành quá hạn
                if (searchModel.TaskProcessCode == ConstTaskStatus.CompletedExpire)
                {
                    TaskProcessCode_CompletedExpire = true;
                }
                //Quá hạn
                if (searchModel.TaskProcessCode == ConstTaskStatus.Expired)
                {
                    TaskProcessCode_Expired = true;
                }
            }
            #endregion

            var result = new List<TaskTicketMLCReportExcelModel>();
            //int FilteredResultsCount = 0;
            DataSet ds = new DataSet();
            string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("[Report].[usp_TaskTicketMLCReport]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        #region Parameters
                        sda.SelectCommand.Parameters.AddWithValue("@ReceiveFromDate", searchModel.ReceiveFromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ReceiveToDate", searchModel.ReceiveToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StartFromDate", searchModel.StartFromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StartToDate", searchModel.StartToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@EndFromDate", searchModel.EndFromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@EndToDate", searchModel.EndToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@CreateBy", searchModel.CreateBy ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProfileId", searchModel.ProfileId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@WorkFlowId", searchModel.WorkFlowId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_Todo", TaskProcessCode_Todo);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_Processing", TaskProcessCode_Processing);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_Incomplete", TaskProcessCode_Incomplete);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_CompletedOnTime", TaskProcessCode_CompletedOnTime);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_CompletedExpire", TaskProcessCode_CompletedExpire);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_Expired", TaskProcessCode_Expired);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskStatusCode", searchModel.TaskStatusCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ServiceTechnicalTeamCode", searchModel.ServiceTechnicalTeamCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@Assignee", searchModel.Assignee ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@DepartmentCode", searchModel.DepartmentCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@Property5", searchModel.Property5 ?? (object)DBNull.Value);
                        #endregion

                        sda.Fill(ds);
                        var dt = ds.Tables[0];
                        //var filteredResultsCount = output.Value;
                        //if (filteredResultsCount != null)
                        //{
                        //    FilteredResultsCount = Convert.ToInt32(filteredResultsCount);
                        //}

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow item in dt.Rows)
                            {
                                #region Convert to list
                                var model = new TaskTicketMLCReportExcelModel();
                                if (!string.IsNullOrEmpty(item["ReceiveDate"].ToString()))
                                {
                                    model.ReceiveDate = Convert.ToDateTime(item["ReceiveDate"].ToString());
                                }
                                if (!string.IsNullOrEmpty(item["StartDate"].ToString()))
                                {
                                    model.StartDate = Convert.ToDateTime(item["StartDate"].ToString());
                                }
                                if (!string.IsNullOrEmpty(item["EndDate"].ToString()))
                                {
                                    model.EndDate = Convert.ToDateTime(item["EndDate"].ToString());
                                }
                                model.CreateByName = item["CreateByName"].ToString();
                                model.ProfileName = item["ProfileName"].ToString();
                                model.ProfileAddress = item["ProfileAddress"].ToString();
                                model.Phone = item["Phone"].ToString();
                                model.WorkFlowName = item["WorkFlowName"].ToString();
                                model.TaskStatusName = item["TaskStatusName"].ToString();
                                model.ServiceTechnicalName = item["ServiceTechnicalName"].ToString();
                                model.TaskAssigneeName = item["TaskAssigneeName"].ToString();
                                model.Description = item["Description"].ToString();
                                model.CustomerReviews = item["CustomerReviews"].ToString();
                                model.ServiceRating = item["ServiceRating"].ToString();
                                model.ProductRating = item["ProductRating"].ToString();
                                model.Property5 = item["Property5"].ToString();
                                if (!string.IsNullOrEmpty(item["ProfileId"].ToString()))
                                {
                                    model.ProfileId = Guid.Parse(item["ProfileId"].ToString());
                                }
                                //Get list sđt
                                var phoneList = _context.ProfilePhoneModel.Where(s => s.ProfileId == model.ProfileId).ToList();
                                                 
                                if (phoneList.Count != 0)
                                {
                                    foreach (var phone in phoneList)
                                    {
                                        model.Phone = model.Phone + ", " + phone.PhoneNumber;
                                    }
                                }

                                result.Add(model);
                                #endregion
                            }
                        }
                    }
                }
            }

            return result;

        }
        public ActionResult ViewDetail(TaskTicketMLCReportSearchModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "TaskTicketMLCSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "TaskTicketMLCTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "TaskTicketMLCModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, TaskTicketMLCReportSearchModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "TaskTicketMLCSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "TaskTicketMLCTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "TaskTicketMLCModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult TaskTicketMLCPivotGridPartial(Guid? templateId = null, TaskTicketMLCReportSearchModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/TaskTicketMLCReport");
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
                return PartialView("_TaskTicketMLCPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<TaskTicketMLCReportSearchModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_TaskTicketMLCPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(TaskTicketMLCReportSearchModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_KET_LICH_HANG_NGAY_DVKT";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}