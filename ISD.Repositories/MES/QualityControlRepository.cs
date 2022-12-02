using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.MES
{

    public class QualityControlRepository
    {
        EntityDataContext _context;

        public QualityControlRepository(EntityDataContext entityDataContext)
        {
            _context = entityDataContext;
        }

        public IQueryable<QualityControlViewModel> Search(QualityControlViewModel searchModel)
        {
            var data = (
                        //Phiếu kiểm tra chất lượng
                        from q in _context.QualityControlModel
                        //Thông tin phiếu thực thi lệnh sản xuất
                        join tt in _context.ThucThiLenhSanXuatModel on q.CustomerReference equals tt.TaskId
                        //LSX SAP
                        join ta in _context.TaskModel on tt.ParentTaskId equals ta.TaskId
                        //DSX
                        join ds in _context.TaskModel on ta.ParentTaskId equals ds.TaskId
                        //Nhà máy
                        join s in _context.StoreModel on q.SaleOrgCode equals s.SaleOrgCode
                        //Phân xưởng
                        join d in _context.WorkShopModel on q.WorkShopCode equals d.WorkShopCode
                        //Công đoạn lớn
                        join w in _context.WorkCenterModel on q.WorkCenterCode equals w.WorkCenterCode
                        //SO 100 => Lấy thông tin khách hàng
                        join so in _context.SaleOrderHeader100Model on q.VBELN equals so.VBELN
                        //Sản phẩm
                        join prTemp in _context.ProductModel on new { CompanyId = s.CompanyId, ERPProductCode = q.ProductCode  } equals new { CompanyId = prTemp.CompanyId.Value, ERPProductCode = prTemp.ERPProductCode  } into prList
                        from pr in prList
                            //Nhân viên QC
                        join aTemp in _context.AccountModel on q.QualityChecker equals aTemp.AccountId into aList
                        from a in aList.DefaultIfEmpty()
                        //Nhân viên tạo
                        join atTemp in _context.AccountModel on q.CreateBy equals atTemp.AccountId into atList
                        from at in atList.DefaultIfEmpty()
                        //Nhân viên chỉnh sửa cuối
                        join alTemp in _context.AccountModel on q.LastEditBy equals alTemp.AccountId into alList
                        from al in alList.DefaultIfEmpty()
                        //Kết quả tổng
                        join rTemp in _context.CatalogModel on new { CatalogTypeCode = ConstQualityControl.QualityControl_Result, CatalogCode = q.Result } equals new { CatalogTypeCode = rTemp.CatalogTypeCode, CatalogCode = rTemp.CatalogCode } into rList
                        from r in rList.DefaultIfEmpty()
                        where  (searchModel.SaleOrgCode == null || searchModel.SaleOrgCode == s.SaleOrgCode)
                             &&(searchModel.WorkShopCode == null || searchModel.WorkShopCode == d.WorkShopCode)
                             &&(searchModel.WorkCenterCode == null || searchModel.WorkCenterCode == w.WorkCenterCode)
                             &&(searchModel.ProfileCode == null || so.KUNNR.Contains(searchModel.ProfileCode))
                             &&(searchModel.ProfileName == null || so.SORTL.Contains(searchModel.ProfileName))
                             &&(searchModel.Status == null || searchModel.Status == q.Status)
                             &&(searchModel.ConfirmFromDate == null || searchModel.ConfirmFromDate <= q.ConfirmDate)
                             &&(searchModel.ConfirmToDate == null || searchModel.ConfirmToDate >= q.ConfirmDate)
                             &&(searchModel.QualityFromDate == null || searchModel.QualityFromDate <= q.QualityDate)
                             &&(searchModel.QualityToDate == null || searchModel.QualityToDate >= q.QualityDate)
                             &&(searchModel.LSXSAP == null || searchModel.LSXSAP == q.LSXSAP)
                             &&(searchModel.LSXDT == null || searchModel.LSXDT == q.LSXDT)
                             &&(searchModel.DSX == null || searchModel.DSX == ds.Summary)
                             &&(searchModel.ProductCode == null || searchModel.ProductCode == q.ProductCode)
                             &&(searchModel.ProductName == null || searchModel.ProductName == q.ProductName)
                             &&(searchModel.QRCode == null || searchModel.QRCode == tt.Barcode)
                        select new QualityControlViewModel
                        {
                            QualityControlId = q.QualityControlId,
                            QualityControlCode = q.QualityControlCode,
                            //Nhà máy
                            SaleOrgCode = q.SaleOrgCode,
                            StoreName = s.SaleOrgCode + " | " + s.StoreName,
                            //Phân xưởng
                            WorkShopCode = q.WorkShopCode,
                            WorkShopName = d.WorkShopCode + " | " + d.WorkShopName,
                            //Công đoạn lớn
                            WorkCenterCode = q.WorkCenterCode,
                            WorkCenterName = w.WorkCenterCode + " | " + w.WorkCenterName,
                            //SO 100 => Khách hàng
                            ProfileCode = so.KUNNR,
                            ProfileName = so.KUNNR + " | " + so.SORTL,
                            //LSX
                            LSXDT = q.LSXDT,
                            LSXSAP = q.LSXSAP,
                            DSX = ds.Summary,
                            //Chi tiết
                            ProductAttribute = q.ProductAttribute,
                            //Sản phẩm
                            ProductCode = q.ProductCode,
                            ProductName = q.ProductCode + " | " + q.ProductName,
                            //TTLSX
                            CustomerReference = q.CustomerReference,
                            //Ngày xác nhận công đoạn lớn
                            ConfirmDate = q.ConfirmDate,
                            //Số lượng xác nhận
                            QuantityConfirm = q.QuantityConfirm,
                            //QC
                            QualityDate = q.QualityDate,
                            QualityChecker = q.QualityChecker,
                            QCSaleEmployee = a.EmployeeCode + " | " + a.FullName,
                            //Số lượng kiểm tra
                            InspectionLotQuantity = q.InspectionLotQuantity,
                            //Tình trạng môi trường
                            Environmental = q.Environmental,
                            //Trạng thái
                            Status = q.Status,
                            //Kết quả tổng
                            Result = r.CatalogText_vi,

                        }).OrderBy(x=>x.ConfirmDate);
            return data;
        }


        public QualityControlViewModel GetById(Guid? Id)
        {
            var data = (
                        //Phiếu kiểm tra chất lượng
                        from q in _context.QualityControlModel
                        //Thông tin phiếu thực thi lệnh sản xuất
                        join tt in _context.ThucThiLenhSanXuatModel on q.CustomerReference equals tt.TaskId
                        //LSX SAP
                        join ta in _context.TaskModel on tt.ParentTaskId equals ta.TaskId
                        //DSX
                        join ds in _context.TaskModel on ta.ParentTaskId equals ds.TaskId
                        //Nhà máy
                        join s in _context.StoreModel on q.SaleOrgCode equals s.SaleOrgCode
                        //Phân xưởng
                        join d in _context.WorkShopModel on q.WorkShopCode equals d.WorkShopCode
                        //Công đoạn lớn
                        join w in _context.WorkCenterModel on q.WorkCenterCode equals w.WorkCenterCode
                        //SO 100 => Lấy thông tin khách hàng
                        join so in _context.SaleOrderHeader100Model on q.VBELN equals so.VBELN
                        //Sản phẩm
                        join prTemp in _context.ProductModel on new { CompanyId = s.CompanyId, ERPProductCode = q.ProductCode } equals new { CompanyId = prTemp.CompanyId.Value, ERPProductCode = prTemp.ERPProductCode } into prList
                        from pr in prList
                            //Nhân viên QC
                        join aTemp in _context.AccountModel on q.QualityChecker equals aTemp.AccountId into aList
                        from a in aList.DefaultIfEmpty()
                            //Nhân viên tạo
                        join atTemp in _context.AccountModel on q.CreateBy equals atTemp.AccountId into atList
                        from at in atList.DefaultIfEmpty()
                            //Nhân viên chỉnh sửa cuối
                        join alTemp in _context.AccountModel on q.LastEditBy equals alTemp.AccountId into alList
                        from al in alList.DefaultIfEmpty()
                            //Kết quả tổng
                        join rTemp in _context.CatalogModel on new { CatalogTypeCode = ConstQualityControl.QualityControl_Result, CatalogCode = q.Result } equals new { CatalogTypeCode = rTemp.CatalogTypeCode, CatalogCode = rTemp.CatalogCode } into rList
                        from r in rList.DefaultIfEmpty()
                            //Loại kiểm tra
                        join qtTemp in _context.CatalogModel on new { CatalogTypeCode = ConstQualityControl.QualityControlType, CatalogCode = q.QualityType } equals new { CatalogTypeCode = qtTemp.CatalogTypeCode, CatalogCode = qtTemp.CatalogCode } into qtList
                        from qt in qtList.DefaultIfEmpty()
                        where q.QualityControlId == Id
                        select new QualityControlViewModel
                        {
                            QualityControlId = q.QualityControlId,
                            QualityControlCode = q.QualityControlCode,
                            //Nhà máy
                            SaleOrgCode = q.SaleOrgCode,
                            StoreName = s.SaleOrgCode + " | " + s.StoreName,
                            //Phân xưởng
                            WorkShopCode = q.WorkShopCode,
                            WorkShopName = d.WorkShopCode + " | " + d.WorkShopName,
                            //Công đoạn lớn
                            WorkCenterCode = q.WorkCenterCode,
                            WorkCenterName = w.WorkCenterCode + " | " + w.WorkCenterName,
                            //SO 100 => Khách hàng
                            ProfileCode = so.KUNNR,
                            ProfileName = so.KUNNR + " | " + so.SORTL,
                            //LSX
                            LSXDT = q.LSXDT,
                            LSXSAP = q.LSXSAP,
                            DSX = ds.Summary,
                            //Chi tiết
                            ProductAttribute = q.ProductAttribute,
                            //Sản phẩm
                            ProductCode = q.ProductCode,
                            ProductName = q.ProductCode + " | " + q.ProductName,
                            //TTLSX
                            CustomerReference = q.CustomerReference,
                            //Ngày xác nhận công đoạn lớn
                            ConfirmDate = q.ConfirmDate,
                            //Số lượng xác nhận
                            QuantityConfirm = q.QuantityConfirm,
                            //QC
                            QualityDate = q.QualityDate,
                            QualityChecker = q.QualityChecker,
                            QCSaleEmployee = a.EmployeeCode + " | " + a.FullName,
                            //Số lượng kiểm tra
                            InspectionLotQuantity = q.InspectionLotQuantity,
                            //Tình trạng môi trường
                            Environmental = q.Environmental,
                            //Trạng thái
                            Status = q.Status,
                            //Kết quả tổng
                            Result = r.CatalogCode,
                            //Loại kiểm tra
                            QualityType = qt.CatalogCode,
                            PO = q.PO,
                            Qty = tt.Qty
                        }).FirstOrDefault();
            if (data != null)
            {
                var routing = _context.RoutingInventorModel.Where(x => x.ITMNO == data.ProductAttribute && x.MATNR == data.ProductCode).FirstOrDefault();
                if (routing != null)
                {
                    data.Unit = routing.BMEIN;
                    data.Qty = routing.BMSCH * (data.Qty ?? 0);
                }

                #region GetDetail
                var detailViewModel = (from dt in _context.QualityControlDetailModel
                                           //Phương pháp kiểm tra
                                       join ppTemp in _context.CatalogModel on new { CatalogTypeCode = ConstQualityControl.QualityControl_Method, CatalogCode = dt.TestMethod } equals new { CatalogTypeCode = ppTemp.CatalogTypeCode, CatalogCode = ppTemp.CatalogCode } into ppList
                                       from pp in ppList.DefaultIfEmpty()
                                           //Mức độ lấy mẫu
                                       join slTemp in _context.CatalogModel on new { CatalogTypeCode = ConstQualityControl.SamplingLevel, CatalogCode = dt.SamplingLevel } equals new { CatalogTypeCode = slTemp.CatalogTypeCode, CatalogCode = slTemp.CatalogCode } into slList
                                       from sl in slList.DefaultIfEmpty()
                                           //Kết quả mẫu
                                       join dtrTemp in _context.CatalogModel on new { CatalogTypeCode = ConstQualityControl.QualityControl_Result, CatalogCode = dt.Result } equals new { CatalogTypeCode = dtrTemp.CatalogTypeCode, CatalogCode = dtrTemp.CatalogCode } into dtrList
                                       from dtr in dtrList.DefaultIfEmpty()
                                       where dt.QualityControlId == data.QualityControlId
                                       select new QualityControlDetailViewModel()
                                       {
                                           QualityControlDetailId = dt.QualityControlDetailId,
                                           QualityControlId = dt.QualityControlId,
                                           //Phương thức
                                           TestMethod = pp.CatalogCode,
                                           //Mức độ lấy mẫu
                                           SamplingLevel = sl != null ? sl.CatalogCode : "OTHER",
                                           SamplingLevelName = dt.SamplingLevel,
                                           //Mức chấp nhận
                                           AcceptableLevel = dt.AcceptableLevel,
                                           //Kết quả kiểm tra mẫu
                                           Result = dtr.CatalogCode,
                                           InspectionQuantity = dt.InspectionQuantity,
                                       }).FirstOrDefault();
                if (detailViewModel != null)
                {
                    data.QualityControlDetailViewModel = detailViewModel;
                }
                #endregion
                #region FileAttachment
                data.FileViewModel = GetFileList(data.QualityControlId);
                #endregion
                #region File error Attachment
                var errorList = (from p in _context.QualityControl_Error_Mapping
                                 join erTemp in _context.CatalogModel on new { CatalogTypeCode = ConstQualityControl.QualityControl_Error, CatalogCode = p.CatalogCode } equals new { CatalogTypeCode = erTemp.CatalogTypeCode, CatalogCode = erTemp.CatalogCode } into erList
                                 from er in erList.DefaultIfEmpty()
                                 where p.QualityControlId == data.QualityControlId
                                 select new QualityControlErrorViewModel()
                                 {
                                     QuanlityControl_Error_Id = p.QuanlityControl_Error_Id,
                                     QualityControlId = data.QualityControlId,
                                     CatalogCode = p.CatalogCode,
                                     CatalogText_vi = er.CatalogText_vi,
                                     LevelError = p.LevelError,
                                     QuantityError = p.QuantityError,
                                     Notes = p.Notes,
                                 }).ToList();
                var errorFileList = GetErrorFileList(errorList);
                foreach (var item in errorList)
                {
                    foreach (var x in errorFileList)
                    {
                        if (item.QuanlityControl_Error_Id == x.ObjectId)
                        {
                            if (item.ErrorFileViewModel == null)
                            {
                                item.ErrorFileViewModel = new List<FileAttachmentViewModel>();
                            }
                            item.ErrorFileViewModel.Add(x);
                        }
                    }
                }
                data.ErrorViewModel = errorList;
                #endregion
                #region QC Information
                var checkedList = (from p in _context.QualityControl_QCInformation_Mapping
                                   join w in _context.WorkCenterModel on p.WorkCenterCode equals w.WorkCenterCode
                                   join pp in _context.QualityControlInformationModel on p.QualityControlInformationId equals pp.Id
                                    where p.QualityControlId == data.QualityControlId
                                    select new QualityControlInformationMappingViewModel()
                                    {
                                        QualityControl_QCInformation_Id = p.QualityControl_QCInformation_Id,
                                        QualityControlId = data.QualityControlId,
                                        WorkCenterCode = p.WorkCenterCode,
                                        WorkCenterName = w.WorkCenterName,
                                        QualityControlInformationId = pp.Id,
                                        QualityControlInformationCode = pp.Code,
                                        QualityControlInformationName = pp.Name,
                                        Notes = p.Notes,
                                    }).ToList();
                var checkedFileList = GetCheckedFileList(checkedList);
                foreach (var item in checkedList)
                {
                    foreach (var x in checkedFileList)
                    {
                        if (item.QualityControl_QCInformation_Id == x.ObjectId)
                        {
                            if (item.CheckedFileViewModel == null)
                            {
                                item.CheckedFileViewModel = new List<FileAttachmentViewModel>();
                            }
                            item.CheckedFileViewModel.Add(x);
                        }
                    }
                }
                data.QualityControlInformationViewModel = checkedList;
                #endregion
            }
            return data;
        }
        /// <summary>
        /// Lấy danh sách file đính kèm của QC Error
        /// </summary>
        /// <param name="commentList"></param>
        /// <param name="TaskId"></param>
        /// <returns>list FileAttachmentViewModel</returns>
        public List<FileAttachmentViewModel> GetErrorFileList(List<QualityControlErrorViewModel> errorList)
        {
            List<FileAttachmentViewModel> errorFileList = new List<FileAttachmentViewModel>();
            if (errorList != null && errorList.Count > 0)
            {
                foreach (var item in errorList)
                {
                    var commentFiles = (from p in _context.FileAttachmentModel
                                        join m in _context.QualityControl_Error_File_Mapping on p.FileAttachmentId equals m.FileAttachmentId
                                        where m.QuanlityControl_Error_Id == item.QuanlityControl_Error_Id
                                        && p.ObjectId == item.QuanlityControl_Error_Id
                                        select new FileAttachmentViewModel
                                        {
                                            FileAttachmentId = p.FileAttachmentId,
                                            ObjectId = p.ObjectId,
                                            FileAttachmentCode = p.FileAttachmentCode,
                                            FileAttachmentName = p.FileAttachmentName,
                                            FileExtention = p.FileExtention,
                                            FileUrl = p.FileUrl,
                                            CreateTime = p.CreateTime
                                        }).ToList();
                        errorFileList.AddRange(commentFiles);
                }
            }
            errorFileList = errorFileList.OrderBy(p => p.CreateTime).ToList();
            return errorFileList;
        }


        /// <summary>
        /// Lấy danh sách file đính kèm của QC Information
        /// </summary>
        /// <param name="commentList"></param>
        /// <param name="TaskId"></param>
        /// <returns>list FileAttachmentViewModel</returns>
        public List<FileAttachmentViewModel> GetCheckedFileList(List<QualityControlInformationMappingViewModel> CheckedList)
        {
            List<FileAttachmentViewModel> checkedFileList = new List<FileAttachmentViewModel>();
            if (CheckedList != null && CheckedList.Count > 0)
            {
                foreach (var item in CheckedList)
                {
                    var Files = (from p in _context.FileAttachmentModel
                                        join m in _context.QualityControl_QCInformation_File_Mapping on p.FileAttachmentId equals m.FileAttachmentId
                                        where m.QualityControl_QCInformation_Id == item.QualityControl_QCInformation_Id
                                        && p.ObjectId == item.QualityControl_QCInformation_Id
                                        select new FileAttachmentViewModel
                                        {
                                            FileAttachmentId = p.FileAttachmentId,
                                            ObjectId = p.ObjectId,
                                            FileAttachmentCode = p.FileAttachmentCode,
                                            FileAttachmentName = p.FileAttachmentName,
                                            FileExtention = p.FileExtention,
                                            FileUrl = p.FileUrl,
                                            CreateTime = p.CreateTime
                                        }).ToList();
                    checkedFileList.AddRange(Files);
                }
            }
            checkedFileList = checkedFileList.OrderBy(p => p.CreateTime).ToList();
            return checkedFileList;
        }

        // <summary>
        /// Lấy danh sách file đính kèm của QC Error
        /// </summary>
        /// <param name="commentList"></param>
        /// <param name="TaskId"></param>
        /// <returns>list FileAttachmentViewModel</returns>
        public List<FileAttachmentViewModel> GetFileList(Guid? QualityControlId)
        {
            var Files = (from p in _context.FileAttachmentModel
                         join m in _context.QualityControl_FileAttachment_Mapping on p.FileAttachmentId equals m.FileAttachmentId
                         where m.QualityControlId == QualityControlId
                         && p.ObjectId == QualityControlId
                         select new FileAttachmentViewModel
                         {
                             FileAttachmentId = p.FileAttachmentId,
                             ObjectId = p.ObjectId,
                             FileAttachmentCode = p.FileAttachmentCode,
                             FileAttachmentName = p.FileAttachmentName,
                             FileExtention = p.FileExtention,
                             FileUrl = p.FileUrl,
                             CreateTime = p.CreateTime
                         }).ToList();
            Files = Files.OrderBy(p => p.CreateTime).ToList();
            return Files;
        }

        // <summary>
        /// Lấy danh sách file đính kèm của QC Error
        /// </summary>
        /// <param name="commentList"></param>
        /// <param name="TaskId"></param>
        /// <returns>list FileAttachmentViewModel</returns>
        public List<FileAttachmentViewModel> GetFileAndErrorList(List<QualityControlErrorViewModel> errorList,Guid? QualityControlId)
        {
            List<FileAttachmentViewModel> errorFileList = new List<FileAttachmentViewModel>();
            if (errorList != null && errorList.Count > 0)
            {
                foreach (var item in errorList)
                {
                    var commentFiles = (from p in _context.FileAttachmentModel
                                        join m in _context.QualityControl_Error_File_Mapping on p.FileAttachmentId equals m.FileAttachmentId
                                        where m.QuanlityControl_Error_Id == item.QuanlityControl_Error_Id
                                        && p.ObjectId == item.QuanlityControl_Error_Id
                                        select new FileAttachmentViewModel
                                        {
                                            FileAttachmentId = p.FileAttachmentId,
                                            ObjectId = p.ObjectId,
                                            FileAttachmentCode = p.FileAttachmentCode,
                                            FileAttachmentName = p.FileAttachmentName,
                                            FileExtention = p.FileExtention,
                                            FileUrl = p.FileUrl,
                                            CreateTime = p.CreateTime
                                        }).ToList();
                    errorFileList.AddRange(commentFiles);
                }
            }
            var Files = (from p in _context.FileAttachmentModel
                         join m in _context.QualityControl_FileAttachment_Mapping on p.FileAttachmentId equals m.FileAttachmentId
                         where m.QualityControlId == QualityControlId
                         && p.ObjectId == QualityControlId
                         select new FileAttachmentViewModel
                         {
                             FileAttachmentId = p.FileAttachmentId,
                             ObjectId = p.ObjectId,
                             FileAttachmentCode = p.FileAttachmentCode,
                             FileAttachmentName = p.FileAttachmentName,
                             FileExtention = p.FileExtention,
                             FileUrl = p.FileUrl,
                             CreateTime = p.CreateTime
                         }).ToList();
            errorFileList.AddRange(Files);
            errorFileList = errorFileList.OrderBy(p => p.CreateTime).ToList();
            return errorFileList;
        }


        public void CreateQualityControl(ConfirmWorkCenterViewModel confirmWorkCenterViewModel)
        {
            //Lấy thông tin lệnh thực thi
            var ttlxs = new ProductionManagementRepository(_context).GetExecutionTaskByTaskId(confirmWorkCenterViewModel.TaskId);

            if (ttlxs != null)
            {
                //Lấy tình trạng môi trường
                //Tình trạng môi trường (Lấy loại H022 Cột TEXT_ID) => LONGTEXT => Lấy TOP 1
                var Environmental = _context.SOTextHeader100Model.Where(x => x.SO == ttlxs.Property1 && x.TEXT_ID == "H022").Select(x => x.LONGTEXT).FirstOrDefault();

                //Lấy danh sách đã confirm
                var StockReceivingDetail = _context.StockReceivingDetailModel.Where(x => x.CustomerReference == ttlxs.TaskId && x.ProductId == ttlxs.ProductId && x.ProductAttributes == ttlxs.ProductAttributes && x.Phase == ttlxs.Phase && x.StockId == ttlxs.StepId && x.IsWorkCenterCompleted == true).FirstOrDefault();
                
                if (StockReceivingDetail != null)
                {
                    #region Department
                    string WorkShopCode = null;
                    var DepartmentId = StockReceivingDetail.DepartmentId;
                    if (DepartmentId != null)
                    {
                        var WorkShopId = _context.DepartmentModel.Where(x => x.DepartmentId == DepartmentId).FirstOrDefault().WorkShopId;
                        if (WorkShopId != null)
                        {
                            WorkShopCode = _context.WorkShopModel.Where(x => x.WorkShopId == WorkShopId).FirstOrDefault().WorkShopCode;
                        }
                    }

                    #endregion
                    #region Nhà máy
                    var SaleOrgCode = (from p in _context.TaskModel
                                       join c in _context.CompanyModel on p.CompanyId equals c.CompanyId
                                       where p.TaskId == ttlxs.ParentTaskId
                                       select c.CompanyCode).FirstOrDefault();
                    #endregion

                    #region Số lượng confirm
                    //decimal? QuanlityConfirm = 0;
                    //foreach (var item in StockReceivingDetail)
                    //{
                    //    QuanlityConfirm += item.Quantity;
                    //}
                    #endregion

                    var data = new QualityControlModel()
                    {
                        QualityControlId = Guid.NewGuid(),
                        //Nhà máy
                        SaleOrgCode = SaleOrgCode,
                        //Phân xưởng
                        WorkShopCode = WorkShopCode,
                        //Công đoạn lớn
                        WorkCenterCode = confirmWorkCenterViewModel.ConfirmWorkCenter,
                        //Thời gian confirm
                        ConfirmDate = confirmWorkCenterViewModel.WorkCenterConfirmTime,
                        //Nhân viên confirm
                        CreateBy = confirmWorkCenterViewModel.ConfirmBy,
                        CreateTime = DateTime.Now,
                        //SO100
                        VBELN = ttlxs.Property1,
                        //Tình trạng môi trường
                        Environmental = Environmental,
                        //LSX SAP
                        LSXSAP = ttlxs.ProductionOrder_SAP,
                        //LSX ĐT
                        LSXDT = ttlxs.ProductionOrder,
                        //Chi tiết
                        ProductAttribute = ttlxs.ProductAttributes,
                        //Sản phẩm
                        ProductCode = ttlxs.ProductCode,
                        ProductName = ttlxs.ProductName,
                        //Số lượng confirm
                        //QuantityConfirm = QuanlityConfirm,
                        CustomerReference = ttlxs.TaskId,
                        Status = false,
                    };

                    #region Kiểm tra đã confirm công đoạn + thẻ treo
                    var check = _context.QualityControlModel.Where(x => x.WorkCenterCode == data.WorkCenterCode && x.CustomerReference == data.CustomerReference).Count() > 0;
                    #endregion
                    if (!check)
                    {
                        _context.Entry(data).State = EntityState.Added;
                        _context.SaveChanges();
                    }
                }
            }
        }


        public QualityControlModel Edit(QualityControlViewModel viewModel)
        {
            QualityControlModel model = new QualityControlModel();
            model = _context.QualityControlModel.Find(viewModel.QualityControlId);
            if (model != null) {
                model.LastEditBy = viewModel.LastEditBy;
                model.LastEditTime = viewModel.LastEditTime;
                model.QualityDate = viewModel.QualityDate;
                model.QualityType = viewModel.QualityType;
                model.InspectionLotQuantity = viewModel.InspectionLotQuantity;
                model.QualityChecker = viewModel.QualityChecker;
                model.Result = viewModel.Result;
                model.Descriptions = viewModel.Descriptions;
                model.PO = viewModel.PO;
                model.Status = true;
            }
            _context.Entry(model).State = EntityState.Modified;

            if (viewModel.QualityControlDetailViewModel != null)
            {
                var detailViewModel = viewModel.QualityControlDetailViewModel;
                //foreach (var item in viewModel.QualityControlDetailViewModel)
                //{
                    var detailModel = _context.QualityControlDetailModel.Find(detailViewModel.QualityControlDetailId);
                    if (detailModel != null)
                    {
                        detailModel.AcceptableLevel = detailViewModel.AcceptableLevel;
                        detailModel.SamplingLevel = detailViewModel.SamplingLevel == "OTHER" ? detailViewModel.SamplingLevelName : detailViewModel.SamplingLevel;
                        detailModel.InspectionQuantity = detailViewModel.InspectionQuantity;   
                        detailModel.TestMethod = detailViewModel.TestMethod;
                        detailModel.Result = detailViewModel.Result;
                        _context.Entry(detailModel).State = EntityState.Modified;
                    }
                    else
                    {
                        detailModel = new QualityControlDetailModel();
                        detailModel.QualityControlDetailId = Guid.NewGuid();
                        detailModel.QualityControlId = model.QualityControlId;
                        detailModel.AcceptableLevel = detailViewModel.AcceptableLevel;
                        detailModel.SamplingLevel = detailViewModel.SamplingLevel == "OTHER" ? detailViewModel.SamplingLevelName : detailViewModel.SamplingLevel;
                        detailModel.InspectionQuantity = detailViewModel.InspectionQuantity;
                        detailModel.TestMethod = detailViewModel.TestMethod;
                        detailModel.Result = detailViewModel.Result;
                    _context.Entry(detailModel).State = EntityState.Added;

                    }
                //}
            }
            _context.SaveChanges();
            return model;
        }

        public bool DeleteError(Guid? Id, out List<string> urlfiles)
        {
            QualityControl_Error_Mapping model = new QualityControl_Error_Mapping();
            model = _context.QualityControl_Error_Mapping.Find(Id);
            urlfiles = new List<string>();
            if (model != null)
            {
                _context.Entry(model).State = EntityState.Deleted;
                var fileMapping = _context.QualityControl_Error_File_Mapping.Where(x=>x.QuanlityControl_Error_Id == model.QuanlityControl_Error_Id);
                foreach (var item in fileMapping)
                {
                    var file = _context.FileAttachmentModel.Where(x => x.ObjectId == item.QuanlityControl_Error_Id);
                    foreach (var x in file)
                    {
                        
                        urlfiles.Add(x.FileUrl);
                        _context.Entry(x).State = EntityState.Deleted;
                    }
                    _context.Entry(item).State = EntityState.Deleted;
                }
                _context.SaveChanges();
                
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DeleteInfo(Guid? Id, out List<string> urlfiles)
        {
            QualityControl_QCInformation_Mapping model = new QualityControl_QCInformation_Mapping();
            model = _context.QualityControl_QCInformation_Mapping.Find(Id);
            urlfiles = new List<string>();
            if (model != null)
            {
                _context.Entry(model).State = EntityState.Deleted;
                var fileMapping = _context.QualityControl_QCInformation_Mapping.Where(x=>x.QualityControl_QCInformation_Id == model.QualityControl_QCInformation_Id);
                foreach (var item in fileMapping)
                {
                    var file = _context.FileAttachmentModel.Where(x => x.ObjectId == item.QualityControl_QCInformation_Id);
                    foreach (var x in file)
                    {
                        
                        urlfiles.Add(x.FileUrl);
                        _context.Entry(x).State = EntityState.Deleted;
                    }
                    _context.Entry(item).State = EntityState.Deleted;
                }
                _context.SaveChanges();
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DeleteFileInServer(string root, string item)
        {
            if (File.Exists(Path.Combine(root, item)))
            {
                // If file found, delete it    
                File.Delete(Path.Combine(root, item));
            }
        }
    }
}
