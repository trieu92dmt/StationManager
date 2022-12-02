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
    public class SummaryProgressOfProductController : BaseController
    {
        // GET: SummaryProgressOfProducts
        public ActionResult Index()
        {
            // seaerch view model
            var searchViewModel = new SummaryProgressOfProductSearchViewModel()
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now

            };

            var searchModel = (SummaryProgressOfProductSearchViewModel)TempData[CurrentUser.AccountId + "SummaryProgressOfProductData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "SummaryProgressOfProductId"];
            var modeSearch = TempData[CurrentUser.AccountId + "SummaryProgressOfProductSearch"];
            var pageId = GetPageId("/Reports/SummaryProgressOfProduct");

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


        [ValidateInput(false)]
        public ActionResult SummaryProgressOfProductPartial(Guid? templateId = null, SummaryProgressOfProductSearchViewModel searchViewModel = null, string jsonReq = null)
        {
            var pageId = GetPageId("/Reports/SummaryProgressOfProduct");
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
                return PartialView("_SummaryProgressOfProductPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<SummaryProgressOfProductSearchViewModel>(jsonReq);
                }
                var model = GetData(searchViewModel);
                ViewBag.Search = searchViewModel;
                return PartialView("_SummaryProgressOfProductPartial", model);
            }
        }

        private List<SummaryProgressOfProductsViewModel> GetData(SummaryProgressOfProductSearchViewModel searchViewModel)
        {
            var reponsitory = new SummaryProgressOfProductRopository(_context);
            var data = reponsitory.SummaryProgressOfProduct();
            return data;
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, SummaryProgressOfProductSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "SummaryProgressOfProductData"] = searchModel;
            TempData[CurrentUser.AccountId + "SummaryProgressOfProductId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "SummaryProgressOfProductSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ViewDetail(SummaryProgressOfProductSearchViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "SummaryProgressOfProductData"] = searchModel;
            TempData[CurrentUser.AccountId + "SummaryProgressOfProductId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "SummaryProgressOfProductSearch"] = modeSearch;
            return RedirectToAction("Index");
        }
    }
}