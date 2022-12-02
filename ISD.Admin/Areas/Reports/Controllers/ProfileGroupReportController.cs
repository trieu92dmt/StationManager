using DevExpress.Web.Mvc;
using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories;
using ISD.Repositories.Excel;
using ISD.ViewModels;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class ProfileGroupReportController : BaseController
    {
        // GET: ProfileGroupReport
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (ProfileGroupSearchModel)TempData[CurrentUser.AccountId + "ProfileGroupSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "ProfileGroupTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "ProfileGroupModeSearch"];
            var pageId = GetPageId("/Reports/ProfileGroupReport");

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
            CreateViewBag();
            return View();
        }
        public void CreateViewBag()
        {
            var _catalogRepository = new CatalogRepository(_context);
            #region //Get list CustomerGroup (Nhóm khách hàng doanh nghiệp)
            var customerGroupList = _catalogRepository.GetCustomerCategory(CurrentUser.CompanyCode);
            ViewBag.ProfileGroupCode = new SelectList(customerGroupList, "CatalogCode", "CatalogText_vi");
            #endregion
        }
        #endregion

        #region Export Excel
        const int startIndex = 8;

        public ActionResult ExportExcel(ProfileGroupSearchModel searchModel)
        {
            var data = GetData(searchModel);
            var modelTotal = new ProfileGroupReportViewModel
            {
                ProfileGroupName = "Tổng",
                NumberOfProfiles = data.Sum(p => p.NumberOfProfiles),
                PercentOfProfiles = data.Sum(p => p.PercentOfProfiles)
            };
            data.Add(modelTotal);
            return Export(data);
        }
        private List<ProfileGroupReportViewModel> GetData(ProfileGroupSearchModel searchModel)
        {
            if (searchModel.ProfileGroupCode != null && searchModel.ProfileGroupCode.Count > 0)
            {
                var firstProfileGroupCode = searchModel.ProfileGroupCode[0];
                if (string.IsNullOrEmpty(firstProfileGroupCode))
                {
                    searchModel.ProfileGroupCode = new List<string>();
                    searchModel.ProfileGroupCode = (from p in _context.CatalogModel
                                                    where p.CatalogTypeCode == ConstCatalogType.CustomerGroup && p.Actived == true
                                                    && p.CatalogText_en.Contains(CurrentUser.CompanyCode)
                                                    select p.CatalogCode).ToList();
                }
            }
            else
            {
                searchModel.ProfileGroupCode = new List<string>();
                searchModel.ProfileGroupCode = (from p in _context.CatalogModel
                                                where p.CatalogTypeCode == ConstCatalogType.CustomerGroup && p.Actived == true
                                                && p.CatalogText_en.Contains(CurrentUser.CompanyCode)
                                                select p.CatalogCode).ToList();
            }

            #region ProfileGroupCode
            //Build your record
            var tableProfileGroupSchema = new List<SqlMetaData>(1)
            {
                new SqlMetaData("StringList", SqlDbType.NVarChar, 50)
            }.ToArray();

            //And a table as a list of those records
            var tableProfileGroup = new List<SqlDataRecord>();
            if (searchModel.ProfileGroupCode != null && searchModel.ProfileGroupCode.Count > 0)
            {
                foreach (var r in searchModel.ProfileGroupCode)
                {
                    var tableRow = new SqlDataRecord(tableProfileGroupSchema);
                    tableRow.SetString(0, r);
                    tableProfileGroup.Add(tableRow);
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableProfileGroupSchema);
                tableProfileGroup.Add(tableRow);
            }
            #endregion

            string sqlQuery = "EXEC [Report].[usp_ProfileGroupReport] @CustomerGroupCode, @CurrentCompanyCode";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CustomerGroupCode",
                    TypeName = "[dbo].[StringList]", //Don't forget this one!
                    Value = tableProfileGroup
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CurrentCompanyCode",
                    Value = CurrentUser.CompanyCode,
                },
            };
            var result = _context.Database.SqlQuery<ProfileGroupReportViewModel>(sqlQuery, parameters.ToArray()).ToList();

            
            return result;
        }
        public FileContentResult Export(List<ProfileGroupReportViewModel> viewModel)
        {
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            //Header
            string fileheader = string.Empty;
            fileheader = "BÁO CÁO SỐ LƯỢNG KHÁCH HÀNG THEO NHÓM KH";
            columns.Add(new ExcelTemplate { ColumnName = "ProfileGroupName", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "NumberOfProfiles", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "PercentOfProfiles", isAllowedToEdit = false });
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
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false, IsMergeCellHeader: false);
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion
        public ActionResult ViewDetail(ProfileGroupSearchModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "ProfileGroupSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "ProfileGroupTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "ProfileGroupModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, ProfileGroupSearchModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "ProfileGroupSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "ProfileGroupTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "ProfileGroupModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }


        [ValidateInput(false)]
        public ActionResult ProfileGroupPivotGridPartial(Guid? templateId = null, ProfileGroupSearchModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/ProfileGroupReport");
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
                return PartialView("_ProfileGroupPivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<ProfileGroupSearchModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_ProfileGroupPivotGridPartial", model);
            }
        }
        [HttpPost]
        public ActionResult ExportPivot(ProfileGroupSearchModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;
            var model = GetData(searchViewModel);

            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            string fileName = "BAO_CAO_SO_LUONG_KHACH_HANG_THEO_NHOM_KH";
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model);
        }
    }
}