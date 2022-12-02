using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.ViewModels;
using Microsoft.SqlServer.Server;
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
    public class CustomerCareActivitiesController : BaseController
    {
        // GET: CustomerCareActivities
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var saleEmployeeList = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist();
            ViewBag.SalesEmployeeCode = new SelectList(saleEmployeeList, "SalesEmployeeCode", "SalesEmployeeName");
            DateTime? fromDate, toDate;
            var CommonDate = "ThisMonth";
            _unitOfWork.CommonDateRepository.GetDateBy(CommonDate, out fromDate, out toDate);
            CustomerCareSearchViewModel searchViewModel = new CustomerCareSearchViewModel()
            {
                //Ngày ghé thăm
                StartCommonDate = CommonDate,
                StartFromDate = fromDate,
                StartToDate = toDate,
            };
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", CommonDate);


            var companyList = _context.CompanyModel.Where(p => p.Actived == true).OrderBy(p => p.CompanyCode).ToList();
            ViewBag.CompanyId = new SelectList(companyList, "CompanyId", "CompanyName");

            var departmentList = _context.DepartmentModel.Where(p => p.Actived == true).OrderBy(p => p.DepartmentCode).ToList();
            ViewBag.DepartmentId = new SelectList(departmentList, "DepartmentId", "DepartmentName");

            var searchModel = (CustomerCareSearchViewModel)TempData[CurrentUser.AccountId + "CustomerCareActivitiesSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "CustomerCareActivitiesTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "CustomerCareActivitiesModeSearch"];
            var pageId = GetPageId("/Reports/CustomerCareActivities");

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

            return View(searchViewModel);
        }

        #region Export to excel
        public ActionResult ExportExcel(CustomerCareSearchViewModel searchViewModel)
        {
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
            if (searchViewModel.StartToDate != null)
            {
                searchViewModel.StartToDate = searchViewModel.StartToDate.Value.AddDays(1).AddSeconds(-1);
            }
            #endregion

            //var data = GetQuantyCusCareWithPersonInChargeReport(searchViewModel, CurrentUser.CompanyCode);
            var resultData = new List<CustomerCareActivitiesViewModel>();
            var data = GetData(searchViewModel);
            if (searchViewModel.ViewTotal == true)
            {
                resultData = (from p in data
                              group p by new { p.SalesEmployeeName, p.DepartmentName } into gr
                              select new CustomerCareActivitiesViewModel
                              {
                                  SalesEmployeeName = gr.Key.SalesEmployeeName,
                                  DepartmentName = gr.Key.DepartmentName,
                                  AppointmentQty = gr.Sum(p => p.AppointmentQty) == 0 ? null : gr.Sum(p => p.AppointmentQty),
                                  THKH_SpecKHQty = gr.Sum(p => p.THKH_SpecKHQty) == 0 ? null : gr.Sum(p => p.THKH_SpecKHQty),
                                  THKH_KhaoSatLdDTBQty = gr.Sum(p => p.THKH_KhaoSatLdDTBQty) == 0 ? null : gr.Sum(p => p.THKH_KhaoSatLdDTBQty),
                                  THKH_SpecDTBQty = gr.Sum(p => p.THKH_SpecDTBQty) == 0 ? null : gr.Sum(p => p.THKH_SpecDTBQty),
                                  THKH_Khac = gr.Sum(p => p.THKH_Khac) == 0 ? null : gr.Sum(p => p.THKH_Khac),
                                  TICKET_XLKNQty = gr.Sum(p => p.TICKET_XLKNQty) == 0 ? null : gr.Sum(p => p.TICKET_XLKNQty),
                                  TICKET_KVQty = gr.Sum(p => p.TICKET_KVQty) == 0 ? null : gr.Sum(p => p.TICKET_KVQty),
                                  TICKET_KSQty = gr.Sum(p => p.TICKET_KSQty) == 0 ? null : gr.Sum(p => p.TICKET_KSQty),
                                  TICKET_LDQty = gr.Sum(p => p.TICKET_LDQty) == 0 ? null : gr.Sum(p => p.TICKET_LDQty),
                                  TICKET_BHQty = gr.Sum(p => p.TICKET_BHQty) == 0 ? null : gr.Sum(p => p.TICKET_BHQty),
                                  TICKET_HDSDQty = gr.Sum(p => p.TICKET_HDSDQty) == 0 ? null : gr.Sum(p => p.TICKET_HDSDQty),
                                  MissionQty = gr.Sum(p => p.MissionQty) == 0 ? null : gr.Sum(p => p.MissionQty),
                                  ActivitiesQty = gr.Sum(p => p.ActivitiesQty) == 0 ? null : gr.Sum(p => p.ActivitiesQty),
                                  Total = gr.Sum(p => p.Total)
                              }).OrderBy(p => p.DepartmentName).ThenByDescending(p => p.Total).ToList();
            }
            else
            {
                resultData = data;
            }
            var totalModel = new CustomerCareActivitiesViewModel
            {
                DepartmentName = "Tổng",
                AppointmentQty = resultData.Sum(p => p.AppointmentQty),
                THKH_SpecKHQty = resultData.Sum(p => p.THKH_SpecKHQty),
                THKH_KhaoSatLdDTBQty = resultData.Sum(p => p.THKH_KhaoSatLdDTBQty),
                THKH_SpecDTBQty = resultData.Sum(p => p.THKH_SpecDTBQty),
                THKH_Khac = resultData.Sum(p => p.THKH_Khac),
                TICKET_XLKNQty = resultData.Sum(p => p.TICKET_XLKNQty),
                TICKET_KVQty = resultData.Sum(p => p.TICKET_KVQty),
                TICKET_KSQty = resultData.Sum(p => p.TICKET_KSQty),
                TICKET_LDQty = resultData.Sum(p => p.TICKET_LDQty),
                TICKET_BHQty = resultData.Sum(p => p.TICKET_BHQty),
                TICKET_HDSDQty = resultData.Sum(p => p.TICKET_HDSDQty),
                MissionQty = resultData.Sum(p => p.MissionQty),
                ActivitiesQty = resultData.Sum(p => p.ActivitiesQty),
                Total = resultData.Sum(p => p.Total)
            };
            resultData.Add(totalModel);

            return Export(resultData, searchViewModel.ViewTotal);
        }


        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<CustomerCareActivitiesViewModel> viewModel, bool isViewTotal)
        {
            #region Master
            List<ExcelTemplate> columns = new List<ExcelTemplate>();

            columns.Add(new ExcelTemplate { ColumnName = "SalesEmployeeName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "DepartmentName", isAllowedToEdit = false });
            if (!isViewTotal)
            {
                columns.Add(new ExcelTemplate { ColumnName = "StartDate", isAllowedToEdit = false, isDateTime = true });
            }
            columns.Add(new ExcelTemplate { ColumnName = "AppointmentQty", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "THKH_SpecKHQty", isAllowedToEdit = false, MergeHeaderTitle = "Thăm hỏi KH" });
            columns.Add(new ExcelTemplate { ColumnName = "THKH_KhaoSatLdDTBQty", isAllowedToEdit = false, MergeHeaderTitle = "Thăm hỏi KH" });
            columns.Add(new ExcelTemplate { ColumnName = "THKH_SpecDTBQty", isAllowedToEdit = false, MergeHeaderTitle = "Thăm hỏi KH" });
            columns.Add(new ExcelTemplate { ColumnName = "THKH_Khac", isAllowedToEdit = false, MergeHeaderTitle = "Thăm hỏi KH" });
            columns.Add(new ExcelTemplate { ColumnName = "TICKET_XLKNQty", isAllowedToEdit = false, MergeHeaderTitle = "DV Sau bán hàng" });
            columns.Add(new ExcelTemplate { ColumnName = "TICKET_KVQty", isAllowedToEdit = false, MergeHeaderTitle = "DV Sau bán hàng" });
            columns.Add(new ExcelTemplate { ColumnName = "TICKET_KSQty", isAllowedToEdit = false, MergeHeaderTitle = "DV Sau bán hàng" });
            columns.Add(new ExcelTemplate { ColumnName = "TICKET_LDQty", isAllowedToEdit = false, MergeHeaderTitle = "DV Sau bán hàng" });
            columns.Add(new ExcelTemplate { ColumnName = "TICKET_BHQty", isAllowedToEdit = false, MergeHeaderTitle = "DV Sau bán hàng" });
            columns.Add(new ExcelTemplate { ColumnName = "TICKET_HDSDQty", isAllowedToEdit = false, MergeHeaderTitle = "DV Sau bán hàng" });
            columns.Add(new ExcelTemplate { ColumnName = "MissionQty", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "ActivitiesQty", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Total", isAllowedToEdit = false });


            #endregion Master

            //Header
            string fileheader = "BÁO CÁO SỐ LƯỢNG HOẠT ĐỘNG CSKH THEO NHÂN VIÊN";
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
            //if (viewTotal == true)
            //{
            //    heading.Add(new ExcelHeadingTemplate()
            //    {
            //        Content = string.Format("Tổng hợp từ {0:dd/MM/yyyy} - {1:dd/MM/yyyy}", excelFromDate, excelToDate),
            //        RowsToIgnore = 1,
            //        isWarning = false,
            //        isCode = true
            //    });
            //}

            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false, headerRowMergeCount: 1);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion

        #region Báo cáo khách hàng theo nhân viên kinh doanh

        public List<CustomerCareActivitiesViewModel> GetData(CustomerCareSearchViewModel searchModel)
        {
            try
            {
                var result = new List<CustomerCareActivitiesViewModel>();
                #region SalesEmployeeCode
                //Build your record
                var tableSalesEmployeeCodeSchema = new List<SqlMetaData>(1)
                {
                    new SqlMetaData("Code", SqlDbType.NVarChar, 100)
                }.ToArray();

                //And a table as a list of those records
                var tableSalesEmployeeCode = new List<SqlDataRecord>();
                List<string> salesEmployeeCodeLst = new List<string>();
                if (searchModel.SalesEmployeeCode != null && searchModel.SalesEmployeeCode.Count > 0)
                {
                    foreach (var r in searchModel.SalesEmployeeCode)
                    {
                        var tableRow = new SqlDataRecord(tableSalesEmployeeCodeSchema);
                        tableRow.SetString(0, r);
                        if (!salesEmployeeCodeLst.Contains(r))
                        {
                            salesEmployeeCodeLst.Add(r);
                            tableSalesEmployeeCode.Add(tableRow);
                        }
                    }
                }
                else
                {
                    var tableRow = new SqlDataRecord(tableSalesEmployeeCodeSchema);
                    tableSalesEmployeeCode.Add(tableRow);
                }
                #endregion

                #region WorkflowCategoryCode
                //Build your record
                //var tableWorkflowCategoryCodeSchema = new List<SqlMetaData>(1)
                //    {
                //        new SqlMetaData("Code", SqlDbType.NVarChar, 100)
                //    }.ToArray();

                //And a table as a list of those records
                var tableWorkflowCategoryCode = new List<SqlDataRecord>();
                List<string> workflowCategoryCodeLst = new List<string>();
                if (searchModel.WorkFlowCategoryCode != null && searchModel.WorkFlowCategoryCode.Count > 0)
                {
                    foreach (var r in searchModel.WorkFlowCategoryCode)
                    {
                        var tableRow = new SqlDataRecord(tableSalesEmployeeCodeSchema);
                        tableRow.SetString(0, r);
                        if (!workflowCategoryCodeLst.Contains(r))
                        {
                            workflowCategoryCodeLst.Add(r);
                            tableWorkflowCategoryCode.Add(tableRow);
                        }
                    }
                }
                else
                {
                    var tableRow = new SqlDataRecord(tableSalesEmployeeCodeSchema);
                    tableWorkflowCategoryCode.Add(tableRow);
                }
                #endregion

                #region Department
                //Build your record
                var tableDepartmentSchema = new List<SqlMetaData>(1)
                    {
                        new SqlMetaData("Id", SqlDbType.UniqueIdentifier)
                    }.ToArray();

                //And a table as a list of those records
                var tableDepartmentList = new List<SqlDataRecord>();
                //List<Guid> departmentIdLst = new List<Guid>();
                if (searchModel.DepartmentId != null && searchModel.DepartmentId.Count > 0)
                {
                    foreach (var d in searchModel.DepartmentId)
                    {
                        var dId = (Guid)d;
                        var tableRow = new SqlDataRecord(tableDepartmentSchema);
                        tableRow.SetSqlGuid(0, dId);
                        tableDepartmentList.Add(tableRow);

                    }
                }
                else
                {
                    var tableRow = new SqlDataRecord(tableDepartmentSchema);
                    tableDepartmentList.Add(tableRow);
                }
                #endregion

                DataSet ds = new DataSet();
                string constr = ConfigurationManager.ConnectionStrings["cnStr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Report.usp_CustomerCareCountReport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            var tableWork = sda.SelectCommand.Parameters.AddWithValue("@WorkflowCategoryCode", tableWorkflowCategoryCode);
                            tableWork.SqlDbType = SqlDbType.Structured;
                            tableWork.TypeName = "[dbo].[StringList]";
                            var tableSalesEmp = sda.SelectCommand.Parameters.AddWithValue("@SalesEmployeeCode", tableSalesEmployeeCode);
                            tableSalesEmp.SqlDbType = SqlDbType.Structured;
                            tableSalesEmp.TypeName = "[dbo].[StringList]";
                            sda.SelectCommand.Parameters.AddWithValue("@CompanyId", searchModel.CompanyId ?? (object)DBNull.Value);
                            var tableDepartment = sda.SelectCommand.Parameters.AddWithValue("@DepartmentId", tableDepartmentList);
                            tableDepartment.SqlDbType = SqlDbType.Structured;
                            tableDepartment.TypeName = "[dbo].[GuidList]";
                            sda.SelectCommand.Parameters.AddWithValue("@VisitTypeCode", searchModel.VisitTypeCode ?? (object)DBNull.Value);
                            sda.SelectCommand.Parameters.AddWithValue("@FromDate", searchModel.StartFromDate ?? (object)DBNull.Value);
                            sda.SelectCommand.Parameters.AddWithValue("@ToDate", searchModel.StartToDate ?? (object)DBNull.Value);

                            sda.Fill(ds);
                            var dt = ds.Tables[0];

                            if (dt != null && dt.Rows.Count > 0)
                            {
                                foreach (DataRow item in dt.Rows)
                                {
                                    var model = new CustomerCareActivitiesViewModel();
                                    //NV Kinh doanh
                                    model.SalesEmployeeName = item["SalesEmployeeName"].ToString();
                                    //Phòng Ban
                                    model.DepartmentName = item["DepartmentName"].ToString();
                                    //Ngày thực hiện
                                    if (!string.IsNullOrEmpty(item["StartDate"].ToString()))
                                    {
                                        model.StartDate = Convert.ToDateTime(item["StartDate"].ToString());
                                    }
                                    //Khách ghé thăm
                                    if (!string.IsNullOrEmpty(item["AppointmentQty"].ToString()))
                                    {
                                        model.AppointmentQty = Convert.ToInt32(item["AppointmentQty"].ToString());
                                    }
                                    //Spec, Chăm sóc KH
                                    if (!string.IsNullOrEmpty(item["THKH_SpecKHQty"].ToString()))
                                    {
                                        model.THKH_SpecKHQty = Convert.ToInt32(item["THKH_SpecKHQty"].ToString());
                                    }
                                    //Khảo sát-Lắp đặt ĐTB
                                    if (!string.IsNullOrEmpty(item["THKH_KhaoSatLdDTBQty"].ToString()))
                                    {
                                        model.THKH_KhaoSatLdDTBQty = Convert.ToInt32(item["THKH_KhaoSatLdDTBQty"].ToString());
                                    }
                                    //Chăm sóc ĐTB
                                    if (!string.IsNullOrEmpty(item["THKH_SpecDTBQty"].ToString()))
                                    {
                                        model.THKH_SpecDTBQty = Convert.ToInt32(item["THKH_SpecDTBQty"].ToString());
                                    }
                                    //Thăm hỏi khác
                                    if (!string.IsNullOrEmpty(item["THKH_Khac"].ToString()))
                                    {
                                        model.THKH_Khac = Convert.ToInt32(item["THKH_Khac"].ToString());
                                    }
                                    //Xư lý Khiếu nại
                                    if (!string.IsNullOrEmpty(item["TICKET_XLKNQty"].ToString()))
                                    {
                                        model.TICKET_XLKNQty = Convert.ToInt32(item["TICKET_XLKNQty"].ToString());
                                    }
                                    //Kiểm Ván
                                    if (!string.IsNullOrEmpty(item["TICKET_KVQty"].ToString()))
                                    {
                                        model.TICKET_KVQty = Convert.ToInt32(item["TICKET_KVQty"].ToString());
                                    }
                                    //Khảo sát
                                    if (!string.IsNullOrEmpty(item["TICKET_KSQty"].ToString()))
                                    {
                                        model.TICKET_KSQty = Convert.ToInt32(item["TICKET_KSQty"].ToString());
                                    }
                                    //Lắp đặt
                                    if (!string.IsNullOrEmpty(item["TICKET_LDQty"].ToString()))
                                    {
                                        model.TICKET_LDQty = Convert.ToInt32(item["TICKET_LDQty"].ToString());
                                    }
                                    //Bảo hành
                                    if (!string.IsNullOrEmpty(item["TICKET_BHQty"].ToString()))
                                    {
                                        model.TICKET_BHQty = Convert.ToInt32(item["TICKET_BHQty"].ToString());
                                    }
                                    //HDSD
                                    if (!string.IsNullOrEmpty(item["TICKET_HDSDQty"].ToString()))
                                    {
                                        model.TICKET_HDSDQty = Convert.ToInt32(item["TICKET_HDSDQty"].ToString());
                                    }
                                    //Giao Việc
                                    if (!string.IsNullOrEmpty(item["MissionQty"].ToString()))
                                    {
                                        model.MissionQty = Convert.ToInt32(item["MissionQty"].ToString());
                                    }
                                    //Số Nhiệm vụ
                                    if (!string.IsNullOrEmpty(item["ActivitiesQty"].ToString()))
                                    {
                                        model.ActivitiesQty = Convert.ToInt32(item["ActivitiesQty"].ToString());
                                    }
                                    if (!string.IsNullOrEmpty(item["Total"].ToString()))
                                    {
                                        model.Total = Convert.ToInt32(item["Total"].ToString());
                                    }
                                    result.Add(model);
                                }
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Báo cáo khách hàng theo nhân viên kinh doanh
        public ActionResult ViewDetail(CustomerCareSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "CustomerCareActivitiesSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "CustomerCareActivitiesTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "CustomerCareActivitiesModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, CustomerCareSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "CustomerCareActivitiesSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "CustomerCareActivitiesTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "CustomerCareActivitiesModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult CustomerCarePivotGridPartial(Guid? templateId = null, CustomerCareSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/CustomerCareActivities");
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
                return PartialView("_CustomerCarePivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<CustomerCareSearchViewModel>(jsonReq);
                }
                if (searchViewModel.StartCommonDate != "Custom")
                {
                    DateTime? fromDate;
                    DateTime? toDate;
                    _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.StartCommonDate, out fromDate, out toDate);
                    //Tìm kiếm kỳ hiện tại
                    searchViewModel.StartFromDate = fromDate;
                    searchViewModel.StartToDate = toDate;
                }
                if (searchViewModel.StartToDate != null)
                {
                    searchViewModel.StartToDate = searchViewModel.StartToDate.Value.AddDays(1).AddSeconds(-1);
                }

                //var data = GetQuantyCusCareWithPersonInChargeReport(searchViewModel, CurrentUser.CompanyCode);
                var resultData = new List<CustomerCareActivitiesViewModel>();
                var data = GetData(searchViewModel);
                if (searchViewModel.ViewTotal == true)
                {
                    resultData = (from p in data
                                  group p by new { p.SalesEmployeeName, p.DepartmentName } into gr
                                  select new CustomerCareActivitiesViewModel
                                  {
                                      SalesEmployeeName = gr.Key.SalesEmployeeName,
                                      DepartmentName = gr.Key.DepartmentName,
                                      AppointmentQty = gr.Sum(p => p.AppointmentQty) == 0 ? null : gr.Sum(p => p.AppointmentQty),
                                      THKH_SpecKHQty = gr.Sum(p => p.THKH_SpecKHQty) == 0 ? null : gr.Sum(p => p.THKH_SpecKHQty),
                                      THKH_KhaoSatLdDTBQty = gr.Sum(p => p.THKH_KhaoSatLdDTBQty) == 0 ? null : gr.Sum(p => p.THKH_KhaoSatLdDTBQty),
                                      THKH_SpecDTBQty = gr.Sum(p => p.THKH_SpecDTBQty) == 0 ? null : gr.Sum(p => p.THKH_SpecDTBQty),
                                      THKH_Khac = gr.Sum(p => p.THKH_Khac) == 0 ? null : gr.Sum(p => p.THKH_Khac),
                                      TICKET_XLKNQty = gr.Sum(p => p.TICKET_XLKNQty) == 0 ? null : gr.Sum(p => p.TICKET_XLKNQty),
                                      TICKET_KVQty = gr.Sum(p => p.TICKET_KVQty) == 0 ? null : gr.Sum(p => p.TICKET_KVQty),
                                      TICKET_KSQty = gr.Sum(p => p.TICKET_KSQty) == 0 ? null : gr.Sum(p => p.TICKET_KSQty),
                                      TICKET_LDQty = gr.Sum(p => p.TICKET_LDQty) == 0 ? null : gr.Sum(p => p.TICKET_LDQty),
                                      TICKET_BHQty = gr.Sum(p => p.TICKET_BHQty) == 0 ? null : gr.Sum(p => p.TICKET_BHQty),
                                      TICKET_HDSDQty = gr.Sum(p => p.TICKET_HDSDQty) == 0 ? null : gr.Sum(p => p.TICKET_HDSDQty),
                                      MissionQty = gr.Sum(p => p.MissionQty) == 0 ? null : gr.Sum(p => p.MissionQty),
                                      ActivitiesQty = gr.Sum(p => p.ActivitiesQty) == 0 ? null : gr.Sum(p => p.ActivitiesQty),
                                      Total = gr.Sum(p => p.Total)
                                  }).OrderBy(p => p.DepartmentName).ThenByDescending(p => p.Total).ToList();
                    ViewBag.Search = searchViewModel;
                    return PartialView("_CustomerCarePivotGridPartial", resultData);
                }
                else
                {
                    resultData = data;
                    ViewBag.Search = searchViewModel;
                    return PartialView("_CustomerCarePivotGridPartial", resultData);
                }
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(CustomerCareSearchViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_SO_LUONG_HOAT_DONG_CSKH_THEO_NHAN_VIEN";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }

        //public ActionResult CustomerCarePivotGridPartial()
        //{
        //    var searchViewModel = (CustomerCareSearchViewModel)Session["CustomerCareSearch"];
        //    #region //Start Date
        //    if (searchViewModel.StartCommonDate != "Custom")
        //    {
        //        DateTime? fromDate;
        //        DateTime? toDate;
        //        _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.StartCommonDate, out fromDate, out toDate);
        //        //Tìm kiếm kỳ hiện tại
        //        searchViewModel.StartFromDate = fromDate;
        //        searchViewModel.StartToDate = toDate;
        //    }
        //    if (searchViewModel.StartToDate != null)
        //    {
        //        searchViewModel.StartToDate = searchViewModel.StartToDate.Value.AddDays(1).AddSeconds(-1);
        //    }
        //    #endregion

        //    //Nếu không chọn nhân viên nào thì add tất cả nhân viên
        //    if (searchViewModel.SalesEmployeeCode == null || searchViewModel.SalesEmployeeCode.Count == 0)
        //    {
        //        var salesEmployeeCodeList = _context.SalesEmployeeModel.Where(p => p.Actived == true).Select(p => p.SalesEmployeeCode).ToList();
        //        searchViewModel.SalesEmployeeCode = new List<string>();
        //        searchViewModel.SalesEmployeeCode.AddRange(salesEmployeeCodeList);
        //    }

        //    //Nếu không chọn loại hoạt động nào thì thêm 5 loại hoạt động
        //    if (searchViewModel.WorkFlowCategoryCode == null || searchViewModel.WorkFlowCategoryCode.Count == 0)
        //    {
        //        searchViewModel.WorkFlowCategoryCode = new List<string>();
        //        var workFlowCategoryList = _context.WorkFlowCategoryModel.Where(p => p.ReportType.Contains("CustomerCareActivities"))
        //           .Select(p => p.WorkFlowCategoryCode)
        //           .ToList();
        //        if (workFlowCategoryList != null && workFlowCategoryList.Count > 0)
        //        {
        //            searchViewModel.WorkFlowCategoryCode.AddRange(workFlowCategoryList);
        //        }
        //    }
        //    //Nêu không chọn phòng ban thì gửi đi tất cả phòng ban
        //    if (searchViewModel.DepartmentId == null || searchViewModel.DepartmentId.Count == 0)
        //    {
        //        var departmentList = _context.DepartmentModel.Where(p => p.Actived == true).Select(p => p.DepartmentId).ToList();
        //        searchViewModel.DepartmentId = new List<Guid>();
        //        searchViewModel.DepartmentId.AddRange(departmentList);
        //    }
        //    //var data = GetQuantyCusCareWithPersonInChargeReport(searchViewModel, CurrentUser.CompanyCode);
        //    var resultData = new List<CustomerCareActivitiesViewModel>();
        //    var data = GetData(searchViewModel);
        //    if (searchViewModel.ViewTotal == true)
        //    {
        //        resultData = (from p in data
        //                      group p by new { p.SalesEmployeeName, p.DepartmentName } into gr
        //                      select new CustomerCareActivitiesViewModel
        //                      {
        //                          SalesEmployeeName = gr.Key.SalesEmployeeName,
        //                          DepartmentName = gr.Key.DepartmentName,
        //                          QtyGheTham = gr.Sum(p => p.QtyGheTham),
        //                          QtyTHKH = gr.Sum(p => p.QtyTHKH),
        //                          QtyBaoHanh = gr.Sum(p => p.QtyBaoHanh),
        //                          QtyXLKN = gr.Sum(p => p.QtyXLKN),
        //                          QtyNhiemVu = gr.Sum(p => p.QtyNhiemVu),
        //                          Total = gr.Sum(p => p.Total)
        //                      }).OrderBy(p => p.DepartmentName).ThenByDescending(p => p.Total).ToList();
        //        return PartialView("_CustomerCarePivotGridPartial", resultData);
        //    }
        //    else
        //    {
        //        resultData = data;
        //        return PartialView("_CustomerCarePivotGridPartial", resultData);
        //    }
        //}
    }
}