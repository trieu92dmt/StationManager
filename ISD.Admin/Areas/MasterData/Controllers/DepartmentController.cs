using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ISD.Core;
using System.Data;
using System.Collections.Generic;
using System.Transactions;
using ISD.Constant;
using ISD.ViewModels.MasterData;

namespace MasterData.Controllers
{
    public class DepartmentController : BaseController
    {
        // GET: Department
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public ActionResult _Search(Guid? WorkShopId = null, string DepartmentCode = "", string DepartmentName = "", bool? Actived = null, List<Guid?> EquipmentList = null,string DepartmentType = "")
        {

            return ExecuteSearch(() =>
            {
                //linq: have to initial new list
                if (EquipmentList == null)
                {
                    EquipmentList = new List<Guid?>();
                }
                var departmentList = (from d in _context.DepartmentModel
                                      join ws in _context.WorkShopModel on d.WorkShopId equals ws.WorkShopId into temp4
                                      from w in temp4.DefaultIfEmpty()
                                      join cata in _context.CatalogModel on d.DepartmentType equals cata.CatalogCode  into temp3
                                      from ca in temp3.DefaultIfEmpty()
                                      join map in _context.Department_Equipment_Mapping on d.DepartmentId equals map.DepartmentId into temp
                                      from m in temp.DefaultIfEmpty()
                                      join equid in _context.EquipmentModel on m.EquipmentId equals equid.EquipmentId into temp2
                                      from eq in temp2.DefaultIfEmpty()

                                          //phân xưởng
                                      where (WorkShopId == null || d.WorkShopId == WorkShopId)
                                      //mã tổ
                                      && (DepartmentCode == "" || d.DepartmentCode == (DepartmentCode))
                                      //tên tổ
                                      && (DepartmentName == "" || d.DepartmentName.Contains(DepartmentName))
                                      //hoạt động
                                      && (Actived == null || d.Actived == Actived)
                                      //máy móc
                                      && (EquipmentList.Count == 0 || EquipmentList.Contains(m.EquipmentId))
                                      //loại phòng ban/tổ
                                       && (DepartmentType == "" || d.DepartmentType == DepartmentType)
                                       //catalog
                                      && ca.CatalogTypeCode == "DepartmentType"
                                      select new
                                      {
                                          DepartmentId = d.DepartmentId,
                                          StoreId = d.StoreId,
                                          DepartmentCode = d.DepartmentCode,
                                          DepartmentName = d.DepartmentName,
                                          OrderIndex = d.OrderIndex,
                                          WorkShopId = d.WorkShopId,
                                          WorkshopName = w.WorkShopName,
                                          Actived = d.Actived,
                                          EquipmentCode = eq.EquipmentCode,
                                          EquipmentName = eq.EquipmentName,
                                          DepartmentType = ca.CatalogText_vi,
                                      })
                                    .GroupBy(c => new
                                    {
                                        c.DepartmentId,
                                        c.StoreId,
                                        c.DepartmentCode,
                                        c.DepartmentName,
                                        c.OrderIndex,
                                        c.WorkShopId,
                                        c.WorkshopName,
                                        c.Actived,
                                        c.DepartmentType,
                                    })
                                 .ToList()
                                 .Select(eg => new DepartmentViewModel()
                                 {
                                     DepartmentId = eg.Key.DepartmentId,
                                     StoreId = eg.Key.StoreId,
                                     DepartmentCode = eg.Key.DepartmentCode,
                                     DepartmentName = eg.Key.DepartmentName,
                                     OrderIndex = eg.Key.OrderIndex,
                                     WorkShopId = eg.Key.WorkShopId,
                                     DepartmentType = eg.Key.DepartmentType,
                                     //WorkCenter = p.WorkCenter,
                                     WorkshopName = eg.Key.WorkshopName,
                                     Actived = eg.Key.Actived,
                                     EquipmentName = string.Join(",", eg.Select(i => i.EquipmentName).OrderBy(x => x)),
                                     EquipmentCode = string.Join(",", eg.Select(i => i.EquipmentCode).OrderBy(x => x)),
                                 }).OrderBy(x => x.DepartmentCode).ToList();
                return PartialView(departmentList);
            });
        }
        #endregion
        public JsonResult GetDepartmentByCompany(Guid? CompanyId)
        {
            if (CompanyId != null && CompanyId != Guid.Empty)
            {
                var listDepartment = _context.DepartmentModel.Where(s => s.Actived == true && s.CompanyId == CompanyId);
                var slDepartment = new SelectList(listDepartment, "DepartmentId", "DepartmentName");
                return Json(slDepartment, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var listDepartment = _context.DepartmentModel.Where(s => s.Actived == true);
                var slDepartment = new SelectList(listDepartment, "DepartmentId", "DepartmentName");
                return Json(slDepartment, JsonRequestBehavior.AllowGet);
            }

        }
        //GET: /Department/Create
        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }
        //POST: Create
        [HttpPost]
        [ValidateAjax]
        [ISDAuthorizationAttribute]
        public JsonResult Create(DepartmentViewModel departmentVM)
        {
            return ExecuteContainer(() =>
            {
                var guidid = Guid.NewGuid();
                var modelNew = new DepartmentModel
                {
                    DepartmentId = guidid,
                    CompanyId = departmentVM.CompanyId,
                    StoreId = departmentVM.StoreId,
                    WorkShopId = departmentVM.WorkShopId,
                    DepartmentCode = departmentVM.DepartmentCode,
                    DepartmentName = departmentVM.DepartmentName,
                    DepartmentType = departmentVM.DepartmentType,
                    OrderIndex = departmentVM.OrderIndex,
                    CreateBy = CurrentUser.AccountId,
                    CreateTime = DateTime.Now,
                    Actived = departmentVM.Actived
                };
                if (departmentVM.EquipmentList != null)
                {
                    foreach (var item in departmentVM.EquipmentList)
                    {
                        var newMapping = new Department_Equipment_Mapping
                        {
                            MappingId = Guid.NewGuid(),
                            DepartmentId = guidid,
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
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_Store.ToLower())
                });

            });
        }
        #endregion

