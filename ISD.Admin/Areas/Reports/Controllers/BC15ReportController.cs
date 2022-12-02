using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class BC15ReportController : BaseController
    {
        // GET: SO100Report
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (BC15ReportViewModel)TempData[CurrentUser.AccountId + "BC15ReportSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "BC15ReportTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "BC15ReportModeSearch"];
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
            var pageId = GetPageId("/Reports/BC15Report");
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

            #region CommonDate
            var SelectedCommonDate = "Custom";
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);

            #endregion

            var saleOrg = CurrentUser.SaleOrg;
            if (searchModel != null && !string.IsNullOrEmpty(searchModel.SaleOrgCode))
            {
                saleOrg = searchModel.SaleOrgCode;
            }
            #region //Nhà máy
            var StoreList = _unitOfWork.StoreRepository.GetStoreByPermission(CurrentUser.AccountId);
            ViewBag.SaleOrgCode = new SelectList(StoreList, "SaleOrgCode", "StoreName", saleOrg);
            #endregion


            #region //Phân xưởng
            Guid? WorkShopId = null;
            if (searchModel != null && searchModel.WorkShopId.HasValue)
            {
                WorkShopId = searchModel.WorkShopId;
            }
            var workShopList = _unitOfWork.WorkShopRepository.GetWorkShopByStore(saleOrg);
            ViewBag.WorkShopId = new SelectList(workShopList, "WorkShopId", "WorkShopName", WorkShopId);
            #endregion

            #region //Công đoạn lớn
            var workCenterList = _context.WorkCenterModel.Where(p => p.SaleOrgCode == saleOrg).OrderBy(x => x.OrderIndex).ToList();
            string WorkCenterCode = null;
            if (searchModel != null && !string.IsNullOrEmpty(searchModel.WorkCenterCode))
            {
                WorkCenterCode = searchModel.WorkCenterCode;
            }
            ViewBag.WorkCenterCode = new SelectList(workCenterList, "WorkCenterCode", "WorkCenterName", WorkCenterCode);
            #endregion

            #region //Xem theo top SL dòng dữ liệu
            int? TopRow = 10;
            if (searchModel != null && searchModel.TopRow.HasValue)
            {
                TopRow = searchModel.TopRow;
            }
            List<ISDSelectIntItem> topList = new List<ISDSelectIntItem>();
            topList.Add(new ISDSelectIntItem()
            {
                id = 10,
                name = "Top 10"
            });
            topList.Add(new ISDSelectIntItem()
            {
                id = 20,
                name = "Top 20"
            });
            topList.Add(new ISDSelectIntItem()
            {
                id = 50,
                name = "Top 50"
            });
            topList.Add(new ISDSelectIntItem()
            {
                id = 100,
                name = "Top 100"
            });
            topList.Add(new ISDSelectIntItem()
            {
                id = 200,
                name = "Top 200"
            });
            topList.Add(new ISDSelectIntItem()
            {
                id = 0,
                name = "Tất cả"
            });
            ViewBag.TopRow = new SelectList(topList, "id", "name", TopRow);
            #endregion

            #region //Freeze column
            List<ISDSelectIntItem> freezeColumnList = new List<ISDSelectIntItem>();
            for (int i = 0; i < 10; i++)
            {
                freezeColumnList.Add(new ISDSelectIntItem()
                {
                    id = i + 1,
                    name = (i + 1).ToString()
                });
            }
            int? FreezeColumn = 1;
            if (searchModel != null && searchModel.FreezeColumn.HasValue)
            {
                FreezeColumn = searchModel.FreezeColumn;
            }
            ViewBag.FreezeColumn = new SelectList(freezeColumnList, "id", "name", FreezeColumn);
            ViewBag.NumberOfFreezeColumn = FreezeColumn;
            #endregion

            #region//Danh sách phân xưởng vật lý
            //var departmentList = _context.AllDepartmentModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).Select(p => new ISDSelectStringItem()
            //{
            //    id = p.DepartmentCode,
            //    name = p.DepartmentName,
            //    additional = "Phòng ban"
            //}).ToList();
            var physicsWorkshopList = _context.PhysicsWorkShopModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).Select(p => new ISDSelectStringItem()
            {
                id = p.PhysicsWorkShopCode,
                name = p.PhysicsWorkShopName,
                additional = "Phân xưởng vật lý"
            }).ToList();
            var allDepartmentList = new List<ISDSelectStringItem>();
            //allDepartmentList.AddRange(departmentList);
            allDepartmentList.AddRange(physicsWorkshopList);
            string PhysicsWorkShopCode = null;
            if (searchModel != null && !string.IsNullOrEmpty(searchModel.PhysicsWorkShopCode))
            {
                PhysicsWorkShopCode = searchModel.PhysicsWorkShopCode;
            }
            ViewBag.PhysicsWorkShopCode = new SelectList(allDepartmentList, "id", "name", PhysicsWorkShopCode);
            #endregion

            return View();

        }
        [HttpGet]
        public PartialViewResult _LoadWorkShopBy(string SaleOrgCode)
        {
            var workShopList = _unitOfWork.WorkShopRepository.GetWorkShopByStore(SaleOrgCode);
            ViewBag.WorkShopId = new SelectList(workShopList, "WorkShopId", "WorkShopName");

            return PartialView();
        }
        [HttpGet]
        public PartialViewResult _LoadWorkCenterBy(string SaleOrgCode)
        {
            var workCenterList = _context.WorkCenterModel.Where(p => p.SaleOrgCode == SaleOrgCode).OrderBy(x => x.OrderIndex).ToList();
            ViewBag.WorkCenterCode = new SelectList(workCenterList, "WorkCenterCode", "WorkCenterName");

            return PartialView();
        }

        public ActionResult ViewDetail(BC15ReportViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "BC15ReportSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "BC15ReportTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "BC15ReportModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, BC15ReportViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "BC15ReportSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "BC15ReportTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "BC15ReportModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult BC15Partial(Guid? templateId = null, BC15ReportViewModel searchViewModel = null, string jsonReq = null)
        {
            #region commonDate

            if (searchViewModel != null && searchViewModel.CreatedCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchViewModel.CreatedCommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchViewModel.CompletedFromDate = fromDate;
                searchViewModel.CompletedToDate = toDate;
            }
            #endregion
            var pageId = GetPageId("/Reports/BC15Report");
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
            //4 công đoạn lớn
            string saleOrgCode = CurrentUser.SaleOrg;
            if (searchViewModel != null && !string.IsNullOrEmpty(searchViewModel.SaleOrgCode))
            {
                saleOrgCode = searchViewModel.SaleOrgCode;
            }
            var workCenterLst = _context.WorkCenterModel.Where(p => p.SaleOrgCode == saleOrgCode).ToList();
            ViewBag.Step1Code = workCenterLst.Where(p => p.OrderIndex == 10).Select(p => p.WorkCenterCode).FirstOrDefault();
            ViewBag.Step2Code = workCenterLst.Where(p => p.OrderIndex == 20).Select(p => p.WorkCenterCode).FirstOrDefault();
            ViewBag.Step3Code = workCenterLst.Where(p => p.OrderIndex == 30).Select(p => p.WorkCenterCode).FirstOrDefault();
            ViewBag.Step4Code = workCenterLst.Where(p => p.OrderIndex == 40).Select(p => p.WorkCenterCode).FirstOrDefault();
            //Danh sách phòng ban lỗi
            //var errorList = _unitOfWork.CatalogRepository.GetErrorList();
            //ViewBag.ErrorDepartment = errorList;
            if (searchViewModel != null && searchViewModel.FreezeColumn.HasValue)
            {
                ViewBag.NumberOfFreezeColumn = searchViewModel.FreezeColumn;
            }

            if ((string.IsNullOrEmpty(jsonReq) || jsonReq == "null") && (searchViewModel == null || searchViewModel.IsView != true))
            {
                ViewBag.Search = null;
                return PartialView("_BC15Partial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<BC15ReportViewModel>(jsonReq);
                }
                searchViewModel.Plant = CurrentUser.CompanyCode;
                var model = _unitOfWork.BC15ReportRepository.GetData(searchViewModel);
                ViewBag.Search = searchViewModel;

                return PartialView("_BC15Partial", model);
            }
        }

        [HttpPost]
        public ActionResult ExportPivot(BC15ReportViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;

            searchViewModel.Plant = CurrentUser.CompanyCode;
            var model = _unitOfWork.BC15ReportRepository.GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            var FullFileName = _unitOfWork.PivotGridTemplateRepository.GetTemplateNameBy(templateId.Value);
            string fileName = _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(FullFileName.ToUpper()).Replace(" ", "_").Replace("/", "_");
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model, FullFileName);
        }
    }
}