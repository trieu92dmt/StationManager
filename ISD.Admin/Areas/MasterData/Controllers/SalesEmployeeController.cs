using AutoMapper;
using AutoMapper.QueryableExtensions;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Resources;
using ISD.ViewModels;
using ISD.Constant;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ISD.ViewModels.MasterData;
using System.Transactions;
using ISD.Core;
using ISD.Repositories.Excel;

namespace MasterData.Controllers
{
    public class SalesEmployeeController : BaseController
    {
        // GET: SalesEmployee
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
           

            ////Thẻ nhân viên
            //var SerialTag = new List<ISDSelectBoolItem>();
            //SerialTag.Add(new ISDSelectBoolItem()
            //{
            //    id = true,
            //    name = "Đã cấp",
            //});
            //SerialTag.Add(new ISDSelectBoolItem()
            //{
            //    id = false,
            //    name = "Chưa cấp",
            //});
            //ViewBag.SerialTag = new SelectList(SerialTag, "id", "name");

            var workShopList = _context.WorkShopModel
                .Where(p => p.Actived == true)
                .OrderBy(p => p.OrderIndex)
                .ToList();
            ViewBag.WorkShopId = new SelectList(workShopList, "WorkShopId", "WorkShopName");
            CreateViewBag();
            return View();
        }

        public ActionResult _Search(string SalesEmployeeCode = "", string SalesEmployeeName = "", string DepartmentId = "", bool? SerialTag = null, Guid? WorkShopId = null)
        {
            return ExecuteSearch(() =>
            {
                List<SalesEmployeeViewModel> laborList = new List<SalesEmployeeViewModel>();
                //not set department
                
                    laborList = (from p in _context.SalesEmployeeModel
                                 join department in _context.DepartmentModel on p.DepartmentId equals department.DepartmentId into dG
                                 from d in dG.DefaultIfEmpty()
                                 join workshop in _context.WorkShopModel on d.WorkShopId equals workshop.WorkShopId into wG
                                 from w in wG.DefaultIfEmpty()
                                 join company in _context.CompanyModel on p.CompanyId equals company.CompanyId
                                 join cata in _context.CatalogModel on p.Position equals cata.CatalogCode
                                 where
                                 //search by SalesEmployeeName
                                 (SalesEmployeeName == "" || p.SalesEmployeeName.Contains(SalesEmployeeName))
                                 //search by SalesEmployeeCode
                                 && (SalesEmployeeCode == "" || p.SalesEmployeeCode.Contains(SalesEmployeeCode))
                                 //search by SerialTag
                                 && (SerialTag == null || (SerialTag == true && !string.IsNullOrEmpty(p.SerialTag)) || (SerialTag == false && string.IsNullOrEmpty(p.SerialTag)))
                                 && (WorkShopId == null || d.WorkShopId == WorkShopId)
                                 && (DepartmentId =="" || p.DepartmentId.ToString() == DepartmentId )
                                 //search actived
                                 && (p.Actived == true)
                                 //catalog
                                 && (cata.CatalogTypeCode == "Position")
                                 select new SalesEmployeeViewModel()
                                 {
                                     SalesEmployeeCode = p.SalesEmployeeCode,
                                     SalesEmployeeName = p.SalesEmployeeName,
                                     Phone = p.Phone,
                                     Email = p.Email,
                                     Actived = p.Actived,
                                     DepartmentName = d.DepartmentName,
                                     SerialTag = p.SerialTag,
                                     WorkShopName = w.WorkShopName,
                                     Position = cata.CatalogText_vi,
                                     Note = p.Note,
                                     CompanyName = company.CompanyName,
                                 }).OrderBy(x=>x.SalesEmployeeCode).ToList();
                return PartialView(laborList);
            });
        }
        #endregion