        //GET: /Department/Edit
        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(Guid id)
        {
            var departmentInDb = _context.DepartmentModel.FirstOrDefault(p => p.DepartmentId == id && p.Actived == true);
            if (departmentInDb == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.Master_Department.ToLower()) });
            }
            var departmentVM = new DepartmentViewModel
            {
                DepartmentId = departmentInDb.DepartmentId,
                CompanyId = departmentInDb.CompanyId,
                StoreId = departmentInDb.StoreId,
                WorkShopId = departmentInDb.WorkShopId,
                DepartmentCode = departmentInDb.DepartmentCode,
                DepartmentName = departmentInDb.DepartmentName,
                DepartmentType = departmentInDb.DepartmentType,
                OrderIndex = departmentInDb.OrderIndex,
                Actived = departmentInDb.Actived,
                RoutingList = _context.Department_Routing_Mapping.Where(p => p.DepartmentId == id).Select(p => p.StepId).ToList(),
                EquipmentList = _context.Department_Equipment_Mapping.Where(x => x.DepartmentId == id).Select(x => x.EquipmentId).ToList()
            };
            //Máy móc chuyền
            ViewBag.EquipmentListAll = _context.EquipmentModel.Where(x => x.Actived == true).OrderBy(x => x.EquipmentTypeCode).ToList();
            ViewBag.EquipmentListSelect = departmentVM.EquipmentList;
            CreateViewBag(departmentVM.StoreId, departmentVM.CompanyId, departmentVM.WorkShopId, departmentVM.RoutingList,departmentVM.DepartmentType);
            return View(departmentVM);
        }
        //POST: Edit
        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(DepartmentViewModel departmentVM)
        {
            return ExecuteContainer(() =>
            {
                var modelDb = _context.DepartmentModel.FirstOrDefault(p => p.DepartmentId == departmentVM.DepartmentId);

                if (modelDb != null)
                {
                    modelDb.CompanyId = departmentVM.CompanyId;
                    modelDb.StoreId = departmentVM.StoreId;
                    modelDb.WorkShopId = departmentVM.WorkShopId;
                    modelDb.DepartmentCode = departmentVM.DepartmentCode;
                    modelDb.DepartmentName = departmentVM.DepartmentName;
                    modelDb.DepartmentType = departmentVM.DepartmentType;
                    modelDb.OrderIndex = departmentVM.OrderIndex;
                    modelDb.LastEditBy = CurrentUser.AccountId;
                    modelDb.Actived = departmentVM.Actived;
                    modelDb.LastEditTime = DateTime.Now;

                    //tìm xóa bảng mapping
                    var mapping = _context.Department_Equipment_Mapping.Where(x => x.DepartmentId == departmentVM.DepartmentId).ToList();
                    _context.Department_Equipment_Mapping.RemoveRange(mapping);
                    if (departmentVM.EquipmentList != null)
                    {
                        foreach (var item in departmentVM.EquipmentList)
                        {
                            var newmodel = new Department_Equipment_Mapping();
                            newmodel.MappingId = Guid.NewGuid();
                            newmodel.EquipmentId = item;
                            newmodel.DepartmentId = modelDb.DepartmentId;

                            _context.Entry(newmodel).State = EntityState.Added;
                        }
                    }
                }
                _context.Entry(modelDb).State = EntityState.Modified;

                //Routing_Department_Mapping
                //remove old mapping
                //var existsMapping = _context.Department_Routing_Mapping.Where(p => p.DepartmentId == departmentVM.DepartmentId).ToList();
                //if (existsMapping != null && existsMapping.Count > 0)
                //{
                //    _context.Department_Routing_Mapping.RemoveRange(existsMapping);
                //}
                //if (departmentVM.RoutingList != null && departmentVM.RoutingList.Count > 0)
                //{

                //    foreach (var stepId in departmentVM.RoutingList)
                //    {
                //        var stepCode = _context.RoutingModel.Where(p => p.StepId == stepId).Select(p => p.StepCode).FirstOrDefault();
                //        var newMapping = new Department_Routing_Mapping
                //        {
                //            DepartmentId = departmentVM.DepartmentId,
                //            StepId = stepId,
                //            Note = string.Format("{0}-{1}", departmentVM.DepartmentCode, stepCode),
                //        };

                //        _context.Entry(newMapping).State = EntityState.Added;
                //    }
                //}

                _context.SaveChanges();

                return Json(new
                {
                    Code = System.Net.HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.Master_Department.ToLower())
                });
            });
        }
        #endregion

        //GET: /Department/Delete
        #region Delete
        [HttpPost]
        [ISDAuthorizationAttribute]
        public ActionResult Delete(Guid id)
        {
            return ExecuteDelete(() =>
            {
                var department = _context.DepartmentModel.FirstOrDefault(p => p.DepartmentId == id);
                if (department != null)
                {
                    _context.Entry(department).State = EntityState.Deleted;
                    _context.SaveChanges();

                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.Master_Department.ToLower())
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = ""
                    });
                }
            });
        }
        #endregion

        #region ViewBag, Helper
        public void CreateViewBag(Guid? StoreId = null, Guid? CompanyId = null, Guid? WorkShopId = null, List<Guid> RoutingList = null,string Department = null)
        {
            //Cong ty
            var companyList = _context.CompanyModel.Where(p => p.Actived).OrderBy(p => p.OrderIndex).ToList();

            ViewBag.CompanyId = new SelectList(companyList, "CompanyId", "CompanyName", CompanyId);
            //Nhà máy
            var storeList = _context.StoreModel.Where(p => p.Actived && (CompanyId != null && p.CompanyId == CompanyId)).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.StoreId = new SelectList(storeList, "StoreId", "StoreName", StoreId);

            //Phân xưởng
            var workshopList = _context.WorkShopModel.Where(p => (bool)p.Actived).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.WorkShopId = new SelectList(workshopList, "WorkShopId", "WorkShopName", WorkShopId);

            //Công đoạn
            var routingList = _context.RoutingModel.Where(p => (bool)p.Actived).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.RoutingList = new MultiSelectList(routingList, "StepId", "StepName", RoutingList);
            //Máy móc chuyền
            var EquipmentList = _context.EquipmentModel.Where(x => x.Actived == true).OrderByDescending(x => x.WorkShopId).ToList();
            ViewBag.EquipmentList = new SelectList(EquipmentList, "EquipmentId", "EquipmentName");
            //Loại phòng ban/tổ
            var departmenttypelist = _context.CatalogModel.Where(x => x.Actived == true && x.CatalogTypeCode == "DepartmentType").ToList();
            ViewBag.DepartmentType = new SelectList(departmenttypelist, "CatalogCode", "CatalogText_vi", Department);
        }
        //Get company by companyId
        public ActionResult GetStoreByCompany(Guid? CompanyId)
        {
            var storeList = _context.StoreModel.Where(p => p.Actived && p.CompanyId == CompanyId)
                                                            .Select(p => new StoreViewModel()
                                                            {
                                                                StoreId = p.StoreId,
                                                                StoreName = p.StoreName
                                                            }).ToList();
            var selectList = new SelectList(storeList, "StoreId", "StoreName");
            return Json(selectList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Remote Validation
        private bool IsExists(string DepartmentCode)
        {
            return (_context.DepartmentModel.FirstOrDefault(p => p.DepartmentCode == DepartmentCode) != null);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CheckExistingDepartmentCode(string DepartmentCode, string DepartmentCodeValid)
        {
            try
            {
                if (DepartmentCodeValid != DepartmentCode)
                {
                    return Json(!IsExists(DepartmentCode));
                }
                else
                {
                    return Json(true);
                }
            }
            catch// (Exception ex)
            {
                return Json(false);
            }
        }
        #endregion
        const string controllerCode = ConstExcelController.Department;
        const int startIndex = 9;
        [ISDAuthorizationAttribute]
        public ActionResult Import()
        {
            return ExcuteImportExcel(() =>
            {
                DataSet ds = GetDataSetFromExcelNew();
                List<string> errorList = new List<string>();
                if (ds.Tables != null && ds.Tables.Count > 0)
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        DataTable dt = ds.Tables[0];
                        //foreach (DataTable dt in ds.Tables)
                        //{
                        //Get controller code from Excel file
                        //  string contCode = dt.Columns[0].ColumnName.ToString()
                        string contCode = dt.Rows[0][0].ToString();
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
                                    DepartmentExcelViewModel departmentValid = CheckTemplate(dr.ItemArray, index);

                                    if (!string.IsNullOrEmpty(departmentValid.Error))
                                    {
                                        string error = departmentValid.Error;
                                        errorList.Add(error);
                                    }
                                    else
                                    {
                                        string result = ExecuteImportExcelSalesEmployee(departmentValid);
                                        if (result != LanguageResource.ImportSuccess)
                                        {
                                            errorList.Add(result);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            string error = string.Format(LanguageResource.Validation_ImportCheckController, LanguageResource.MasterData_SalesEmployee);
                            errorList.Add(error);
                        }

                        // }
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
            });

        }

        #region Insert/Update data from excel file
        public string ExecuteImportExcelSalesEmployee(DepartmentExcelViewModel departmentValid)
        {
            //Get employee in Db by employeeCode
            DepartmentModel depatmentExist = _context.DepartmentModel
                                                     .FirstOrDefault(p => p.DepartmentCode == departmentValid.DepartmentCode);

            //Check:
            //1. If employee == null then => Insert
            //2. Else then => Update
            #region Insert
            if (depatmentExist == null)
            {
                try
                {
                    DepartmentModel deparment = new DepartmentModel();
                    deparment.DepartmentId = Guid.NewGuid();
                    deparment.DepartmentCode = departmentValid.DepartmentCode;
                    deparment.DepartmentName = departmentValid.DepartmentName;
                    deparment.Actived = departmentValid.Actived;
                    _context.Entry(deparment).State = EntityState.Added;

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
                    depatmentExist.DepartmentName = departmentValid.DepartmentName;
                    depatmentExist.DepartmentCode = departmentValid.DepartmentCode;
                    depatmentExist.Actived = departmentValid.Actived;

                    _context.Entry(depatmentExist).State = EntityState.Modified;

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

        public DepartmentExcelViewModel CheckTemplate(object[] row, int index)
        {
            DepartmentExcelViewModel departmentViewModel = new DepartmentExcelViewModel();
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
                            departmentViewModel.RowIndex = rowIndex;
                            break;
                        //SalesEmployee_Code
                        case 1:
                            fieldName = LanguageResource.Department_DepartmentCode;
                            string departmentCode = row[i].ToString();
                            if (string.IsNullOrEmpty(departmentCode))
                            {
                                departmentViewModel.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.Department_DepartmentCode), departmentViewModel.RowIndex);
                            }
                            else
                            {
                                departmentViewModel.DepartmentCode = departmentCode;
                            }
                            break;
                        //SalesEmployee_Name
                        case 2:
                            fieldName = LanguageResource.Department_DepartmentName;
                            string departmentName = row[i].ToString();
                            if (string.IsNullOrEmpty(departmentName))
                            {
                                departmentViewModel.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.Department_DepartmentName), departmentViewModel.RowIndex);
                            }
                            else
                            {
                                departmentViewModel.DepartmentName = departmentName;
                            }
                            break;
                        //SalesEmployee_Name
                        case 3:
                            fieldName = LanguageResource.Company_CompanyCode;
                            string companyCode = row[i].ToString();
                            if (string.IsNullOrEmpty(companyCode))
                            {
                                departmentViewModel.CompanyId = null;
                            }
                            else
                            {
                                departmentViewModel.CompanyId = _context.CompanyModel.FirstOrDefault(d => d.CompanyCode == companyCode).CompanyId;
                            }
                            break;
                        case 4:
                            fieldName = LanguageResource.Store_StoreCode;
                            string storeCode = row[i].ToString();
                            if (string.IsNullOrEmpty(storeCode))
                            {
                                departmentViewModel.StoreId = null;
                            }
                            else
                            {
                                departmentViewModel.StoreId = _context.StoreModel.FirstOrDefault(d => d.SaleOrgCode == storeCode).StoreId;
                            }
                            break;
                        //Actived
                        case 5:
                            fieldName = LanguageResource.Actived;
                            departmentViewModel.Actived = GetTypeFunction<bool>(row[i].ToString(), i);
                            break;
                    }
                    #endregion Convert data to import
                }
            }
            catch (FormatException ex)
            {
                var Message = ex.Message;
                departmentViewModel.Error = string.Format(LanguageResource.Validation_ImportCastValid, fieldName, index);
            }
            catch (InvalidCastException ex)
            {
                var Message = ex.Message;
                departmentViewModel.Error = string.Format(LanguageResource.Validation_ImportCastValid, fieldName, index);
            }
            catch (Exception ex)
            {
                var Message = ex.Message;
                departmentViewModel.Error = string.Format(LanguageResource.Validate_ImportException, fieldName, index);
            }
            return departmentViewModel;
        }

    }
}
