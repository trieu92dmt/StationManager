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
    
    public class TimelineController : BaseController
    {
        // GET: Timeline
        public ActionResult Index()
        {
            var SelectedCommonDate = "Custom";
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate2);
            ViewBag.CommonDate1 = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            ViewBag.CommonDate2 = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            ViewBag.CommonDate3 = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            ViewBag.CommonDate4 = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", "Today");
            

            // begin view bag
            var searchModel = (TimeLineSearchViewModel)TempData[CurrentUser.AccountId + "TimelineSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "TimelineId"];
            var modeSearch = TempData[CurrentUser.AccountId + "TimelineSearch"];

            TimeLineSearchViewModel dt = new TimeLineSearchViewModel();
            if (modeSearch != null)
            {
                dt.StartDCFromDate = searchModel.StartDCFromDate;
                dt.StartDCToDate = searchModel.StartDCToDate;
            }    
            else
            {
                DateTime date = DateTime.Now;
                var firstDayOfMonth = new DateTime(date.Year, date.Month, date.Day);
                var lastDayOfMonth = new DateTime(date.Year, date.Month, date.Day);
                dt.EndDCFromDate = firstDayOfMonth;
                dt.EndDCToDate = lastDayOfMonth;
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
            var pageId = GetPageId("/Work/Timeline");
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
            //var result = _unitOfWork.TimelineRepository.Timeline(timeLineSearchView).OrderBy(x => x.StartDate).ThenBy(x => x.EstimateEndDate);
            timeLineSearchView.CompanyId = CurrentUser.CompanyId.Value;
            var result = _unitOfWork.TimelineRepository.Timeline(timeLineSearchView);
            
            if(result != null)
            {
                var jsonResult = Json(new
                {
                    Code = HttpStatusCode.OK,
                    Success = true,
                    Data = result
                }, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
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

        //Danh sách Bom detail
        public ActionResult _GetBomDetails(Guid TaskId)
        {
            var lst = _unitOfWork.TimelineRepository.GetBomDetailWithLSXD(TaskId);
            var SAP = _unitOfWork.TaskRepository.GetTaskById(TaskId);
            if (SAP!=null)
            {
                var LSXD = _unitOfWork.TaskRepository.GetTaskById((Guid)SAP.ParentTaskId);
                ViewBag.LSXD = LSXD.Summary;
                ViewBag.LSXDT = SAP.Property5;

            }
            return PartialView(lst);
        }

        public ActionResult LoadSubTaskInTimeLine(Guid parentTaskId)
        {
            var result = _unitOfWork.TimelineRepository.LoadSubTaskInTimeline(parentTaskId);
            
            if(result != null)
            {
                return Json(new { 
                    Code= HttpStatusCode.OK,
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

        #region Edit Task
        public ActionResult _Edit(Guid TaskId)
        {
            //Type
            var type = _unitOfWork.TaskRepository.GetWorkflowCategoryCode(TaskId);
            TaskViewModel task = new TaskViewModel();
            var ListProcess = (_context.CatalogModel.Where(x => x.CatalogTypeCode == "process")).ToList();
            ViewBag.ProcessCode = new SelectList(ListProcess, "CatalogCode", "CatalogCode", null);
            return PartialView("~/Areas/Work/Views/Task/_FormUpdateTask.cshtml", task);
        }
        #endregion

        #region Timeline full view
        public ActionResult FullView(TimeLineSearchViewModel searchViewModel)
        {
            // begin view bag
            var searchModel = (TimeLineSearchViewModel)TempData[CurrentUser.AccountId + "TimelineSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "TimelineId"];
            var modeSearch = TempData[CurrentUser.AccountId + "TimelineSearch"];

            TimeLineSearchViewModel dt = new TimeLineSearchViewModel();
            //if (modeSearch != null)
            //{
            //    dt.StartFromDate = searchModel.StartFromDate;
            //    dt.StartToDate = searchModel.StartToDate;
            //}
            //else
            //{
            //    DateTime date = DateTime.Now;
            //    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            //    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            //    dt.StartFromDate = firstDayOfMonth;
            //    dt.StartToDate = lastDayOfMonth;
            //}

            dt.VBELN = searchViewModel.VBELN;
            dt.Summary = searchViewModel.Summary;
            dt.Summary_Dot = searchViewModel.Summary_Dot;
            dt.Material = searchViewModel.Material;
            dt.StartFromDate = searchViewModel.StartFromDate;
            dt.StartToDate = searchViewModel.StartToDate;

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
            var pageId = GetPageId("/Work/Timeline");
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
            ViewBag.fromDate = searchViewModel.StartFromDate;
            ViewBag.toDate = searchViewModel.StartToDate;
            return View(dt);
        }
        #endregion 

        #region CreateViewBag, Helper
        public void CreateViewBag(Guid? CompanyId = null, Guid? WorkshopId = null)
        {
            //Get list Comapny
            var companyList = _context.CompanyModel.Where(p => p.Actived == true).ToList();
            ViewBag.CompanyId = new SelectList(companyList, "CompanyId", "CompanyName", CompanyId);

            //Get list WorkShop
            var workshopList = _context.WorkShopModel.Where(p => p.Actived == true).ToList();
            ViewBag.WorkshopId= new SelectList(workshopList, "WorkShopId", "WorkShopName", WorkshopId);
        }
        //Get Workshop By Compamy
        public ActionResult GetWorkShopBy(Guid? CompanyId)
        {
            var workshopList = _context.WorkShopModel.Where(p => p.Actived == true && p.CompanyId == CompanyId)
                                                     .Select(p => new ISD.ViewModels.WorkShopViewModel()
                                                     {
                                                         WorkShopId = p.WorkShopId,
                                                         WorkShopName = p.WorkShopName,
                                                        
                                                     }).ToList();
            var lst = new SelectList(workshopList, "WorkShopId", "WorkShopName");
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, TimeLineSearchViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "TimelineSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "TimelineId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "TimelineSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        #endregion
    }
}