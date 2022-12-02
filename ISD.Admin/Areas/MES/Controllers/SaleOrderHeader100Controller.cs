using ISD.Constant;
using ISD.Core;
using ISD.Extensions;
using ISD.Repositories;
using ISD.Repositories.Excel;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class SaleOrderHeader100Controller : BaseController
    {
        // GET: SaleOrderHeader100

        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            var SelectedCommonDate = "Custom";
            var SelectedCommonDate2 = "Custom";
            CreateViewBag(SelectedCommonDate, SelectedCommonDate2);
            return View();

        }
        public ActionResult _Search(SaleOrderHeader100ViewModel searchViewModel)
        {
            return ExecuteSearch(() =>
            {
                searchViewModel.VKORG = CurrentUser.CompanyCode;
                var data = _unitOfWork.SaleOrderHeader100Repository.Search(searchViewModel).ToList();

                return PartialView(data);
            });
        }

        public ActionResult _PaggingServerSide(DatatableViewModel model, SaleOrderHeader100ViewModel searchViewModel)
        {
            try
            {
                // action inside a standard controller
                int filteredResultsCount;
                int totalResultsCount;

                var query = _unitOfWork.SaleOrderHeader100Repository.Search(searchViewModel);
                var res = CustomSearchRepository.CustomSearchFunc<SaleOrderHeader100ViewModel>(model, out filteredResultsCount, out totalResultsCount, query, "STT");
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
            var SaleOrder = _unitOfWork.SaleOrderHeader100Repository.GetSaleOrderHeader100BySaleOrder(Id);

            return View(SaleOrder);
        }

        //Xem chi tiết SOLine
        public ActionResult ViewDetailSO(string VBELN, string POSNR)
        {
            // Lấy thông tin Sale Order
            var SaleOrder = _unitOfWork.SaleOrderHeader100Repository.GetSaleOrderItem100BySaleOrderAndLine(VBELN, POSNR);

            return View(SaleOrder);
        }

        //Danh sách BOM detail
        public ActionResult _GetBOMDetails(string VBELN, string POSNR = null, string STLAN = null)
        {
            var lst = _unitOfWork.SaleOrderHeader100Repository.GetBOMDetailWithSaleOrder(VBELN, POSNR, STLAN);
            return PartialView(lst);
        }

        //Schedule lines
        public ActionResult ScheduleLines(string VBELN, string POSNR)
        {
            // Lấy thông tin Sale Order
            var SaleOrder = _unitOfWork.SaleOrderHeader100Repository.GetScheduleLines(VBELN, POSNR);

            return PartialView("_ScheduleLines", SaleOrder);
        }

        //Danh sách SO Line
        public ActionResult _SaleOrderItemList(string VBELN)
        {
            return ExecuteSearch(() =>
            {
                //Danh sách sale order item
                var SaleOrderItemList = _unitOfWork.SaleOrderHeader100Repository.GetSaleOrderItem100BySaleOrder(VBELN);

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

        #region Export to excel
        public ActionResult ExportExcel(string VBELN)
        {
            var lst = _unitOfWork.SaleOrderHeader100Repository.GetBOMDetailWithSaleOrder(VBELN);
            return Export(lst, VBELN);
        }
        public FileContentResult Export(List<BomDetailViewModel> data, string VBELN)
        {
            
            #region //Header
            string fileheader = "DANH SÁCH BOM THEO SALE ORDER " + VBELN;

            //List<ExcelHeadingTemplate> heading initialize in BaseController
            //Default:
            //          1. heading[0] is controller code
            //          2. heading[1] is file name
            //          3. headinf[2] is warning (edit)
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = string.Empty,
                RowsToIgnore = 0,
                isWarning = false,
                isCode = true
            });
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = fileheader.ToUpper(),
                RowsToIgnore = 1,
                isWarning = false,
                isCode = false
            });
            #endregion //Header

            #region //Columns to take
            List<ExcelTemplate> columns = new List<ExcelTemplate>();
            columns.Add(new ExcelTemplate() { ColumnName = "SOLineNumber", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "MATNR", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "MATNR_DES", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "BOMBaseQty", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "BOMBaseUnit", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "IDNRK_DISPLAY", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "MAKTX", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "POSNR", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "MENGE", isAllowedToEdit = false, isNumber = true });
            columns.Add(new ExcelTemplate() { ColumnName = "MEINS", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "KLMENG", isAllowedToEdit = false, isNumber = true });
            columns.Add(new ExcelTemplate() { ColumnName = "AUSCH", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "Total", isAllowedToEdit = false, isNumber = true });
            columns.Add(new ExcelTemplate() { ColumnName = "TotalWithScrap", isAllowedToEdit = false, isNumber = true });
            #endregion //Columns to take

            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(data, columns, heading, true);
            //File name
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_") + "_" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion Export to excel
    }
}