        // GET: SalesEmployee/Create
        #region Create
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }

        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Create(SalesEmployeeModel model)
        {
            return ExecuteContainer(() =>
            {
                model.CreateBy = CurrentUser.AccountId;
                model.CreateTime = DateTime.Now;
                model.SalesEmployeeName = model.SalesEmployeeName.FirstCharToUpper();
                model.AbbreviatedName = model.SalesEmployeeName.ToAbbreviation();
                _context.Entry(model).State = EntityState.Added;
                _context.SaveChanges();
                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = (string.Format(LanguageResource.Alert_Create_Success, LanguageResource.MasterData_SalesEmployee.ToLower()))
                });
            });
        }
        #endregion

        // GET: SalesEmployee/Edit
        #region Edit
        [ISDAuthorizationAttribute]
        public ActionResult Edit(string id)
        {
            var employee = _context.SalesEmployeeModel.FirstOrDefault(p => p.SalesEmployeeCode == id);
            if (employee == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.MasterData_SalesEmployee.ToLower()) });
            }
            CreateViewBag(null, null, employee.DepartmentId);
            return View(employee);
        }

        [HttpPost]
        [ISDAuthorizationAttribute]
        public JsonResult Edit(SalesEmployeeModel model)
        {
            return ExecuteContainer(() =>
            {
                model.SalesEmployeeName = model.SalesEmployeeName.FirstCharToUpper();
                model.AbbreviatedName = model.SalesEmployeeName.ToAbbreviation();
                model.LastEditBy = CurrentUser.AccountId;
                model.LastEditTime = DateTime.Now;
                //Tìm trong account nếu có user reference đến thì => update tên luôn
                var acc = _context.AccountModel.Where(p => p.EmployeeCode == model.SalesEmployeeCode).FirstOrDefault();
                if (acc != null)
                {
                    acc.FullName = model.SalesEmployeeName;
                    _context.Entry(acc).State = EntityState.Modified;
                }
                _context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.MasterData_SalesEmployee.ToLower())
                });
            });
        }


        public ActionResult EditTag(string id)
        {
            var employee = (from p in _context.SalesEmployeeModel
                            join d in _context.DepartmentModel on p.DepartmentId equals d.DepartmentId into tmpDep
                            from dep in tmpDep.DefaultIfEmpty()
                            where p.SalesEmployeeCode == id
                            select new SalesEmployeeViewModel
                            {
                                SalesEmployeeCode = p.SalesEmployeeCode,
                                SalesEmployeeName = p.SalesEmployeeName,
                                DepartmentName = dep.DepartmentName,
                                SerialTag = p.SerialTag
                            }).FirstOrDefault();
            return View(employee);
        }

        [HttpPost]
        public JsonResult EditTag(string SalesEmployeeCode, string SerialTag)
        {
            return ExecuteContainer(() =>
            {
                var exitSerialTag = _context.SalesEmployeeModel.Where(p => p.SerialTag == SerialTag).FirstOrDefault();
                //Check xem thẻ có đang được liên kết với nhân viên khác không
                if (exitSerialTag != null && exitSerialTag.SalesEmployeeCode != SalesEmployeeCode)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.BadRequest,
                        Success = false,
                        Data = "Thẻ đã được sử dụng bởi nhân viên khác!"
                    });
                }
                var employee = _context.SalesEmployeeModel.Where(p => p.SalesEmployeeCode == SalesEmployeeCode).FirstOrDefault();
                employee.SerialTag = SerialTag;
                _context.Entry(employee).State = EntityState.Modified;
                _context.SaveChanges();
                return Json(new
                {
                    Code = HttpStatusCode.OK,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, LanguageResource.SerialTag.ToLower())
                });
            });
        }
        #endregion

        // GET: SalesEmployee/Delete
        #region Delete
        [HttpPost]
        [ISDAuthorizationAttribute]
        public ActionResult Delete(string id)
        {
            return ExecuteDelete(() =>
            {
                var employee = _context.SalesEmployeeModel.FirstOrDefault(p => p.SalesEmployeeCode == id);
                if (employee != null)
                {
                    _context.Entry(employee).State = EntityState.Deleted;
                    _context.SaveChanges();
                    return Json(new
                    {
                        Code = HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.MasterData_SalesEmployee.ToLower())
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

        const string controllerCode = ConstExcelController.SalesEmployee;
        const int startIndex = 9;
        #region Export to Excell
        public ActionResult ExportCreate()
        {
            List<SalesEmployeeExcelViewModel> viewModel = new List<SalesEmployeeExcelViewModel>();
            return Export(viewModel, isEdit: false);
        }

        public ActionResult ExportEdit(SalesEmployeeSearchViewModel searchViewModel)
        {
            //searchViewModel = (SalesEmployeeSearchViewModel)Session["frmSearchSalesEmployee"];

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<SalesEmployeeViewModel, SalesEmployeeExcelViewModel>(); });
            //Get data from server
            var salesEmployees = (from p in _context.SalesEmployeeModel.Include(s => s.DepartmentModel)
                                  orderby p.SalesEmployeeCode
                                  select new SalesEmployeeViewModel
                                  {
                                      SalesEmployeeCode = p.SalesEmployeeCode,
                                      SalesEmployeeName = p.SalesEmployeeName,
                                      Actived = p.Actived,
                                      DepartmentCode = p.DepartmentModel.DepartmentCode,
                                      Email = p.Email,
                                      Phone = p.Phone,
                                      Position = p.Position,
                                      Note = p.Note,
                                  }).ToList();
            List<SalesEmployeeExcelViewModel> salesEmployee = new List<SalesEmployeeExcelViewModel>();
            foreach (var e in salesEmployees)
            {
                salesEmployee.Add(new SalesEmployeeExcelViewModel
                {
                    SalesEmployeeCode = e.SalesEmployeeCode,
                    DepartmentCode = e.DepartmentCode,
                    SalesEmployeeName = e.SalesEmployeeName,
                    Email = e.Email,
                    Phone = e.Phone,
                    Position = e.Position,
                    Note = e.Note,
                    Actived = e.Actived
                });
            }
            //var salesEmployee = (from p in _context.SalesEmployeeModel.Include(s => s.DepartmentModel)
            //orderby p.SalesEmployeeCode
            //select new SalesEmployeeViewModel {
            //    SalesEmployeeCode = p.SalesEmployeeCode,
            //    SalesEmployeeName = p.SalesEmployeeName,
            //    Actived = p.Actived,
            //    DepartmentName = p.DepartmentModel.DepartmentName
            //})
            //.ProjectTo<SalesEmployeeExcelViewModel>(config).ToList();

            return Export(salesEmployee, isEdit: true);
        }


        [ISDAuthorizationAttribute]
        public FileContentResult Export(List<SalesEmployeeExcelViewModel> viewModel, bool isEdit)
        {
            #region Master
            //Tạo mẫu
            List<ExcelTemplate> columns = new List<ExcelTemplate>();

            if (isEdit == true)
            {
                columns.Add(new ExcelTemplate() { ColumnName = "SalesEmployeeCode", isAllowedToEdit = false });
            }
            else
            {
                columns.Add(new ExcelTemplate() { ColumnName = "SalesEmployeeCode", isAllowedToEdit = true, });
            }
            columns.Add(new ExcelTemplate() { ColumnName = "SalesEmployeeName", isAllowedToEdit = true });
            columns.Add(new ExcelTemplate() { ColumnName = "DepartmentCode", isAllowedToEdit = true });
            columns.Add(new ExcelTemplate() { ColumnName = "Email", isAllowedToEdit = true });
            columns.Add(new ExcelTemplate() { ColumnName = "Phone", isAllowedToEdit = true });
            columns.Add(new ExcelTemplate() { ColumnName = "Position", isAllowedToEdit = true });
            columns.Add(new ExcelTemplate() { ColumnName = "Note", isAllowedToEdit = true });
            columns.Add(new ExcelTemplate() { ColumnName = "Actived", isAllowedToEdit = true, isBoolean = true });

            #endregion

            //Header
            string fileHeader = string.Format(LanguageResource.Export_ExcelHeader, LanguageResource.MasterData_SalesEmployee);
            //thêm Mã code excel "I-00300
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = controllerCode,
                RowsToIgnore = 1,
                isWarning = false,
                isCode = true,
            });
            //thêm Tên bảng Excel
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = fileHeader.ToUpper(),
                RowsToIgnore = 1,
                isWarning = false,
                isCode = false,
            });
            //thêm dòng Lưu ý
            heading.Add(new ExcelHeadingTemplate()
            {
                Content = LanguageResource.Export_ExcelWarning1,
                RowsToIgnore = 0,
                isWarning = true,
                isCode = false
            });
            // thêm dòng "Bắt buộc nhập những cột có chứa dấu (*)"
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
                Content = string.Format(LanguageResource.Export_ExcelWarningActived, LanguageResource.MasterData_SalesEmployee),
                RowsToIgnore = 1,
                isWarning = true,
                isCode = false
            });

            //Nội dung file
            byte[] fileContent = ClassExportExcel.ExportExcel(viewModel, columns, heading, true);

            //Insert or Edit
            //Nếu isEdit = false => exportType = "THEM_MOI"
            //Nếu isEdit = true => exportType = "CAP_NHAT"
            string exportType = LanguageResource.exportType_Insert;
            if (isEdit == true)
            {
                exportType = LanguageResource.exportType_Edit;
            }
            //Ten file sau khi export
            string fileNameWithFormat = string.Format("{0}_{1}.xlsx", exportType, _unitOfWork.UtilitiesRepository.RemoveSign4VietnameseString(fileHeader.ToUpper().Replace(" ", "_")));

            return File(fileContent, ClassExportExcel.ExcelContentType, fileNameWithFormat);

        }
        #endregion

        #region Import from excel

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
                        string contCode = dt.Rows[0][0].ToString();
                        // string contCode = dt.Columns[0].ColumnName.ToString();
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
                                    SalesEmployeeExcelViewModel salesEmployeeIsValid = CheckTemplate(dr.ItemArray, index);

                                    if (!string.IsNullOrEmpty(salesEmployeeIsValid.Error))
                                    {
                                        string error = salesEmployeeIsValid.Error;
                                        errorList.Add(error);
                                    }
                                    else
                                    {
                                        string result = ExecuteImportExcelSalesEmployee(salesEmployeeIsValid);
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
        public string ExecuteImportExcelSalesEmployee(SalesEmployeeExcelViewModel salesEmployeeIsValid)
        {
            //Get employee in Db by employeeCode
            SalesEmployeeModel salesEmployee = _context.SalesEmployeeModel
                                                     .FirstOrDefault(p => p.SalesEmployeeCode == salesEmployeeIsValid.SalesEmployeeCode);
            //Check:
            //1. If employee == null then => Insert
            //2. Else then => Update
            #region Insert
            if (salesEmployee == null)
            {
                try
                {
                    SalesEmployeeModel salesEmployeeNew = new SalesEmployeeModel();
                    salesEmployeeNew.SalesEmployeeCode = salesEmployeeIsValid.SalesEmployeeCode;
                    salesEmployeeNew.SalesEmployeeName = salesEmployeeIsValid.SalesEmployeeName;
                    salesEmployeeNew.DepartmentId = salesEmployeeIsValid.DepartmentId;
                    salesEmployeeNew.CompanyId = salesEmployeeIsValid.CompanyId;
                    salesEmployeeNew.StoreId = salesEmployeeIsValid.StoreId;
                    salesEmployeeNew.Position = salesEmployeeIsValid.Position;
                    salesEmployeeNew.Phone = salesEmployeeIsValid.Phone;
                    salesEmployeeNew.Email = salesEmployeeIsValid.Email;
                    salesEmployeeNew.Note = salesEmployeeIsValid.Note;
                    salesEmployeeNew.Actived = salesEmployeeIsValid.Actived;
                    _context.Entry(salesEmployeeNew).State = EntityState.Added;

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
                    salesEmployee.SalesEmployeeName = salesEmployeeIsValid.SalesEmployeeName;
                    salesEmployee.Position = salesEmployeeIsValid.Position;
                    salesEmployee.Phone = salesEmployeeIsValid.Phone;
                    salesEmployee.Email = salesEmployeeIsValid.Email;
                    salesEmployee.DepartmentId = salesEmployeeIsValid.DepartmentId;
                    salesEmployee.CompanyId = salesEmployeeIsValid.CompanyId;
                    salesEmployee.StoreId = salesEmployeeIsValid.StoreId;
                    salesEmployee.Position = salesEmployeeIsValid.Position;
                    salesEmployee.Actived = salesEmployeeIsValid.Actived;
                    _context.Entry(salesEmployee).State = EntityState.Modified;

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
        public SalesEmployeeExcelViewModel CheckTemplate(object[] row, int index)
        {
            SalesEmployeeExcelViewModel salesEmployeeVM = new SalesEmployeeExcelViewModel();
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
                            salesEmployeeVM.RowIndex = rowIndex;
                            break;
                        //SalesEmployee_Code
                        case 1:
                            fieldName = LanguageResource.SalesEmployee_Code;
                            string salesEmployeeCode = row[i].ToString();
                            if (string.IsNullOrEmpty(salesEmployeeCode))
                            {
                                salesEmployeeVM.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.SalesEmployee_Code), salesEmployeeVM.RowIndex);
                            }
                            else
                            {
                                salesEmployeeVM.SalesEmployeeCode = salesEmployeeCode;
                            }
                            break;
                        //SalesEmployee_Name
                        case 2:
                            fieldName = LanguageResource.SalesEmployee_Name;
                            string salesEmployeeName = row[i].ToString();
                            if (string.IsNullOrEmpty(salesEmployeeName))
                            {
                                salesEmployeeVM.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.SalesEmployee_Name), salesEmployeeVM.RowIndex);
                            }
                            else
                            {
                                salesEmployeeVM.SalesEmployeeName = salesEmployeeName;
                            }
                            break;
                        //Master_DepartmentCode
                        case 3:
                            fieldName = LanguageResource.Master_Department;
                            string department = row[i].ToString();
                            if (string.IsNullOrEmpty(department))
                            {
                                salesEmployeeVM.DepartmentId = null;
                            }
                            else
                            {
                                var existDM = _context.DepartmentModel.FirstOrDefault(d => d.DepartmentCode == department);
                                salesEmployeeVM.DepartmentId = existDM.DepartmentId;
                                salesEmployeeVM.CompanyId = existDM.CompanyId;
                                salesEmployeeVM.StoreId = existDM.StoreId;

                            }
                            break;
                        //Depart
                        //Email
                        case 4:
                            fieldName = LanguageResource.Email;
                            string email = row[i].ToString();
                            if (string.IsNullOrEmpty(email))
                            {
                                salesEmployeeVM.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.Email), salesEmployeeVM.RowIndex);
                            }
                            else
                            {
                                salesEmployeeVM.Email = email;
                            }
                            break;
                        //Phone
                        case 5:
                            fieldName = LanguageResource.PhoneNumber;
                            string phoneNumber = row[i].ToString();
                            if (string.IsNullOrEmpty(phoneNumber))
                            {
                                salesEmployeeVM.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required,
                                    LanguageResource.PhoneNumber), salesEmployeeVM.RowIndex);
                            }
                            else
                            {
                                salesEmployeeVM.Phone = phoneNumber;
                            }
                            break;
                        //Positon
                        case 6:
                            fieldName = LanguageResource.Position;
                            string position = row[i].ToString();
                            if (string.IsNullOrEmpty(position))
                            {
                                salesEmployeeVM.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required, LanguageResource.Position), salesEmployeeVM.RowIndex);
                            }
                            else
                            {
                                salesEmployeeVM.Position = position;
                            }
                            break;
                        case 7:
                            fieldName = LanguageResource.Note;
                            string note = row[i].ToString();
                            if (string.IsNullOrEmpty(note))
                            {
                                salesEmployeeVM.Error = string.Format(LanguageResource.Validation_ImportRequired, string.Format(LanguageResource.Required,
                                    LanguageResource.Note), salesEmployeeVM.RowIndex);
                            }
                            else
                            {
                                salesEmployeeVM.Note = note;
                            }
                            break;
                        //Actived
                        case 8:
                            fieldName = LanguageResource.Actived;
                            salesEmployeeVM.Actived = GetTypeFunction<bool>(row[i].ToString(), i);
                            break;
                    }
                    #endregion Convert data to import
                }
            }
            catch (FormatException ex)
            {
                var Message = ex.Message;
                salesEmployeeVM.Error = string.Format(LanguageResource.Validation_ImportCastValid, fieldName, index);
            }
            catch (InvalidCastException ex)
            {
                var Message = ex.Message;
                salesEmployeeVM.Error = string.Format(LanguageResource.Validation_ImportCastValid, fieldName, index);
            }
            catch (Exception ex)
            {
                var Message = ex.Message;
                salesEmployeeVM.Error = string.Format(LanguageResource.Validate_ImportException, fieldName, index);
            }
            return salesEmployeeVM;
        }
        #endregion
        #endregion

        #region Remote Validation

        private bool IsExists(string SalesEmployeeCode)
        {
            return (_context.SalesEmployeeModel.FirstOrDefault(p => p.SalesEmployeeCode == SalesEmployeeCode) != null);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CheckExistingSalesEmployeeCode(string SalesEmployeeCode, string SalesEmployeeCodeValid)
        {
            try
            {
                if (SalesEmployeeCodeValid != SalesEmployeeCode)
                {
                    return Json(!IsExists(SalesEmployeeCode));
                }
                else
                {
                    return Json(true);
                }
            }
            catch//(Exception ex)
            {
                return Json(false);
            }
        }
        #endregion

        //Lấy danh sách tổ
        public ActionResult GetDepartmentBy(Guid? WorkShopId)
        {
            return ExecuteAPIWithoutAuthContainer(() =>
            {
                var result = (from a in _context.DepartmentModel
                              where a.WorkShopId == WorkShopId
                              select new ISDSelectGuidItem()
                              {
                                  id = a.DepartmentId,
                                  name = a.DepartmentName,
                              }).ToList();
                //result.Add(new ISDSelectGuidItem()
                //{
                //    id = Guid.Empty,
                //    name = LanguageResource.Department_NotSet
                //});

                return _APISuccess(result);
            });
        }

        public void CreateViewBag(Guid? CompanyId = null, Guid? StoreId = null, Guid? DepartmentId = null)
        {
            //Get list Company
            var compList = _context.CompanyModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.CompanyId = new SelectList(compList, "CompanyId", "CompanyName", CompanyId);

            //Get Store
            var storeList = _context.StoreModel.Where(p => p.Actived == true).OrderBy(p => p.OrderIndex).ToList();
            ViewBag.StoreId = new SelectList(storeList, "StoreId", "StoreName", StoreId);

            //Get department
            var deparmentList = _context.DepartmentModel.Include(p => p.StoreModel).Where(p => p.Actived == true).OrderBy(p => p.DepartmentCode).ToList();

            ViewBag.DepartmentId = new SelectList(deparmentList, "DepartmentId", "DepartmentName", DepartmentId);

            //list Role Model
            var Positionlst = _context.CatalogModel.Where(p => p.CatalogTypeCode == "Position" && p.Actived == true).OrderBy(p => p.OrderIndex ).ToList();
            ViewBag.Position = new SelectList(Positionlst, "CatalogCode", "CatalogText_vi");
        }
        //create department by store id
        public ActionResult CreateDepartmentByStore(Guid? StoreId)
        {
            var departmentList = _context.DepartmentModel.Where(p => p.Actived == true && p.StoreId == StoreId)
                                                            .OrderBy(p => p.OrderIndex).ToList();
            var selectList = new SelectList(departmentList, "DepartmentId", "DepartmentName");
            return Json(selectList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeByDepartment(Guid? DepartmentId)
        {
            if (DepartmentId != null && DepartmentId != Guid.Empty)
            {
                var listEmployee = (from p in _context.SalesEmployeeModel
                                    join a in _context.AccountModel on p.SalesEmployeeCode equals a.EmployeeCode
                                    where p.Actived == true && p.DepartmentId == DepartmentId
                                    orderby p.SalesEmployeeCode
                                    select new SalesEmployeeViewModel
                                    {
                                        SalesEmployeeCode = p.SalesEmployeeCode,
                                        SalesEmployeeName = p.SalesEmployeeCode + " | " + p.SalesEmployeeName,
                                        RolesName = a.RolesModel.Where(m => m.isEmployeeGroup == true).Select(p => p.RolesName).FirstOrDefault(),
                                    }).ToList();
                var slEmployee = new SelectList(listEmployee, "SalesEmployeeCode", "SalesEmployeeName");
                return Json(slEmployee, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var listEmployee = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlist(); ;
                var slEmployee = new SelectList(listEmployee, "SalesEmployeeCode", "SalesEmployeeName");
                return Json(slEmployee, JsonRequestBehavior.AllowGet);
            }

        }
    }
}