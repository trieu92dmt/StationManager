using ISD.EntityModels;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using ISD.ViewModels.Work;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class ProductionOrderRepository
    {
        private EntityDataContext _context;
        public ProductionOrderRepository(EntityDataContext context)
        {
            _context = context;
        }
        #region Tìm kiếm
        public List<ProductionOrderViewModel> Search(ProductionOrderViewModel searchModel)
        {
            object[] SqlParams =
            {
                new SqlParameter("@ZZLSX",searchModel.ZZLSX??(object)DBNull.Value),
                new SqlParameter("@SaleOrg",searchModel.SaleOrg??(object)DBNull.Value),
                new SqlParameter("@KTDK",searchModel.KTDK??(object)DBNull.Value),
                new SqlParameter("@KTDC",searchModel.KTDC??(object)DBNull.Value),
            };
            var res = _context.Database.SqlQuery<ProductionOrderViewModel>("[MES].[GetProductionOrder] @ZZLSX,@SaleOrg,@KTDK,@KTDC", SqlParams).ToList();
            return res;
        }

        #endregion
        #region Get By ZZLSX ( Lệnh sản xuất đại trà )
        public ProductionOrderViewModel GetByProductionOrder(string ZZLSX)
        {
            var listTask = (from p in _context.ProductionOrderModel
                            where p.ZZLSX == ZZLSX
                            select new ProductionOrderViewModel
                            {
                                ZZLSX = p.ZZLSX,
                                GSTRS = p.GSTRS,
                                GLTRS = p.GLTRS,
                                QuantityPrinted = 100,
                                TimesIssued = 2,
                            }).FirstOrDefault();

            return listTask;
        }

        #endregion

        #region Export excel
        public List<LSXDTExportViewModel> ExportData(ProductionOrderViewModel searchModel)
        {
            object[] SqlParams =
            {
                new SqlParameter("@ZZLSX",searchModel.ZZLSX??(object)DBNull.Value),
                new SqlParameter("@SaleOrg",searchModel.SaleOrg??(object)DBNull.Value),
                new SqlParameter("@KTDK",searchModel.KTDK??(object)DBNull.Value),
                new SqlParameter("@KTDC",searchModel.KTDC??(object)DBNull.Value),
            };
            var res = _context.Database.SqlQuery<LSXDTExportViewModel>("[MES].[GetProductionOrder] @ZZLSX,@SaleOrg,@KTDK,@KTDC", SqlParams).ToList();
            return res;
        }
        #endregion

    }
}
