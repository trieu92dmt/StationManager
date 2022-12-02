﻿using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.ViewModels;
using Microsoft.SqlServer.Server;
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
    public class StatisticLikeViewProductController : BaseController
    {
        // GET: StatisticLikeViewProduct
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (StatisticLikeViewSearchViewModel)TempData[CurrentUser.AccountId + "StatisticLikeViewProductSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "StatisticLikeViewProductTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "StatisticLikeViewProductModeSearch"];
            var pageId = GetPageId("/Reports/StatisticLikeViewProduct");

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
            DateTime? fromDate, toDate;
            var CommonDate = "ThisMonth";
            _unitOfWork.CommonDateRepository.GetDateBy(CommonDate, out fromDate, out toDate);
            StatisticLikeViewSearchViewModel searchViewModel = new StatisticLikeViewSearchViewModel()
            {
                //Ngày ghé thăm
                CommonDate = CommonDate,
                FromDate = fromDate,
                ToDate = toDate,
            };
            //CommonDate
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", searchViewModel.CommonDate);

            return View(searchViewModel);
        }
        public ActionResult ExportExcel(StatisticLikeViewSearchViewModel searchViewModel)
        {
            var data = new List<StatisticLikeViewProductViewModel>();
            if (searchViewModel.ToDate.HasValue)
            {
                searchViewModel.ToDate = searchViewModel.ToDate.Value.Date.AddDays(1).AddSeconds(-1);
            }
            bool? isAllStore = false;
            if (searchViewModel.StoreId == null || searchViewModel.StoreId.Count == 0)
            {
                var storeList = _unitOfWork.StoreRepository.GetStoreByPermission(CurrentUser.AccountId);
                if (storeList != null && storeList.Count > 0)
                {
                    searchViewModel.StoreId = storeList.Select(p => p.StoreId).ToList();
                    isAllStore = true;
                }
            }

            data = GetData(searchViewModel);
            return Export(data, searchViewModel, isAllStore: isAllStore);
        }
        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<StatisticLikeViewProductViewModel> viewModel, StatisticLikeViewSearchViewModel searchViewModel, bool? isAllStore = null)
        {
            #region Master
            List<ExcelTemplate> columns = new List<ExcelTemplate>();

            columns.Add(new ExcelTemplate { ColumnName = "NhomVT", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "MaSAP", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "MaSP", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "TenSP", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "SoLuotLiked", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "SoLuotViewed", isAllowedToEdit = false });

            #endregion Master

            //Header
            string fileheader = "BÁO CÁO số lượt LIKED và lượt VIEW SP";
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
                Content = string.Format("Từ ngày: {0:dd/MM/yyyy}", searchViewModel.FromDate),
                RowsToIgnore = 0,
                isWarning = false,
                isCode = true
            });
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = string.Format("Đến ngày: {0:dd/MM/yyyy}", searchViewModel.ToDate),
                RowsToIgnore = 0,
                isWarning = false,
                isCode = true
            });
            //Chi nhánh
            string Store = "- Tất cả chi nhánh -";
            if (isAllStore == null || isAllStore == false)
            {
                var storeLst = _context.StoreModel.Where(p => searchViewModel.StoreId.Contains(p.StoreId)).Select(p => p.StoreName).ToList();
                if (storeLst != null && storeLst.Count > 0)
                {
                    Store = String.Join(",", storeLst.ToArray());
                }
            }
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = string.Format("Cửa hàng: {0}", Store),
                RowsToIgnore = 0,
                isWarning = false,
                isCode = true
            });
            //Nhân viên kinh doanh
            //var SaleEmployee = "- Tất cả nhân viên -";
            //if (!string.IsNullOrEmpty(searchViewModel.SaleEmployeeCode))
            //{
            //    SaleEmployee = _context.SalesEmployeeModel.Where(p => p.SalesEmployeeCode == searchViewModel.SaleEmployeeCode).Select(p => p.SalesEmployeeName).FirstOrDefault();
            //}
            //heading.Add(new ExcelHeadingTemplate()
            //{
            //    Content = string.Format("Nhân viên kinh doanh: {0}", SaleEmployee),
            //    RowsToIgnore = 1,
            //    isWarning = false,
            //    isCode = true
            //});
            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true);
            //File name
           
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }


        private List<StatisticLikeViewProductViewModel> GetData(StatisticLikeViewSearchViewModel searchViewModel)
        {
            if (searchViewModel.StoreId == null || searchViewModel.StoreId.Count == 0)
            {
                var storeList = _unitOfWork.StoreRepository.GetStoreByPermission(CurrentUser.AccountId);
                if (storeList != null && storeList.Count > 0)
                {
                    searchViewModel.StoreId = storeList.Select(p => p.StoreId).ToList();
                }
            }
            string sqlQuery = "EXEC [Report].[usp_StatisticLikeViewProduct] @FromDate, @ToDate";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "FromDate",
                    Value = searchViewModel.FromDate ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "ToDate",
                    Value = searchViewModel.ToDate ?? (object)DBNull.Value,
                },
                

            };

            #region RolesId parameter
            //Build your record
            var tableSchema = new List<SqlMetaData>(1)
                {
                    new SqlMetaData("Id", SqlDbType.UniqueIdentifier)
                }.ToArray();

            //And a table as a list of those records
            var table = new List<SqlDataRecord>();
            if (searchViewModel.StoreId != null && searchViewModel.StoreId.Count > 0)
            {
                foreach (var r in searchViewModel.StoreId)
                {
                    var tableRow = new SqlDataRecord(tableSchema);
                    tableRow.SetGuid(0, r);
                    table.Add(tableRow);
                }
                parameters.Add(
                    new SqlParameter
                    {
                        SqlDbType = SqlDbType.Structured,
                        Direction = ParameterDirection.Input,
                        ParameterName = "StoreId",
                        TypeName = "[dbo].[GuidList]", //Don't forget this one!
                        Value = table
                    });
                sqlQuery += ", @StoreId";
            }
            #endregion

            var result = _context.Database.SqlQuery<StatisticLikeViewProductViewModel>(sqlQuery, parameters.ToArray()).ToList();
            return result;
        }
        public ActionResult ViewDetail(StatisticLikeViewSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "StatisticLikeViewProductSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "StatisticLikeViewProductTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "StatisticLikeViewProductModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, StatisticLikeViewSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "StatisticLikeViewProductSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "StatisticLikeViewProductTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "StatisticLikeViewProductModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult StatisticLikeViewProductPivotGridPartial(Guid? templateId = null, StatisticLikeViewSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/StatisticLikeViewProduct");
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
                return PartialView("_StatisticLikeViewProductPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<StatisticLikeViewSearchViewModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_StatisticLikeViewProductPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(StatisticLikeViewSearchViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "THONG_KE_SO_LUOT_LIKE_VIEW_SAN_PHAM";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}