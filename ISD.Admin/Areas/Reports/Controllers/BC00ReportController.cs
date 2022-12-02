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
    public class BC00ReportController : BaseController
    {
        // GET: SO100Report
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var searchModel = (BC00ReportViewModel)TempData[CurrentUser.AccountId + "BC00ReportSearchData"];
            var tempalteIdString = TempData[CurrentUser.AccountId + "BC00ReportTemplateId"];
            var modeSearch = TempData[CurrentUser.AccountId + "BC00ReportModeSearch"];
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
            var pageId = GetPageId("/Reports/BC00Report");
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
                id = 30,
                name = "Top 30"
            });
            topList.Add(new ISDSelectIntItem()
            {
                id = 40,
                name = "Top 40"
            });
            topList.Add(new ISDSelectIntItem()
            {
                id = 50,
                name = "Top 50"
            });
            //topList.Add(new ISDSelectIntItem()
            //{
            //    id = 100,
            //    name = "Top 100"
            //});
            //topList.Add(new ISDSelectIntItem()
            //{
            //    id = 200,
            //    name = "Top 200"
            //});
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
            int? FreezeColumn = 5;
            if (searchModel != null && searchModel.FreezeColumn.HasValue)
            {
                FreezeColumn = searchModel.FreezeColumn;
            }
            ViewBag.FreezeColumn = new SelectList(freezeColumnList, "id", "name", FreezeColumn);
            ViewBag.NumberOfFreezeColumn = FreezeColumn;
            #endregion

            #region//Danh sách phòng ban lỗi
            var errorDepartmentList = _unitOfWork.CatalogRepository.GetErrorDepartmentList();
            ViewBag.ErrorDepartment = errorDepartmentList;
            #endregion
            
             #region//Danh sách danh mục lỗi (nhóm lỗi)
            var errorGroupList = _unitOfWork.CatalogRepository.GetErrorList();
            ViewBag.ErrorGroup = errorGroupList;
            #endregion

            #region//Danh sách phân xưởng
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
            ViewBag.WorkShop = allDepartmentList;
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

        public ActionResult ViewDetail(BC00ReportViewModel searchModel, Guid pivotTemplate, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "BC00ReportSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "BC00ReportTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "BC00ReportModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        public ActionResult ChangeTemplate(Guid pivotTemplate, BC00ReportViewModel searchModel, string modeSearch)
        {
            TempData[CurrentUser.AccountId + "BC00ReportSearchData"] = searchModel;
            TempData[CurrentUser.AccountId + "BC00ReportTemplateId"] = pivotTemplate;
            TempData[CurrentUser.AccountId + "BC00ReportModeSearch"] = modeSearch;
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult BC00ReportPivotGridPartial(Guid? templateId = null, BC00ReportViewModel searchViewModel = null, string jsonReq = null)
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
            var pageId = GetPageId("/Reports/BC00Report");
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
                return PartialView("_BC00PivotGridPartial", null);
            }
            else
            {
                if (!string.IsNullOrEmpty(jsonReq))
                {
                    searchViewModel = JsonConvert.DeserializeObject<BC00ReportViewModel>(jsonReq);
                }
                searchViewModel.Plant = CurrentUser.CompanyCode;
                var model = _unitOfWork.BC00ReportRepository.GetData(searchViewModel);
                ViewBag.Search = searchViewModel;

                return PartialView("_BC00PivotGridPartial", model);
            }
        }

        [HttpPost]
        public ActionResult ExportPivot(BC00ReportViewModel searchViewModel, Guid? templateId)
        {
            PivotGridExportOptions options = new PivotGridExportOptions();
            options.ExportType = PivotGridExportFormats.Excel;
            options.WYSIWYG.PrintRowHeaders = true;
            options.WYSIWYG.PrintFilterHeaders = false;
            options.WYSIWYG.PrintDataHeaders = false;
            options.WYSIWYG.PrintColumnHeaders = false;

            searchViewModel.Plant = CurrentUser.CompanyCode;
            var model = _unitOfWork.BC00ReportRepository.GetData(searchViewModel);
            var pivotSetting = _unitOfWork.PivotGridTemplateRepository.GetSettingByTemplate(templateId.Value).ToList();
            var FullFileName = _unitOfWork.PivotGridTemplateRepository.GetTemplateNameBy(templateId.Value);
            string fileName = _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(FullFileName.ToUpper()).Replace(" ", "_").Replace("/", "_");
            return PivotGridExportExcel.GetExportActionResult(fileName, options, pivotSetting, model, FullFileName);
        }

        #region Lấy thông tin cấu hình theo công đoạn
        public ActionResult GetConfigFieldEdit(Guid? BC01Id)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                bool? isEditLeadTime = false;
                bool? isEditStartDate = false;
                bool? isEditEndDate = false;
                bool? isEditCompletedPercent = false;
                bool? isEditWorkShop = false;
                var report = _context.BC01Model.Where(p => p.BC01Id == BC01Id).FirstOrDefault();
                if (report != null)
                {
                    var config = _context.PlantRoutingConfigModel.Where(p => p.PlantRoutingCode == report.StockCode).FirstOrDefault();
                    //Leadtime
                    if (!config.LeadTime.HasValue && string.IsNullOrEmpty(config.LeadTimeFormula))
                    {
                        isEditLeadTime = true;
                    }
                    //StartDate
                    if (string.IsNullOrEmpty(config.FromDate))
                    {
                        isEditStartDate = true;
                    }
                    //EndDate
                    if (string.IsNullOrEmpty(config.ToDate))
                    {
                        isEditEndDate = true;
                    }
                    //CompletedPercent
                    if (string.IsNullOrEmpty(config.Attribute8))
                    {
                        isEditCompletedPercent = true;
                    }
                    //WorkShop
                    if (string.IsNullOrEmpty(config.Attribute1))
                    {
                        isEditWorkShop = true;
                    }
                }
                return _APISuccess(new { isEditLeadTime, isEditStartDate, isEditEndDate, isEditCompletedPercent, isEditWorkShop });
            });
        }

        public ActionResult GetConfigFieldCreate(string StockCode)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                bool? isCreateLeadTime = false;
                bool? isCreateStartDate = false;
                bool? isCreateEndDate = false;
                bool? isCreateCompletedPercent = false;
                if (!string.IsNullOrEmpty(StockCode))
                {
                    int stockCode = 0;
                    bool successfullyParsed = int.TryParse(StockCode, out stockCode);
                    if (successfullyParsed)
                    {
                        var config = _context.PlantRoutingConfigModel.Where(p => p.PlantRoutingCode == stockCode).FirstOrDefault();
                        //Leadtime
                        if (!config.LeadTime.HasValue && string.IsNullOrEmpty(config.LeadTimeFormula))
                        {
                            isCreateLeadTime = true;
                        }
                        //StartDate
                        if (string.IsNullOrEmpty(config.FromDate))
                        {
                            isCreateStartDate = true;
                        }
                        //EndDate
                        if (string.IsNullOrEmpty(config.ToDate))
                        {
                            isCreateEndDate = true;
                        }
                        //CompletedPercent
                        if (string.IsNullOrEmpty(config.Attribute8))
                        {
                            isCreateCompletedPercent = true;
                        }
                    }
                }

                return _APISuccess(new { isCreateLeadTime, isCreateStartDate, isCreateEndDate, isCreateCompletedPercent });
            });
        }
        #endregion

        #region Lưu thông tin cập nhật 
        public ActionResult SaveEditReport(BC00ReportFormViewModel viewModel)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                _unitOfWork.BC00ReportRepository.UpdateDataBC01(viewModel, CurrentAccountId: CurrentUser.AccountId);
                return _APISuccess(null);
            });
        }
        #endregion

        #region Lưu thông tin thêm mới 
        public ActionResult SaveCreateReport(BC00ReportFormViewModel viewModel)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                _unitOfWork.BC00ReportRepository.CreateDataBC01(viewModel, CurrentAccountId: CurrentUser.AccountId);
                return _APISuccess(null);
            });
        }
        #endregion

        #region Helper cho autocomplete
        //Tìm kiếm tất cả LSXDT có chứa LSX SAP + SO + SO Line
        public ActionResult SearchLSXDT(string SearchText)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    SearchText = null;
                }
                var result = (from a in _context.TaskModel
                                  //Plant
                              join c in _context.CompanyModel on a.CompanyId equals c.CompanyId
                              //Loại
                              join w in _context.WorkFlowModel on a.WorkFlowId equals w.WorkFlowId
                              //Loại: LSX SAP
                              where w.WorkFlowCode == ConstWorkFlow.LSXC
                              && !string.IsNullOrEmpty(a.Property3)
                              //Tìm theo nội dung search
                              && (SearchText == null || a.Property3.Contains(SearchText))
                              //Theo plant mà user đang đăng nhập hiện tại
                              && c.CompanyCode == CurrentUser.CompanyCode
                              //Có chứa thông tin SO + SO Line
                              && !string.IsNullOrEmpty(a.Property1) && !string.IsNullOrEmpty(a.Property2)
                              group a by new { a.Property3, a.Property5 } into g
                              select g.Key.Property3).Take(10).ToList();

                return _APISuccess(result);
            });
        }
        //Tìm kiếm DSX theo LSXDT
        public ActionResult SearchDSX(string SearchText, string LSXDT)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    SearchText = null;
                }
                var result = new List<ISDSelectGuidItem>();
                if (!string.IsNullOrEmpty(LSXDT))
                {
                    result = (from a in _context.TaskModel
                                  //Loại
                              join w in _context.WorkFlowModel on a.WorkFlowId equals w.WorkFlowId
                              //Loại: Đợt
                              where w.WorkFlowCode == ConstWorkFlow.LSXD
                              //Tìm theo LSX DT
                              && (a.Property3 == LSXDT)
                              //Tìm theo nội dung search
                              && (SearchText == null || a.Summary.Contains(SearchText))
                              orderby a.Number1
                              select new ISDSelectGuidItem
                              {
                                  id = a.TaskId,
                                  name = a.Summary,
                              }).Take(10).ToList();
                }

                return _APISuccess(result);
            });
        }
        //Tìm kiếm LSXSAP theo DSX
        public ActionResult SearchLSXSAP(string SearchText, Guid? DSXId)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    SearchText = null;
                }
                var result = new List<ISDSelectGuidItem>();
                if (DSXId.HasValue)
                {
                    result = (from a in _context.TaskModel
                                  //Loại
                              join w in _context.WorkFlowModel on a.WorkFlowId equals w.WorkFlowId
                              //Loại: LSX SAP
                              where w.WorkFlowCode == ConstWorkFlow.LSXC
                              //Tìm theo đợt
                              && a.ParentTaskId == DSXId
                              //Tìm theo nội dung search
                              && (SearchText == null || a.Summary.Contains(SearchText))
                              orderby a.Summary
                              select new ISDSelectGuidItem
                              {
                                  id = a.TaskId,
                                  name = a.Summary,
                              }).Take(10).ToList();
                }

                return _APISuccess(result);
            });
        }

        //Lấy thông tin LSX SAP
        public ActionResult GetLSXSAPInfo(Guid? LSXSAPId)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = new TaskViewModel();
                if (LSXSAPId.HasValue)
                {
                    result = (from a in _context.TaskModel
                                  //Loại
                              join w in _context.WorkFlowModel on a.WorkFlowId equals w.WorkFlowId
                              //Sản phẩm
                              join p in _context.ProductModel on a.ProductId equals p.ProductId
                              //Báo cáo 
                              join rpt in _context.BC01Model on a.TaskId equals rpt.LSXSAPId into rTemp
                              from r in rTemp.DefaultIfEmpty()
                              //Loại: LSX SAP
                              where w.WorkFlowCode == ConstWorkFlow.LSXC
                              //Tìm theo đợt
                              && a.TaskId == LSXSAPId
                              orderby a.Summary
                              select new TaskViewModel
                              {
                                  ProductId = p.ProductId,
                                  ProductName = p.ERPProductCode + " | " + p.ProductName,
                                  SONumber = a.Property1,
                                  SOLineNumber = a.Property2,
                                  STT = r.STT
                              }).FirstOrDefault();
                }

                return _APISuccess(result);
            });
        }

        //Lấy danh sách thông tin công đoạn thực hiện theo LSXSAP 
        public ActionResult GetStockList(string SearchText, Guid? LSXSAPId)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    SearchText = null;
                }
                var result = new List<ISDSelectIntItem>();
                if (LSXSAPId.HasValue)
                {
                    //Lấy tất cả danh sách công đoạn thực hiện có field Condition = MANUAL và đang sử dụng
                    result = (from a in _context.PlantRoutingConfigModel
                              where a.Actived == true && a.Condition == "MANUAL"
                              //Tìm theo text
                              && (SearchText == null || a.PlantRoutingName.Contains(SearchText))
                              orderby a.OrderIndex
                              select new ISDSelectIntItem
                              {
                                  id = a.PlantRoutingCode,
                                  name = a.PlantRoutingName,
                                  orderindex = a.OrderIndex,
                              }).ToList();

                    //Lọc lại chỉ lấy các công đoạn chưa tồn tại theo LSX SAP trong báo cáo
                    var existStockList = (from a in _context.TaskModel
                                              //Loại
                                          join w in _context.WorkFlowModel on a.WorkFlowId equals w.WorkFlowId
                                          //Sản phẩm
                                          join p in _context.ProductModel on a.ProductId equals p.ProductId
                                          //Báo cáo 
                                          join rpt in _context.BC01Model on a.TaskId equals rpt.LSXSAPId
                                          //Loại: LSX SAP
                                          where w.WorkFlowCode == ConstWorkFlow.LSXC
                                          //Tìm theo đợt
                                          && a.TaskId == LSXSAPId
                                          //Tìm theo text
                                            && (SearchText == null || rpt.Stock.Contains(SearchText))
                                          orderby a.Summary
                                          select rpt.StockCode).ToList();

                    if (existStockList != null && existStockList.Count > 0)
                    {
                        result = result.Where(p => !existStockList.Contains(p.id)).OrderBy(p => p.orderindex).ToList();
                    }
                }

                return _APISuccess(result);
            });
        }
        #endregion
    }
}