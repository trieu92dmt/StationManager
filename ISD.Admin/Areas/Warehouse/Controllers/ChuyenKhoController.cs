using ISD.Extensions;
using ISD.Resources;
using ISD.ViewModels;
using ISD.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;


namespace Warehouse.Controllers
{
    public class ChuyenKhoController : BaseController
    {
        // GET: ChuyenKho
        public ActionResult Index()
        {
            ViewBag.PageId = GetPageId("/Warehouse/ChuyenKho");
            CreateViewBagForSearch();
            return View();
        }
        public ActionResult View(Guid id)
        {
            //Lấy phiếu chuyển kho bằng mã id
            var stockReceiving = _unitOfWork.TransferRepository.GetBy(id);
            //Nếu không tìm thấy thì đưa ra trang lỗi 
            if (stockReceiving == null)
            {
                return RedirectToAction("ErrorText", "Error", new { area = "", statusCode = 404, exception = string.Format(LanguageResource.Error_NotExist, LanguageResource.StockReceive_Bill.ToLower()) });
            }
            ViewBag.ListTransferDetail = _unitOfWork.TransferDetailRepository.GetBy(id);
            ViewBag.ViewType = "Transfer";
            return View(stockReceiving);
        }
        #region Thêm chuyển kho
        [ISDAuthorizationAttribute]
        public ActionResult Create()
        {
            //Gán công ty người dùng đang đăng nhập 
            var companyId = CurrentUser.CompanyId;
            //Chi nhánh nhà máy
            var storeId = _unitOfWork.StoreRepository.GetStoreIdBySaleOrgCode(CurrentUser.SaleOrg);
            //Mã nhân viên của nhân viên đang đăng nhập 
            var employeeCode = CurrentUser.EmployeeCode;
            CreateViewBag(companyId, storeId, employeeCode);
            //Từ khi thêm 2 dòng này thì không còn lỗi khi gửi anh Nam
            ViewBag.StockList = _unitOfWork.StockRepository.GetAll();
            ViewBag.ViewType = "Transfer";
            ViewBag.CreateBy = _unitOfWork.AccountRepository.GetUserNameBy(CurrentUser.AccountId);

            return View();
        }
        [HttpPost]
        public JsonResult Create(TransferViewModel viewModel, List<TransferDetailViewModel> transferDetailList)
        {
            return ExecuteContainer(() =>
            {
                //Tạo ra mã chuyển kho
                viewModel.TransferId = Guid.NewGuid();
                //Lấy tài khoản đang đăng nhập là người tạo chuyển kho
                viewModel.CreateBy = CurrentUser.AccountId;
                //Lấy ngày giờ hiện tại là ngày tạo chuyển kho
                viewModel.CreateTime = DateTime.Now;
                _unitOfWork.TransferRepository.Create(viewModel);
                //Convert datetime sang kiểu int
                var dateKey = _unitOfWork.UtilitiesRepository.ConvertDateTimeToInt(viewModel.DocumentDate);
                //Nếu danh sách chi tiết chuyển kho không rỗng
                if (transferDetailList != null && transferDetailList.Count > 0)
                {
                    //Group lại các kho xuât, và tính tổng các sản phẩm được xuất
                    var dataForCheckStock = from p in transferDetailList
                                            group p by new { p.FromStockId, p.ProductId } into tmpList
                                            select new
                                            {
                                                //Groupby Kho xuất
                                                tmpList.Key.FromStockId,
                                                //Groupby Sản phẩm
                                                tmpList.Key.ProductId,
                                                //Tính tổng số lượng 
                                                Sum = tmpList.Sum(p => p.Quantity),
                                            };
                    //Check tồn kho
                    foreach (var item in dataForCheckStock)
                    {
                        //Lấy thông tin kho xuất từ hàm gộp trên
                        var stockId = (Guid)item.FromStockId;
                        //Lấy thông tin sản phẩm từ hàm gộp trên
                        var productId = (Guid)item.ProductId;
                        var quantyOnHand = _unitOfWork.StockRepository.GetStockOnHandBy(stockId, productId).Qty;
                        //Nếu số lượng tổng nhiều hơn số lương tồn
                        if (item.Sum > quantyOnHand)
                        {
                            return Json(new
                            {
                                Code = HttpStatusCode.NotModified,
                                Success = false,
                                Data = "Đã xảy ra lỗi: Số lượng chuyển không thể lớn hơn số lượng tồn kho!"
                            });
                        }
                    }
                    //Thêm mới dữ liệu
                    foreach (var transferDetailVM in transferDetailList)
                    {
                        //Thêm mới detail
                        transferDetailVM.TransferDetailId = Guid.NewGuid();
                        transferDetailVM.TransferId = viewModel.TransferId;
                        transferDetailVM.DateKey = dateKey;
                        _unitOfWork.TransferDetailRepository.Create(transferDetailVM);
                    }
                }
                //Nếu chi tiết danh sách chuyển kho rỗng
                else
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = string.Format("Vui lòng nhập ít nhất 1 dòng dữ liệu sản phẩm để thực hiện chuyển kho!")
                    });
                }
                //Nếu thành công
                _context.SaveChanges();
                return Json(new
                {
                    Code = HttpStatusCode.Created,
                    Success = true,
                    Data = string.Format(LanguageResource.Alert_Create_Success, LanguageResource.TransferMaterial.ToLower()),
                    Id = viewModel.TransferId,
                });



            });

        }
        #endregion
        [HttpPost]
        [ISDAuthorization]
        public ActionResult Delete(Guid? id,string DeletedReason)
        {
            return ExecuteDelete(() =>
            {
                //Nếu khách hàng chưa nhập lý do hủy
                if (string.IsNullOrEmpty(DeletedReason))
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Vui lòng nhập lý do hủy!"
                    });
                }
                //Tìm phiếu chuyển kho
                var TransSearch = _context.TransferModel.FirstOrDefault(p => p.TransferId == id);
                //Nếu không tìm thấy phiếu chuyển kho
                if (TransSearch == null)
                {
                    return Json(new
                    {
                        Code = HttpStatusCode.NotFound,
                        Sucess = false,
                        Data = "Không tìm thấy phiếu chuyển kho để hủy"

                    });
                }
                TransSearch.DeletedBy = CurrentUser.AccountId;
                TransSearch.DeletedTime = DateTime.Now;
                TransSearch.DeletedReason = DeletedReason;
                TransSearch.isDeleted = true;
                //Không hiểu ý nghĩa của dòng này
                _context.Entry(TransSearch).State = EntityState.Modified;
                //Tìm chi tiết phiếu chuyển kho
                var ListTransDetailSearch = _context.TransferDetailModel.Where(p => p.TransferDetailId == id).ToList();
                //Nếu tìm thấy được phiếu chuyển kho
                if(ListTransDetailSearch!=null&&ListTransDetailSearch.Count>0)
                {
                    foreach (var transDetail in ListTransDetailSearch)
                    {
                        transDetail.isDeleted = true;
                        _context.Entry(transDetail).State = EntityState.Modified;
                    }
                }
                _context.SaveChanges();
                return Json(new { 
                    Code=HttpStatusCode.Created,
                    Success=true,
                    Data= string.Format(LanguageResource.Alert_Delete_Success, LanguageResource.TransferMaterial.ToLower())
                });
            });
        }
        public ActionResult _Search(TransferSearchViewModel searchViewModel)
        {
            var listTransSearch = _unitOfWork.TransferRepository.Search(searchViewModel);
            return PartialView(listTransSearch);
        }
        #region ViewBag,Helper
        //Hàm ViewBag lấy các dropdownlist cho trang thêm mới chuyển kho
        private void CreateViewBag(Guid? CompanyId = null, Guid? StoreId = null, string SaleEmployeeCode = "")
        {
            //Dropdown Company
            var companyList = _unitOfWork.CompanyRepository.GetAll();
            ViewBag.CompanyId = new SelectList(companyList, "CompanyId", "CompanyName", CompanyId);

            //Dropdown Store
            var storeList = _unitOfWork.StoreRepository.GetStoreByCompany(CompanyId);
            ViewBag.StoreId = new SelectList(storeList, "StoreId", "StoreName", StoreId);


            //Dropdown Nhân viên
            var saleEmployeeList = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlistWithoutAccount();
            ViewBag.SalesEmployeeCode = new SelectList(saleEmployeeList, "SalesEmployeeCode", "SalesEmployeeName", SaleEmployeeCode);
            //Dropdown Stock
            var listStockFrom = _unitOfWork.StockRepository.GetBy("KHOKHUON");
            ViewBag.FromStockId = new SelectList(listStockFrom, "StockId", "StockName");
            var listStockTo = _unitOfWork.StockRepository.GetBy("KHOKHUON");
            ViewBag.ToStockId = new SelectList(listStockTo, "StockId", "StockName");

            //Dropdown Product
            //ViewBag.ProductList = _productRepository.GetProInventory();
        }
        //Hàm ViewBag lấy các DropdownList cho các mục ở trang tìm kiếm chuyển kho
        public void CreateViewBagForSearch()
        {
            //Dropdown Công ty (Company)
            var listCompany = _unitOfWork.CompanyRepository.GetAll();
            ViewBag.SearchCompanyId = new SelectList(listCompany, "CompanyId","CompanyName");

            //Dropdown nhà máy(Store)
            var listStore = _unitOfWork.StoreRepository.GetAllStore();
            ViewBag.SearchStoreId = new SelectList(listStore, "StoreId", "StoreName");

            //Dropdown nhân viên (người yêu cầu)
            var listEmployee = _unitOfWork.SalesEmployeeRepository.GetAllForDropdownlistWithoutAccount();
            ViewBag.SearchSalesEmployeeCode = new SelectList(listEmployee, "SalesEmployeeCode", "SalesEmployeeName");

            //Dropdown Kho (Cửa hàng)
            var listStock = _unitOfWork.StockRepository.GetBy("KHOKHUON");
            //Tìm bằng kho xuất
            ViewBag.SearchFromStockId = new SelectList(listStock, "StockId", "StockName");
            //Tìm bằng kho nhập
            ViewBag.SearchToStockId = new SelectList(listStock, "StockId", "StockName");

        }
        #endregion
        public ActionResult GetProductTon (Guid StockId,Guid ProductId)
        {
            var quantity = _unitOfWork.StockRepository.GetStockOnHandBy((Guid)StockId, ProductId);
            return Json(quantity, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InsertProductStock(TransferDetailViewModel model, List<TransferDetailViewModel> transferDetailList)
        {
            if (transferDetailList == null)
            {
                transferDetailList = new List<TransferDetailViewModel>();
            }
            var exist = transferDetailList.Where(p => p.ProductId == model.ProductId).FirstOrDefault();
            #region 
            //StockCode, StockName(Xuat va nhap kho)
            var stock = _context.StockModel.FirstOrDefault(p => p.StockId == model.FromStockId);
            if (stock != null)
            {
                model.FromStockCode = stock.StockCode;
                model.FromStockName = stock.StockName;
            }
            var stock1 = _context.StockModel.FirstOrDefault(p => p.StockId == model.ToStockId);
            if (stock1 != null)
            {
                model.ToStockCode = stock1.StockCode;
                model.ToStockName = stock1.StockName;
            }
            #endregion
            if (exist != null)
            {
                exist.Quantity += model.Quantity;
            }
            else
            {
                exist = new TransferDetailViewModel();
                exist.Quantity = model.Quantity;
            }
            if (exist.Quantity > model.QuantinyOnHand || model.QuantinyOnHand == null)
            {
                return Json(new { Message = "Vui lòng nhập số lưọng chuyển kho không vượt quá số lượng tồn!" }, JsonRequestBehavior.AllowGet);
            }
            exist.Quantity -= model.Quantity;
            if (transferDetailList.FirstOrDefault(p => p.ProductId == model.ProductId) == null)
            {
                //Add thêm data
                transferDetailList.Add(model);
            }

            return PartialView("ChuyenKhoDataTableView", transferDetailList);
        }
        public ActionResult RemoveProductStock(List<TransferDetailViewModel> transferDetailList, int STT)
        {
            transferDetailList = transferDetailList.Where(p => p.STT != STT).ToList();
            return PartialView("ChuyenKhoDataTableView", transferDetailList);
        }
    }
}