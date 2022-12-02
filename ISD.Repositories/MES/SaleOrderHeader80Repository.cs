using ISD.EntityModels;
using ISD.ViewModels;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.MES
{

    public class SaleOrderHeader80Repository
    {
        EntityDataContext _context;

        public SaleOrderHeader80Repository(EntityDataContext entityDataContext)
        {
            _context = entityDataContext;
        }

        public IQueryable<SaleOrderHeader80ViewModel> Search(SaleOrderHeader80ViewModel searchModel)
        {
            var data = (
                        //SO header 80
                        from s in _context.SaleOrderHeader80Model
                        //Đối với SO 80 chỉ hiển thị những SO có item
                        join dTemp in _context.SaleOrderItem80Model on s.VBELN equals dTemp.VBELN into dList
                        from d in dList.DefaultIfEmpty()
                            //Tìm theo Sale Order
                        where d.WERKS == searchModel.Plant
                        && (searchModel.VBELN == null || s.VBELN.Contains(searchModel.VBELN))
                        //Tìm theo Requirement type
                        && (searchModel.BEDAE == null || s.BEDAE.Contains(searchModel.BEDAE))
                        //Tìm theo Sale Document type
                        && (searchModel.AUART == null || s.AUART.Contains(searchModel.AUART))
                        //Tìm theo Document date
                        && (searchModel.Document_Date_From == null || searchModel.Document_Date_From <= s.AUDAT)
                        && (searchModel.Document_Date_To == null || searchModel.Document_Date_To >= s.AUDAT)
                        //Tìm theo Created on
                        && (searchModel.Created_On_From == null || searchModel.Created_On_From <= s.ERDAT)
                        && (searchModel.Created_On_To == null || searchModel.Created_On_To >= s.ERDAT)
                        //Tìm theo current company code
                        && (searchModel.VKORG == null || searchModel.VKORG == s.VKORG)
                        group s by new { s.VBELN, s.BEDAE, s.AUART, s.AUDAT, s.BSTNK, s.ERDAT, s.ERNAM, s.ERZET, s.KUNNR, s.LFGSK, s.PS_PSP_PNR, s.PS_PSP_PNR_OUTPUT, s.SPART, s.VDATU, s.VKORG, s.VTWEG, s.SOHeaderId } into g
                        select new SaleOrderHeader80ViewModel()
                        {
                            // Sale order
                            VBELN = g.Key.VBELN,
                            // Requirement type
                            BEDAE = g.Key.BEDAE,
                            // Sale Document type
                            AUART = g.Key.AUART,
                            // Document date
                            AUDAT = g.Key.AUDAT,
                            // Cust. Reference
                            BSTNK = g.Key.BSTNK,
                            // Created on
                            ERDAT = g.Key.ERDAT,
                            //Created By
                            ERNAM = g.Key.ERNAM,
                            //Time
                            ERZET = g.Key.ERZET,
                            //Sold-to party
                            KUNNR = g.Key.KUNNR,
                            //Ovrl Delvry St.
                            LFGSK = g.Key.LFGSK,
                            //WBS element
                            PS_PSP_PNR = g.Key.PS_PSP_PNR,
                            PS_PSP_PNR_OUTPUT = g.Key.PS_PSP_PNR_OUTPUT,

                            //Division
                            SPART = g.Key.SPART,
                            //Requested delivery Date
                            VDATU = g.Key.VDATU,
                            //Sale Org
                            VKORG = g.Key.VKORG,
                            //Distribution channel
                            VTWEG = g.Key.VTWEG,
                            //Id
                            SOHeaderId = g.Key.SOHeaderId
                        }).OrderByDescending(x => x.ERDAT).ThenByDescending(x => x.ERZET);
            return data;
        }

        #region GetData
        public List<SO80ReportViewModel> GetData(SO80ReportViewModel sO80ReportViewModel)
        {
            #region LSXSAP list
            if (!string.IsNullOrEmpty(sO80ReportViewModel.LSXSAP))
            {
                var LSXSAPLst = sO80ReportViewModel.LSXSAP.Split(',');
                if (LSXSAPLst != null && LSXSAPLst.Length > 0)
                {
                    sO80ReportViewModel.LSXSAPList = new List<string>();
                    for (int i = 0; i < LSXSAPLst.Length; i++)
                    {
                        string lsxc = LSXSAPLst[i].Trim();
                        sO80ReportViewModel.LSXSAPList.Add(lsxc);
                    }
                }
            }
            //Build your record
            var tableLSXSAPSchema = new List<SqlMetaData>(1)
                {
                    new SqlMetaData("LSXSAP", SqlDbType.NVarChar,50)
                }.ToArray();

            //And a table as a list of those records
            var tableLSXSAP = new List<SqlDataRecord>();
            if (sO80ReportViewModel.LSXSAPList != null && sO80ReportViewModel.LSXSAPList.Count() > 0)
            {
                foreach (var r in sO80ReportViewModel.LSXSAPList)
                {
                    var tableRow = new SqlDataRecord(tableLSXSAPSchema);
                    tableRow.SetString(0, r);
                    tableLSXSAP.Add(tableRow);
                }
            }
            else
            {
                var tableRow = new SqlDataRecord(tableLSXSAPSchema);
                tableLSXSAP.Add(tableRow);
            }
            #endregion
            string sqlQuery = "[MES].[GetBomDetailByListSO80] @LSXSAP, @LSXDT, @DSX, @WERKS, @VBELN, @AUART, @DocumentDateFrom, @DocumentDateTo, @CreatedOnFrom, @CreatedOnTo, @PYCSXDT";
            #region Parameters
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter
                {
                    SqlDbType = SqlDbType.Structured,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSXSAP",
                    TypeName = "[dbo].[StringList]", //Don't forget this one!
                    Value = tableLSXSAP ?? (object)DBNull.Value
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "LSXDT",
                    Value = sO80ReportViewModel.LSXDT ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DSX",
                    Value = sO80ReportViewModel.DSX ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "WERKS",
                    Value = sO80ReportViewModel.WERKS ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "VBELN",
                    Value = sO80ReportViewModel.VBELN ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "AUART",
                    Value = sO80ReportViewModel.AUART ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DocumentDateFrom",
                    Value = sO80ReportViewModel.Document_Date_From ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "DocumentDateTo",
                    Value = sO80ReportViewModel.Document_Date_To ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreatedOnFrom",
                    Value = sO80ReportViewModel.Created_On_From ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    ParameterName = "CreatedOnTo",
                    Value = sO80ReportViewModel.Created_On_To ?? (object)DBNull.Value,
                },
                new SqlParameter
                {
                    SqlDbType = SqlDbType.NVarChar,
                    Direction = ParameterDirection.Input,
                    ParameterName = "PYCSXDT",
                    Value = sO80ReportViewModel.PYCSXDT ?? (object)DBNull.Value,
                },
            };
            #endregion
            List<SO80ReportViewModel> res = _context.Database.SqlQuery<SO80ReportViewModel>(sqlQuery, parameters.ToArray()).ToList();
            return res;
        }
        #endregion
        public SaleOrderHeader80ViewModel GetSaleOrderHeader80BySaleOrder(string VBELN)
        {
            var data = (
                        //SO header 80
                        from s in _context.SaleOrderHeader80Model
                            // VBELN: Sale order
                        where s.VBELN == VBELN
                        select new SaleOrderHeader80ViewModel()
                        {
                            // Sale order
                            VBELN = s.VBELN,
                            // Requirement type
                            BEDAE = s.BEDAE,
                            // Sale Document type
                            AUART = s.AUART,
                            // Document date
                            AUDAT = s.AUDAT,
                            // Cust. Reference
                            BSTNK = s.BSTNK,
                            // Created on
                            ERDAT = s.ERDAT,
                            //Created By
                            ERNAM = s.ERNAM,
                            //Time
                            ERZET = s.ERZET,
                            //Sold-to party
                            KUNNR = s.KUNNR,
                            //Ovrl Delvry St.
                            LFGSK = s.LFGSK,
                            //WBS element
                            PS_PSP_PNR = s.PS_PSP_PNR,
                            PS_PSP_PNR_OUTPUT = s.PS_PSP_PNR_OUTPUT,
                            //Division
                            SPART = s.SPART,
                            //Requested delivery Date
                            VDATU = s.VDATU,
                            //Sale Org
                            VKORG = s.VKORG,
                            //Distribution channel
                            VTWEG = s.VTWEG,
                            //Id
                            SOHeaderId = s.SOHeaderId
                        }).FirstOrDefault();
            return data;
        }

        public SaleOrderItem80ViewModel GetSaleOrderItem80BySaleOrderAndLine(string VBELN, string POSNR)
        {
            var data = (
                //SO line 80
                from i in _context.SaleOrderItem80Model
                    //Product 
                join p in _context.ProductModel on i.MATNR equals p.ProductCode
                //Company
                join c in _context.CompanyModel on p.CompanyId equals c.CompanyId
                //VBELN: SO Number
                //WERKS: Company Code
                //POSNR: Sale order item line
                where i.VBELN == VBELN && c.CompanyCode == i.WERKS && i.POSNR == POSNR
                select new SaleOrderItem80ViewModel()
                {
                    //Sale order
                    VBELN = i.VBELN,
                    //Sale order item line
                    POSNR = i.POSNR,
                    //Material Number
                    MATNR = i.MATNR,
                    //Item Description
                    ItemDescription = p.ProductName,
                    //Short Text for Sales Order Item
                    ARKTX = i.ARKTX,
                    //Requirement type
                    BEDAE = i.BEDAE,
                    //Plant
                    WERKS = i.WERKS,
                    //Comulative Confirmed Quantity in Sales Unit
                    KBMENG = i.KBMENG,
                    //Comulative Confirmed Quantity in Base Unit
                    KLMENG = i.KLMENG,
                    //Schedule line
                    ETENR = i.ETENR,
                    //Schedule line date
                    EDATU = i.EDATU,
                    //Order Quantity in Sales Units
                    WMENG = i.WMENG,
                    //Sales unit
                    VRKME = i.VRKME,
                    //Quantity in stock keeping unit
                    LMENG = i.LMENG,
                    //Delivered quantity
                    DLVQTY_BU = i.DLVQTY_BU,
                    //Open quantity
                    ORDQTY_BU = i.ORDQTY_BU,
                    //Special stock
                    SOBKZ = i.SOBKZ,
                    //Work Breakdown Structure Element (WBS Element)
                    PS_PSP_PNR = i.PS_PSP_PNR,
                    PS_PSP_PNR_OUTPUT = i.PS_PSP_PNR_OUTPUT,
                    //Numerator (factor) for conversion of sales quantity into SKU
                    UMVKZ = i.UMVKZ,
                    //Denominator (Divisor) for Conversion of Sales Qty into SKU
                    UMVKN = i.UMVKN,
                    //Based Unit
                    MEINS = i.MEINS,
                    //Reason for Rejection of Quotations and Sales Orders
                    ABGRU = i.ABGRU,
                    //Overall Processing Status of the SD Document Item
                    GBSTA = i.GBSTA,
                    //Created on
                    ERDAT = i.ERDAT,
                    //Created By
                    ERNAM = i.ERNAM,
                    //Time
                    ERZET = i.ERZET,
                    //Id
                    SOItemId = i.SOItemId
                }).FirstOrDefault();
            return data;
        }

        //Lấy dữ liệu Sale Order 80 bằng SaleOrder của Sale Order Header 80
        public List<SaleOrderItem80ViewModel> GetSaleOrderItem80BySaleOrder(string VBELN)
        {

            var data = (
                //SO Item 80
                from i in _context.SaleOrderItem80Model
                    //Product
                join p in _context.ProductModel on i.MATNR equals p.ProductCode
                //Company
                join c in _context.CompanyModel on p.CompanyId equals c.CompanyId
                //VBELN: SO Number
                //WERKS: Company Code
                where i.VBELN == VBELN && c.CompanyCode == i.WERKS
                select new SaleOrderItem80ViewModel
                {
                    //Sale order
                    VBELN = i.VBELN,
                    //Sale order item
                    POSNR = i.POSNR,
                    //Material Number
                    MATNR = i.MATNR,
                    //Item Description
                    ItemDescription = p.ProductName,
                    //UPMAT: mã cha
                    UPMAT = i.UPMAT,
                    //Short Text for Sales Order Item
                    ARKTX = i.ARKTX,
                    //Requirement type
                    BEDAE = i.BEDAE,
                    //Plant
                    WERKS = i.WERKS,
                    //Comulative Confirmed Quantity in Sales Unit
                    KBMENG = i.KBMENG,
                    //Comulative Confirmed Quantity in Base Unit
                    KLMENG = i.KLMENG,
                    //Special stock
                    SOBKZ = i.SOBKZ,
                    //Work Breakdown Structure Element (WBS Element)
                    PS_PSP_PNR = i.PS_PSP_PNR,
                    PS_PSP_PNR_OUTPUT = i.PS_PSP_PNR_OUTPUT,
                    //Numerator (factor) for conversion of sales quantity into SKU
                    UMVKZ = i.UMVKZ,
                    //Denominator (Divisor) for Conversion of Sales Qty into SKU
                    UMVKN = i.UMVKN,
                    //Based Unit
                    MEINS = i.MEINS,
                    //Reason for Rejection of Quotations and Sales Orders
                    ABGRU = i.ABGRU,
                    //Overall Processing Status of the SD Document Item
                    GBSTA = i.GBSTA,
                    //Created on
                    ERDAT = i.ERDAT,
                    //Created By
                    ERNAM = i.ERNAM,
                    //Time
                    ERZET = i.ERZET,
                    //Id
                    SOItemId = i.SOItemId,
                    //Danh sách nguyên vật liệu
                });
            return data.OrderBy(x => x.POSNR).ToList();
        }

        public List<BomDetailViewModel> GetBOMDetailWithSaleOrder(string VBELN, string POSNR = null, string STLAN = null)
        {
            object[] SqlParams =
            {
                new SqlParameter("@VBELN",VBELN),
                new SqlParameter("@POSNR", POSNR ?? (object)DBNull.Value),
                new SqlParameter("@STLAN", STLAN ?? (object)DBNull.Value),
            };
            var res = _context.Database.SqlQuery<BomDetailViewModel>("[MES].[GetBomDetailWithSaleOrder80] @VBELN, @POSNR, @STLAN", SqlParams).ToList();
            return res;
        }

        #region Schedule lines
        public IEnumerable<SO80ScheduleLineViewModel> GetScheduleLines(string VBELN, string POSNR)
        {
            var data = (
                //SO line 80
                from i in _context.SO80ScheduleLineModel
                //POSNR: Sale order item line
                where i.VBELN == VBELN && i.POSNR == POSNR
                select new SO80ScheduleLineViewModel()
                {
                    //Schedule line
                    ETENR = i.ETENR,
                    //Item is relevant for delivery
                    LFREL = i.LFREL,
                    //Schedule line date
                    EDATU = i.EDATU,
                    //Order Quantity in Sales Units
                    WMENG = i.WMENG,
                    //Confirmed Quantity
                    BMENG = i.BMENG,
                    //Sales unit
                    VRKME = i.VRKME,
                    //Required quantity for mat.management in stockkeeping units
                    LMENG = i.LMENG,
                    //Base Unit of Measure
                    MEINS = i.MEINS,
                    //Schedule Line Blocked for Delivery
                    LIFSP = i.LIFSP,
                    //Delivered Quantity
                    DLVQTY_BU = i.DLVQTY_BU,
                    //Delivered Quantity
                    DLVQTY_SU = i.DLVQTY_SU,
                    //Open Confirmed Delivery Quantity
                    OCDQTY_BU = i.OCDQTY_BU,
                    //Open Confirmed Delivery Quantity
                    OCDQTY_SU = i.OCDQTY_SU,
                    //Open requested Delivery Quantity
                    ORDQTY_BU = i.ORDQTY_BU,
                    //Open requested Delivery Quantity
                    ORDQTY_SU = i.ORDQTY_SU,
                    //Delivery Creation Date
                    CREA_DLVDATE = i.CREA_DLVDATE,
                    //Schedule line date
                    REQ_DLVDATE = i.REQ_DLVDATE,
                });
            return data;
        }
        #endregion
    }
}
