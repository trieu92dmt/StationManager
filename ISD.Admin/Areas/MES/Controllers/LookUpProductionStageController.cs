using ISD.Core;
using ISD.Extensions;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class LookUpProductionStageController : BaseController
    {
        #region Index
        [ISDAuthorizationAttribute]
        // GET: LookUpProductionStage
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public ActionResult _Search(List<string> ProductCode)
        {
            return ExecuteSearch(() =>
            {
                if (ProductCode == null)
                {
                    ProductCode = new List<string>();
                }
                var routings = _unitOfWork.LookUpProductionStageRepository.SearchRouting(ProductCode);
                //var routings = (from product_Routing_Mapping in _context.Product_Routing_Mapping
                //                    //leftjoin với bảng ProductModel
                //                join productModel in _context.ProductModel on product_Routing_Mapping.ProductCode equals productModel.ProductCode
                //                //leftjoin với bảng RoutingModel
                //                join routingModel in _context.RoutingModel on product_Routing_Mapping.StepCode equals routingModel.StepCode
                //                //leftjoin với bảng Product_Routing_Mold_Mapping
                //                join productRoutingMoldMapping in _context.Product_Routing_Mold_Mapping on product_Routing_Mapping.Product_Routing_MappingId equals productRoutingMoldMapping.Product_Routing_MappingId into prm4
                //                from temp in prm4.DefaultIfEmpty()

                //                where
                //                //search by ProductCode
                //                (ProductCode.Count == 0 || ProductCode.Contains(product_Routing_Mapping.ProductCode))
                //                select new ProductRoutingMappingViewModel()
                //                {
                //                    //1. Phiên bản Routing
                //                    RoutingVersion = product_Routing_Mapping.RoutingVersion,
                //                    //2. Mã thành phẩm/bán thành phẩm
                //                    ProductCode = product_Routing_Mapping.ProductCode,
                //                    //3. Tên thành phẩm/bán thành phẩm
                //                    ProductName = productModel.ProductName,
                //                    //4. Thứ tự công đoạn
                //                    OrderIndex = product_Routing_Mapping.OrderIndex,
                //                    //5. Mã công đoạn
                //                    StepCode = product_Routing_Mapping.StepCode,
                //                    //6. Tên công đoạn
                //                    StepName = routingModel.StepName,
                //                    //7. Hướng dẫn sản xuất
                //                    ProductionGuide = product_Routing_Mapping.ProductionGuide,
                //                    //8. % ước tình hoàn thành
                //                    EstimateComplete = product_Routing_Mapping.EstimateComplete,
                //                    //9. Thời gian định mức
                //                    RatedTime = product_Routing_Mapping.RatedTime,
                //                    //10. Khuôn
                //                    MoldCode = temp.ProductCode,
                //                    //11.Số SP/tờ
                //                    ProductPerPage = product_Routing_Mapping.ProductPerPage,
                //                })
                //                .OrderBy(x => new { x.RoutingVersion, x.OrderIndex })
                //                .GroupBy(x => new
                //                {
                //                    //1. Phiên bản Routing
                //                    x.RoutingVersion,
                //                    //2. Mã thành phẩm/bán thành phẩm
                //                    x.ProductCode,
                //                    //3. Tên thành phẩm/bán thành phẩm
                //                    x.ProductName,
                //                    //4. Thứ tự công đoạn
                //                    x.OrderIndex,
                //                    //5. Mã công đoạn
                //                    x.StepCode,
                //                    //6. Tên công đoạn
                //                    x.StepName,
                //                    //7. Hướng dẫn sản xuất
                //                    x.ProductionGuide,
                //                    //8. % ước tình hoàn thành
                //                    x.EstimateComplete,
                //                    //9. Thời gian định mức
                //                    x.RatedTime,
                //                    //10.Số SP/tờ
                //                    x.ProductPerPage
                //                }).ToList()
                //            .Select(eg => new ProductRoutingMappingViewModel()
                //            {
                //                //1. Phiên bản Routing
                //                RoutingVersion = eg.Key.RoutingVersion,
                //                //2. Mã thành phẩm/bán thành phẩm
                //                ProductCode = eg.Key.ProductCode,
                //                //3. Tên thành phẩm/bán thành phẩm
                //                ProductName = eg.Key.ProductName,
                //                //4. Thứ tự công đoạn
                //                OrderIndex = eg.Key.OrderIndex,
                //                //5. Mã công đoạn
                //                StepCode = eg.Key.StepCode,
                //                //6. Tên công đoạn
                //                StepName = eg.Key.StepName,
                //                //7. Hướng dẫn sản xuất
                //                ProductionGuide = eg.Key.ProductionGuide,
                //                //8. % ước tình hoàn thành
                //                EstimateComplete = eg.Key.EstimateComplete,
                //                //9. Thời gian định mức
                //                RatedTime = eg.Key.RatedTime,
                //                //10. Khuôn
                //                MoldCode = string.Join(", ", eg.Select(i => i.MoldCode)),
                //                //11.Số SP/tờ
                //                ProductPerPage = eg.Key.ProductPerPage
                //            })
                //            .ToList();
                return PartialView(routings);
            });
        }
        #endregion

        /// <summary>
        /// Lấy danh sách routingInventer
        /// </summary>
        /// <returns></returns>
        public PartialViewResult ListProductionAttribute(string ProductCode)
        {
            var data = _unitOfWork.ProductionManagementRepository.GetRoutingInventer(ProductCode).Select(x => new { x.ProductAttributes, view = x.ProductAttributes + " | " + x.KTEXT }).ToList();
            ViewBag.ProductAttributes = new SelectList(data, "ProductAttributes", "view");
            return PartialView("_ListRoutingInventer");
        }

        #region ViewBag, Helper
        public void CreateViewBag()
        {
            //Máy móc chuyền
            var ProductList = (from product_Routing_Mapping in _context.Product_Routing_Mapping
                               join productModel in _context.ProductModel on product_Routing_Mapping.ProductCode equals productModel.ProductCode
                               select new ProductRoutingMappingViewModel()
                               {
                                   ProductCode = productModel.ProductCode,
                                   ProductName = productModel.ProductName,
                               }).ToList()
                               .GroupBy(x => new
                               {
                                   x.ProductCode,
                                   x.ProductName
                               }).ToList()
                               .Select(eg => new ProductRoutingMappingViewModel()
                               {
                                   ProductCode = eg.Key.ProductCode,
                                   ProductName = eg.Key.ProductCode + " | " + eg.Key.ProductName
                               })
                               .OrderBy(x => x.ProductCode)
                               .ToList();
            ViewBag.ProductList = new SelectList(ProductList, "ProductCode", "ProductName");
        }
        #endregion

        #region ViewBag, Helper
        public ActionResult SearchProductCode(string searchTerm)
        {
            //Máy móc chuyền
            var ProductList = _context.ProductModel.Where(x => x.ProductCode != null && x.ProductCode != "" && x.ProductCode.Contains(searchTerm))
                                                    .OrderBy(x => x.ProductCode)
                                                    .Select(x => new
                                                    {
                                                        value = x.ProductCode,
                                                        text = x.ProductCode + " | " + x.ProductName,
                                                    }).Distinct().Take(10).ToList();
            return Json(ProductList, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}