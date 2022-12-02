using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Report;
using ISD.ViewModels;
using ISD.ViewModels.Reports;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class WorkShopProductionProgressReportController : BaseController
    {
        // GET: WorkShopProductionProgressReport
        #region Index
        
        public ActionResult Index()
        {
            var searchViewModel = new WorkShopProductionProgressReportSearchViewModel()
            { 
                FromDate = DateTime.Now,
                ToDate = DateTime.Now
            
            };

            var searchModel = (WorkShopProductionProgressReportSearchViewModel)TempData[CurrentUser.AccountId + "WorkShopProductionProgressReportData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "WorkShopProductionProgressReportId"];
            var modeSearch = TempData[CurrentUser.AccountId + "WorkShopProductionProgressReportSearch"];
            var pageId = GetPageId("/Reports/WorkShopProductionProgressReport");

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


        #endregion

        [ValidateInput(false)]
        public ActionResult WorkShopProductionProgressReportPartial(Guid? templateId = null, WorkShopProductionProgressReportSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/WorkShopProductionProgressReport");
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
                return PartialView("_WorkShopProductionProgressReport", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<WorkShopProductionProgressReportSearchViewModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_WorkShopProductionProgressReport", model);
            }
        }


        private List<WorkShopProductionProgressReportViewModel> GetData(WorkShopProductionProgressReportSearchViewModel searchViewModel)
        {
            var reponsitory = new WorkShopProductionProgressRepository(_context);
            var data = reponsitory.WorkshopProductionProgress();
          
           
            //if (searchViewModel.ToDate.HasValue)
            //{
            //    searchViewModel.ToDate = searchViewModel.ToDate.Value.Date.AddDays(1).AddSeconds(-1);
            //}
            //if (searchViewModel.StoreId == null || searchViewModel.StoreId.Count == 0)
            //{
            //    var storeList = _unitOfWork.StoreRepository.GetStoreByPermission(CurrentUser.AccountId);
            //    if (storeList != null && storeList.Count > 0)
            //    {
            //        searchViewModel.StoreId = storeList.Select(p => p.StoreId).ToList();
            //    }
            //}
            //CustomerTastesSearchViewModel searchModel = new CustomerTastesSearchViewModel();
            //searchModel.FromDate = searchViewModel.FromDate;
            //searchModel.ToDate = searchViewModel.ToDate;
            //searchModel.SaleEmployeeCode = searchViewModel.SaleEmployeeCode;
            //searchModel.StoreId = searchViewModel.StoreId;
            //data = _unitOfWork.CustomerTasteRepository.GetCustomerTastesReport(searchModel).ToList();
            return data;
        }


        public ActionResult ChangeTemplate(Guid pivotTemplate, WorkShopProductionProgressReportSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "WorkShopProductionProgressReportData"] = searchModel;
            TempData[CurrentUser.AccountId + "WorkShopProductionProgressReportId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "WorkShopProductionProgressReportSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ViewDetail(WorkShopProductionProgressReportSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "WorkShopProductionProgressReportData"] = searchModel;
            TempData[CurrentUser.AccountId + "WorkShopProductionProgressReportId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "WorkShopProductionProgressReportSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

    }
}