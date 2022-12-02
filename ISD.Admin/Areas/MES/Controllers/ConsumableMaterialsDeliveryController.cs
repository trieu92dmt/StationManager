using ISD.Constant;
using ISD.Extensions;
using ISD.ViewModels;
using ISD.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class ConsumableMaterialsDeliveryController : BaseController
    {
        // GET: ConsumableMaterialsDelivery
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index(string Type)
        {
            var model = new ConsumableMaterialsDeliveryViewModel();
            model.DocumentDate = DateTime.Now;
            if (Type == "CONFIRM")
            {
                CreateViewBag();
                return View(model);
            }
            return View("List", model);
        }

        public ActionResult _Search(DateTime? DocumentDate)
        {
            return ExecuteSearch(() =>
            {
                List<AllotmentResultViewModel> list = new List<AllotmentResultViewModel>();

                list = (from p in _context.ConsumableMaterialsDeliveryModel
                        join bom in _context.ProductModel on p.BOM equals bom.ProductCode into aG
                        from a in aG.DefaultIfEmpty()
                        where p.DocumentDate == DocumentDate
                        select new AllotmentResultViewModel()
                        {
                            DocumentDate = p.DocumentDate,
                            LSXSAP = p.ProductionOrderCode,
                            ProductCode = p.ProductCode,
                            ProductDetailCode = p.ProductDetailCode,
                            ProductDetailName = p.ProductDetailName,
                            ProductDetailActualQty = p.ProductDetailActualQty,
                            BOM = p.BOM + " | " + a.ProductName,
                            BOMUnit = p.BOMUnit,
                            BOMQty = p.BOMQty,
                            StepName = p.StepName,
                            ActualQty = p.ActualQty,
                        }).ToList();


                return PartialView(list);
            });
        }
        #endregion

        #region Kết quả phân bổ
        public ActionResult _AllotResult(AllotmentViewModel viewModel)
        {
            var result = new List<AllotmentResultViewModel>();
            result = _unitOfWork.ConsumableMaterialsDeliveryRepository.AllotBOM(viewModel);
            return PartialView(result);
        }
        #endregion

        #region Xác nhận
        public ActionResult Confirm(ConsumableMaterialsDeliveryFormViewModel viewModel)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                string error =_unitOfWork.ConsumableMaterialsDeliveryRepository.Confirm(viewModel, CurrentUser.AccountId);
                if (!string.IsNullOrEmpty(error))
                {
                    return _APIError(error);
                }
                return _APISuccess(null, "Đã xác nhận xuất nguyên liệu tiêu hao thành công");
            });
        }
        #endregion

        #region Helper
        public void CreateViewBag()
        {
            //Danh sách nguyên vật liệu
            var bomDetailLst = (from a in _context.ProductModel
                                where a.ERPProductCode.StartsWith("2")
                                group a by new { a.ERPProductCode, a.ProductName } into g
                                select new ISDSelectStringItem()
                                {
                                    id = g.Key.ERPProductCode,
                                    name = g.Key.ERPProductCode + " | " + g.Key.ProductName,
                                }).Take(10).ToList();
            foreach (var item in bomDetailLst)
            {
                item.name = item.name.TrimStart(new Char[] { '0' });
            }
            ViewBag.DataAllotment = new SelectList(bomDetailLst, "id", "name");

            //Danh sách công đoạn
            var stepCodeLst = (from a in _context.RoutingModel
                               where a.Actived == true && a.StepCode == ConstStep.TKP
                               orderby a.OrderIndex
                               select new ISDSelectStringItem()
                               {
                                   id = a.StepCode,
                                   name = a.StepCode + " | " + a.StepName,
                               }).ToList();
            ViewBag.StepCode = new SelectList(stepCodeLst, "id", "name");
        }

        //Autocomplete
        public ActionResult SearchDataAllotmentBy(string search)
        {
            var bomDetailLst = (from a in _context.ProductModel
                                where a.ERPProductCode.StartsWith("2")
                                    && (search == null || a.ERPProductCode.Contains(search) || a.ProductName.Contains(search))
                                group a by new { a.ERPProductCode, a.ProductName } into g
                                select new ISDSelectItem2()
                                {
                                    value = g.Key.ERPProductCode,
                                    text = g.Key.ERPProductCode + " | " + g.Key.ProductName,
                                }).Take(10).ToList();
            foreach (var item in bomDetailLst)
            {
                item.text = item.text.TrimStart(new Char[] { '0' });
            }
            return Json(bomDetailLst, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}