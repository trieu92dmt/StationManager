using ISD.Core;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.Resources;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class ProductionOrderController : BaseController
    {
        // GET: ProductionOrder
        [ISDAuthorization]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _Search(ProductionOrderViewModel productionOrderViewModel)
        {
            return ExecuteSearch(() =>
            {
                productionOrderViewModel.SaleOrg = CurrentUser.SaleOrg;
                var resultList = _unitOfWork.ProductionOrderRepository.Search(productionOrderViewModel);
                return PartialView(resultList);
            });
          
        }

        [HttpGet]
        public ActionResult _PrintQRCode(Guid TaskId, string Summary)
        {
            var result = _unitOfWork.HangtagRepository.GetHangTagByTTLSX(TaskId, Summary);

            //Danh sách chi tiết
            var chiTietList = new List<ISDSelectStringItem>();
            if (result != null)
            {
                chiTietList = _unitOfWork.AssignmentRepository.GetItemByMaterialNumberAndPlant(result.CompanyCode, result.ProductCode, SearchText: null, SLKH: result.Number2 ?? result.Qty);
            }

            ViewBag.ChiTiet = new SelectList(chiTietList, "id", "name");

            return PartialView(result);
        }
        
        [HttpPost]
        public JsonResult _PrintQRCode(HangTagViewModel viewModel)
        {
            return ExecuteContainer(() =>
            {
                //Create HangTag
                _unitOfWork.HangtagRepository.Create(viewModel);
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Empty,
                });
            });
        }

        //Export
        #region Export to excel
        //const string controllerCode = ConstExcelController.Appointment;
        const int startIndex = 8;

        public ActionResult ExportExcel(ProductionOrderViewModel productionOrderViewModel)
        {
            //Get data from server
            productionOrderViewModel.SaleOrg = CurrentUser.SaleOrg;
            var result = _unitOfWork.ProductionOrderRepository.ExportData(productionOrderViewModel);

            return Export(result);
        }
        public FileContentResult Export(List<LSXDTExportViewModel> viewModel)
        {
            #region Master
            List<ExcelTemplate> columns = new List<ExcelTemplate>();

            columns.Add(new ExcelTemplate { ColumnName = "LSXDT", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "SoDSX", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "SoLSXSAP", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate { ColumnName = "SoCont", isAllowedToEdit = false, isNumber = true });
            columns.Add(new ExcelTemplate { ColumnName = "BDDK", isAllowedToEdit = false, isDateTime = true });
            columns.Add(new ExcelTemplate { ColumnName = "KTDK", isAllowedToEdit = false, isDateTime = true });
            columns.Add(new ExcelTemplate { ColumnName = "BDDC", isAllowedToEdit = false, isDateTime = true });
            columns.Add(new ExcelTemplate { ColumnName = "KTDC", isAllowedToEdit = false, isDateTime = true });
            columns.Add(new ExcelTemplate { ColumnName = "Duration", isAllowedToEdit = false });

            #endregion Master

            //Header
            string fileheader = string.Format(LanguageResource.Export_ExcelHeader, LanguageResource.ProductionOrder_LSXDT);
            //List<ExcelHeadingTemplate> heading initialize in BaseController
            //Default:
            //          1. heading[0] is controller code
            //          2. heading[1] is file name
            //          3. headinf[2] is warning (edit)
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = "",//controllerCode,
                RowsToIgnore = 1,
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
            //heading.Add(new ExcelHeadingTemplate()
            //{
            //    Content = LanguageResource.Export_ExcelWarning1,
            //    RowsToIgnore = 0,
            //    isWarning = true,
            //    isCode = false
            //});
            //heading.Add(new ExcelHeadingTemplate()
            //{
            //    Content = LanguageResource.Export_ExcelWarning2,
            //    RowsToIgnore = 1,
            //    isWarning = true,
            //    isCode = false
            //});

            //Trạng thái
            //heading.Add(new ExcelHeadingTemplate()
            //{
            //    Content = string.Format(LanguageResource.Export_ExcelWarningActived, LanguageResource.MasterData_District),
            //    RowsToIgnore = 1,
            //    isWarning = true,
            //    isCode = false
            //});

            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, HasExtraSheet: false, IsMergeCellHeader: false);
            //File name
            //Insert => THEM_MOI
            //Edit => CAP_NHAT
            //string exportType = LanguageResource.exportType_Insert;
            //if (isEdit == true)
            //{
            //    exportType = LanguageResource.exportType_Edit;
            //}
            string fileNameWithFormat = string.Format("{0}.xlsx", _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion Export to excel
    }
}