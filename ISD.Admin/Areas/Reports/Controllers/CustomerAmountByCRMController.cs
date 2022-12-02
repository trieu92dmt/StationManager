using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories;
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
    public class CustomerAmountByCRMController : BaseController
    {
        // GET: CustomerAmountByCRM
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (CustomerAmountByCRMSearchViewModel)TempData[CurrentUser.AccountId + "CustomerAmountByCRMSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "CustomerAmountByCRMTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "CustomerAmountByCRMModeSearch"];
            var pageId = GetPageId("/Reports/CustomerAmountByCRM");
            
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
            var customerTypeList = _context.CatalogModel.Where(
                p => p.CatalogTypeCode == ConstCatalogType.CustomerType
                && p.CatalogCode != ConstCustomerType.Contact
                && p.Actived == true).OrderBy(p => p.OrderIndex).ToList();

            ViewBag.CustomerTypeCode = new SelectList(customerTypeList, "CatalogCode", "CatalogText_vi");

            var _catalogRepository = new CatalogRepository(_context);
            var customerGroupList = _catalogRepository.GetCustomerCategory(CurrentUser.CompanyCode);
            ViewBag.CustomerGroupCode = new SelectList(customerGroupList, "CatalogCode", "CatalogText_vi");


            //CommonDate
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi");


            return View();
        }

        public ActionResult ExportExcel(CustomerAmountByCRMSearchViewModel searchViewModel)
        {
            //DateTime? excelFromDate = searchViewModel.FromDate;
           // DateTime? excelToDate = searchViewModel.ToDate;
           // DateTime? excelRatioFromDate, excelRatioToDate;
            var data = GetData(searchViewModel);
            return Export(data);
        }

        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<CustomerAmountByCRMViewModel> viewModel)
        {
            #region Master
            List<ExcelTemplate> columns = new List<ExcelTemplate>();

            columns.Add(new ExcelTemplate { ColumnName = "CustomerTypeName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "CustomerGroupName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "QtyECC", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "QtyCRM", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "Total", isAllowedToEdit = false });
            #endregion Master

            //Header
            string fileheader = "BÁO CÁO SỐ LƯỢNG KHÁCH HÀNG THEO CRM/ECC";
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
            //if (!string.IsNullOrEmpty(CommonDate) && CommonDate != "Custom")
            //{
            //    var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate).ToList();
            //    var commonDateStr = commonDateList.Where(p => p.CatalogCode == CommonDate).Select(p => p.CatalogText_vi).FirstOrDefault();
            //    heading.Add(new ExcelHeadingTemplate()
            //    {
            //        Content = string.Format("Tỷ lệ: {0} ({1:dd/MM/yyyy}-{2:dd/MM/yyyy} so với {3:dd/MM/yyyy}-{4:dd/MM/yyyy})", commonDateStr, excelFromDate, excelToDate, excelRatioFromDate, excelRatioToDate),
            //        RowsToIgnore = 1,
            //        isWarning = false,
            //        isCode = true
            //    });
            //}

            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false, IsMergeCellHeader: false);

            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }

        private List<CustomerAmountByCRMViewModel> GetData(CustomerAmountByCRMSearchViewModel searchModel)
        {
            string sqlQuery = "EXEC Report.usp_CustomerAmountByCRMReport @CustomerTypeCode, @CustomerGroupCode, @CurrentCompanyCode, @FromDate, @ToDate";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerTypeCode",
                    Value = searchModel.CustomerTypeCode ?? (object)DBNull.Value
                },new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerGroupCode",
                    Value = searchModel.CustomerGroupCode ?? (object)DBNull.Value
                },new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CurrentCompanyCode",
                    Value = CurrentUser.CompanyCode
                },
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

            var data = _context.Database.SqlQuery<CustomerAmountByCRMViewModel>(sqlQuery, parameters.ToArray()).ToList();

            return data;
        }
        public ActionResult ViewDetail(CustomerAmountByCRMSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "CustomerAmountByCRMSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "CustomerAmountByCRMTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "CustomerAmountByCRMModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, CustomerAmountByCRMSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "CustomerAmountByCRMSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "CustomerAmountByCRMTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "CustomerAmountByCRMModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult CustomerAmountByCRMGridPartial(Guid? templateId = null, CustomerAmountByCRMSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/CustomerAmountByCRM");
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
                return PartialView("_CustomerAmountByCRMPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<CustomerAmountByCRMSearchViewModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_CustomerAmountByCRMPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(CustomerAmountByCRMSearchViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false; 
            var model = GetData(searchViewModel);

            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_SO_LUONG_KHACH_THEO_CRM_ECC";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}