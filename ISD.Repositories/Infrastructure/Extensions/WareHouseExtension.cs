using ISD.EntityModels;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.Infrastructure.Extensions
{
    public static class WareHouseExtension
    {
        /// <summary>
        /// Map Stock Receiving Master từ View Model qua Entity Model
        /// </summary>
        /// <param name="model">Stock Receiving Master Model</param>
        /// <param name="viewModel">Stock Receiving View Model</param>
        public static void MapStockReceivingMaster(this StockReceivingMasterModel model, StockReceivingViewModel viewModel)
        {
            //1. GUID
            model.StockReceivingId = viewModel.StockReceivingId;
            //2. Mã phiếu nhập kho
            model.StockReceivingCode = viewModel.StockReceivingCode;
            //3. Ngày chứng từ
            model.DocumentDate = viewModel.DocumentDate;
            //4. Công ty
            model.CompanyId = viewModel.CompanyId;
            //5. Chi nhánh
            model.StoreId = viewModel.StoreId;
            //6. Nhân viên
            model.SalesEmployeeCode = viewModel.SalesEmployeeCode;
            //7. Nhà cung cấp
            model.ProfileId = viewModel.ProfileId;
            //8. Ghi chú
            model.Note = viewModel.Note;
        }
        public static void MapStockReceivingMaster(this StockReceivingMasterModel model, StockReceivingMasterViewModel viewModel)
        {
            //1. GUID
            model.StockReceivingId = viewModel.StockReceivingId;
            //2. Mã phiếu nhập kho
            model.StockReceivingCode = viewModel.StockReceivingCode;
            //3. Ngày chứng từ
            model.DocumentDate = viewModel.DocumentDate;
            //4. Công ty
            model.CompanyId = viewModel.CompanyId;
            //5. Chi nhánh
            model.StoreId = viewModel.StoreId;
            //6. Nhân viên
            model.SalesEmployeeCode = viewModel.SalesEmployeeCode;
            //7. Nhà cung cấp
            model.ProfileId = viewModel.ProfileId;
            //8. Ghi chú
            model.Note = viewModel.Note;
        }
        /// <summary>
        /// Ánh xạ Stock Receiving từ ViewModel qua EntityModel
        /// </summary>
        /// <param name="viewModel">Stock Receving Detail View Model</param>
        /// <param name="model">Stock Receving Detail Model</param>
        public static void MapStockRecevingDetail(this StockReceivingDetailModel model, StockReceivingDetailViewModel viewModel)
        {
            //1. Mã chi tiết
            model.StockReceivingDetailId = viewModel.StockReceivingDetailId;
            //2. Mã phiếu nhập kho
            model.StockReceivingId = viewModel.StockReceivingId;
            //3. Sản phẩm
            model.ProductId = viewModel.ProductId;
            model.ProductAttributes = viewModel.ProductAttributes;
            //4. Kho
            model.StockId = viewModel.StockId;
            //5. Ngày chứng từ
            model.DateKey = viewModel.DateKey;
            //6. Số lượng
            model.Quantity = viewModel.Quantity;
            //7. Giá
            model.Price = viewModel.Price;
            //8. Thành tiền
            model.UnitPrice = viewModel.UnitPrice;
            //9. Ghi chú
            model.Note = viewModel.DetailNote;
            //10. Custom
            model.CustomerReference = viewModel.CustomerReference;
            //11. Type
            model.StockRecevingType = viewModel.StockRecevingType;
            //12. Từ ngày
            model.FromTime = viewModel.FromTime;
            //13. Đến ngày
            model.ToTime = viewModel.ToTime;
            //14. Datekey
            model.DateKey = viewModel.DateKey;
            //15. Thời gian tạo
            model.CreateTime = viewModel.CreateTime;
            //16. Người tạo
            model.CreateBy = viewModel.CreateBy;
            //17. Tổ
            model.DepartmentId = viewModel.DepartmentId;
            //18. Lần đi qua công đoạn
            model.Phase = viewModel.Phase;
            //19. MovementType
            model.MovementType = viewModel.MovementType;

        }

        public static void MapTransfer(this TransferModel model, TransferViewModel viewModel)
        {
            model.TransferId = viewModel.TransferId;
            model.TransferCode = viewModel.TransferCode;
            model.DocumentDate = viewModel.DocumentDate;
            model.CompanyId = viewModel.CompanyId;
            model.StoreId = viewModel.StoreId;
            model.SalesEmployeeCode = viewModel.SalesEmployeeCode;
            model.Note = viewModel.Note;
            //Thông tin người gửi
            model.SenderName = viewModel.SenderName;
            model.SenderPhone = viewModel.SenderPhone;
            model.SenderAddress = viewModel.SenderAddress;
            //Thông tin người nhận
            model.RecipientName = viewModel.RecipientName;
            model.RecipientPhone = viewModel.RecipientPhone;
            model.RecipientAddress = viewModel.RecipientAddress;
            model.RecipientCompany = viewModel.RecipientCompany;
        }

        public static void MapTranferDetail(this TransferDetailModel model, TransferDetailViewModel viewModel)
        {
            //1. TransferDetailId
            model.TransferDetailId = viewModel.TransferDetailId;
            //2. TransferId
            model.TransferId = viewModel.TransferId;
            //3. Sản phẩm
            model.ProductId = viewModel.ProductId;
            //4. Từ công đoạn
            model.FromStockId = viewModel.FromStockId;
            //5. Đến công đoạn
            model.ToStockId = viewModel.ToStockId;
            //6. Datekey
            model.DateKey = viewModel.DateKey;
            //7. Số lượng
            model.Quantity = viewModel.Quantity;
            //8. Giá
            model.Price = viewModel.Price;
            //9. Đơn giá
            model.UnitPrice = viewModel.UnitPrice;
            //10. Ghi chú
            model.Note = viewModel.DetailNote;
            //11. Từ ngày
            model.FromTime = viewModel.FromTime;
            //12. Đến ngày
            model.ToTime = viewModel.ToTime;
            //13. Sản phẩm
            model.ProductId = viewModel.ProductId;
            //14. Chi tiết
            model.ProductAttributes = viewModel.ProductAttributes;
            //15. Số lượng
            model.Quantity = viewModel.Quantity;
            //16. Trạng thái
            model.StockRecevingType = viewModel.StockRecevingType;
            //17. Từ TTLSX 
            model.FromCustomerReference = viewModel.FromCustomerReference;
            //18. Đến TTLSX
            model.CustomerReference = viewModel.CustomerReference;
            //19. Ngày tạo
            model.CreateTime = viewModel.CreateTime;
            //20. Người tạo
            model.CreateBy = viewModel.CreateBy;
            //21. Lần ghi nhận công đoạn
            model.Phase = viewModel.Phase;
            //22. MovementType
            model.MovementType = viewModel.MovementType;

        }

        public static void MapDelivery(this DeliveryModel model, DeliveryViewModel viewModel)
        {
            model.DeliveryId = viewModel.DeliveryId;
            model.DeliveryCode = viewModel.DeliveryCode;
            model.DocumentDate = viewModel.DocumentDate;
            model.CompanyId = viewModel.CompanyId;
            model.StoreId = viewModel.StoreId;
            model.SalesEmployeeCode = viewModel.SalesEmployeeCode;
            model.ProfileId = viewModel.ProfileId;
            model.Note = viewModel.Note;

            model.SenderName = viewModel.SenderName;
            model.SenderAddress = viewModel.SenderAddress;
            model.SenderPhone = viewModel.SenderPhone;
            model.RecipientName = viewModel.RecipientName;
            model.RecipientAddress = viewModel.RecipientAddress;
            model.RecipientPhone = viewModel.RecipientPhone;
            model.RecipientCompany = viewModel.RecipientCompany;
            model.ShippingTypeCode = viewModel.ShippingTypeCode;

            model.TaskId = viewModel.TaskId;
            model.DeliveryType = viewModel.DeliveryType;
        }

        public static void MapDeliveryDetail(this DeliveryDetailModel model, DeliveryDetailViewModel viewModel)
        {
            model.DeliveryDetailId = viewModel.DeliveryDetailId;
            model.DeliveryId = viewModel.DeliveryId;
            model.StockId = viewModel.StockId;
            model.ProductId = viewModel.ProductId;
            model.DateKey = viewModel.DateKey;
            model.Quantity = viewModel.Quantity;
            model.Price = viewModel.Price;
            model.UnitPrice = viewModel.UnitPrice;
            model.Note = viewModel.DetailNote;
        }

    }
}

