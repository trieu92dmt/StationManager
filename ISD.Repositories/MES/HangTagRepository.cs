using ISD.EntityModels;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using ISD.ViewModels.Work;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories
{
    public class HangtagRepository
    {
        private EntityDataContext _context;
        public HangtagRepository(EntityDataContext context)
        {
            _context = context;
        }

        #region Get By ProductionOrder ( Lệnh sản xuất đại trà )
        /// <summary>
        /// Lấy thông tin HangTag
        /// </summary>
        /// <param name="TaskId">TaskId SAP</param>
        /// <param name="LSXSAP">Mã SAP</param>
        /// <returns>HangTag View Model</returns>
        public HangTagViewModel GetHangTagByTTLSX(Guid TaskId, string LSXSAP)
        {

            var listTask = (from p in _context.TaskModel
                                //đợt sx
                            join t in _context.TaskModel on p.ParentTaskId equals t.TaskId
                            join w in _context.WorkFlowModel on p.WorkFlowId equals w.WorkFlowId
                            join pro in _context.ProductModel on p.ProductId equals pro.ProductId
                            join c in _context.CompanyModel on pro.CompanyId equals c.CompanyId
                            join sTemp in _context.SaleOrderItem100Model on new { a= p.Property1, b =p.Property2 } equals new {a = sTemp.VBELN,b= sTemp.POSNR } into sList
                            from s in sList.DefaultIfEmpty()
                            join spTemp in _context.ProductModel on s.UPMAT equals spTemp.ERPProductCode into spList
                            from sp in spList.DefaultIfEmpty()
                            where p.Summary == LSXSAP && p.TaskId == TaskId && w.WorkFlowCode == "LSXC"
                            select new HangTagViewModel
                            {
                                LSXDT = p.Property5,
                                LSXD = t.Summary,
                                MassProductionOrder = p.Summary,
                                ProductCode = pro.ERPProductCode,
                                ProductName = pro.ProductName,
                                Qty = p.Qty,
                                Number2 = p.Number2,
                                CustomerReference = TaskId,
                                BatchPrinting = 1,
                                QuantityPrinted = 0,
                                CompanyCode = c.CompanyCode,
                                ProductSOName = sp.ProductName,
                                ProductSOCode = sp.ERPProductCode
                            }).FirstOrDefault();
            if (listTask != null)
            {
                var data = (from h in _context.HangTagModel
                            where h.CustomerReference == listTask.CustomerReference
                            select new { 
                                BatchPrinting = h.BatchPrinting, 
                                EffectiveDate = h.EffectiveDate,                       
                            }).OrderByDescending(x=>x.BatchPrinting).ToList();
                if (data.Count() > 0)
                {
                    listTask.BatchPrinting = data.FirstOrDefault().BatchPrinting + 1;
                    listTask.QuantityPrinted = data.Count();
                }
               
            }
            return listTask;

        }
        
        public void Create(HangTagViewModel viewModel)
        {
            for (int i = 0; i < viewModel.QuantityPrintMore; i++)
            {
                HangTagModel model = new HangTagModel()
                {
                    HangTagId = Guid.NewGuid(),
                    BatchPrinting = viewModel.BatchPrinting,
                    EffectiveDate = DateTime.Now,
                    MassProductionOrder = viewModel.MassProductionOrder,
                    QRCode = "",
                    CreatedTime = DateTime.Now,
                    CreatedUser = viewModel.CreatedUser,
                    CustomerReference = viewModel.CustomerReference,
                    ProductAttribute = viewModel.ChiTiet,
                };
                // Add hangtag
                _context.HangTagModel.Add(model);

            }
            _context.SaveChanges();

        }

        #endregion

        #region Lấy thông tin LSX SAP bằng Id
        public HangTagViewModel GetLSXSAPInfo(Guid TaskId)
        {

            var LSXSAP = (from p in _context.TaskModel
                                //đợt sx
                            join t in _context.TaskModel on p.ParentTaskId equals t.TaskId
                            join w in _context.WorkFlowModel on p.WorkFlowId equals w.WorkFlowId
                            join pro in _context.ProductModel on p.ProductId equals pro.ProductId
                            join c in _context.CompanyModel on pro.CompanyId equals c.CompanyId
                            //join sTemp in _context.SaleOrderItem100Model on new { a = p.Property1, b = p.Property2 } equals new { a = sTemp.VBELN, b = sTemp.POSNR } into sList
                            //from s in sList.DefaultIfEmpty()
                            //join spTemp in _context.ProductModel on s.UPMAT equals spTemp.ERPProductCode into spList
                            //from sp in spList.DefaultIfEmpty()
                            where p.TaskId == TaskId && w.WorkFlowCode == "LSXC"
                            select new HangTagViewModel
                            {
                                //LSXDT = p.Property5,
                                //LSXD = t.Summary,
                                //MassProductionOrder = p.Summary,
                                //ProductCode = pro.ERPProductCode,
                                //ProductName = pro.ProductName,
                                Qty = p.Qty,
                                Number2 = p.Number2,
                                //CustomerReference = TaskId,
                                //BatchPrinting = 1,
                                //QuantityPrinted = 0,
                                //CompanyCode = c.CompanyCode,
                                //ProductSOName = sp.ProductName,
                                //ProductSOCode = sp.ERPProductCode
                            }).FirstOrDefault();
           
            return LSXSAP;

        }
        #endregion
    }
}
