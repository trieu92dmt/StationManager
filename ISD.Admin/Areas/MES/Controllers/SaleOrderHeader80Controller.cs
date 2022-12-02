using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class SaleOrderHeader80Controller : BaseController
    {
        // GET: SaleOrderHeader80

        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var SelectedCommonDate = "Custom";
            var SelectedCommonDate2 = "Custom";
            CreateViewBag(SelectedCommonDate, SelectedCommonDate2);
            return View();

        }
        public ActionResult _Search(SaleOrderHeader80ViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                searchViewModel.VKORG = CurrentUser.CompanyCode;
                var data = _unitOfWork.SaleOrderHeader80Repository.Search(searchViewModel).ToList();

                return PartialView(data);
            });
        }

        public ActionResult _PaggingServerSide(DatatableViewModel model, SaleOrderHeader80ViewModel searchViewModel)
        {
            try
            {
                // action inside a standard controller
                int filteredResultsCount;
                int totalResultsCount;

                var query = _unitOfWork.SaleOrderHeader80Repository.Search(searchViewModel);
                var res = CustomSearchRepository.CustomSearchFunc<SaleOrderHeader80ViewModel>(model, out filteredResultsCount, out totalResultsCount, query, "STT");
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
                    data = res
                });
            }
            catch //(Exception ex)
            {
                return Json(null);
            }
        }

        public ActionResult ViewSO(string Id)
        {
            // Lấy thông tin Sale Order
            var SaleOrder = _unitOfWork.SaleOrderHeader80Repository.GetSaleOrderHeader80BySaleOrder(Id);

            return View(SaleOrder);
        }
        //Danh sách Bom detail
        public ActionResult _GetBomDetails(string VBELN, string MATNR = null, string STLAN = null)
        {
            var lst = _unitOfWork.SaleOrderHeader80Repository.GetBOMDetailWithSaleOrder(VBELN, MATNR, STLAN);
            return PartialView(lst);
        }
        public ActionResult ScheduleLines(string VBELN, string POSNR)
        {
            // Lấy thông tin Sale Order
            var SaleOrder = _unitOfWork.SaleOrderHeader80Repository.GetScheduleLines(VBELN, POSNR);

            return PartialView("_ScheduleLines",SaleOrder);
        }


        //Xem chi tiết SOLine
        public ActionResult ViewDetailSO(string VBELN, string POSNR)
        {
            // Lấy thông tin Sale Order
            var SaleOrder = _unitOfWork.SaleOrderHeader80Repository.GetSaleOrderItem80BySaleOrderAndLine(VBELN, POSNR);

            return View(SaleOrder);
        }

 

        //Danh sách SO Line
        public ActionResult _SaleOrderItemList(string VBELN)
        {
            return ExecuteSearch(() =>
            {
                //Danh sách sale order item
                var SaleOrderItemList = _unitOfWork.SaleOrderHeader80Repository.GetSaleOrderItem80BySaleOrder(VBELN);

                return PartialView(SaleOrderItemList);
            });
        }

        public void CreateViewBag(string SelectedCommonDate, string SelectedCommonDate2)
        {
            //Get list commonDate
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.CommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            ViewBag.CommonDate2 = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate2);
        }
    }
}