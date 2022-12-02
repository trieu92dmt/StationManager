using ISD.Core;
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
    public class ReportOfExpectedMaterialController : BaseController
    {
        // GET: ReportOfExpectedMaterial
        public ActionResult Index()
        {
            var searchViewModel = new ReportOfExpectedMaterialSearchViewModel()
            {

                FromDate = DateTime.Now,
                ToDate = DateTime.Now
            };

            var searchModel = (ReportOfExpectedMaterialSearchViewModel)TempData[CurrentUser.AccountId + "ReportOfExpectedMaterialData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "ReportOfExpectedMaterialId"];
            var modeSearch = TempData[CurrentUser.AccountId + "ReportOfExpectedMaterialSearch"];
            var pageId = GetPageId("/Reports/ReportOfExpectedMaterial");

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



        public ActionResult _ReportOfExpectedMaterialPartial(Guid? templateId = null, ReportOfExpectedMaterialSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/ReportOfExpectedMaterial");
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
                return PartialView("_ReportOfExpectedMaterialPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<ReportOfExpectedMaterialSearchViewModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_ReportOfExpectedMaterialPartial", model);
            }
        }

        private List<ReportOfExpectedMaterialViewModel> GetData(ReportOfExpectedMaterialSearchViewModel searchViewModel)
        {
            var reponsitory = new ReportOfExpectedMaterialRepository(_context);
            var data = reponsitory.ReportOfExpectedMaterial();

            return data;
        }


        public ActionResult ChangeTemplate(Guid pivotTemplate, ReportOfExpectedMaterialSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "ReportOfExpectedMaterialData"] = searchModel;
            TempData[CurrentUser.AccountId + "ReportOfExpectedMaterialId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "ReportOfExpectedMaterialSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ViewDetail(ReportOfExpectedMaterialSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "ReportOfExpectedMaterialData"] = searchModel;
            TempData[CurrentUser.AccountId + "ReportOfExpectedMaterialId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "ReportOfExpectedMaterial"] = modeSearch;
            return RedirectToAction("Index");
        }


    }
}