using ISD.Constant;
using ISD.Core;
using ISD.Repositories;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Work.Controllers
{
    public class WorkOrderController : BaseController
    {
        // GET: WorkOrder
        #region Index
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult _PaggingServerSide(DatatableViewModel model, WorkOrderViewModel searchModel)
        {
            try
            {
                //Get list WorkOrder
                var workOrderList = _unitOfWork.WorkOrderRepository.SearchQuery(searchModel);

                //Phân trang
                int filteredResultsCount = 0;
                int totalResultsCount = 0;

                var res = CustomSearchRepository.CustomSearchFunc<WorkOrderViewModel>(model, out filteredResultsCount, out totalResultsCount, workOrderList, "STT");
                if (res.Any())
                {
                    int i = model.start;
                    foreach (var item in res)
                    {
                        i++;
                        item.STT = i;
                    }
                }
                return Json(new
                {
                    draw = model.draw,
                    recordsTotal = totalResultsCount,
                    recordsFiltered = filteredResultsCount,
                    data = res,
                });
            }
            catch//(Exception ex)
            {
                return Json(null);
            }
        }
        #endregion
        public ActionResult Detail(Guid? Id)
        {
            var products = _context.ProductModel.AsNoTracking();
            // Lấy detail cảu lệnh sx 
            var workOrder = _context.WorkOrderModel
                .Where(x => x.WorkOrderId == Id)
                .Include(x => x.WorkOrder_Mold_Mapping)
                .Select(x => new WorkOrderViewModel
                {
                    //Id
                    WorkOrderId = x.WorkOrderId,
                    //Ưu tiên
                    Priority = x.Priority,
                    //Lệnh sản xuất tổng hợp
                    ParentWorkOrderCode = x.ParentWorkOrderCode.Trim(),
                    //Lệnh sản xuất 
                    WorkOrderCode = x.WorkOrderCode,
                    //Mã TP/BTP
                    ProductCode = x.ProductCode,
                    //Tên TP/BTP
                    ProductName = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                    //BOM Version
                    BOMVersion = x.BOMVersion,
                    //Số lượng phát lệnh
                    WorkOrderQty = x.Quantity,
                    //Đơn vị phát lệnh 
                    WorkOrderUnit = x.Unit,
                    //Ngày phát lệnh
                    DocumentDate = x.DocumentDate,
                    //Ngày bắt đầu sx dự kiến
                    EstimateFromDate = x.EstimateFromDate,
                    //Ngày kết thúc sx dự kiến 
                    EstimateToDate = x.EstimateToDate,
                    //Ngày bắt đầu sx thực tế
                    ActualStartDate = x.ActualStartDate,
                    //Ngày kết thúc sx thực tế
                    ActualEndDate = x.ActualEndDate,
                    PONumber = x.SOCode,
                    ProductSize = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).Size,
                    //Kích thước khổ trải SP(mm)
                    ProductSpreadSize = x.ProductSpreadSize,
                    //Đơn vị gia công TP
                    ProcessingUnit = x.ProcessingUnit,
                    //Mã khuôn
                    WorkOrder_Mold_Mapping = x.WorkOrder_Mold_Mapping.Distinct().ToList(),
                    //Số phiếu thiết kế
                    DesignVote = x.DesignVote,
                    //Phụ trách thiết kế
                    DesignBy = x.DesignBy,
                    //Ghép bài 
                    MatchCard = x.MatchCard,
                    //Kiểu in
                    PrintStyle = x.PrintStyle,
                    //NCC kẽm
                    ZincSupplier = x.ZincSupplier,
                    //Ngày yêu cầu có hàng in
                    PrintReqDate = x.PrintReqDate,
                    //Trạng thái
                    PrintStatus = x.PrintStatus,
                    //Component
                    WorkOrder_Product_Mapping = x.WorkOrder_Product_Mapping.Distinct().ToList(),
                    //Description
                    LSX_ProductName = products.FirstOrDefault(p => p.ProductCode == x.ProductCode).ProductName,
                })
                .FirstOrDefault();

            // Lấy danh sách Khuoon Mapping
            ViewBag.MoldMapping = workOrder.WorkOrder_Mold_Mapping;
            // Lấy danh sách component
            ViewBag.ComponentMapping = workOrder.WorkOrder_Product_Mapping;
            //get routingLSX
            ViewBag.RoutingLSX = (from db in _context.WorkOrderModel
                                  join rt in _context.Product_Routing_Mapping on db.ProductCode equals rt.ProductCode
                                  join r in _context.RoutingModel on rt.StepCode equals r.StepCode

                                  where db.WorkOrderId == Id

                                  select new WorkOrderDetailViewModel
                                  {
                                      WorkOrderId = db.WorkOrderId,
                                      StepCode = rt.StepCode,
                                      StepName = r.StepName,
                                      TotalProduct = db.Quantity,
                                      //Unit = db.WorkOrderUnit,
                                      ProductPerPage = rt.ProductPerPage,
                                      ProductByStep = (db.Quantity / rt.ProductPerPage),
                                      ProductionGuide = rt.ProductionGuide,
                                      OrderIndex = (int)rt.OrderIndex
                                  }).OrderBy(x => x.OrderIndex).ToList();

            return View(workOrder);
        }
        public void CreateViewBag(string status = null)
        {
            //trạng thái LSX
            var lststatus = _context.CatalogModel.Where(x => x.Actived == true && x.CatalogTypeCode == "WorkOrderStatus").ToList();
            ViewBag.PrintStatus = new SelectList(lststatus, "CatalogCode", "CatalogText_vi", status);
            //Common date
            var CurrentDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.ReceiveDate = new SelectList(CurrentDateList, "CatalogCode", "CatalogText_vi");
        }

        public ActionResult SerialSearch(Guid? WorkOrderId)
        {
            ProductViewModel model = new ProductViewModel();
            //return PartialView("~/Areas/Work/Views/WorkOrder/_SerialSearch.cshtml", model);

            // Lấy danh sách khuôn cùa workOrderId
            var workOrder = _context.WorkOrderModel.Where(x => x.WorkOrderId == WorkOrderId).Include(x => x.WorkOrder_Mold_Mapping).FirstOrDefault();
            ViewBag.LstMold = workOrder.WorkOrder_Mold_Mapping.ToList();
            return View();
        }

        public ActionResult SetWorkOrderPrintMoldSerial(Guid printMoldMappingId, string serial)
        {
            var mapping = _context.WorkOrder_Mold_Mapping.Where(x => x.Id == printMoldMappingId).FirstOrDefault();

            if (mapping == null)
                return Json(new { sucess = false, message = "Không tìm thấy khuôn" }, JsonRequestBehavior.AllowGet);

            var productId = _context.ProductModel.Where(x => x.ProductCode == mapping.MoldCode && x.Serial == serial).Select(x => x.ProductId).FirstOrDefault();

            mapping.ProductId = productId;

            _context.Entry(mapping).State = EntityState.Modified;
            _context.SaveChanges();

            return Json(new { sucess = true, message = "Cập nhật mã serial thành công" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _GetListSerialByMoldCode(string code)
        {
            // lấy danh sách sản phẩm với mã phân cấp snả phẩm là 32 => Khuôn + mã khuôn

            var parentCateId = _context.CategoryModel.Where(x => x.CategoryCode == "32").Select(x => x.CategoryId).FirstOrDefault();
            /*
             * 1. Mã khuôn 
             * 2. Số Serial
             * 3. Tình trạng
             * 4. Dài
             * 5. Rộng
             * 6. Cao
             * 7. Yếm
             * 8. Hông
             * 9. Mã film kiểm tra
             * 10. Tên khắc trên khuôn
             * 11. Kho khuôn
             * 12. Bin
             * 13. Ngày bảo trì tiếp theo
             * 14. Số lần dập
             * 15. Mô tả
             */
            var lstProduct = (from p in _context.ProductModel
                              join sttCate in _context.CatalogModel on new { stt = p.Status, type = "Status" } equals new { stt = sttCate.CatalogCode, type = sttCate.CatalogTypeCode } into temp
                              from sttCateLeftJoin in temp.DefaultIfEmpty()

                              where p.ParentCategoryId == parentCateId && p.ProductCode == code

                              select new SerialSearchResultViewModel
                              {
                                  //1. Mã khuôn
                                  ProductCode = p.ProductCode,
                                  //2. Số Serial
                                  Serial = p.Serial,
                                  //3. Tình trạng mã
                                  Status = p.Status,
                                  //3. Tình trạng text
                                  StatusName = sttCateLeftJoin.CatalogText_vi,
                                  //4. Dài
                                  Specifications_Length = p.Specifications_Length,
                                  //5. Rộng
                                  Specifications_Width = p.Specifications_Width,
                                  //6. Cao
                                  Specifications_Height = p.Specifications_Height,
                                  //7. Yếm
                                  Specifications_Overalls = p.Specifications_Overalls,
                                  //8. Hông
                                  Specifications_Side = p.Specifications_Side,
                                  //9. Mã film kiểm tra
                                  PrintMoldFilm = p.PrintMoldFilm,
                                  //10. Tên khắc trên khuôn
                                  PrintMoldName = p.PrintMoldName,
                                  //Kho khuôn
                                  MoldStorage = "",
                                  //12. Bin
                                  Bin = p.Bin,
                                  //13. Ngày bảo trì tiếp theo
                                  LastMaintenanceDate = p.LastMaintenanceDate,
                                  NumberMaintenanceDateAlert = p.MaintenanceAlert,
                                  //14.Số lần dập
                                  StamQty = p.CurrentStampeQuantity + "/" + p.StampQuantityAlert,
                                  //15.Mô tả
                                  Description = p.Description
                              }).OrderBy(x => x.Serial).ToList();

            return PartialView(lstProduct);
        }

        public ActionResult PaggingServerSide_SerialSearch(DatatableViewModel model, ProductViewModel searchModel)
        {
            try
            {
                int filteredResultsCount;
                int totalResultsCount;

                var query = (from db in _context.ProductModel
                             join cate in _context.CategoryModel on db.ParentCategoryId equals cate.CategoryId
                             where cate.CategoryCode == "32"
                             && db.ProductCode == searchModel.ProductCode
                             select new ProductViewModel
                             {
                                 ProductId = db.ProductId,
                                 ProductName = db.ProductName,
                                 ProductCode = db.ProductCode,
                                 Serial = db.Serial,
                                 Status = db.Status,
                                 LocationNote = db.LocationNote,
                                 Specifications_Length = db.Specifications_Length,
                                 Specifications_Width = db.Specifications_Width,
                                 Specifications_Height = db.Specifications_Height,
                                 Specifications_Side = db.Specifications_Side,
                                 Specifications_Overalls = db.Specifications_Overalls,
                                 LastMaintenanceDate = db.LastMaintenanceDate,
                                 Bin = db.Bin,
                                 PrintMoldFilm = db.PrintMoldFilm,
                                 PrintMoldDate = db.PrintMoldDate,
                                 Description = db.Description,
                                 MaintenanceAlert = db.MaintenanceAlert,
                                 StampQuantityAlert = db.StampQuantityAlert,
                                 CurrentStampeQuantity = db.CurrentStampeQuantity,
                                 StampQuantity = db.StampQuantity,
                                 WorkOrderId = searchModel.ProductId, // lưu lại mã work order
                             }
                             ).OrderBy(x => x.Serial);

                var res = CustomSearchRepository.CustomSearchFunc<ProductViewModel>(model, out filteredResultsCount, out totalResultsCount, query, "STT");
                if (res != null && res.Count() > 0)
                {
                    int i = model.start;
                    foreach (var item in res)
                    {
                        i++;
                        item.STT = i;
                    }
                }
                return Json(new
                {
                    draw = model.draw,
                    recordsTotal = totalResultsCount,
                    recordsFiltered = filteredResultsCount,
                    data = res,
                });
            }
            catch //(Exception ex)
            {
                return Json(null);
            }
        }
        public ActionResult UpdateSerial(Guid? WorkOrderId, string Serial = null)
        {
            //lấy mapping LSX - khuôn 
            var workorder = _context.WorkOrder_Mold_Mapping.FirstOrDefault(x => x.WorkOrderId == WorkOrderId);
            if (workorder != null)
            {
                //thêm serial
                _context.Entry(workorder).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Serial.ToLower()),
                    JsonRequestBehavior.AllowGet
                });
            }
            return Json(false);
        }
    }
}