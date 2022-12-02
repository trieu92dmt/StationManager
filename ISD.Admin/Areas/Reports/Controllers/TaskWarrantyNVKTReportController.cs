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
    public class TaskWarrantyNVKTReportController : BaseController
    {
        // GET: TaskWarrantyNVKTReport
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (TaskWarrantyNVKTSearchModel)TempData[CurrentUser.AccountId + "TaskWarrantyNVKTSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "TaskWarrantyNVKTTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "TaskWarrantyNVKTModeSearch"];
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
            var pageId = GetPageId("/Reports/TaskWarrantyNVKTReport");
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
            CreateViewBagForSearch();
            return View();
        }
        private void CreateViewBagForSearch()
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
            //Dropdown nhóm trạng thái
            var statusLst = _unitOfWork.TaskStatusRepository.GetTaskStatusList();
            ViewBag.TaskProcessCode = new SelectList(statusLst, "StatusCode", "StatusName");

            //Dropdown trạng thái
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

            //Dropdown Nhân viên
            var saleEmployeeList = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist();
            ViewBag.Assignee = new SelectList(saleEmployeeList, "SalesEmployeeCode", "SalesEmployeeName");

            //Dropdown TT bảo hành
            var serviceTechnicalTeamCodeList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ServiceTechnicalTeam);
            ViewBag.ServiceTechnicalTeamCode = new SelectList(serviceTechnicalTeamCodeList, "CatalogCode", "CatalogText_vi");

            //Dropdown Department
            var rolesList = _context.DepartmentModel.Where(p => p.Actived == true).ToList();
            ViewBag.DepartmentId = new SelectList(rolesList, "DepartmentId", "DepartmentName");
            //Loại bảo hành
            var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(ConstWorkFlowCategory.TICKET_MLC);
            listWorkFlow = listWorkFlow.Where(p => p.WorkFlowCode != ConstWorkFlow.GT).ToList();

            ViewBag.WorkFlowId = new SelectList(listWorkFlow, "WorkFlowId", "WorkFlowName");
            ViewBag.PageId = GetPageId("/Reports/TaskWarrantyNVKTReport");
        }

        #region export to excel
        public ActionResult ExportExcel(TaskWarrantyNVKTSearchModel searchModel)
        {
            var data = GetData(searchModel);
            return Export(data);
        }
        private List<TaskWarrantyNVKTReportExcelModel> GetData(TaskWarrantyNVKTSearchModel searchModel)
        {
            List<TaskWarrantyNVKTReportExcelModel> result = new List<TaskWarrantyNVKTReportExcelModel>();

            #region TaskProcessCode
            var TaskProcessCode_Todo = false;
            var TaskProcessCode_Processing = false;
            var TaskProcessCode_Incomplete = false;
            var TaskProcessCode_CompletedOnTime = false;
            var TaskProcessCode_CompletedExpire = false;
            var TaskProcessCode_Expired = false;
            if (string.IsNullOrEmpty(searchModel.TaskProcessCode))
            {
                TaskProcessCode_Todo = true;
                TaskProcessCode_Processing = true;
                TaskProcessCode_Incomplete = true;
                TaskProcessCode_CompletedOnTime = true;
                TaskProcessCode_CompletedExpire = true;
                TaskProcessCode_Expired = true;
            }
            else
            {
                if (searchModel.TaskProcessCode == ConstTaskStatus.Todo)
                {
                    TaskProcessCode_Todo = true;
                }
                if (searchModel.TaskProcessCode == ConstTaskStatus.Processing)
                {
                    TaskProcessCode_Processing = true;
                }
                if (searchModel.TaskProcessCode == ConstTaskStatus.Incomplete)
                {
                    TaskProcessCode_Incomplete = true;
                }
                if (searchModel.TaskProcessCode == ConstTaskStatus.CompletedOnTime)
                {
                    TaskProcessCode_CompletedOnTime = true;
                }
                if (searchModel.TaskProcessCode == ConstTaskStatus.CompletedExpire)
                {
                    TaskProcessCode_CompletedExpire = true;
                }
                if (searchModel.TaskProcessCode == ConstTaskStatus.Expired)
                {
                    TaskProcessCode_Expired = true;
                }
            }

            #endregion

            DataSet ds = new DataSet();
            string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("[Report].[usp_TaskWarrantyNVKTReport]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        #region Parameters
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_Todo", TaskProcessCode_Todo);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_Processing", TaskProcessCode_Processing);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_Incomplete", TaskProcessCode_Incomplete);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_CompletedOnTime", TaskProcessCode_CompletedOnTime);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_CompletedExpire", TaskProcessCode_CompletedExpire);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskProcessCode_Expired", TaskProcessCode_Expired);

                        sda.SelectCommand.Parameters.AddWithValue("@StartFromDate", searchModel.StartFromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@StartToDate", searchModel.StartToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@EndFromDate", searchModel.EndFromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@EndToDate", searchModel.EndToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@TaskStatusCode", searchModel.TaskStatusCode ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ServiceTechnicalTeamCode", searchModel.ServiceTechnicalTeamCode ?? (object)DBNull.Value);

                        sda.SelectCommand.Parameters.AddWithValue("@Assignee", searchModel.Assignee ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@DepartmentId", searchModel.DepartmentId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@WorkFlowId", searchModel.WorkFlowId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProfileId", searchModel.ProfileId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@Property5", searchModel.Property5 ?? (object)DBNull.Value);
                        #endregion

                        sda.Fill(ds);
                        var dt = ds.Tables[0];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow item in dt.Rows)
                            {
                                #region Convert to list
                                TaskWarrantyNVKTReportExcelModel model = new TaskWarrantyNVKTReportExcelModel();
                                model.SalesEmployeeName = item["SalesEmployeeName"].ToString();
                                model.WorkFlowName = item["WorkflowName"].ToString();

                                if (!string.IsNullOrEmpty(item["QtyComplete"].ToString()))
                                {
                                    model.QtyComplete = Convert.ToInt32(item["QtyComplete"].ToString());
                                }
                                if (!string.IsNullOrEmpty(item["QtyCancel"].ToString()))
                                {
                                    model.QtyCancel = Convert.ToInt32(item["QtyCancel"].ToString());
                                }
                                if (!string.IsNullOrEmpty(item["QtyAdvisoryPhone"].ToString()))
                                {
                                    model.QtyAdvisoryPhone = Convert.ToInt32(item["QtyAdvisoryPhone"].ToString());
                                }
                                if (!string.IsNullOrEmpty(item["QtyBookLater"].ToString()))
                                {
                                    model.QtyBookLater = Convert.ToInt32(item["QtyBookLater"].ToString());
                                }
                                if (!string.IsNullOrEmpty(item["QtyTotal"].ToString()))
                                {
                                    model.QtyTotal = Convert.ToInt32(item["QtyTotal"].ToString());
                                }
                                model.CompleteRate = (decimal)((decimal)model.QtyComplete / (decimal)model.QtyTotal) * 100;
                                result.Add(model);
                                #endregion
                            }
                        }
                    }
                }
            }
            return result;
        }
        public ActionResult Export(List<TaskWarrantyNVKTReportExcelModel> viewModel)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;
            fileheader = "Báo cáo Tỉ lệ lịch bảo hành của NVKT";

            #region Master
            columns.Add(new ExcelTemplate { ColumnName = "SalesEmployeeName", isAllowedToEdit = false }); //1. NV được phân công  
            columns.Add(new ExcelTemplate { ColumnName = "WorkFlowName", isAllowedToEdit = false });//2.  Loại bảo hành
            columns.Add(new ExcelTemplate { ColumnName = "QtyComplete", isAllowedToEdit = false,MergeHeaderTitle = "Số lượng", isCurrency = true });  //3. Đã thực hiện 
            columns.Add(new ExcelTemplate { ColumnName = "QtyCancel", isAllowedToEdit = false , MergeHeaderTitle = "Số lượng", isCurrency = true });//4. Lịch hủy
            columns.Add(new ExcelTemplate { ColumnName = "QtyAdvisoryPhone", isAllowedToEdit = false, MergeHeaderTitle = "Số lượng", isCurrency = true });//5. Tư vấn qua điện thoại
            columns.Add(new ExcelTemplate { ColumnName = "QtyBookLater", isAllowedToEdit = false,MergeHeaderTitle = "Số lượng", isCurrency = true });//6. Lịch hẹn lại
            columns.Add(new ExcelTemplate { ColumnName = "QtyTotal", isAllowedToEdit = false,MergeHeaderTitle = "Số lượng", isCurrency = true });//9. Tổng cộng
            columns.Add(new ExcelTemplate { ColumnName = "CompleteRate", isAllowedToEdit = false, isNumber = true });//10. Tỷ lệ hoàn thành %
            #endregion
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
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false, headerRowMergeCount:1);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion
        public ActionResult ViewDetail(TaskWarrantyNVKTSearchModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "TaskWarrantyNVKTSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "TaskWarrantyNVKTTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "TaskWarrantyNVKTModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, TaskWarrantyNVKTSearchModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "TaskWarrantyNVKTSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "TaskWarrantyNVKTTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "TaskWarrantyNVKTModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult TaskWarrantyNVKTPivotGridPartial(Guid? templateId = null, TaskWarrantyNVKTSearchModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/TaskWarrantyNVKTReport");
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
                return PartialView("_TaskWarrantyNVKTPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<TaskWarrantyNVKTSearchModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_TaskWarrantyNVKTPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(TaskWarrantyNVKTSearchModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_TI_LE_LICH_BAO_HANH_DVKT";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}