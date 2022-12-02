using ISD.Constant;
using ISD.Core;
using ISD.Repositories;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class ProductionRecordingController : BaseController
    {
        // GET: ProductionRecording
        public ActionResult Index()
        {
            #region CommonDate
            var SelectedCommonDate = "Custom";
            //Common Date
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);

            #endregion

            #region CreateBy
            var accounts = _unitOfWork.AccountRepository.GetAll();
            ViewBag.CreateBy = new SelectList(accounts, "AccountId", "UserName");
            #endregion

            #region Công đoạn
            var StepCodeList = _unitOfWork.StockRepository.GetAll().Select(x=> new { x.StockCode, display = x.StockCode +" | "+ x.StockName });
            ViewBag.StepCode = new SelectList(StepCodeList, "StockCode", "display");
            
            ViewBag.ToStockCode = new SelectList(StepCodeList, "StockCode", "display");
            #endregion

            return View(new ProductionManagementViewModel());

        }
        //public ActionResult _Search(TaskSearchViewModel searchModel)
        //{
        //    return ExecuteSearch(() =>
        //    {
        //        var searchResult = _unitOfWork.ProductionManagementRepository.Search(searchModel);
        //        return PartialView(searchResult);
        //    });
        //}

        public ActionResult _PaggingServerSide(DatatableViewModel model, ProductionManagementViewModel searchModel)
        {
            try
            {
                // action inside a standard controller
                int filteredResultsCount;
                int totalResultsCount = model.length;

                //Page Size 
                searchModel.PageSize = model.length;
                //Page Number
                searchModel.PageNumber = model.start / model.length + 1;

                if (searchModel.CreatedCommonDate != "Custom")
                {
                    DateTime? fromDate;
                    DateTime? toDate;
                    _unitOfWork.CommonDateRepository.GetDateBy(searchModel.CreatedCommonDate, out fromDate, out toDate);
                    //Tìm kiếm kỳ hiện tại
                    searchModel.CreatedFromDate = fromDate;
                    searchModel.CreatedToDate = toDate;
                }
                //Tìm theo plant của user đang chọn hiện tại
                searchModel.CompanyId = CurrentUser.CompanyId;
                var res = _unitOfWork.ProductionManagementRepository.SearchQueryProc(searchModel, out filteredResultsCount);

                if (res != null && res.Count() > 0)
                {
                    int i = model.start;
                    //foreach (var item in res)
                    //{
                    //    i++;
                    //    item.STT = i;
                    //}
                }

                return Json(new
                {
                    draw = model.draw,
                    recordsTotal = totalResultsCount,
                    recordsFiltered = filteredResultsCount,
                    data = res
                });
            }
            catch //(Exception ex)
            {
                return Json(null);
            }
        }

        [HttpPost]
        //Hàm tìm kiếm dành cho popup
        public ActionResult _ProductionOrderSearchResult(DatatableViewModel model, TaskSearchViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                // action inside a standard controller
                int filteredResultsCount;
                int totalResultsCount = model.length;

                //Page Size 
                searchViewModel.PageSize = model.length;
                //Page Number
                searchViewModel.PageNumber = model.start / model.length + 1;

                //CurrentCompanyCode
                searchViewModel.CompanyId = CurrentUser.CompanyId;

                var res = _unitOfWork.ProductionManagementRepository.SearchProductionOrderQueryProc(searchViewModel, out filteredResultsCount);

                if (res != null && res.Count() > 0)
                {
                    int i = model.start;
                }

                return Json(new
                {
                    draw = model.draw,
                    recordsTotal = totalResultsCount,
                    recordsFiltered = filteredResultsCount,
                    data = res
                });
            });
        }

        [HttpGet]
        public ActionResult _UpdateBarCode(string TaskId)
        {
            var getTask = _unitOfWork.ProductionManagementRepository.GetExecutionTaskByTaskId(Guid.Parse(TaskId));
            return PartialView(getTask);
        }

        [HttpPost]
        public JsonResult _UpdateBarCode(UpdateBarCodeViewModel updateBarCodeViewModel)
        {
            return ExecuteContainer(() =>
            {
                if (_unitOfWork.ProductionManagementRepository.QRCodeExits(updateBarCodeViewModel.Barcode))
                {
                    return _Error("Thẻ treo đã được sử dụng!");
                }
                if (!_unitOfWork.ProductionManagementRepository.CheckHangTag(updateBarCodeViewModel.Barcode, updateBarCodeViewModel.TaskId))
                {
                    return _Error("Vui lòng chọn thẻ treo dành cho Lệnh thực thi: "+ updateBarCodeViewModel.ProductionOrder_SAP);
                }
                _unitOfWork.ProductionManagementRepository.UpdateBarCode(updateBarCodeViewModel.TaskId, updateBarCodeViewModel.Barcode);


                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.ProductionRecording.ToLower())
                });
            });
        }

        [HttpGet]
        public ActionResult _RecordProduction(Guid? TaskId)
        {

            #region Nếu có task theo barcode => show popup theo dữ liệu
            var data = new ProductionManagementViewModel();
            data = _unitOfWork.ProductionManagementRepository.GetExecutionTaskByTaskId(TaskId);
            #endregion
            #region Create ViewBag "Công đoạn"
            var listStep = _unitOfWork.ProductionManagementRepository.LoadRoutingOf(data.ProductCode, data.ProductAttributes).Select(x => new { x.ARBPL_SUB, display = x.ARBPL_SUB + " | " + x.LTXA1 }).ToList();

            if (!string.IsNullOrEmpty(data.StepCode))
            {
                ViewBag.StepCode = new SelectList(listStep, "ARBPL_SUB", "display", data.StepCode);
            }
            else
            {
                ViewBag.StepCode = new SelectList(listStep, "ARBPL_SUB", "display");
            }
            #endregion

            //#region Create ViewBag "Tổ"
            //var listDeparment = _context.DepartmentModel.OrderBy(x => x.DepartmentCode).Select(x => new { x.DepartmentId, display = x.DepartmentCode + " | " + x.DepartmentName }).ToList();
            //ViewBag.DepartmentId = new SelectList(listDeparment, "DepartmentId", "display");
            //#endregion
            #region Phân xưởng
            var WorkShopList = _context.WorkShopModel.Where(p => p.Actived == true && p.CompanyId == CurrentUser.CompanyId).OrderBy(p => p.OrderIndex).Select(x => new { x.WorkShopId, display = x.WorkShopCode + " | " + x.WorkShopName }).ToList();
            ViewBag.WorkShopId = new SelectList(WorkShopList, "WorkShopId", "display");
            #endregion

            //Check nếu có routing mới cho phép ghi nhận tiếp
            string sqlQuery = "SELECT * FROM MES.View_Product_Material WHERE WERKS = '" + CurrentUser.CompanyCode + "' AND MATNR = '" + data.ProductCode + "' AND ITMNO = '" + data.ProductAttributes + "' AND ARBPL_SUB = '" + data.StepCode + "' ";
            var routingLst = _context.Database.SqlQuery<View_Product_MaterialViewModel>(sqlQuery).ToList();
            if (routingLst != null && routingLst.Count > 0)
            {
                data.isHasRouting = true;
            }
            return PartialView("~/Areas/MES/Views/ProductionManagement/_RecordProduction.cshtml", data);
            //return PartialView("_PopupConfirmCreateTask");
        }

        [HttpGet]
        public ActionResult _SwitchingStages(Guid? TaskId)
        {
            
            #region Nếu có task theo barcode => show popup theo dữ liệu
            var data = new SwitchingStagesViewModel();
            data = _unitOfWork.ProductionManagementRepository.GetTTLSXForSwitchingStageByTaskId(TaskId);
            #endregion
            #region Create ViewBag "Công đoạn"
            var listStep = _unitOfWork.ProductionManagementRepository.LoadRoutingOf(data.ProductCode, data.ProductAttributes).Where(x => x.ARBPL_SUB != data.FromStepCode).Select(x => new { x.ARBPL_SUB, display = x.ARBPL_SUB + " | " + x.LTXA1 }).ToList();
            ViewBag.ToStepCode = new SelectList(listStep, "ARBPL_SUB", "display");

            //khi chuyển công đoạn: cho phép lấy công đoạn của cụm 
            if (data.ProductAttributes.Split('.').Length > 1)
            {
                string ProductParentAtrributes = data.ProductAttributes.Split('.')[0];
                var parentStepCodeLst = _unitOfWork.ProductionManagementRepository.LoadRoutingOf(data.ProductCode, ProductParentAtrributes).Where(x => x.ARBPL_SUB != data.FromStepCode).Select(x => new { x.ARBPL_SUB, display = x.ARBPL_SUB + " | " + x.LTXA1 }).ToList();
                listStep.AddRange(parentStepCodeLst);
            }
            listStep = listStep.GroupBy(p => new { p.ARBPL_SUB, p.display }).Select(p => new { p.Key.ARBPL_SUB, p.Key.display }).ToList();
            #endregion

            #region Create ViewBag "Tổ"
            //var listDeparment = _context.DepartmentModel.Select(x => new { x.DepartmentId, display = x.DepartmentCode + " | " + x.DepartmentName });
            //ViewBag.DepartmentId = new SelectList(listDeparment, "DepartmentId", "display");
            #endregion
            return PartialView("~/Areas/MES/Views/ProductionManagement/_SwitchingStages.cshtml", data);
        }
        [HttpGet]
        public ActionResult _ConfirmWorkCenter(Guid? TaskId)
        {
          
            #region Nếu có task theo barcode => show popup theo dữ liệu
            var data = new ProductionManagementViewModel();
            data = _unitOfWork.ProductionManagementRepository.GetExecutionTaskByTaskId(TaskId);
            #endregion
            return PartialView("~/Areas/MES/Views/ProductionManagement/_ConfirmWorkCenter.cshtml", data);
        }

        [HttpGet]
        public ActionResult _CreateTTLSX(Guid? ParentTaskId)
        {
            //Thêm mới lệnh thực thi và lấy dữ liệu
            var taskId = _unitOfWork.ProductionManagementRepository.CreateNewExecutionTask(ParentTaskId.Value, null, CurrentUserId: CurrentUser.AccountId);

            return _RecordProduction(taskId); //TaskId
       
        }


    }
}