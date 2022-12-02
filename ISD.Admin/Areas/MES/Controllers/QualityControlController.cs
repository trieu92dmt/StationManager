using ISD.Constant;
using ISD.Core;
using ISD.EntityModels;
using ISD.Repositories;
using ISD.Resources;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class QualityControlController : BaseController
    {
        // GET: QualityControl
        #region Index
        public ActionResult Index()
        {
            CreateViewBag();
            return View();
        }

        public ActionResult QRCode()
        {
            CreateViewBag();
            return View();
        }
        #endregion

        #region Search
        public ActionResult _PaggingServerSide(DatatableViewModel model, QualityControlViewModel searchModel)
        {
            // action inside a standard controller
            int filteredResultsCount;
            int totalResultsCount = model.length;

            //Page Size 
            searchModel.PageSize = model.length;
            //Page Number
            searchModel.PageNumber = model.start / model.length + 1;

            if (searchModel.ConfirmCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchModel.ConfirmCommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchModel.ConfirmFromDate = fromDate;
                searchModel.ConfirmToDate = toDate;
            }
            if (searchModel.QualityCommonDate != "Custom")
            {
                DateTime? fromDate;
                DateTime? toDate;
                _unitOfWork.CommonDateRepository.GetDateBy(searchModel.QualityCommonDate, out fromDate, out toDate);
                //Tìm kiếm kỳ hiện tại
                searchModel.QualityFromDate = fromDate;
                searchModel.QualityToDate = toDate;
            }
            //Tìm theo plant của user đang chọn hiện tại
            searchModel.SaleOrgCode = CurrentUser.CompanyCode;
            var query = _unitOfWork.QualityControlRepository.Search(searchModel);
            var res = CustomSearchRepository.CustomSearchFunc<QualityControlViewModel>(model, out filteredResultsCount, out totalResultsCount, query, "ConfirmDate");
            if (res != null && res.Count() > 0)
            {
                int i = model.start;
                foreach (var item in res)
                {
                    i++;
                    item.STT = i;

                    #region Files
                    var errorList = (from p in _context.QualityControl_Error_Mapping
                                     join erTemp in _context.CatalogModel on new { CatalogTypeCode = ConstQualityControl.QualityControl_Error, CatalogCode = p.CatalogCode } equals new { CatalogTypeCode = erTemp.CatalogTypeCode, CatalogCode = erTemp.CatalogCode } into erList
                                     from er in erList.DefaultIfEmpty()
                                     where p.QualityControlId == item.QualityControlId
                                     select new QualityControlErrorViewModel()
                                     {
                                         QuanlityControl_Error_Id = p.QuanlityControl_Error_Id,
                                         QualityControlId = item.QualityControlId,
                                         CatalogCode = p.CatalogCode,
                                         CatalogText_vi = er.CatalogText_vi,
                                         Notes = p.Notes,
                                     }).ToList();
                    item.FileViewModel = _unitOfWork.QualityControlRepository.GetFileAndErrorList(errorList, item.QualityControlId);
                    #endregion


                    #region BackgroundColor - Color
                    //1. Đã kiểm tra - Pass - không có lỗi
                    if (item.Status == true && item.Result == ConstQualityControl.QualityControl_Result_Pass && errorList == null)
                    {
                        item.BackgroundColor = "#00FF00";
                        item.Color = "#000000";
                    }
                    //2. Đã kiểm tra - Pass - có lỗi
                    else if (item.Status == true && item.Result == ConstQualityControl.QualityControl_Result_Pass && errorList != null)
                    {
                        item.BackgroundColor = "#FFFF00";
                        item.Color = "#000000";
                    }
                    //2. Đã kiểm tra - Fail
                    else if (item.Status == true && item.Result == ConstQualityControl.QualityControl_Result_Fail)
                    {
                        item.BackgroundColor = "#DD0000";
                        item.Color = "#FFFFFF";
                    }
                    // Chưa kiểm tra
                    else
                    {
                        item.BackgroundColor = "#FFFFFF";
                        item.Color = "#000000";
                    }
                    #endregion BackgroundColor - Color

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

        #endregion

        #region Edit
        public ActionResult Edit(Guid? Id)
        {
            QualityControlViewModel viewModel = _unitOfWork.QualityControlRepository.GetById(Id);
            if (viewModel == null)
            {
                return Json(new
                {
                    Code = HttpStatusCode.NotModified,
                    Success = false,
                    Data = string.Format(LanguageResource.Error_NotExist, "phiếu " + LanguageResource.QualityControl.ToLower()),
                });
            }
            CreateViewBag(viewModel);
            return PartialView(viewModel);
        }

        [HttpPost]
        public JsonResult Edit(QualityControlViewModel viewModel)
        {
            return ExecuteContainer(() =>
            {
                #region CreateBy - CreateTime
                viewModel.LastEditBy = CurrentUser.AccountId;
                viewModel.LastEditTime = DateTime.Now;
                #endregion
                QualityControlModel model = _unitOfWork.QualityControlRepository.Edit(viewModel);
                #region Lưu file QC
                if (viewModel.File != null && viewModel.File.Count > 0)
                {
                    foreach (var item in viewModel.File)
                    {
                        FileAttachmentModel fileNew = SaveFileAttachment(model.QualityControlId, item, "QualityControl/" + model.QualityControlCode);

                        QualityControl_FileAttachment_Mapping mapping = new QualityControl_FileAttachment_Mapping();
                        mapping.FileAttachmentId = fileNew.FileAttachmentId;
                        mapping.QualityControlId = model.QualityControlId;
                        _context.Entry(mapping).State = EntityState.Added;
                    }
                }
                #endregion
                #region Lưu file thông tin kiểm tra
                if (viewModel.QualityControlInformationViewModel != null)
                {
                    foreach (var item in viewModel.QualityControlInformationViewModel)
                    {
                        if (item.QualityControlInformationId != null && (item.QualityControl_QCInformation_Id == null || item.QualityControl_QCInformation_Id == Guid.Empty))
                        {
                            QualityControl_QCInformation_Mapping info = new QualityControl_QCInformation_Mapping();
                            info.QualityControl_QCInformation_Id = Guid.NewGuid();
                            info.QualityControlInformationId = item.QualityControlInformationId;
                            info.QualityControlId = model.QualityControlId;
                            info.WorkCenterCode = model.WorkCenterCode;
                            info.Notes = item.Notes;
                            _context.Entry(info).State = EntityState.Added;

                            if (item.File != null && item.File.Count > 0)
                            {
                                foreach (var x in item.File)
                                {
                                    FileAttachmentModel fileNew = SaveFileAttachment(info.QualityControl_QCInformation_Id, x, "QualityControl/" + model.QualityControlCode + "/QCInfo");

                                    QualityControl_QCInformation_File_Mapping mapping = new QualityControl_QCInformation_File_Mapping();
                                    mapping.FileAttachmentId = fileNew.FileAttachmentId;
                                    mapping.QualityControl_QCInformation_Id = info.QualityControl_QCInformation_Id;
                                    _context.Entry(mapping).State = EntityState.Added;
                                }
                            }
                        }
                        else if (item.QualityControlInformationId != null && item.QualityControl_QCInformation_Id != null && item.QualityControl_QCInformation_Id != Guid.Empty)
                        {
                            QualityControl_QCInformation_Mapping info = _context.QualityControl_QCInformation_Mapping.Find(item.QualityControl_QCInformation_Id);
                            info.QualityControlInformationId = item.QualityControlInformationId;
                            info.WorkCenterCode = model.WorkCenterCode;
                            info.Notes = item.Notes;
                            if (item.File != null && item.File.Count > 0)
                            {
                                foreach (var x in item.File)
                                {
                                    FileAttachmentModel fileNew = SaveFileAttachment(info.QualityControl_QCInformation_Id, x, "QualityControl/" + model.QualityControlCode + "/QCInfo");

                                    QualityControl_QCInformation_File_Mapping mapping = new QualityControl_QCInformation_File_Mapping();
                                    mapping.FileAttachmentId = fileNew.FileAttachmentId;
                                    mapping.QualityControl_QCInformation_Id = info.QualityControl_QCInformation_Id;
                                    _context.Entry(mapping).State = EntityState.Added;
                                }
                            }
                        }

                    }

                }
                #endregion
                #region Lưu file lỗi
                if (viewModel.ErrorViewModel != null)
                {
                    foreach (var item in viewModel.ErrorViewModel)
                    {
                        if (item.CatalogCode != null && (item.QuanlityControl_Error_Id == null || item.QuanlityControl_Error_Id == Guid.Empty))
                        {
                            QualityControl_Error_Mapping error = new QualityControl_Error_Mapping();
                            error.QuanlityControl_Error_Id = Guid.NewGuid();
                            error.CatalogCode = item.CatalogCode;
                            error.QualityControlId = model.QualityControlId;
                            error.LevelError = item.LevelError;
                            error.QuantityError = item.QuantityError;
                            error.Notes = item.Notes;
                            _context.Entry(error).State = EntityState.Added;

                            if (item.File != null && item.File.Count > 0)
                            {
                                foreach (var x in item.File)
                                {
                                    FileAttachmentModel fileNew = SaveFileAttachment(error.QuanlityControl_Error_Id, x, "QualityControl/" + model.QualityControlCode + "/Error");

                                    QualityControl_Error_File_Mapping mapping = new QualityControl_Error_File_Mapping();
                                    mapping.FileAttachmentId = fileNew.FileAttachmentId;
                                    mapping.QuanlityControl_Error_Id = error.QuanlityControl_Error_Id;
                                    _context.Entry(mapping).State = EntityState.Added;
                                }
                            }
                        }
                        else if (item.CatalogCode != null && item.QuanlityControl_Error_Id != null && item.QuanlityControl_Error_Id != Guid.Empty)
                        {
                            QualityControl_Error_Mapping error = _context.QualityControl_Error_Mapping.Find(item.QuanlityControl_Error_Id);
                            error.QualityControlId = model.QualityControlId;
                            error.CatalogCode = item.CatalogCode;
                            error.LevelError = item.LevelError;
                            error.QuantityError = item.QuantityError;
                            error.Notes = item.Notes;
                            if (item.File != null && item.File.Count > 0)
                            {
                                foreach (var x in item.File)
                                {
                                    FileAttachmentModel fileNew = SaveFileAttachment(error.QuanlityControl_Error_Id, x, "QualityControl/" + model.QualityControlCode + "/Error");

                                    QualityControl_Error_File_Mapping mapping = new QualityControl_Error_File_Mapping();
                                    mapping.FileAttachmentId = fileNew.FileAttachmentId;
                                    mapping.QuanlityControl_Error_Id = error.QuanlityControl_Error_Id;
                                    _context.Entry(mapping).State = EntityState.Added;
                                }
                            }
                        }

                    }

                }
                #endregion
                _context.SaveChanges();


                return Json(new
                {
                    Code = HttpStatusCode.Accepted,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Edit_Success, "phiếu " + LanguageResource.QualityControl.ToLower()),
                });
            });
        }

        #endregion

        #region Lưu file đính kèm
        /// <summary>
        /// Lưu file đính kèm
        /// </summary>
        /// <param name="ObjectId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private FileAttachmentModel SaveFileAttachment(Guid ObjectId, HttpPostedFileBase item, string folder)
        {
            //FileAttachmentModel
            var fileNew = new FileAttachmentModel();
            //1. GUID
            fileNew.FileAttachmentId = Guid.NewGuid();
            //2. Mã Profile
            fileNew.ObjectId = ObjectId;
            //3. Tên file
            fileNew.FileAttachmentName = item.FileName;
            //4. Đường dẫn
            fileNew.FileUrl = UploadDocumentFile(item, folder);
            //5. Đuôi file
            fileNew.FileExtention = _unitOfWork.UtilitiesRepository.FileExtension(item.FileName);
            //7. Loại file
            fileNew.FileAttachmentCode = _unitOfWork.UtilitiesRepository.GetFileTypeByExtension(fileNew.FileExtention);
            //7. Người tạo
            fileNew.CreateBy = CurrentUser.AccountId;
            //8. Thời gian tạo
            fileNew.CreateTime = DateTime.Now;
            _context.Entry(fileNew).State = EntityState.Added;
            return fileNew;
        }

        #endregion

        #region Xoá lỗi
        [HttpPost]
        public JsonResult DeleteError(Guid? Id)
        {
            return ExecuteContainer(() =>
            {
                List<string> urlfiles = new List<string>();
                var model = _context.QualityControl_Error_Mapping.Find(Id);
                var qc = _context.QualityControlModel.Find(model.QualityControlId);
                var result = _unitOfWork.QualityControlRepository.DeleteError(Id, out urlfiles);
                if (result)
                {

                    if (urlfiles != null && urlfiles.Count() > 0)
                    {
                        foreach (var item in urlfiles)
                        {
                            var root = Server.MapPath("~/Upload/" + qc.QualityControlCode + "/Error");
                            _unitOfWork.QualityControlRepository.DeleteFileInServer(root, item);
                        }
                    }
                    return Json(new
                    {
                        Code = HttpStatusCode.Accepted,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, "lỗi"),
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotFound,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, "lỗi"),
                    });
                }

            });
        }
        #endregion

        #region Xoá thông tin kiểm tra
        [HttpPost]
        public JsonResult DeleteInfo(Guid? Id)
        {
            return ExecuteContainer(() =>
            {
                List<string> urlfiles = new List<string>();
                var model = _context.QualityControl_QCInformation_Mapping.Find(Id);
                var qc = _context.QualityControlModel.Find(model.QualityControlId);
                var result = _unitOfWork.QualityControlRepository.DeleteInfo(Id, out urlfiles);
                if (result)
                {
                    if (urlfiles != null && urlfiles.Count() > 0)
                    {
                        foreach (var item in urlfiles)
                        {
                            var root = Server.MapPath("~/Upload/" + qc.QualityControlCode + "/QCInfo");
                            _unitOfWork.QualityControlRepository.DeleteFileInServer(root, item);
                        }
                    }
                    return Json(new
                    {
                        Code = HttpStatusCode.Accepted,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_Delete_Success, "lỗi"),
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotFound,
                        Success = false,
                        Data = string.Format(LanguageResource.Alert_NotExist_Delete, "lỗi"),
                    });
                }

            });
        }
        #endregion

        #region Helper
        public void CreateViewBag(QualityControlViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new QualityControlViewModel();
            }
            #region  Common Date
            var SelectedCommonDate = "Today";
            //Ngày confirm
            var commonDateList = _unitOfWork.CatalogRepository.GetBy(ConstCatalogType.CommonDate);
            ViewBag.ConfirmCommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", SelectedCommonDate);
            //Ngày QC
            ViewBag.QualityCommonDate = new SelectList(commonDateList, "CatalogCode", "CatalogText_vi", "Custom");
            #endregion

            #region Công đoạn lớn
            var WorkCenterList = _context.WorkCenterModel.Where(x => x.SaleOrgCode == CurrentUser.SaleOrg).ToList();
            ViewBag.WorkCenterCode = new SelectList(WorkCenterList, "WorkCenterCode", "WorkCenterName");
            #endregion

            #region Nhà máy
            var StoreList = _context.StoreModel.Where(x => x.Actived == true).ToList();
            ViewBag.SaleOrgCode = new SelectList(StoreList, "SaleOrgCode", "StoreName", CurrentUser.SaleOrg);
            #endregion

            #region Phân xưởng
            var WorkShopList = _context.WorkShopModel.Where(p => p.Actived == true && p.CompanyId == CurrentUser.CompanyId).OrderBy(p => p.OrderIndex).Select(x => new { x.WorkShopCode, display = x.WorkShopCode + " | " + x.WorkShopName }).ToList();
            ViewBag.WorkShopCode = new SelectList(WorkShopList, "WorkShopCode", "display");
            #endregion

            #region Loại kiểm tra
            var QualityTypeList = _unitOfWork.CatalogRepository.GetBy(ConstQualityControl.QualityControlType);
            ViewBag.QualityType = new SelectList(QualityTypeList, "CatalogCode", "CatalogText_vi", viewModel?.QualityType);
            #endregion

            #region Nhân viên nhóm QC
            var QCEmployeeList = _unitOfWork.SalesEmployeeRepository.GetSalesEmployeeByRoles(ConstRoleCode.QC);
            ViewBag.QualityChecker = new SelectList(QCEmployeeList, "AccountId", "SalesEmployeeName", viewModel?.QualityChecker ?? CurrentUser.AccountId);
            #endregion

            #region TestMethod  
            var TestMethodList = _unitOfWork.CatalogRepository.GetBy(ConstQualityControl.QualityControl_Method);
            ViewBag.TestMethod = new SelectList(TestMethodList, "CatalogCode", "CatalogText_vi", viewModel.QualityControlDetailViewModel?.TestMethod);
            #endregion

            #region Mức độ lấy mẫu
            var SamplingLevelList = _unitOfWork.CatalogRepository.GetBy(ConstQualityControl.SamplingLevel);
            ViewBag.SamplingLevel = new SelectList(SamplingLevelList, "CatalogCode", "CatalogText_vi", viewModel.QualityControlDetailViewModel?.SamplingLevel);
            #endregion

            #region Result
            var ResultList = _unitOfWork.CatalogRepository.GetBy(ConstQualityControl.QualityControl_Result);
            ViewBag.Result = new SelectList(ResultList, "CatalogCode", "CatalogText_vi", viewModel?.Result);
            #endregion

            #region Result Detail
            var ResultDetailList = _unitOfWork.CatalogRepository.GetBy(ConstQualityControl.QualityControl_Result);
            ViewBag.ResultDetail = new SelectList(ResultDetailList, "CatalogCode", "CatalogText_vi", viewModel.QualityControlDetailViewModel?.Result);
            #endregion

            #region Error
            var ErrorList = _unitOfWork.CatalogRepository.GetBy(ConstQualityControl.QualityControl_Error).Select(x=>new CatalogViewModel { CatalogCode =  x.CatalogCode, CatalogText_vi = x.CatalogCode + " | " + x.CatalogText_vi }).ToList();
            ViewBag.CatalogCode = new SelectList(ErrorList, "CatalogCode", "CatalogText_vi");
            ViewBag.ErrorList = ErrorList;
            #endregion

            #region Thông tin kiểm tra
            var QCInfoList = (from p in _context.QualityControlInformationModel
                              from w in p.WorkCenterModel.DefaultIfEmpty()
                              orderby p.Code ascending
                              select new QualityControlInformationViewModel
                              {
                                  Id = p.Id,
                                  Code = p.Code,
                                  Name = p.Code + " | " + p.Name,
                                  WorkCenterCode = w.WorkCenterCode
                              }).ToList();
            if (viewModel != null)
            {
                QCInfoList = QCInfoList.Where(x => x.WorkCenterCode == viewModel.WorkCenterCode).ToList();

            }
            ViewBag.QualityControlInformationId = new SelectList(QCInfoList, "Id", "Name");
            ViewBag.QCInfoList = QCInfoList;
            #endregion
        }


        #endregion

        #region Thêm row lỗi
        [HttpPost]
        public ActionResult AddRowError(List<QualityControlErrorViewModel> errorViewModel)
        {
            var newRow = new QualityControlErrorViewModel();
            ViewBag.Index = errorViewModel?.Count() ?? 0;
            CreateViewBag();
            return PartialView("_AddRowError", newRow);
        }
        #endregion

        #region Thêm row thông tin kiểm tra
        [HttpPost]
        public ActionResult AddRowInfo(List<QualityControlInformationMappingViewModel> QualityControlInformationViewModel, string WorkCenterCode)
        {
            var newRow = new QualityControlInformationMappingViewModel();
            ViewBag.Index = QualityControlInformationViewModel?.Count() ?? 0;
            CreateViewBag(new QualityControlViewModel() { WorkCenterCode = WorkCenterCode });
            return PartialView("_AddRowInfo", newRow);
        }
        #endregion

        #region Hiển thị hình ảnh
        public ActionResult _ImageInfo(Guid? id, string type)
        {
            List<FileAttachmentViewModel> file = new List<FileAttachmentViewModel>();
            if (type == "QC")
            {
                var qc = _unitOfWork.QualityControlRepository.GetById(id);
                if (qc != null)
                {
                    file = _unitOfWork.QualityControlRepository.GetFileList(id);
                    ViewBag.URL = "/Upload/QualityControl/" + qc.QualityControlCode;
                }
                ViewBag.Title = "Hình ảnh kiểm tra";
            }
            else if(type == "QCERROR")
            {
                var errorList = (from p in _context.QualityControl_Error_Mapping
                                 join erTemp in _context.CatalogModel on new { CatalogTypeCode = ConstQualityControl.QualityControl_Error, CatalogCode = p.CatalogCode } equals new { CatalogTypeCode = erTemp.CatalogTypeCode, CatalogCode = erTemp.CatalogCode } into erList
                                 from er in erList.DefaultIfEmpty()
                                 where p.QuanlityControl_Error_Id == id
                                 select new QualityControlErrorViewModel()
                                 {
                                     QuanlityControl_Error_Id = p.QuanlityControl_Error_Id,
                                     QualityControlId = p.QualityControlId,
                                     CatalogCode = p.CatalogCode,
                                     CatalogText_vi = er.CatalogText_vi,
                                     Notes = p.Notes,
                                 }).ToList();
                file = _unitOfWork.QualityControlRepository.GetErrorFileList(errorList);
                ViewBag.Title = "Hình ảnh lỗi";
                if (errorList != null && errorList.Count() > 0)
                {
                    ViewBag.Title = ViewBag.Title + " " + errorList.FirstOrDefault().CatalogText_vi;
                    var qc = _unitOfWork.QualityControlRepository.GetById(errorList.FirstOrDefault().QualityControlId);
                    if (qc != null)
                    {
                        ViewBag.URL = "/Upload/QualityControl/" + qc.QualityControlCode +"/Error";
                    }
                }
              
            }
            else if (type == "QCINFO")
            {
                var checkedList = (from p in _context.QualityControl_QCInformation_Mapping
                                 join w in _context.WorkCenterModel on p.WorkCenterCode equals w.WorkCenterCode
                                 join pp in _context.QualityControlInformationModel on p.QualityControlInformationId equals pp.Id
                                 where p.QualityControl_QCInformation_Id == id
                                 select new QualityControlInformationMappingViewModel()
                                 {
                                     QualityControl_QCInformation_Id = p.QualityControl_QCInformation_Id,
                                     QualityControlId = p.QualityControlId,
                                     WorkCenterCode = p.WorkCenterCode,
                                     WorkCenterName = w.WorkCenterName,
                                     QualityControlInformationId = pp.Id,
                                     QualityControlInformationCode = pp.Code,
                                     QualityControlInformationName = pp.Name,
                                     Notes = p.Notes,
                                 }).ToList();
                file = _unitOfWork.QualityControlRepository.GetCheckedFileList(checkedList);
                ViewBag.Title = "Thông tin kiểm tra";
                if (checkedList != null && checkedList.Count() > 0)
                {
                    ViewBag.Title = ViewBag.Title + " | " + checkedList.FirstOrDefault().QualityControlInformationCode;
                    var qc = _unitOfWork.QualityControlRepository.GetById(checkedList.FirstOrDefault().QualityControlId);
                    if (qc != null)
                    {
                        ViewBag.URL = "/Upload/QualityControl/" + qc.QualityControlCode + "/QCInfo";
                    }
                }
            }
            ViewBag.Type = type;
            return PartialView(file);
        }
        #endregion

    }
}