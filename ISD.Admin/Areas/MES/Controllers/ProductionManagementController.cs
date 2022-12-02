using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class ProductionManagementController : BaseController
    {
        // GET: ProductionManagement
        [ISDAuthorizationAttribute]
        public ActionResult Index(string Type)
        {
            ViewBag.Type = Type;
            return View();
        }


        #region Lấy thông tin lệnh thực thi sản xuất
        [HttpGet]
        public ActionResult _RecordProduction(Guid? Barcode = null)
        {
            //Tìm LSX SAP theo barcode
            var taskId = _unitOfWork.ProductionManagementRepository.GetTTLSXByBarcode(Barcode);

            //Nếu có lệnh thực thi thì hiển thị popup ghi nhận sản lượng
            //Nếu chưa có thì thêm thực thi lệnh sản xuất=> get data để hiển thị popup ghi nhận sản lượng
            if (taskId == null || taskId == Guid.Empty)
            {
                Guid HangTagId = (Guid)Barcode;
                var handTag = _context.HangTagModel.Where(p => p.HangTagId == HangTagId).FirstOrDefault();
                if (handTag != null && handTag.CustomerReference.HasValue)
                {
                    //Thêm mới lệnh thực thi và lấy dữ liệu
                    taskId = _unitOfWork.ProductionManagementRepository.CreateNewExecutionTask(handTag.CustomerReference.Value, Barcode, CurrentUserId: CurrentUser.AccountId);
                }
                else
                {
                    //Báo lỗi không tìm thấy barcode
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Thẻ treo không hợp lệ!",
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            #region Nếu có task theo barcode => show popup theo dữ liệu
            var data = new ProductionManagementViewModel();
            data = _unitOfWork.ProductionManagementRepository.GetExecutionTaskByTaskId(taskId);
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

            return PartialView(data);
            //return PartialView("_PopupConfirmCreateTask");
        }

        //[HttpGet]
        public ActionResult _RecordProductionDetail(Guid TaskId)
        {
            var data = _unitOfWork.ProductionManagementRepository.GetProductDetailsHistoryByTTLSX(TaskId);

            return PartialView(data);
        }
        public ActionResult _RecordProductionDetailTransfer(Guid TaskId)
        {
            var data = _unitOfWork.ProductionManagementRepository.GetProductDetailsHistoryByTTLSXForTransfer(TaskId);

            return PartialView(data);
        }
        /// <summary>
        /// Lấy thông tin lịch sử ghi nhận sản lượng
        /// </summary>
        /// <param name="TTLSX">TaskId TTLSX</param>
        /// <param name="fromTime">Thời gian bắt đầu</param>
        /// <param name="toTime">Thời gian kết thúc</param>
        /// <param name="itmno">Chi tiết</param>
        /// <returns></returns>
        public ActionResult _ProductionRecordhistory(Guid TTLSX, DateTime fromTime, DateTime toTime, string itmno, string StepCode)
        {
            var data = _unitOfWork.ProductionManagementRepository.GetProductionRecordhistory(TTLSX, fromTime, toTime, itmno, StepCode);

            return PartialView(data);
        }

        public ActionResult _ShowDepartment(Guid TTLSX, DateTime fromTime, DateTime toTime, string itmno)
        {
            var Department = _unitOfWork.ProductionManagementRepository.GetDepartment(TTLSX, fromTime, toTime, itmno);
            if (Department != null && (Department.DepartmentId != null || Department.DepartmentId != Guid.Empty))
            {
                var Workshop = _unitOfWork.WorkShopRepository.GetWorkShopByDepartment(Department.DepartmentId);
                return Json(new
                {
                    Department = Department.DepartmentCode + " | " + Department.DepartmentName,
                    Workshop = Workshop.WorkShopCode + " | " + Workshop.WorkShopName
                });
            }
            else
            {
                return Json(new { });
            }

        }

        #endregion

        #region Lưu thông tin lệnh thực thi sản xuất
        [HttpPost]
        public JsonResult _RecordProduction(List<ProductionOrderDetailViewModel> productionOrderDetailOld, ProductionManagementViewModel productionOrderViewModel, List<UsageQuantityViewModel> usageQuantityViewModels)
        {
            return ExecuteContainer(() =>
            {

                //Báo lỗi nếu chưa nhập tổ
                if (productionOrderViewModel.DepartmentId == null)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Vui lòng chọn thông tin tổ!",
                    });
                }
                //Báo lỗi nếu chưa nhập Stepcode
                if (productionOrderViewModel.StepCode == null)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Vui lòng chọn thông tin công đoạn!",
                    });
                }

                if (productionOrderViewModel.WorkDate == null && (productionOrderViewModel.FromDate == null || productionOrderViewModel.ToDate == null))
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Vui lòng chọn ngày!",
                    });

                }
                //Lấy ngày làm việc

                DateTime? FromDate = productionOrderViewModel.WorkDate;
                DateTime? WorkDate = productionOrderViewModel.WorkDate;
                //Nếu có ngày bắt đầu và ngày kết thúc thì sẽ tính theo ngày bắt đầu và ngày kết thúc
                if (productionOrderViewModel.FromDate != null)
                {
                    FromDate = productionOrderViewModel.FromDate;
                }
                if (FromDate.Value > WorkDate.Value)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Ngày bắt đầu không được nhỏ hơn ngày kết thúc!",
                    });
                }

                //Ghi nhận sản lượng - không cho ghi nhận trước ngày khóa sổ
                var dateClosed = _context.DateClosedModel.FirstOrDefault();
                if (dateClosed != null)
                {
                    if (productionOrderViewModel.WorkDate.HasValue && productionOrderViewModel.WorkDate.Value.Date < dateClosed.DateClosed.Date)
                    {
                        return Json(new
                        {
                            Code = HttpStatusCode.NotModified,
                            Success = false,
                            Data = string.Format("Không thể ghi nhận sản lượng trước ngày khóa sổ! ({0:dd/MM/yyyy})", dateClosed.DateClosed.Date),
                        });
                    }
                }

                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                //Ghi nhận sản lượng - không cho nhập thời gian trong tương lai
                if (productionOrderViewModel.WorkDate.HasValue && productionOrderViewModel.WorkDate.Value.Date > currentDate)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Không thể ghi nhận sản lượng trong tương lai!",
                    });
                }
                //Lấy thông tin stock
                var stockid = _unitOfWork.StockRepository.GetStockIdByStockCode(productionOrderViewModel.StepCode);
                //Lấy thông tin lênh thực thi
                var ttlxs = _context.ThucThiLenhSanXuatModel.FirstOrDefault(x => x.TaskId == productionOrderViewModel.TaskId);
                if (ttlxs != null)
                {
                    #region //Cập nhật công đoạn sản xuất
                    ttlxs.Property6 = productionOrderViewModel.StepCode;
                    ttlxs.StockId = stockid;
                    #endregion

                }
                _context.Entry(ttlxs).State = EntityState.Modified;
                //Tạo Stock master
                var catalogType = _context.CatalogModel.Where(x => x.CatalogTypeCode == "StockRecevingType").ToList();
                //Add Stock Receiving Detail
                if (productionOrderDetailOld != null && productionOrderDetailOld.Count > 0)
                {
                    foreach (var stockRecevingDetailVM in productionOrderDetailOld)
                    {
                        foreach (var item in catalogType)
                        {
                            //Thêm stock receiving detail nếu type =  đạt
                            if (item.CatalogCode == "D")
                            {
                                #region //Lưu tồn kho
                                if (stockRecevingDetailVM.Quantity_D != null && stockRecevingDetailVM.Quantity_D != 0)
                                {
                                    //Get datekey from documentDate

                                    var dateKey = _unitOfWork.UtilitiesRepository.ConvertDateTimeToInt(WorkDate);
                                    if ((stockRecevingDetailVM.FromTime.Value.Hour == 0 && stockRecevingDetailVM.FromTime.Value.Minute == 0) || (stockRecevingDetailVM.ToTime.Value.Hour == 0 && stockRecevingDetailVM.ToTime.Value.Minute == 0))
                                    {
                                        return Json(new
                                        {
                                            Code = HttpStatusCode.NotModified,
                                            Success = false,
                                            Data = "Vui lòng chọn thời gian ghi nhận!",
                                        });
                                    }
                                    StockReceivingDetailViewModel stockReceivingDetailViewModel = new StockReceivingDetailViewModel
                                    {
                                        StockReceivingDetailId = Guid.NewGuid(),
                                        DateKey = dateKey,
                                        FromTime = new DateTime(FromDate.Value.Year, FromDate.Value.Month, FromDate.Value.Day, stockRecevingDetailVM.FromTime.Value.Hour, stockRecevingDetailVM.FromTime.Value.Minute, 0),
                                        ToTime = new DateTime(WorkDate.Value.Year, WorkDate.Value.Month, WorkDate.Value.Day, stockRecevingDetailVM.ToTime.Value.Hour, stockRecevingDetailVM.ToTime.Value.Minute, 0),
                                        ProductId = productionOrderViewModel.ProductId,
                                        ProductAttributes = productionOrderViewModel.ProductAttributes,
                                        Quantity = stockRecevingDetailVM.Quantity_D,
                                        StockRecevingType = item.CatalogCode,
                                        CustomerReference = productionOrderViewModel.TaskId,
                                        StockId = stockid,
                                        CreateTime = currentDate,
                                        CreateBy = CurrentUser.AccountId,
                                        DepartmentId = productionOrderViewModel.DepartmentId,
                                        Phase = productionOrderViewModel.Phase,
                                        MovementType = "ADD"

                                    };
                                    _unitOfWork.StockRecevingDetailRepository.Create(stockReceivingDetailViewModel);
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        Code = HttpStatusCode.NotModified,
                                        Success = false,
                                        Data = "Vui lòng nhập số lượng ghi nhận!",
                                    });
                                }
                                #endregion


                            }
                            //Thêm stock receiving detail nếu type = không  đạt
                            if (item.CatalogCode == "KD")
                            {

                                if (stockRecevingDetailVM.Quantity_KD != null && stockRecevingDetailVM.Quantity_KD != 0)
                                {
                                    if ((stockRecevingDetailVM.FromTime.Value.Hour == 0 && stockRecevingDetailVM.FromTime.Value.Minute == 0) || (stockRecevingDetailVM.ToTime.Value.Hour == 0 && stockRecevingDetailVM.ToTime.Value.Minute == 0))
                                    {
                                        return Json(new
                                        {
                                            Code = HttpStatusCode.NotModified,
                                            Success = false,
                                            Data = "Vui lòng chọn thời gian ghi nhận!",
                                        });
                                    }
                                    #region //Lưu tồn kho
                                    //Get datekey from documentDate
                                    var dateKey = _unitOfWork.UtilitiesRepository.ConvertDateTimeToInt(WorkDate);

                                    StockReceivingDetailViewModel stockReceivingDetailViewModel = new StockReceivingDetailViewModel
                                    {
                                        StockReceivingDetailId = Guid.NewGuid(),
                                        DateKey = dateKey,
                                        FromTime = new DateTime(FromDate.Value.Year, FromDate.Value.Month, FromDate.Value.Day, stockRecevingDetailVM.FromTime.Value.Hour, stockRecevingDetailVM.FromTime.Value.Minute, 0),
                                        ToTime = new DateTime(WorkDate.Value.Year, WorkDate.Value.Month, WorkDate.Value.Day, stockRecevingDetailVM.ToTime.Value.Hour, stockRecevingDetailVM.ToTime.Value.Minute, 0),
                                        ProductId = productionOrderViewModel.ProductId,
                                        ProductAttributes = productionOrderViewModel.ProductAttributes,
                                        Quantity = stockRecevingDetailVM.Quantity_KD,
                                        StockRecevingType = item.CatalogCode,
                                        CustomerReference = productionOrderViewModel.TaskId,
                                        StockId = stockid,
                                        CreateTime = currentDate,
                                        CreateBy = CurrentUser.AccountId,
                                        DepartmentId = productionOrderViewModel.DepartmentId,
                                        Phase = productionOrderViewModel.Phase,
                                        MovementType = "ADD"
                                    };
                                    _unitOfWork.StockRecevingDetailRepository.Create(stockReceivingDetailViewModel);
                                }
                                #endregion

                            }

                        }
                    }
                }
                #region Giảm tồn kho (transferModel)
                if (usageQuantityViewModels != null && usageQuantityViewModels.Count > 0)
                {
                    foreach (var stockTransferVM in usageQuantityViewModels)
                    {
                        #region //Giảm tồn kho
                        if (stockTransferVM.BMSCHDC != null && stockTransferVM.BMSCHDC != 0 && stockTransferVM.ProductAttributes == productionOrderViewModel.ProductAttributes)
                        {
                            //Get datekey from documentDate

                            var dateKey = _unitOfWork.UtilitiesRepository.ConvertDateTimeToInt(WorkDate);

                            TransferDetailViewModel transferDetailViewModel = new TransferDetailViewModel
                            {
                                TransferDetailId = Guid.NewGuid(),
                                DateKey = dateKey,
                                FromTime = currentDate,
                                ToTime = currentDate,
                                ProductId = productionOrderViewModel.ProductId,
                                ProductAttributes = stockTransferVM.ITMNO,
                                Quantity = -stockTransferVM.BMSCHDC,
                                StockRecevingType = "D",
                                FromCustomerReference = productionOrderViewModel.TaskId,
                                //CustomerReference = switchingStagesViewModel.CustomerReference,
                                //FromStockId = stockid,
                                ToStockId = stockid,
                                CreateTime = currentDate,
                                CreateBy = CurrentUser.AccountId,
                                Phase = productionOrderViewModel.Phase,
                                MovementType = "USING"
                            };
                            _unitOfWork.TransferDetailRepository.Create(transferDetailViewModel);
                        }
                        #endregion
                    }
                }
                #endregion

                _context.SaveChanges();
                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = "Ghi nhận thông tin thành công!",
                });
            });
        }

        #endregion

        #region Tìm kiếm thông tin routing để thêm vào lệnh thực thi sản xuất
        [HttpPost]
        public ActionResult _RoutingSearch(ProductionManagementViewModel productionOrderViewModel)
        {
            if (string.IsNullOrEmpty(productionOrderViewModel.StepName))
            {
                productionOrderViewModel.StepName = _unitOfWork.StockRepository.GetStockNameByStockCode(productionOrderViewModel.StepCode);
            }
            return PartialView(productionOrderViewModel);
        }

        [HttpPost]
        public ActionResult _RoutingSearchResult(ProductionManagementViewModel productionOrderViewModel)
        {
            ConfigRepository config = new ConfigRepository(_context);
            var checkinDistance = config.GetBy("GetFullProductionAttributes");
            if (checkinDistance == "1")
            {
                productionOrderViewModel.StepCode = null;
            }
            var data = _unitOfWork.ProductionManagementRepository.GetRoutingInventer(productionOrderViewModel.ProductCode, productionOrderViewModel.StepCode);
            return PartialView(data);
        }


        [HttpPost]
        public ActionResult _RoutingResult(List<ProductionOrderDetailViewModel> productionOrderDetailNew, List<ProductionOrderDetailViewModel> productionOrderDetailOld, Guid TaskId)
        {
            var data = _unitOfWork.ProductionManagementRepository.GetRoutingChecked(productionOrderDetailNew, productionOrderDetailOld, TaskId);
            return PartialView(data);
        }

        public ActionResult _RecordProductionRouting(Guid TaskId, string StepCode)
        {
            var data = _unitOfWork.ProductionManagementRepository.GetExecutionTaskByTaskId(TaskId);
            if (data != null)
            {
                data.Quantity_DLD = (from sR in _context.StockReceivingDetailModel
                                         //Stock
                                     join sTemp in _context.StockModel on sR.StockId equals sTemp.StockId into sList
                                     from s in sList.DefaultIfEmpty()
                                         //ttlsx
                                     join ttlsx in _context.ThucThiLenhSanXuatModel on sR.CustomerReference equals ttlsx.TaskId
                                     where
                                     //Sản phẩm
                                     sR.ProductId == data.ProductId
                                     //Mã Chi tiết
                                     && sR.ProductAttributes == data.ProductAttributes
                                     && s.StockCode == StepCode
                                     && sR.StockRecevingType == "D"
                                     && ttlsx.ParentTaskId == data.ParentTaskId
                                     select sR.Quantity).Sum();
                data.Quantity_DLKD = (from sR in _context.StockReceivingDetailModel
                                          //Stock
                                      join sTemp in _context.StockModel on sR.StockId equals sTemp.StockId into sList
                                      from s in sList.DefaultIfEmpty()
                                          //ttlsx
                                      join ttlsx in _context.ThucThiLenhSanXuatModel on sR.CustomerReference equals ttlsx.TaskId
                                      where
                                      //Sản phẩm
                                       sR.ProductId == data.ProductId
                                      //Mã Chi tiết
                                      && sR.ProductAttributes == data.ProductAttributes
                                      && s.StockCode == StepCode
                                      && sR.StockRecevingType == "KD"
                                      && ttlsx.ParentTaskId == data.ParentTaskId
                                      select sR.Quantity).Sum();
            }
            return PartialView(data);
        }

        [HttpPost]
        public ActionResult _UsageQuantity(string StepCode, string radio, Guid TaskId)
        {
            var data = _unitOfWork.ProductionManagementRepository.UsageQuantity(CurrentUser.SaleOrg, StepCode, radio, TaskId);
            return PartialView(data);
        }

        [HttpPost]
        public ActionResult _DeleteRowRouting(List<ProductionOrderDetailViewModel> productionOrderDetailOld, string ITMNO)
        {
            var data = _unitOfWork.ProductionManagementRepository.DelteRowRouting(productionOrderDetailOld, ITMNO);
            return PartialView("_RoutingResult", data);
        }

        #endregion

        #region Chuyển công đoạn 
        [HttpGet]
        public ActionResult _SwitchingStages(Guid? Barcode)
        {
            //Tìm LSX SAP theo barcode
            var taskId = _unitOfWork.ProductionManagementRepository.GetTTLSXByBarcode(Barcode);

            //Nếu có lệnh thực thi thì hiển thị popup ghi nhận sản lượng
            //Nếu chưa có thì thêm thực thi lệnh sản xuất=> get data để hiển thị popup ghi nhận sản lượng
            if (taskId == null || taskId == Guid.Empty)
            {
                return Json(new
                {
                    Code = HttpStatusCode.NotModified,
                    Success = false,
                    Data = "Thẻ treo không hợp lệ, Do chưa được ghi nhận thẻ treo này lần nào, Anh / Chị vui lòng sử dụng chức năng \"PDA - Ghi nhận sản lượng\"!",
                }, JsonRequestBehavior.AllowGet);
                //Báo lỗi không tìm thấy barcode
            }
            #region Nếu có task theo barcode => show popup theo dữ liệu
            var data = new SwitchingStagesViewModel();
            data = _unitOfWork.ProductionManagementRepository.GetTTLSXForSwitchingStageByTaskId(taskId);
            data.Barcode = Barcode;
            #endregion
            #region Create ViewBag "Công đoạn"
            var listStep = _unitOfWork.ProductionManagementRepository.LoadRoutingOf(data.ProductCode, data.ProductAttributes).Where(x => x.ARBPL_SUB != data.FromStepCode).Select(x => new { x.ARBPL_SUB, display = x.ARBPL_SUB + " | " + x.LTXA1 }).ToList();
            //khi chuyển công đoạn: cho phép lấy công đoạn của cụm 
            if (data.ProductAttributes.Split('.').Length > 1)
            {
                string ProductParentAtrributes = data.ProductAttributes.Split('.')[0];
                var parentStepCodeLst = _unitOfWork.ProductionManagementRepository.LoadRoutingOf(data.ProductCode, ProductParentAtrributes).Where(x => x.ARBPL_SUB != data.FromStepCode).Select(x => new { x.ARBPL_SUB, display = x.ARBPL_SUB + " | " + x.LTXA1 }).ToList();
                listStep.AddRange(parentStepCodeLst);
            }
            listStep = listStep.GroupBy(p => new { p.ARBPL_SUB, p.display }).Select(p => new { p.Key.ARBPL_SUB, p.Key.display }).ToList();

            ViewBag.ToStepCode = new SelectList(listStep, "ARBPL_SUB", "display");
            #endregion
            #region Create ViewBag "Tổ"
            //var listDeparment = _context.DepartmentModel.Select(x => new { x.DepartmentId, display = x.DepartmentCode + " | " + x.DepartmentName });
            //ViewBag.DepartmentId = new SelectList(listDeparment, "DepartmentId", "display");
            #endregion
            return PartialView(data);
        }


        [HttpGet]
        public ActionResult _SwitchingStagesDetail(Guid TaskId)
        {
            var data = _unitOfWork.ProductionManagementRepository.GetTTLSXForSwitchingStageHistoryByTTLSX(TaskId);

            return PartialView(data);
        }
        #endregion

        #region Lưu thông tin chuyển công đoạn
        [HttpPost]
        public JsonResult _SwitchingStages(SwitchingStagesViewModel switchingStagesViewModel)
        {

            return ExecuteContainer(() =>
            {
                if (switchingStagesViewModel == null)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Lỗi! Vui lòng thử lại hoặc liên hệ bộ phận kỹ thuật",
                    });
                }
                //Lấy ngày hiện tại
                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                //Lấy thông tin Barcode
                var Barcode = switchingStagesViewModel.ToBarcode;

                #region Kiểm tra nhập
                //Báo lỗi nếu chưa nhập tổ
                //if (string.IsNullOrEmpty(Barcode) || Guid.Parse(Barcode) == Guid.Empty)
                //{
                //    return Json(new
                //    {
                //        Code = HttpStatusCode.NotModified,
                //        Success = false,
                //        Data = "Vui lòng quét mã QR!",
                //    });
                //}

                //Báo lỗi nếu chưa nhập Stepcode
                //if (string.IsNullOrEmpty(switchingStagesViewModel.ToStepCode))
                //{
                //    return Json(new
                //    {
                //        Code = HttpStatusCode.NotModified,
                //        Success = false,
                //        Data = "Vui lòng chọn công đoạn cần chuyển!",
                //    });
                //}
                if (switchingStagesViewModel.TaskId == null || switchingStagesViewModel.TaskId == Guid.Empty)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Lỗi! Vui lòng thử lại hoặc liên hệ bộ phận kỹ thuật",
                    });
                }
                if (switchingStagesViewModel.FromStepCode == null)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Vui lòng ghi nhận sản lượng trước khi chuyển công đoạn!",
                    });
                }

                if (switchingStagesViewModel.FromStepCode == switchingStagesViewModel.ToStepCode)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Công đoạn \"kế tiếp\" không được trùng công đoạn \"hoàn thành\"!",
                    });
                }

                //Lấy chi tiết thông tin stock của lệnh thực thi cũ
                List<SwitchingStagesViewModel> detailTTLSX = _unitOfWork.ProductionManagementRepository.GetTTLSXForSwitchingStageHistoryByTTLSX(switchingStagesViewModel.TaskId);
                if (detailTTLSX == null || detailTTLSX.Count() == 0)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Không có dữ liệu để chuyển!",
                    });
                }
                #endregion


                #region Công đoạn
                //Lấy thông tin stock cũ
                var stockIdOld = _unitOfWork.StockRepository.GetStockIdByStockCode(switchingStagesViewModel.FromStepCode);

                //Lấy thông tin stock mới
                var stockIdNew = _unitOfWork.StockRepository.GetStockIdByStockCode(switchingStagesViewModel.ToStepCode);
                #endregion

                //Kiểm tra Barcode đã tồn tại chưa
                //var CheckBarcode = _unitOfWork.ProductionManagementRepository.GetTTLSXByBarcode(Barcode);

                #region Lệnh thực thi sản xuất mới
                //if (CheckBarcode == null || CheckBarcode == Guid.Empty)
                //{
                //    //Tạo TTLSX mới theo Barcode
                //    Guid HangTagId = Guid.Parse(Barcode);
                //    var handTag = _context.HangTagModel.Where(p => p.HangTagId == HangTagId).FirstOrDefault();
                //    if (handTag != null && handTag.CustomerReference.HasValue)
                //    {
                //        //Thêm mới lệnh thực thi và lấy dữ liệu
                //        switchingStagesViewModel.CustomerReference = _unitOfWork.ProductionManagementRepository.CreateNewExecutionTask(handTag.CustomerReference.Value, Barcode, CurrentUserId: CurrentUser.AccountId);
                //        var ttlxsNew = _context.ThucThiLenhSanXuatModel.FirstOrDefault(x => x.TaskId == switchingStagesViewModel.CustomerReference);
                //        if (ttlxsNew != null)
                //        {
                //            #region //Cập nhật công đoạn sản xuất
                //            ttlxsNew.Property6 = switchingStagesViewModel.ToStepCode;
                //            ttlxsNew.StockId = stockIdNew;
                //            ttlxsNew.Actived = true;
                //            #endregion
                //        }
                //        _context.Entry(ttlxsNew).State = EntityState.Modified;
                //    }
                //    else
                //    {
                //        return Json(new
                //        {
                //            Code = HttpStatusCode.NotModified,
                //            Success = false,
                //            Data = "Vui lòng chọn thẻ treo khác!",
                //        });
                //    }
                //}
                //else
                //{
                //    var ttlxsNew = _context.ThucThiLenhSanXuatModel.FirstOrDefault(x => x.TaskId == CheckBarcode);
                //    if (ttlxsNew.StockId == null || ttlxsNew.StockId == Guid.Empty)
                //    {
                //        ttlxsNew.Property6 = switchingStagesViewModel.ToStepCode;
                //        ttlxsNew.StockId = stockIdNew;
                //        ttlxsNew.Actived = true;
                //        _context.Entry(ttlxsNew).State = EntityState.Modified;
                //    }
                //    else
                //    {
                //        if (ttlxsNew.StockId != stockIdNew || ttlxsNew.ToStockCode != null)
                //        {
                //            return Json(new
                //            {
                //                Code = HttpStatusCode.NotModified,
                //                Success = false,
                //                Data = "Công đoạn chuyển đến không khớp. Vui lòng chọn thẻ treo khác!",
                //            });
                //        }
                //    }
                //    switchingStagesViewModel.CustomerReference = CheckBarcode;
                //}

                #endregion



                #region Lệnh thực thi sản xuất cũ
                //Lấy thông tin lênh thực thi cũ
                var ttlxsOld = _context.ThucThiLenhSanXuatModel.FirstOrDefault(x => x.TaskId == switchingStagesViewModel.TaskId);
                if (ttlxsOld != null)
                {
                    #region //Cập nhật công đoạn sản xuất
                    //ttlxsOld.Actived = false;
                    //ttlxsOld.ToStockId = stockIdNew;
                    //ttlxsOld.ToStockCode = switchingStagesViewModel.ToStepCode;
                    ttlxsOld.Property6 = switchingStagesViewModel.ToStepCode;
                    ttlxsOld.StockId = stockIdNew;
                    ttlxsOld.TransferTime = currentDate;
                    ttlxsOld.ToTaskId = switchingStagesViewModel.CustomerReference;

                    #endregion
                }
                _context.Entry(ttlxsOld).State = EntityState.Modified;
                #endregion


                //Lưu thông tin vào Transfer Detai
                foreach (var item in detailTTLSX)
                {
                    //Get datekey from documentDate

                    var dateKey = _unitOfWork.UtilitiesRepository.ConvertDateTimeToInt(currentDate);
                    #region Lưu thông tin StockReceivingDetail
                    StockReceivingDetailViewModel stockReceivingDetailViewModel = new StockReceivingDetailViewModel
                    {
                        StockReceivingDetailId = Guid.NewGuid(),
                        DateKey = dateKey,
                        FromTime = currentDate,
                        ToTime = currentDate,
                        ProductId = item.ProductId,
                        ProductAttributes = item.ITMNO,
                        Quantity = -item.Quantity,
                        StockRecevingType = "D",
                        CustomerReference = item.CustomerReference,
                        StockId = stockIdOld,
                        CreateTime = currentDate,
                        CreateBy = CurrentUser.AccountId,
                        Phase = item.Phase,
                        MovementType = "TRANSFER"
                    };
                    _unitOfWork.StockRecevingDetailRepository.Create(stockReceivingDetailViewModel);
                    #endregion

                    #region Lưu thông tin transferDetail
                    TransferDetailViewModel transferDetail = new TransferDetailViewModel
                    {
                        TransferDetailId = Guid.NewGuid(),
                        DateKey = dateKey,
                        FromTime = currentDate,
                        ToTime = currentDate,
                        ProductId = item.ProductId,
                        ProductAttributes = item.ITMNO,
                        Quantity = item.Quantity,
                        StockRecevingType = "D",
                        FromCustomerReference = item.CustomerReference,
                        //CustomerReference = switchingStagesViewModel.CustomerReference,
                        FromStockId = stockIdOld,
                        ToStockId = stockIdNew,
                        CreateTime = currentDate,
                        CreateBy = CurrentUser.AccountId,
                        Phase = item.Phase,
                        MovementType = "MOVEIN"
                    };
                    _unitOfWork.TransferDetailRepository.Create(transferDetail);
                    #endregion
                }
                _context.SaveChanges();
                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = "Chuyển công đoạn thành công!",
                });
            });
        }
        #endregion

        #region Xác nhận hoàn tất công đoạn lớn
        [HttpGet]
        public ActionResult _ConfirmWorkCenter(Guid? Barcode = null)
        {
            //Tìm LSX SAP theo barcode
            var taskId = _unitOfWork.ProductionManagementRepository.GetTTLSXByBarcode(Barcode);

            //Nếu không có lệnh thực thi thì báo lỗi
            if (taskId == null || taskId == Guid.Empty)
            {
                //Báo lỗi không tìm thấy barcode
                return Json(new
                {
                    Code = HttpStatusCode.NotModified,
                    Success = false,
                    Data = "Thẻ treo không hợp lệ!",
                }, JsonRequestBehavior.AllowGet);
            }
            #region Nếu có task theo barcode => show popup theo dữ liệu
            var data = new ProductionManagementViewModel();
            data = _unitOfWork.ProductionManagementRepository.GetExecutionTaskByTaskId(taskId);
            #endregion
            return PartialView(data);
        }

        [HttpGet]
        public ActionResult _ConfirmWorkCenterDetail(Guid TaskId, string StepCode)
        {
            var data = _unitOfWork.ProductionManagementRepository.GetTTLXSForConfirmWorkCenter(TaskId, StepCode);

            return PartialView(data);
        }
        #endregion
        #region Lưu thông tin Xác nhận hoàn tất công đoạn lớn
        [HttpPost]
        public JsonResult _ConfirmWorkCenter(ConfirmWorkCenterViewModel confirmWorkCenterViewModel)
        {
            return ExecuteContainer(() =>
            {
                if (confirmWorkCenterViewModel == null || confirmWorkCenterViewModel.TaskId == null || confirmWorkCenterViewModel.TaskId == Guid.Empty)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = true,
                        Data = "Lỗi, vui lòng tải lại trang và thử lại!",
                    });
                }

                var currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                //Hoàn tất công đoạn lớn - không cho lưu trước ngày khóa sổ
                var dateClosed = _context.DateClosedModel.FirstOrDefault();
                if (dateClosed != null)
                {
                    if (confirmWorkCenterViewModel.WorkCenterConfirmTime.HasValue && confirmWorkCenterViewModel.WorkCenterConfirmTime.Value.Date < dateClosed.DateClosed.Date)
                    {
                        return Json(new
                        {
                            Code = HttpStatusCode.NotModified,
                            Success = false,
                            Data = string.Format("Không thể hoàn tất công đoạn lớn trước ngày khóa sổ! ({0:dd/MM/yyyy})", dateClosed.DateClosed.Date),
                        });
                    }
                }
                var dateKey = _unitOfWork.UtilitiesRepository.ConvertDateTimeToInt(confirmWorkCenterViewModel.WorkCenterConfirmTime);
                //Lấy thông tin lênh thực thi
                var ttlxs = _unitOfWork.ProductionManagementRepository.GetExecutionTaskByTaskId(confirmWorkCenterViewModel.TaskId);
                #region nhân viên confirm
                confirmWorkCenterViewModel.ConfirmBy = CurrentUser.AccountId;
                #endregion

                var StockReceivingDetail = _context.StockReceivingDetailModel.Where(x => x.CustomerReference == ttlxs.TaskId && x.ProductId == ttlxs.ProductId && x.ProductAttributes == ttlxs.ProductAttributes && x.Phase == ttlxs.Phase && x.StockId == ttlxs.StepId);

                if (StockReceivingDetail != null)
                {

                    foreach (var item in StockReceivingDetail)
                    {
                        #region hoàn tất công đoạn lớn
                        item.IsWorkCenterCompleted = true;
                        item.ConfirmWorkCenter = confirmWorkCenterViewModel.ConfirmWorkCenter;
                        item.ConfirmDatekey = dateKey;
                        item.WorkCenterConfirmTime = confirmWorkCenterViewModel.WorkCenterConfirmTime;
                        item.ConfirmBy = confirmWorkCenterViewModel.ConfirmBy;
                        _context.Entry(item).State = EntityState.Modified;
                        #endregion
                    }
                    _context.SaveChanges();

                }
                //Lấy thông tin StockReceivingDetail
                //Sau khi confirm thì đẩy lại thông tin lên SAP
                var StockReceiving = _context.StockReceivingDetailModel.Where(x => x.CustomerReference == ttlxs.TaskId);
                if (StockReceiving != null && StockReceiving.Count() > 0)
                {
                    foreach (var item in StockReceiving)
                    {
                        item.isSendToSAP = null;
                        _context.Entry(item).State = EntityState.Modified;

                    }
                }
                _context.SaveChanges();

                #region Tạo phiếu QC sau khi confirm (Loại "Cụm", "CUM", "cụm")
                var routing = _context.RoutingInventorModel.Where(x => x.ITMNO == ttlxs.ProductAttributes && x.MATNR == ttlxs.ProductCode).FirstOrDefault();
                if (routing != null && (routing.BMEIN.ToLower() == "cum" || routing.BMEIN.ToLower() == "cụm"))
                {
                    _unitOfWork.QualityControlRepository.CreateQualityControl(confirmWorkCenterViewModel);
                }

                #endregion

                #region Cập nhật dữ liệu cho BC01
                var DSXId = _context.TaskModel.Where(p => p.TaskId == ttlxs.ParentTaskId).Select(p => p.ParentTaskId).FirstOrDefault();
                new Thread(delegate ()
                {
                    UpdateBC01(DSXId, ttlxs.ParentTaskId);
                }).Start();
                #endregion

                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = "Cập nhật công đoạn lớn thành công!",
                });
            });
        }
        private void UpdateBC01(Guid? DSXId, Guid? LSXSAPId)
        {
            BC00ReportRepository _bcRepository = new BC00ReportRepository(_context);
            _bcRepository.UpdateBC01FromDSX(DSXId, LSXSAPId);
        }
        #endregion
        #region Helper
        [HttpGet]
        public JsonResult GetWorkShopBy(Guid DepartmentId)
        {
            //Lấy thông tin WorkShop
            var data = _unitOfWork.WorkShopRepository.GetWorkShopByDepartment(DepartmentId);
            if (data == null)
            {
                //Báo lỗi khi Department không thuộc phân xưởng nào
            }
            return Json(new
            {
                Data = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult _GetDepartmentAndWorkShop(Guid DepartmentId)
        {
            var department = _context.DepartmentModel.Where(x => x.DepartmentId == DepartmentId).FirstOrDefault();
            //Lấy thông tin WorkShop
            var WorkShopList = _context.WorkShopModel.Where(x=> x.Actived == true && x.CompanyId == CurrentUser.CompanyId).Select(x => new { x.WorkShopId, display = x.WorkShopCode + " | " + x.WorkShopName }).ToList();
            ViewBag.WorkShopId = new SelectList(WorkShopList, "WorkShopId", "display", department.WorkShopId);
            //Lấy thông tin Department
            var departmentList = _context.DepartmentModel.Where(x => x.Actived == true && x.WorkShopId == department.WorkShopId).Select(x => new { x.DepartmentId, display = x.DepartmentCode + " | " + x.DepartmentName });
            ViewBag.DepartmentId = new SelectList(departmentList, "DepartmentId", "display", department.DepartmentId);
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetDepartmentBy(Guid WorkShopId)
        {
            //Lấy thông tin WorkShop
            var data = _context.DepartmentModel.Where(x => x.Actived == true && x.WorkShopId == WorkShopId).Select(x => new { x.DepartmentId, display = x.DepartmentCode + " | " + x.DepartmentName });
            ViewBag.DepartmentId = new SelectList(data, "DepartmentId", "display");
            return PartialView();
        }
        #endregion

        #region Check thông tin routing nếu có dữ liệu thì mới hiển thị nút "Chuyển và Ghi nhận"
        public ActionResult CheckIsHasRouting(string ProductCode, string ProductAttributes, string ToStockCode)
        {
            return ExecuteContainer(() =>
            {
                bool? isHasRouting = false;
                //Check nếu có routing mới cho phép ghi nhận tiếp
                string sqlQuery = "SELECT * FROM MES.View_Product_Material WHERE WERKS = '" + CurrentUser.CompanyCode + "' AND MATNR = '" + ProductCode + "' AND ITMNO = '" + ProductAttributes + "' AND ARBPL_SUB = '" + ToStockCode + "' ";
                var routingLst = _context.Database.SqlQuery<View_Product_MaterialViewModel>(sqlQuery).ToList();
                if (routingLst != null && routingLst.Count > 0)
                {
                    isHasRouting = true;
                }
                return _APISuccess(isHasRouting);
            });
        }
        #endregion
    }
}