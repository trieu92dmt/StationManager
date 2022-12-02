using ISD.Constant;
using ISD.Core;
using ISD.Repositories.MES;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Work.Controllers
{
    public class GranttChart2Controller : BaseController
    {
        // GET: GranttChart2
        public ActionResult Index()
        {
            var SelectedCommonDate = "Custom";
            //Common Date 2
            var commonDateList2 = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate2);
            ViewBag.CommonDate2 = new SelectList(commonDateList2, "CatalogCode", "CatalogText_vi", SelectedCommonDate);


            // begin view bag
            var searchModel = (TimeLineSearchViewModel)TempData[CurrentUser.AccountId + "TimelineSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "TimelineId"];
            var modeSearch = TempData[CurrentUser.AccountId + "TimelineSearch"];

            TimeLineSearchViewModel dt = new TimeLineSearchViewModel();
            if (modeSearch != null)
            {
                dt.StartFromDate = searchModel.StartFromDate;
                dt.StartToDate = searchModel.StartToDate;
            }
            else
            {
                DateTime date = DateTime.Now;
                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                dt.StartFromDate = firstDayOfMonth;
                dt.StartToDate = lastDayOfMonth;
            }

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
            var pageId = GetPageId("/Work/TimeLine");
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
            List<FieldSettingGranttModel> pivotSetting = new List<FieldSettingGranttModel>();
            //nếu đang có template đang xem
            if (templateId != Guid.Empty && templateId != null)
            {

                pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingGranttByTemplate(templateId);
                ViewBag.PivotSetting = pivotSetting;
                ViewBag.TemplateId = templateId;
            }
            else
            {
                var userDefaultTemplate = listUserTemplate.FirstOrDefault(s => s.IsDefault == true);
                //nếu ko có template đang xem thì lấy default của user
                if (userDefaultTemplate != null)
                {
                    pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingGranttByTemplate(userDefaultTemplate.SearchResultTemplateId);
                    ViewBag.PivotSetting = pivotSetting;
                    ViewBag.TemplateId = userDefaultTemplate.SearchResultTemplateId;
                }
                else
                {
                    var sysDefaultTemplate = listSystemTemplate.FirstOrDefault(s => s.IsDefault == true);
                    //nếu user không có template thì lấy default của hệ thống
                    if (sysDefaultTemplate != null)
                    {
                        pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingGranttByTemplate(sysDefaultTemplate.SearchResultTemplateId);
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
            // end view bag
            ViewBag.PageId = pageId;
            ViewBag.SystemTemplate = listSystemTemplate;
            ViewBag.UserTemplate = listUserTemplate;

            CreateViewBag();
            return View(dt);
        }


        public ActionResult _search(TimeLineSearchViewModel timeLineSearchView)
        {
            TimelineRepository _reponsitory = new TimelineRepository(_context);
            var result = _reponsitory.Timeline(timeLineSearchView).OrderBy(x => x.StartDate);

            if (result != null)
            {
                return Json(new
                {
                    Code = HttpStatusCode.OK,
                    Success = true,
                    Data = result
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Code = HttpStatusCode.NotFound,
                    Success = false,
                    Data = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public void CreateViewBag(Guid? CompanyId = null, Guid? WorkshopId = null)
        {
            //Get list Comapny
            var companyList = _context.CompanyModel.Where(p => p.Actived == true).ToList();
            ViewBag.CompanyId = new SelectList(companyList, "CompanyId", "CompanyName", CompanyId);

            //Get list WorkShop
            var workshopList = _context.WorkShopModel.Where(p => p.Actived == true).ToList();
            ViewBag.WorkshopId = new SelectList(workshopList, "WorkShopId", "WorkShopName", WorkshopId);
        }
    }
}