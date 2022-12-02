using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class MaterialExportedReportController : BaseController
    {
        // GET: MaterialExportedReport
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (MaterialExportedSearchModel)TempData[CurrentUser.AccountId + "MaterialExportedSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "MaterialExportedTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "MaterialExportedModeSearch"];
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
            var pageId = GetPageId("/Reports/MaterialExportedReport");
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
            DateTime? fromDate, toDate;
            var CommonDate = "ThisMonth";
            _unitOfWork.CommonDateRepository.GetDateBy(CommonDate, out fromDate, out toDate);
            MaterialExportedSearchModel searchViewModel = new MaterialExportedSearchModel()
            {
                //Ngày ghé thăm
                CommonDate = CommonDate,
                FromDate = fromDate,
                ToDate = toDate,
            };

            //CommonDate
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", CommonDate);
            return View(searchViewModel);
        }


        #region Export to Excel
        public ActionResult ExportExcel(MaterialExportedSearchModel searchViewModel)
        {
            
            
            var data = GetData(searchViewModel);
            
            return Export(data);
        }

        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<MaterialExportedViewModel> viewModel)
        {
            #region Master
            List<ExcelTemplate> columns = new List<ExcelTemplate>();

            columns.Add(new ExcelTemplate { ColumnName = "AreaName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "DepartmentName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "QtyGVL", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "QtyKeCTL", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "QtyKeMau", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "QtyBasInox", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "QtyBangHieu", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "QtyKhayA5", isAllowedToEdit = false });
            #endregion Master

            //Header
            string fileheader = "BÁO CÁO VẬT TƯ ĐÃ XUẤT CHO GÓC VẬT LIỆU";

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
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false, IsMergeCellHeader: false);
       
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion

        #region GetData for Report
        private List<MaterialExportedViewModel> GetData(MaterialExportedSearchModel searchModel)
        {
            if (searchModel.CommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;

                _unitOfWork.CommonDateRepository.GetDateBy(searchModel.CommonDate, out fromDate, out toDate);
                searchModel.FromDate = fromDate;
                searchModel.ToDate = toDate;
            }
            string sqlQuery = "EXEC Report.usp_MaterialExportGVL @FromDate, @ToDate";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "FromDate",
                    Value = searchModel.FromDate ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ToDate",
                    Value = searchModel.ToDate ?? (object)DBNull.Value
                }
            };

            var data = _context.Database.SqlQuery<MaterialExportedViewModel>(sqlQuery, parameters.ToArray()).ToList();
            if (data != null && data.Count > 0)
            {
                foreach (var item in data)
                {
                    
                    item.DepartmentName = GetDepartmentName(item.ProfileId);
                    if(!string.IsNullOrEmpty(item.SaleOfficeCode))
                    {
                        item.AreaName = _context.CatalogModel.FirstOrDefault(p => p.CatalogCode == item.SaleOfficeCode && p.CatalogTypeCode == ConstCatalogType.SaleOffice).CatalogText_vi;
                    }            
                }
            }

            var newData = (from p in data
                           group p by new { p.AreaName, p.DepartmentName } into gr
                           select new MaterialExportedViewModel
                           {
                               AreaName = gr.Key.AreaName,
                               DepartmentName = gr.Key.DepartmentName,
                               QtyGVL = gr.Sum(p => p.QtyGVL),
                               QtyKeCTL = gr.Sum(p => p.QtyKeCTL),
                               QtyKeMau = gr.Sum(p => p.QtyKeMau),
                               QtyBasInox = gr.Sum(p => p.QtyBasInox),
                               QtyBangHieu = gr.Sum(p => p.QtyBangHieu),
                               QtyKhayA5 = gr.Sum(p=>p.QtyKhayA5)
                           }).OrderBy(p => p.AreaName).ToList();
            return newData;
        }
        #endregion

        private string GetDepartmentName(Guid? ProfileId)
        {
            var departmentName = string.Empty;
            //Nhân viên kinh doanh
            var saleSupervisorList = (from p in _context.PersonInChargeModel
                                       join acc in _context.AccountModel on p.SalesEmployeeCode equals acc.EmployeeCode
                                       from ro in acc.RolesModel
                                       where p.ProfileId == ProfileId && p.CompanyCode == CurrentUser.CompanyCode
                                       select new
                                       {
                                           SalesEmployeeCode = p.SalesEmployeeCode,
                                           DepartmentName = ro.RolesName,
                                           isEmployeeGroup = ro.isEmployeeGroup
                                       }).ToList();
            //Lấy 1 thông tin phòng ban NV kinh doanh
            if (saleSupervisorList != null && saleSupervisorList.Count > 0)
            {
                var SaleSupervisor = saleSupervisorList.Where(p => p.isEmployeeGroup == true).FirstOrDefault();
                if (SaleSupervisor == null)
                {
                    SaleSupervisor = saleSupervisorList.FirstOrDefault();
                }
                departmentName = SaleSupervisor.isEmployeeGroup == true ? SaleSupervisor.DepartmentName : "";
            }

            return departmentName;
        }
        public ActionResult ViewDetail(MaterialExportedSearchModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "MaterialExportedSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "MaterialExportedTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "MaterialExportedModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, MaterialExportedSearchModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "MaterialExportedSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "MaterialExportedTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "MaterialExportedModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult MaterialExportedPivotGridPartial(Guid? templateId = null, MaterialExportedSearchModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/MaterialExportedReport");
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
                return PartialView("_MaterialExportedPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<MaterialExportedSearchModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_MaterialExportedPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(MaterialExportedSearchModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_VAT_TU_DA_XUAT_CHO_GOC_VAT_LIEU";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}