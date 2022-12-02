using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.Resources;
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
    public class TaskCustomerReviewReportController : BaseController
    {
        // GET: TaskCustomerReviewReport
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            #region ViewBag
            #region //Get list ServiceTechnicalTeamCode (Trung tâm bảo hành)
            var serviceTechnicalTeamCodeList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.ServiceTechnicalTeam);
            ViewBag.ServiceTechnicalTeamCode = new SelectList(serviceTechnicalTeamCodeList, "CatalogCode", "CatalogText_vi");
            #endregion
            //Employee
            var empLst = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist();
            //Assignee
            ViewBag.AssigneeList = new SelectList(empLst, "SalesEmployeeCode", "SalesEmployeeName");
            var listWorkFlow = _unitOfWork.WorkFlowRepository.GetWorkFlowBy(ConstWorkFlowCategory.TICKET_MLC);
            listWorkFlow = listWorkFlow.Where(p => p.WorkFlowCode != ConstWorkFlow.GT).ToList();

            ViewBag.WorkFlowIdList = new SelectList(listWorkFlow, "WorkFlowId", "WorkFlowName");
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
            #endregion
            var searchModel = (TaskTicketMLCReportSearchModel)TempData[CurrentUser.AccountId + "TaskCustomerReviewSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "TaskCustomerReviewTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "TaskCustomerReviewModeSearch"];
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
            var pageId = GetPageId("/Reports/TaskCustomerReviewReport");
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

            columns.Add(new ExcelTemplate { ColumnName = "TaskAssigneeName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "WorkFlowName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ServiceRating", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ProductRating", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Property5", isAllowedToEdit = false });

            #endregion
            //Header
            string fileheader = "Báo cáo Tổng hợp ý kiến khách hàng";

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
            var result = new List<TaskTicketMLCReportExcelModel>();
            //int FilteredResultsCount = 0;
            DataSet ds = new DataSet();
            string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("[Report].[usp_TaskCustomerReviewReport]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 1800;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        #region Parameters
                        sda.SelectCommand.Parameters.AddWithValue("@EndFromDate", searchModel.EndFromDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@EndToDate", searchModel.EndToDate ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@ProfileId", searchModel.ProfileId ?? (object)DBNull.Value);
                        sda.SelectCommand.Parameters.AddWithValue("@WorkFlowId", searchModel.WorkFlowId ?? (object)DBNull.Value);
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
                                model.WorkFlowName = item["WorkFlowName"].ToString();
                                model.TaskAssigneeName = item["TaskAssigneeName"].ToString();
                                model.ServiceRating = item["ServiceRating"].ToString();
                                model.ProductRating = item["ProductRating"].ToString();
                                model.Property5 = item["Property5"].ToString();
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
            TempData[CurrentUser.AccountId + "TaskCustomerReviewSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "TaskCustomerReviewTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "TaskCustomerReviewModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, TaskTicketMLCReportSearchModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "TaskCustomerReviewSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "TaskCustomerReviewTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "TaskCustomerReviewModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult TaskCustomerReviewPivotGridPartial(Guid? templateId = null, TaskTicketMLCReportSearchModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/TaskCustomerReviewReport");
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
                return PartialView("_TaskCustomerReviewPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<TaskTicketMLCReportSearchModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_TaskCustomerReviewPivotGridPartial", model);
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
            string fileName = "BAO_CAO_TONG_HOP_Y_KIEN_KHACH_HANG";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}