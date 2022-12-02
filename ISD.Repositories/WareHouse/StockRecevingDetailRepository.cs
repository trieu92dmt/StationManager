using ISD.EntityModels;
using ISD.Repositories.Infrastructure.Extensions;
using ISD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class StockRecevingDetailRepository
    {
        private EntityDataContext _context;

        /// <summary>
        /// Khởi tạo Repository
        /// </summary>
        /// <param name="dataContext">EntityDataContext</param>
        public StockRecevingDetailRepository(EntityDataContext dataContext)
        {
            _context = dataContext;
        }

        /// <summary>
        /// Thêm mới một Stock Receving Detail
        /// </summary>
        /// <param name="viewModel">Stock Receving Detail View Model</param>
        /// <returns>Stock Receving Detail Model</returns>
        public StockReceivingDetailModel Create(StockReceivingDetailViewModel viewModel)
        {
            var stockReDetailNew = new StockReceivingDetailModel();
            stockReDetailNew.MapStockRecevingDetail(viewModel);

            _context.Entry(stockReDetailNew).State = EntityState.Added;
            return stockReDetailNew;
        }

        public List<StockReceivingDetailViewModel> GetByStockReceiveMaster(Guid stockRecevingId)
        {
            var result = (from p in _context.StockReceivingDetailModel
                          where p.StockReceivingId == stockRecevingId
                          orderby p.ProductModel.ProductName
                          select new StockReceivingDetailViewModel
                          {
                              StockReceivingDetailId = p.StockReceivingDetailId,
                              ProductCode = p.ProductModel.ProductCode,
                              ProductName = p.ProductModel.ProductName,
                              Serial=p.ProductModel.Serial,
                              StockCode = p.StockModel.StockCode,
                              StockName = p.StockModel.StockName,
                              DateKey = p.DateKey,
                              Quantity = p.Quantity,
                              Price = p.Price,
                              DetailNote = p.Note
                          }).ToList();
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="ProductAttribute"></param>
        /// <param name="StockId"></param>
        /// <param name="TaskId"></param>
        /// <returns></returns>
        public StockReceivingDetailModel CheckExist(Guid? ProductId, Guid StockId, Guid? CustomerReference, string ProductAttributes = null, string StockRecevingType = null)
        {
            
            StockReceivingDetailModel model = null;

            // Kiểm tra Có tồn tại hay không
            var result = (from p in _context.StockReceivingDetailModel
                          where p.ProductId == ProductId
                          && (ProductAttributes == null || p.ProductAttributes == ProductAttributes)
                          && p.StockId == StockId
                          && p.CustomerReference == CustomerReference
                          && (StockRecevingType == null || p.StockRecevingType == StockRecevingType)
                          orderby p.ProductModel.ProductName
                          select new StockReceivingDetailViewModel
                          {
                              StockReceivingId = p.StockReceivingId,
                              StockReceivingDetailId = p.StockReceivingDetailId,
                              ProductCode = p.ProductModel.ProductCode,
                              ProductName = p.ProductModel.ProductName,
                              StockId = p.StockId,
                              ProductAttributes = p.ProductAttributes,
                              CustomerReference = p.CustomerReference,
                              StockCode = p.StockModel.StockCode,
                              StockName = p.StockModel.StockName,
                              ProductId = p.ProductId,
                              DateKey = p.DateKey,
                              Quantity = p.Quantity,
                              Price = p.Price,
                              DetailNote = p.Note,
                              StockRecevingType = p.StockRecevingType
                          }).FirstOrDefault();
            if (result != null)
            {
                model = _context.StockReceivingDetailModel.Where(x => x.StockReceivingDetailId == result.StockReceivingDetailId).FirstOrDefault();
                model.MapStockRecevingDetail(result);   
            }
            return model;
        }
    }
}
