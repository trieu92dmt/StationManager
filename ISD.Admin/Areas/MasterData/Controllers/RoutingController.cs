using ISD.Constant;
using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories.Excel;
using ISD.Resources;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace MasterData.Controllers
{
    public class RoutingController : BaseController
    {
        // Index
        #region Index
        // GET: Routing
        [ISDAuthorization]
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public ActionResult _Search(string StepCode = "", string StepName = "", bool? Actived = null, List<Guid?> EquipmentList = null)
        {
            return ExecuteSearch(() =>
            {
                if (EquipmentList == null)
                {
                    EquipmentList = new List<Guid?>();
                }
                var routings = (from p in _context.RoutingModel
                                    //WorkShop
                                join ptemp in _context.WorkShopModel on p.WorkShopId equals ptemp.WorkShopId into plist
                                from pr in plist.DefaultIfEmpty()
                                join map in _context.Routing_Equipment_Mapping on p.StepId equals map.StepId into temp
                                from m in temp.DefaultIfEmpty()
                                join equid in _context.EquipmentModel on m.EquipmentId equals equid.EquipmentId into temp2
                                from eq in temp2.DefaultIfEmpty()
                                join ca in _context.CatalogModel on eq.Unit equals ca.CatalogCode into temp3
                                from ut in temp3.DefaultIfEmpty()
                                where
                                //search by stepcode
                                (StepCode == "" || p.StepCode == StepCode)
                                //search by stepname
                                && (StepName == "" || p.StepName == StepName)
                                //search by actived
                                && (Actived == null || p.Actived == Actived)
                                //search by list equidment
                                && (EquipmentList.Count == 0 || EquipmentList.Contains(m.EquipmentId))
                                select new RoutingViewModel()
                                {
                                    StepId = p.StepId,
                                    StepCode = p.StepCode,
                                    StepName = p.StepName,
                                    WorkShopId = p.WorkShopId,
                                    WorkShopName = pr.WorkShopName,
                                    OrderIndex = p.OrderIndex,
                                    EquipmentProductionUnit = ut.CatalogText_vi,
                                    EquipmentProduction = eq.EquipmentProduction,
                                    //WorkCenter = p.WorkCenter,
                                    Actived = p.Actived,
                                    Plant = p.Plant,
                                    EquipmentName = eq.EquipmentName,
                                    EquipmentCode = eq.EquipmentCode,
                                }).OrderBy(x => new { x.EquipmentCode, x.EquipmentName })
                                .GroupBy(c => new
                                {
                                    c.StepId,
                                    c.StepCode,
                                    c.StepName,
                                    c.WorkShopId,
                                    c.WorkShopName,
                                    c.OrderIndex,
                                    c.Actived,
                                    c.Plant,
                                    c.EquipmentProductionUnit,
                                }).ToList()
                                 .Select(eg => new RoutingViewModel()
                                 {
                                     StepId = eg.Key.StepId,
                                     StepCode = eg.Key.StepCode,
                                     StepName = eg.Key.StepName,
                                     WorkShopId = eg.Key.WorkShopId,
                                     WorkShopName = eg.Key.WorkShopName,
                                     OrderIndex = eg.Key.OrderIndex,
                                     //WorkCenter = p.WorkCenter,
                                     Actived = eg.Key.Actived,
                                     Plant = eg.Key.Plant,
                                     EquipmentName = string.Join(",", eg.Select(i => i.EquipmentName).OrderBy(x => x)),
                                     EquipmentCode = string.Join(",", eg.Select(i => i.EquipmentCode).OrderBy(x => x)),
                                     EquipmentProduction = eg.Select(x => x.EquipmentProduction).Sum(),
                                     EquipmentProductionUnit = eg.Key.EquipmentProductionUnit,
                                 }).ToList();
                return PartialView(routings);
            });

        }
        #endregion end Index
        // Create

        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }
        //POST: Create
        [HttpPost]
        //[ValidateAjax]
        //[ISDAuthorizationAttribute]
        public JsonResult Create(RoutingViewModel model)
        {
            return ExecuteContainer(() =>
            {
                    var guidid = Guid.NewGuid();
                    var modelNew = new RoutingModel
                    {
                        StepId = guidid,
                        StepCode = model.StepCode,
                        StepName = model.StepName,
                        Description = model.Description,
                        OrderIndex = model.OrderIndex,
                        CreateBy = CurrentUser.AccountId,
                        CreateTime = DateTime.Now,
                        Actived = true
                    };
                    if (model.EquipmentList != null)
                    {
                        foreach (var item in model.EquipmentList)
                        {
                            var newMapping = new Routing_Equipment_Mapping
                            {
                                Routing_Equipment_MappingId = Guid.NewGuid(),
                                StepId = guidid,
                                EquipmentId = item
                            };
                            _context.Entry(newMapping).State = EntityState.Added;
                        }
                    }
                    _context.Entry(modelNew).State = EntityState.Added;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.Master_Routing.ToLower())
                    });
            });
        }
        #endregion
        public bool IsExists(string StepCode, Guid? Code)
        {
            return (_context.RoutingModel.FirstOrDefault(x => x.StepCode == StepCode) != null);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingStepCode(string StepCode, string StepCodeValid, Guid? Code)
        {
            try
            {
                if (StepCodeValid != StepCode)
                {
                    return Json(!IsExists(StepCode, Code));
                }
                else
                {
                    return Json(true);
                }
            }
            catch //(Exception ex)
            {
                return Json(false);
            }
        }

        // Edit
        // Input: StepId  
        // Output: Thông tin
        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var routing = (from p in _context.RoutingModel
                           join prTemp in _context.WorkShopModel on p.WorkShopId equals prTemp.WorkShopId into prList
                           from pr in prList.DefaultIfEmpty()
                           join map in _context.Routing_Equipment_Mapping on p.StepId equals map.StepId into prMap
                           from m in prMap.DefaultIfEmpty()
                           join equid in _context.EquipmentModel on m.EquipmentId equals equid.EquipmentId into temp2
                           from eq in temp2.DefaultIfEmpty()
                           join ca in _context.CatalogModel on eq.Unit equals ca.CatalogCode into temp3
                           from ut in temp3.DefaultIfEmpty()
                           where p.StepId == id
                           select new RoutingViewModel()
                           {
                               StepId = p.StepId,
                               StepName = p.StepName,
                               StepCode = p.StepCode,
                               WorkShopId = pr.WorkShopId,
                               WorkShopName = pr.WorkShopName,
                               WorkCenter = p.WorkCenter,
                               OrderIndex = p.OrderIndex,
                               Plant = p.Plant,
                               Actived = p.Actived,
                               //EquipmentList = _context.Routing_Equipment_Mapping.Where(x => x.StepId == id).Select(x => x.EquipmentId).ToList(),
                               EquipmentProductionUnit = ut.CatalogText_vi,
                               EquipmentProduction = eq.EquipmentProduction,
                           }).GroupBy(c => new
                           {
                               c.StepId,
                               c.StepCode,
                               c.StepName,
                               c.WorkShopId,
                               c.WorkShopName,
                               c.WorkCenter,
                               c.OrderIndex,
                               c.Actived,
                               c.Plant,
                               c.EquipmentProductionUnit,
                           }).ToList()
                           .Select(eg => new RoutingViewModel()
                           {
                               StepId = eg.Key.StepId,
                               StepCode = eg.Key.StepCode,
                               StepName = eg.Key.StepName,
                               WorkShopId = eg.Key.WorkShopId,
                               WorkShopName = eg.Key.WorkShopName,
                               WorkCenter = eg.Key.WorkCenter,
                               OrderIndex = eg.Key.OrderIndex,
                               Actived = eg.Key.Actived,
                               EquipmentList = _context.Routing_Equipment_Mapping.Where(x => x.StepId == id).Select(x => x.EquipmentId).ToList(),
                               Plant = eg.Key.Plant,
                               EquipmentName = string.Join(",", eg.Select(i => i.EquipmentName).OrderBy(x => x)),
                               EquipmentCode = string.Join(",", eg.Select(i => i.EquipmentCode).OrderBy(x => x)),
                               EquipmentProduction = eg.Select(x => x.EquipmentProduction).Sum(),
                               EquipmentProductionUnit = eg.Key.EquipmentProductionUnit,
                           })
                         .FirstOrDefault();
            if (routing == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Master_Routing.ToLower()) });
            }
            //Máy móc chuyền
            ViewBag.EquipmentListAll = _context.EquipmentModel.Where(x => x.Actived == true)
                 .Select(x => new EquimentSearchModel()
                 {
                     EquipmentId = x.EquipmentId,
                     EquipmentName = x.EquipmentCode + " | " + x.EquipmentName,
                     WorkShopId = x.WorkShopId,
                     EquipmentTypeCode = x.EquipmentTypeCode,
                     EquipmentCode = x.EquipmentCode
                 })
                 .OrderBy(x => x.EquipmentCode).ToList();
            ViewBag.EquipmentListSelect = routing.EquipmentList;
            ViewBag.EquipmentProduction = routing.EquipmentProduction;
            ViewBag.EquipmentProductionUnit = routing.EquipmentProductionUnit;
            CreateViewBag(routing.Plant, routing.WorkShopId);
            return View(routing);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(RoutingViewModel model)
        {
            return ExecuteContainer(() =>
            {
                var modelDb = _context.RoutingModel.FirstOrDefault(p => p.StepId == model.StepId);

                if (modelDb != null)
                {
                    modelDb.StepCode = model.StepCode;
                    modelDb.StepName = model.StepName;
                    modelDb.Description = model.Description;
                    modelDb.OrderIndex = model.OrderIndex;
                    modelDb.LastEditBy = CurrentUser.AccountId;
                    modelDb.Actived = model.Actived;
                    modelDb.LastEditTime = DateTime.Now;

                    //tìm xóa bảng mapping
                    var mapping = _context.Routing_Equipment_Mapping.Where(x => x.StepId == model.StepId).ToList();
                    _context.Routing_Equipment_Mapping.RemoveRange(mapping);
                    if (model.EquipmentList != null)
                    {
                        foreach (var item in model.EquipmentList)
                        {
                            var newmodel = new Routing_Equipment_Mapping();
                            newmodel.Routing_Equipment_MappingId = Guid.NewGuid();
                            newmodel.EquipmentId = item;
                            newmodel.StepId = modelDb.StepId;

                            _context.Entry(newmodel).State = EntityState.Added;
                        }
                    }
                }
                _context.Entry(modelDb).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Master_Routing.ToLower())
                });
            });
        }
        #endregion

        #region ViewBag, Helper
        public void CreateViewBag(string Plant = null, Guid? WorkShopId = null)
        {
            //Plant
            var companyList = _context.CompanyModel.Where(p => p.Actived).Select(p => new CompanyViewModel()
            {
                CompanyCode = p.CompanyCode,
                CompanyName = p.CompanyCode + " | " + p.CompanyName,
                OrderIndex = p.OrderIndex,
            })
            .OrderBy(p => p.OrderIndex).ToList();

            ViewBag.Plant = new SelectList(companyList, "CompanyCode", "CompanyName", Plant);

            //Phân xưởng
            var workshopList = _context.WorkShopModel.Where(p => (bool)p.Actived).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.WorkShopId = new SelectList(workshopList, "WorkShopId", "WorkShopName", WorkShopId);

            //Máy móc chuyền
            var EquipmentList = _context.EquipmentModel.Where(x => x.Actived == true)
                                .Select(x => new
                                {
                                    EquipmentId = x.EquipmentId,
                                    EquipmentCode = x.EquipmentCode,
                                    EquipmentName = x.EquipmentCode + " | " + x.EquipmentName,
                                    WorkShopId = x.WorkShopId
                                })
                                .OrderBy(x => x.EquipmentCode).ToList();
            ViewBag.EquipmentList = new SelectList(EquipmentList, "EquipmentId", "EquipmentName");
        }
        #endregion

        //GET: /Routing/Order
        #region Order Step

        public ActionResult Order()
        {
            //Get all step
            var routingList = (from p in _context.RoutingModel
                               orderby p.OrderIndex.HasValue descending, p.OrderIndex
                               where p.Actived == true
                               select new RoutingViewModel()
                               {
                                   StepId = p.StepId,
                                   StepCode = p.StepCode,
                                   StepName = p.StepName,
                                   OrderIndex = p.OrderIndex,
                                   Actived = p.Actived,
                               })
                               .ToList();

            int item = (int)Math.Round(((double)routingList.Count / 4), MidpointRounding.AwayFromZero);
            //Split routingList into 4 list, split into 4 column
            ViewBag.List1 = routingList.Skip(0).Take(item).ToList();
            ViewBag.List2 = routingList.Skip(item).Take(item).ToList();
            ViewBag.List3 = routingList.Skip(item * 2).Take(item).ToList();
            ViewBag.List4 = routingList.Skip(item * 3).Take(item).ToList();
            return View();
        }
        [HttpPost]

        public ActionResult Order(List<RoutingOrderViewModel> orderedList)
        {
            return ExecuteContainer(() =>
            {
                foreach (var item in orderedList)
                {
                    var routing = _context.RoutingModel.FirstOrDefault(p => p.StepId == item.StepId);
                    if (routing != null)
                    {
                        routing.OrderIndex = item.OrderIndex;
                        routing.LastEditBy = CurrentUser.AccountId;
                        routing.LastEditTime = DateTime.Now;
                        _context.Entry(routing).State = EntityState.Modified;
                    }
                }
                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.OrderStep.ToLower())
                });
            });
        }
        #endregion Order Step
        //Export
        #region Export to excel
        public ActionResult ExportCreate()
        {
            List<RoutingViewModel> viewModel = new List<RoutingViewModel>();
            return Export(viewModel, isEdit: false);
        }

        public ActionResult ExportEdit(RoutingSearchViewModel searchViewModel)
        {
            searchViewModel = (RoutingSearchViewModel)Session["frmSearchRouting"];

            //Get data filter
            //Get data from server
            var routings = (from p in _context.RoutingModel
                            join pr in _context.WorkShopModel on p.WorkShopId equals pr.WorkShopId
                            where
                            //search by workshop name
                            (searchViewModel.WorkShopId == null || p.WorkShopId == searchViewModel.WorkShopId)
                            //search by actived
                            && (searchViewModel.Actived == null || p.Actived == searchViewModel.Actived)
                             // search by routing name
                             && (searchViewModel.StepName == null || p.StepName.Contains(searchViewModel.StepName))
                            select new RoutingViewModel()
                            {
                                StepId = p.StepId,
                                StepCode = p.StepCode,
                                StepName = p.StepName,
                                WorkShopId = p.WorkShopId,
                                WorkShopName = pr.WorkShopName,
                                OrderIndex = p.OrderIndex,
                                Actived = p.Actived
                            })
                             .Take(200)
                              .ToList();
            return Export(routings, isEdit: true);
        }

        const string controllerCode = ConstExcelController.Routing;
        const int startIndex = 8;
        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<RoutingViewModel> viewModel, bool isEdit)
        {
            #region Dropdownlist
            //Phân xưởng
            List<DropdownModel> workshopId = (from p in _context.WorkShopModel
                                              where p.Actived == true
                                              select new DropdownModel
                                              {
                                                  Id = p.WorkShopId,
                                                  Name = p.WorkShopName
                                              }).ToList();
            #endregion Dropdownlist

            #region Master
            List<ExcelTemplate> columns = new List<ExcelTemplate>();

            columns.Add(new ExcelTemplate() { ColumnName = "StepId", isAllowedToEdit = false });

            //columns.Add(new ExcelTemplate() { ColumnName = "StepCode", isAllowedToEdit = false });
            columns.Add(new ExcelTemplate() { ColumnName = "StepName", isAllowedToEdit = true });
            //insert => dropdownlist
            //edit => not allow edit
            if (isEdit == true)
            {
                columns.Add(new ExcelTemplate()
                {
                    ColumnName = "WorkShopName",
                    isAllowedToEdit = false
                });
                columns.Add(new ExcelTemplate()
                {
                    ColumnName = "StepCode",
                    isAllowedToEdit = false
                });
            }
            else
            {
                columns.Add(new ExcelTemplate()
                {
                    ColumnName = "WorkShopName",
                    isAllowedToEdit = true,
                    isDropdownlist = true,
                    TypeId = ConstExcelController.GuidId,
                    DropdownData = workshopId
                });
                columns.Add(new ExcelTemplate()
                {
                    ColumnName = "StepCode",
                    isAllowedToEdit = true
                });
            }


            columns.Add(new ExcelTemplate() { ColumnName = "OrderIndex", isAllowedToEdit = true });
            columns.Add(new ExcelTemplate() { ColumnName = "Actived", isAllowedToEdit = true, isBoolean = true });
            // TODO: Upload hình ảnh
            //columns.Add(new ExcelTemplate() { ColumnName = "ImageUrl ", isAllowedToEdit = false });
            #endregion Master

            //Header
            string fileheader = string.Format(LanguageResource.Export_ExcelHeader, LanguageResource.Master_Routing);
            //List<ExcelHeadingTemplate> heading initialize in BaseController
            //Default:
            //          1. heading[0] is controller code
            //          2. heading[1] is file name
            //          3. headinf[2] is warning (edit)
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = controllerCode,
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
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = LanguageResource.Export_ExcelWarning1,
                RowsToIgnore = 0,
                isWarning = true,
                isCode = false
            });
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = LanguageResource.Export_ExcelWarning2,
                RowsToIgnore = 0,
                isWarning = true,
                isCode = false
            });

            //Trạng thái
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = string.Format(LanguageResource.Export_ExcelWarningActived, LanguageResource.Master_Routing),
                RowsToIgnore = 1,
                isWarning = true,
                isCode = false
            });

            //Body
            byte[] filecontent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true, true);
            //File name
            //Insert => THEM_MOI
            //Edit => CAP_NHAT
            string exportType = LanguageResource.exportType_Insert;
            if (isEdit == true)
            {
                exportType = LanguageResource.exportType_Edit;
            }
            string fileNameWithFormat = string.Format("{0}_{1}.xlsx", exportType, _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileheader.ToUpper()).Replace(" ", "_"));

            return File(filecontent, ClassExportExcel.ExcelContentType, fileNameWithFormat);
        }
        #endregion Export to excel

        //Import
        #region Import from excel
        [ISDAuthorizationAttribute]
        public ActionResult Import()
        {
            //return ExcuteImportExcel(() =>
            //{
            DataSet ds = GetDataSetFromExcel();
            List<string> errorList = new List<string>();
            if (ds.Tables != null && ds.Tables.Count > 0)
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Get controller code from Excel file
                        string contCode = dt.Columns[0].ColumnName.ToString();
                        //Import data with accordant controller and action
                        if (contCode == controllerCode)
                        {
                            var index = 0;
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dt.Rows.IndexOf(dr) >= startIndex && !string.IsNullOrEmpty(dr.ItemArray[0].ToString()))
                                {
                                    index++;
                                    //Check correct template
                                    RoutingExcelViewModel routingIsValid = CheckTemplate(dr.ItemArray, index);

                                    if (!string.IsNullOrEmpty(routingIsValid.Error))
                                    {
                                        string error = routingIsValid.Error;
                                        errorList.Add(error);
                                    }
                                    else
                                    {
                                        string result = ExecuteImportExcelDistrict(routingIsValid);
                                        if (result != LanguageResource.ImportSuccess)
                                        {
                                            errorList.Add(result);
                                        }
                                    }
                                }
                            }
                        }
                        //else
                        //{
                        //    string error = string.Format(LanguageResource.Validation_ImportCheckController, LanguageResource.Master_Routing);
                        //    errorList.Add(error);
                        //}

                    }
                    if (errorList != null && errorList.Count > 0)
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = false,
                            Data = errorList
                        });
                    }
                    ts.Complete();
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = LanguageResource.ImportSuccess
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = false,
                    Data = LanguageResource.Validation_ImportExcelFile
                });
            }
            //});

        }

        #region Insert/Update data from excel file
        public string ExecuteImportExcelDistrict(RoutingExcelViewModel routingIsValid)
        {
            //Check:
            //1. If Id == "" then => Insert
            //2. Else then => Update
            #region Insert
            if (routingIsValid.isNullValueId == true)
            {
                try
                {
                    //Bỏ check trùng mã
                    //var districtCodeIsExist = _context.DistrictModel
                    //                                  .FirstOrDefault(p => p.ProvinceId == districtIsValid.ProvinceId 
                    //                                                    && p.DistrictCode == districtIsValid.DistrictCode);
                    //if (districtCodeIsExist != null)
                    //{
                    //    return string.Format(LanguageResource.Validation_Already_Exists_District, districtIsValid.DistrictCode, districtIsValid.ProvinceName);
                    //}
                    //else
                    //{

                    //}

                    RoutingModel routing = new RoutingModel();
                    routing.StepId = Guid.NewGuid();
                    routing.StepCode = routingIsValid.StepCode;
                    routing.StepName = routingIsValid.StepName;
                    routing.WorkShopId = routingIsValid.WorkShopId;
                    routing.OrderIndex = routingIsValid.OrderIndex;
                    routing.Actived = routingIsValid.Actived;
                    _context.Entry(routing).State = EntityState.Added;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        return string.Format("Thêm mới đã xảy ra lỗi: {0}", ex.InnerException.Message);
                    }
                    else
                    {
                        return string.Format("Thêm mới đã xảy ra lỗi: {0}", ex.Message);
                    }
                }
            }
            #endregion Insert

            #region Update
            else
            {
                try
                {
                    RoutingModel routing = _context.RoutingModel
                                                     .FirstOrDefault(p => p.StepId == routingIsValid.StepId);
                    if (routing != null)
                    {
                        //Bỏ check trùng mã
                        //var districtCodeIsExist = _context.DistrictModel
                        //                              .FirstOrDefault(p => p.ProvinceId == district.ProvinceId
                        //                                                && p.DistrictCode == districtIsValid.DistrictCode);
                        //if (districtCodeIsExist != null)
                        //{
                        //    return string.Format(LanguageResource.Validation_Already_Exists_District, districtIsValid.DistrictCode, districtIsValid.ProvinceName);
                        //}
                        routing.StepCode = routingIsValid.StepCode;
                        routing.StepName = routingIsValid.StepName;
                        routing.WorkShopId = routingIsValid.WorkShopId;
                        routing.OrderIndex = routingIsValid.OrderIndex;
                        routing.Actived = routingIsValid.Actived;
                        _context.Entry(routing).State = EntityState.Modified;
                    }
                    else
                    {
                        return string.Format(LanguageResource.Validation_ImportExcelIdNotExist,
                                                LanguageResource.Excel_DistrictId, routingIsValid.StepId,
                                                string.Format(LanguageResource.Export_ExcelHeader,
                                                LanguageResource.MasterData_District));
                    }

                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        return string.Format("Cập nhật đã xảy ra lỗi: {0}", ex.InnerException.Message);
                    }
                    else
                    {
                        return string.Format("Cập nhật đã xảy ra lỗi: {0}", ex.Message);
                    }
                }
            }
            #endregion Update

            _context.SaveChanges();
            return LanguageResource.ImportSuccess;
        }
        #endregion Insert/Update data from excel file

        #region Check data type 
        public RoutingExcelViewModel CheckTemplate(object[] row, int index)
        {
            RoutingExcelViewModel routingVM = new RoutingExcelViewModel();
            var fieldName = "";
            try
            {
                for (int i = 0; i <= row.Length; i++)
                {
                    #region Convert data to import
                    switch (i)
                    {
                        //Index
                        case 0:
                            fieldName = LanguageResource.NumberIndex;
                            int rowIndex = int.Parse(row[i].ToString());
                            routingVM.RowIndex = rowIndex;
                            break;
                        //steptId
                        case 1:
                            fieldName = LanguageResource.StepId;
                            string stepId = row[i].ToString();
                            if (string.IsNullOrEmpty(stepId))
                            {
                                routingVM.StepId = Guid.NewGuid();
                                routingVM.isNullValueId = true;
                            }
                            else
                            {
                                routingVM.StepId = Guid.Parse(stepId);
                                routingVM.isNullValueId = false;
                            }
                            break;
                        //stepCode
                        case 2:
                            fieldName = LanguageResource.Routing_StepCode;
                            string stepCode = row[i].ToString();
                            if (string.IsNullOrEmpty(stepCode))
                            {
                                routingVM.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.Routing_StepCode), routingVM.RowIndex);
                            }
                            else
                            {
                                routingVM.StepCode = stepCode;
                            }
                            break;
                        //StepName
                        case 3:
                            fieldName = LanguageResource.Routing_StepName;
                            string stepName = row[i].ToString();
                            if (string.IsNullOrEmpty(stepName))
                            {
                                routingVM.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.Routing_StepName), routingVM.RowIndex);
                            }
                            else
                            {
                                routingVM.StepName = stepName;
                            }
                            break;
                        // Order Index
                        case 4:
                            fieldName = LanguageResource.OrderIndex;
                            routingVM.OrderIndex = GetTypeFunction<int>(row[i].ToString(), i);
                            break;

                        //active
                        case 5:
                            fieldName = LanguageResource.Actived;
                            routingVM.Actived = GetTypeFunction<bool>(row[i].ToString(), i);
                            break;

                        //workshop
                        case 6:
                            fieldName = LanguageResource.MasterData_WorkShop;
                            string workshopId = row[i].ToString();
                            //if excel type is insert
                            if (routingVM.isNullValueId == true)
                            {
                                if (string.IsNullOrEmpty(workshopId))
                                {
                                    routingVM.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.MasterData_WorkShop), routingVM.RowIndex);
                                }
                                else
                                {
                                    routingVM.WorkShopId = GetTypeFunction<Guid>(workshopId, i);
                                }
                            }
                            break;
                    }
                    #endregion Convert data to import
                }
            }
            catch (FormatException ex)
            {
                routingVM.Error = string.Format(LanguageResource.Validation_ImportCastValid, fieldName, index) + ex.Message;
            }
            catch (InvalidCastException ex)
            {
                routingVM.Error = string.Format(LanguageResource.Validation_ImportCastValid, fieldName, index) + ex.Message;
            }
            catch (Exception ex)
            {
                routingVM.Error = string.Format(LanguageResource.Validate_ImportException, fieldName, index) + ex.Message;
            }
            return routingVM;
        }
        #endregion Check data type

        #endregion Import from excel
        //Check Unit
        public ActionResult CheckUnit(List<Guid> EquipmentIds)
        {
            // 1. Cùng đơn vị tính công suất mới tính, không thì báo lỗi
            // 2. Sum công suất
            // 3. Lấy unit
            //Input: Danh sách các mã máy
            // 1. Cùng đơn vị tính công suất mới tính, không thì báo lỗi
            if (EquipmentIds != null && EquipmentIds.Count > 0)
            {
                var check = _context.EquipmentModel.Where(p => EquipmentIds.Contains(p.EquipmentId)).Select(p => p.Unit).Distinct().ToList();
                if (check != null && check.Count() >= 2)
                {
                    //Báo lỗi
                    return Json(new { isSuccess = false, message = "Không cùng đơn vị công suất. Vui lòng chọn lại!" }, JsonRequestBehavior.AllowGet);
                    //return
                }
                // 2. Sum công suất
                var total = _context.EquipmentModel.Where(p => EquipmentIds.Contains(p.EquipmentId)).Sum(p => p.EquipmentProduction);
                // 3. Lấy unit
                var FirstE = EquipmentIds[0];
                var unit = (from eq in _context.EquipmentModel
                            join ca in _context.CatalogModel on eq.Unit equals ca.CatalogCode
                            where eq.EquipmentId == FirstE
                            select ca.CatalogText_vi).FirstOrDefault();
                return Json(new { isSuccess = true, data = new { EquipmentProduction = total, Unit = unit } }, JsonRequestBehavior.AllowGet);
            }
            //Nếu EquipmentIds == null => return false
            return Json(new { isSuccess = false });
        }
    }
}