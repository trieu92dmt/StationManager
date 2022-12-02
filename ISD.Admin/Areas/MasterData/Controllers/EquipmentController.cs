using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MasterData.Controllers
{
    public class EquipmentController : BaseController
    {
        // GET: MachineChain
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }
        public ActionResult _Search(string EquipmentName = "", string EquipmentCode = "", string EquipmentGroupCode = "", string EquipmentTypeCode = "", string WorkShopId = "", string Status = "", bool Actived = true)
        {

            return ExecuteSearch(() =>
            {
                //  join ptemp in _context.WorkShopModel on p.WorkShopId equals ptemp.WorkShopId into plist
                // from pr in plist.DefaultIfEmpty()

                var EquipmentList = 
                                        // Máy móc
                                    (from eq in _context.EquipmentModel
                                         //MachineChainType: Nhóm máy móc
                                     join catGrpCode in _context.CatalogModel 
                                     on new { CatalogCode = eq.EquipmentGroupCode, EquipmentTypeCode = "MachineChainType" } equals new { CatalogCode = catGrpCode.CatalogCode, EquipmentTypeCode = catGrpCode.CatalogTypeCode } into catGrpCodelst
                                     from grp in catGrpCodelst.DefaultIfEmpty()
                                         //MachineChain: Phân loại (Máy móc || Chuyền sản xuất)
                                     join catTypeCode in _context.CatalogModel
                                     on new { CatalogCode = eq.EquipmentCode, EquipmentTypeCode = "MachineChain" } equals new { CatalogCode = catTypeCode.CatalogCode, EquipmentTypeCode = catTypeCode.CatalogTypeCode } into catTypeCodelst
                                     from type in catTypeCodelst.DefaultIfEmpty()
                                         //MachineChainStatus: Trạng thái máy móc (Hoạt động || Bảo trì || Đóng)
                                     join catStatusCode in _context.CatalogModel
                                     on new { CatalogCode = eq.EquipmentStatus, EquipmentTypeCode = "MachineChainStatus" } equals new { CatalogCode = catStatusCode.CatalogCode, EquipmentTypeCode = catStatusCode.CatalogTypeCode } into catStatusCodelst
                                     from stt in catStatusCodelst.DefaultIfEmpty()
                                         //EquipmentPowerUnit: Đơn vị tính công suất
                                     join catUnit in _context.CatalogModel
                                     on new { CatalogCode = eq.Unit, EquipmentTypeCode = "EquipmentPowerUnit" } equals new { CatalogCode = catUnit.CatalogCode, EquipmentTypeCode = catUnit.CatalogTypeCode } into catUnitlst
                                     from unit in catUnitlst.DefaultIfEmpty()
                                         //workShop
                                     join workShop in _context.WorkShopModel on eq.WorkShopId equals workShop.WorkShopId into work
                                     from w in work
                                     where
                                     //Tên máy móc/chuyền
                                     (EquipmentName == "" || eq.EquipmentName.Contains(EquipmentName))
                                     //Mã máy móc/chuyền
                                     && (EquipmentCode == "" || eq.EquipmentCode == (EquipmentCode))
                                     //Mã nhóm
                                     && (EquipmentGroupCode == "" || eq.EquipmentGroupCode == (EquipmentGroupCode))
                                     //Phân loại
                                     && (EquipmentTypeCode == "" || eq.EquipmentTypeCode == (EquipmentTypeCode))
                                     //Phân xưởng
                                     && (WorkShopId == "" || eq.WorkShopId.ToString() == (WorkShopId))
                                     // Trạng thái máy móc
                                     && (Status == "" || eq.EquipmentStatus == Status)
                                     // Tình trạng hoạt động
                                     && (eq.Actived == true)
                                     orderby eq.WorkShopId descending
                                     select new EquimentSearchModel
                                     {
                                         EquipmentId = eq.EquipmentId,
                                         EquipmentCode = eq.EquipmentCode,
                                         EquipmentName = eq.EquipmentName,
                                         Description = eq.Description,
                                         EquipmentTypeCode = eq.EquipmentTypeCode,
                                         EquipmentProduction = eq.EquipmentProduction,
                                         EquipmentGroupCode = eq.EquipmentGroupCode,
                                         grpCatalogCode = grp.CatalogCode,
                                         grpCatalogText_vi = grp.CatalogText_vi,
                                         typeCatalogCode = type.CatalogCode,
                                         typeCatalogText_vi = type.CatalogText_vi,
                                         statusCatalogCode = stt.CatalogCode,
                                         statusCatalogText_vi = stt.CatalogText_vi,
                                         WorkShopId = w.WorkShopId,
                                         WorkShopName = w.WorkShopName,
                                         Unit = unit.CatalogText_vi
                                     }).OrderBy(x => x.EquipmentCode)
                                        .ToList();



                return PartialView(EquipmentList);
            });
        }

        #region Print Barcode
        public ActionResult _PrintEquipmentPopup(List<Guid?> EquipmentIds = null)
        {
            if (EquipmentIds == null)
            {
                EquipmentIds = new List<Guid?>();
            }
            var result = _context.EquipmentModel.Where(x => EquipmentIds.Contains(x.EquipmentId))
                                                .Select(x => new EquimentSearchModel
                                                {
                                                    EquipmentId = x.EquipmentId,
                                                    EquipmentCode = x.EquipmentCode,
                                                    EquipmentName = x.EquipmentName,
                                                })
                                                .OrderBy(x => x.EquipmentCode).ToList();

            //Công đoạn chuyền 
            var stepList = _context.RoutingModel.Select(x => new
            {
                value = x.StepCode,
                text = x.StepName
            }).OrderBy(x => x.value).Distinct().ToList();
            ViewBag.StepCode = new SelectList(stepList, "value", "text");

            return PartialView(result);
        }
        #endregion


        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }

        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Create(EquipmentModel model)
        {
            return ExecuteContainer(() =>
            {
                model.EquipmentId = Guid.NewGuid();
                model.CreateBy = CurrentUser.AccountId;
                model.CreateTime = DateTime.Now;
                model.Actived = true;

                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = (string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_Equipment.ToLower()))
                });
            });
        }
        #endregion

        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var machineChain = _context.EquipmentModel.FirstOrDefault(p => p.EquipmentId == id);

            if (machineChain == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MasterData_Equipment.ToLower()) });
            }

            CreateViewBag(null, machineChain.EquipmentId, machineChain.EquipmentStatus);

            return View(machineChain);
        }

        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(EquimentSearchModel model)
        {
            var newitem = new EquipmentModel();
            return ExecuteContainer(() =>
            {
                newitem.EquipmentId = model.EquipmentId;
                newitem.EquipmentIntId = (int)model.EquipmentIntId;
                newitem.EquipmentName = model.EquipmentName;
                newitem.EquipmentCode = model.EquipmentCode;
                newitem.EquipmentName = model.EquipmentName;
                newitem.EquipmentGroupCode = model.EquipmentGroupCode;
                newitem.EquipmentTypeCode = model.EquipmentTypeCode;
                newitem.WorkShopId = model.WorkShopId;
                newitem.Description = model.Description;
                newitem.EquipmentProduction = model.EquipmentProduction;
                newitem.EquipmentStatus = model.EquipmentStatus;
                newitem.CreateBy = model.CreateBy;
                newitem.CreateTime = model.CreateTime;
                newitem.Actived = model.Actived;
                newitem.LastEditBy = CurrentUser.AccountId;
                newitem.LastEditTime = DateTime.Now;
                newitem.Unit = model.Unit;

                _context.Entry(newitem).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_Equipment.ToLower())
                });
            });
        }


        #endregion

        #region Delete
        [HttpPost]
        [ISDAuthorizationAttribute]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var machineChain = _context.EquipmentModel.FirstOrDefault(p => p.EquipmentId == id);
                if (machineChain != null)
                {
                    machineChain.Actived = false;
                    _context.Entry(machineChain).State = EntityState.Modified;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.MasterData_Equipment.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = ""
                    });
                }
            });
        }
        #endregion

        #region ViewBag, Helper
        public void CreateViewBag(string Plant = null, Guid? WorkShopId = null, string Status = null)
        {
            //Phân loại
            var typeList = _context.CatalogModel.Where(x => x.CatalogTypeCode == "MachineChain").Select(x => x).ToList();
            ViewBag.EquipmentTypeCode = new SelectList(typeList, "CatalogCode", "CatalogText_vi");
            //Trạng thái
            var MachineChainStatus = _context.CatalogModel.Where(x => x.CatalogTypeCode == "MachineChainStatus").Select(x => x).ToList();
            ViewBag.EquipmentStatus = new SelectList(MachineChainStatus, "CatalogCode", "CatalogText_vi", Status);
            //Phân xưởng
            var workshopList = _context.WorkShopModel.Where(p => (bool)p.Actived).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.WorkShopId = new SelectList(workshopList, "WorkShopId", "WorkShopName", WorkShopId);
            //machine typecode 
            var MachineChainTypeCodeList = _context.CatalogModel.Where(x => x.CatalogTypeCode == "MachineChainType").Select(x => x).ToList();
            ViewBag.EquipmentGroupCode = new SelectList(MachineChainTypeCodeList, "CatalogCode", "CatalogText_vi");
            //Đơn vị tính
            var UnitList = _context.CatalogModel.Where(x => x.CatalogTypeCode == "EquipmentPowerUnit" && x.Actived == true)
                                            .Select(x => new
                                            {
                                                CatalogCode = x.CatalogCode,
                                                CatalogText_vi = x.CatalogCode + "  |  "+x.CatalogText_vi
                                            }).ToList();
            ViewBag.Unit = new SelectList(UnitList, "CatalogCode", "CatalogText_vi");
        }
        #endregion
    }
}