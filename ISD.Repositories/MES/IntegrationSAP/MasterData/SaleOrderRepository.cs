using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ISD.Repositories.MES.IntegrationSAP.MasterData
{
    public class SaleOrderRepository
    {
        EntityDataContext _context;
        //SAPRepository _sap;
        LoggerRepository _loggerRepository;
        public SaleOrderRepository(EntityDataContext entityDataContext)
        {
            _context = entityDataContext;
            // 1. SET AutoDetectChangesEnabled = false
            _context.Configuration.AutoDetectChangesEnabled = false;
            //_sap = new SAPRepository();
            _loggerRepository = new LoggerRepository();
        }

        #region SaleOrder 80
        private List<DataTable> GetSaleOrderData(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_SALE_ORDER);

                //function.SetValue("I_WERKS", paramRequest.CompanyCode);
                //function.SetValue("I_VBELN", paramRequest.VBELN);
                //function.SetValue("I_RECORD", paramRequest.IRecord);

                //if (!string.IsNullOrEmpty(paramRequest.FromDate))
                //{
                //    DateTime fromDate = DateTime.ParseExact(paramRequest.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //    function.SetValue("I_FROM_DATE", fromDate);
                //    function.SetValue("I_FROM_TIME", paramRequest.FromTime);
                //    function.SetValue("I_TO_TIME", paramRequest.ToTime);
                //}

                //function.Invoke(destination);

                //var table1 = function.GetTable("IT_HEADER").ToDataTable("IT_HEADER");
                //dataTables.Add(table1);
                //var table2 = function.GetTable("IT_ITEM").ToDataTable("IT_ITEM");
                //dataTables.Add(table2);
                //var table3 = function.GetTable("IT_SCHEDULE_LINE").ToDataTable("IT_SCHEDULE_LINE");
                //dataTables.Add(table3);
                //var table4 = function.GetTable("IT_TEXT_HEADER").ToDataTable("IT_TEXT_HEADER");
                //dataTables.Add(table4);
                //var table5 = function.GetTable("IT_TEXT_ITEM").ToDataTable("IT_TEXT_ITEM");
                //dataTables.Add(table5);
                //dataTables.Add(new DataTable());
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        errorMessage = ex.InnerException.InnerException.Message;
                    }
                }
            }
            return dataTables;

        }

        private OutputFromSAPViewModel ConfirmInsert(string VBELN, string SOType = null)
        {
            var result = new OutputFromSAPViewModel();
            //var destination = _sap.GetRfcWithConfig();
            //Định nghĩa hàm cần gọi
            //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_SALE_ORDER);

            ////function.SetValue("I_BEDAE", SOType);
            //function.SetValue("I_VBELN", VBELN);
            //function.SetValue("I_INSERT", 'X');//Tham số xác nhận insert thành công

            //function.Invoke(destination);

            //string successMessage = function.GetString("E_SUCCESS");
            //string errorMessage = function.GetString("E_ERROR");

            //if (!string.IsNullOrEmpty(successMessage))
            //{
            //    result.IsSuccess = true;
            //    result.Message = successMessage;
            //}
            //else if (!string.IsNullOrEmpty(errorMessage))
            //{
            //    result.IsSuccess = false;
            //    result.Message = errorMessage;
            //}

            return result;
        }

        public async Task<string> SyncSaleOrder(ParamRequestSyncSapModel paramRequest)
        {
            var error = string.Empty;
            var message = string.Empty;
            var dataTables = GetSaleOrderData(paramRequest, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return error;
            }

            if (dataTables != null && dataTables.Count > 0)
            {
                try
                {
                    //Stopwatch stopwatch = new Stopwatch();
                    //stopwatch.Start();
                    var IT_HEADER = dataTables[0];
                    var IT_ITEM = dataTables[1];
                    var LT_SCHEDULE_LINE = dataTables[2];
                    var IT_TEXT_HEADER = dataTables[3];
                    var IT_TEXT_ITEM = dataTables[4];


                    if (IT_HEADER != null && IT_HEADER.Rows.Count > 0)
                    {
                        foreach (DataRow soHeader in IT_HEADER.Rows)
                        {
                            var VBELN = soHeader["VBELN"].ToString();
                            //var BEDAE = soHeader["BEDAE"].ToString();
                            // 1. Check SO Number đã có trong table SO 80 chưa
                            // 1.1 Nếu chưa có insert SO 80
                            var existsSO80 = await _context.SaleOrderHeader80Model.Where(p => p.VBELN == VBELN).FirstOrDefaultAsync();
                            if (existsSO80 == null)
                            {
                                InsertSaleOrder80(IT_HEADER, IT_ITEM, LT_SCHEDULE_LINE, IT_TEXT_HEADER, IT_TEXT_ITEM);
                            }

                            // 2. Check đã có trong table SO 100 chưa
                            // 2.1 Nếu chưa có insert SO 100
                            // 2.2 Nếu có rồi thì update SO 100

                            var existsSO100 = await _context.SaleOrderHeader100Model.Where(p => p.VBELN == VBELN).FirstOrDefaultAsync();
                            if (existsSO100 == null)
                            {
                                InsertSaleOrder100(IT_HEADER, IT_ITEM, LT_SCHEDULE_LINE, IT_TEXT_HEADER, IT_TEXT_ITEM);
                            }
                            else
                            {
                                UpdateSaleOrder100(IT_HEADER, IT_ITEM, LT_SCHEDULE_LINE, IT_TEXT_HEADER, IT_TEXT_ITEM);
                            }


                            //if (!string.IsNullOrEmpty(BEDAE))
                            //{
                            //    if (BEDAE == ConstSOType.ZPN || BEDAE == ConstSOType.ZO || BEDAE == ConstSOType.ZP || BEDAE == ConstSOType.ZS)
                            //    {
                            //        // 1. Check đã có trong table SO 80 chưa
                            //        // 1.1 Nếu chưa có insert SO 80
                            //        var existsSO80 = await _context.SaleOrderHeader80Model.Where(p => p.VBELN == VBELN).FirstOrDefaultAsync();
                            //        if (existsSO80 == null)
                            //        {
                            //            InsertSaleOrder80(IT_HEADER, IT_ITEM, LT_SCHEDULE_LINE);
                            //        }

                            //        // 2. Check đã có trong table SO 100 chưa
                            //        // 2.1 Nếu chưa có insert SO 100
                            //        // 2.2 Nếu có rồi thì update SO 100
                            //        if (BEDAE == ConstSOType.ZO || BEDAE == ConstSOType.ZP || BEDAE == ConstSOType.ZS)
                            //        {
                            //            var existsSO100 = await _context.SaleOrderHeader100Model.Where(p => p.VBELN == VBELN).FirstOrDefaultAsync();
                            //            if (existsSO100 == null)
                            //            {
                            //                InsertSaleOrder100(IT_HEADER, IT_ITEM, LT_SCHEDULE_LINE);
                            //            }
                            //            else
                            //            {
                            //                UpdateSaleOrder100(IT_HEADER, IT_ITEM, LT_SCHEDULE_LINE);
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        ConfirmInsert(VBELN, BEDAE);
                            //    }
                            //}
                            //else
                            //{
                            //    ConfirmInsert(VBELN, BEDAE);
                            //}
                        }
                    }
                    //var taskScheduleLine = await InsertUpdateSheduleLine80(LT_SCHEDULE_LINE);
                    //if (!string.IsNullOrEmpty(taskScheduleLine))
                    //{
                    //    return taskScheduleLine;
                    //}

                    //var taskItem = await InsertUpdateItem(IT_ITEM);
                    //if (!string.IsNullOrEmpty(taskItem))
                    //{
                    //    return taskItem;
                    //}

                    //var taskHeader = await InsertUpdateHeader(IT_HEADER);
                    //if (!string.IsNullOrEmpty(taskHeader.Item2))
                    //{
                    //    return taskHeader.Item2;
                    //}
                    //message = taskHeader.Item1;
                    //stopwatch.Stop();
                    //TimeSpan timeSpan = stopwatch.Elapsed;
                    //string elapsedTime = String.Format("Sync SO:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

                    //_loggerRepository.Logging(elapsedTime, "INFO");
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            error = ex.InnerException.InnerException.Message;
                        }
                    }
                    return error;
                }
            }
            else
            {
                error = "No data returned from SAP";
                return error;
            }
            return message;
        }

        private string InsertSaleOrder80(DataTable IT_HEADER, DataTable IT_ITEM, DataTable LT_SCHEDULE_LINE, DataTable IT_TEXT_HEADER, DataTable IT_TEXT_ITEM)
        {
            string error = string.Empty;

            //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            //{
            try
            {
                foreach (DataRow dataRow in IT_HEADER.Rows)
                {
                    #region SOHeader

                    #region Collect Data
                    //var BEDAE = dataRow["BEDAE"].ToString();
                    var VBELN = dataRow["VBELN"].ToString();
                    DateTime? AUDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["AUDAT"].ToString()) && dataRow["AUDAT"].ToString() != "00000000")
                    {
                        string date = dataRow["AUDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        AUDAT = DateTime.Parse(date);
                    }

                    DateTime? ERDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["ERDAT"].ToString()) && dataRow["ERDAT"].ToString() != "00000000")
                    {
                        string date = dataRow["ERDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        ERDAT = DateTime.Parse(date);
                    }
                    TimeSpan? ERZET = null;
                    if (!string.IsNullOrEmpty(dataRow["ERZET"].ToString()))
                    {
                        ERZET = TimeSpan.Parse(dataRow["ERZET"].ToString());
                    }

                    var ERNAM = dataRow["ERNAM"].ToString();
                    var AUART = dataRow["AUART"].ToString();
                    var VKORG = dataRow["VKORG"].ToString();
                    var VTWEG = dataRow["VTWEG"].ToString();
                    var SPART = dataRow["SPART"].ToString();
                    var BSTNK = dataRow["BSTNK"].ToString();
                    var KUNNR = dataRow["KUNNR"].ToString();
                    var PSPSPPNR = dataRow["PS_PSP_PNR"].ToString();
                    var PS_PSP_PNR_OUTPUT = dataRow["PS_PSP_PNR_OUTPUT"].ToString();
                    DateTime? VDATU = null;
                    if (!string.IsNullOrEmpty(dataRow["VDATU"].ToString()) && dataRow["VDATU"].ToString() != "00000000")
                    {
                        string date = dataRow["VDATU"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        VDATU = DateTime.Parse(date);
                    }
                    var LFGSK = dataRow["LFGSK"].ToString();
                    var ZZTERM = dataRow["ZZTERM"].ToString();
                    var ZZTERM_DES = dataRow["ZZTERM_DES"].ToString();
                    var SORTL = dataRow["SORTL"].ToString();
                    #endregion

                    #region Insert
                    var saleHeaderNew = new SaleOrderHeader80Model
                    {
                        SOHeaderId = Guid.NewGuid(),
                        AUART = AUART,
                        //BEDAE = BEDAE,
                        AUDAT = AUDAT,
                        BSTNK = BSTNK,
                        ERDAT = ERDAT,
                        ERNAM = ERNAM,
                        ERZET = ERZET,
                        KUNNR = KUNNR,
                        LFGSK = LFGSK,
                        PS_PSP_PNR = PSPSPPNR,
                        PS_PSP_PNR_OUTPUT = PS_PSP_PNR_OUTPUT,
                        SPART = SPART,
                        VBELN = VBELN,
                        VDATU = VDATU,
                        VKORG = VKORG,
                        VTWEG = VTWEG,
                        ZZTERM = ZZTERM,
                        ZZTERM_DES = ZZTERM_DES,
                        SORTL = SORTL,
                        CreateTime = DateTime.Now
                    };
                    _context.Entry(saleHeaderNew).State = EntityState.Added;
                    #endregion

                    #endregion

                    //SO80: ZPN | ZO | ZP | ZS
                    #region SOItem
                    foreach (DataRow dataRowItem in IT_ITEM.Rows)
                    {
                        var BEDAE_ITEM = dataRowItem["BEDAE"].ToString();
                        if (string.IsNullOrEmpty(BEDAE_ITEM) || BEDAE_ITEM == ConstSOType.ZNP || BEDAE_ITEM == ConstSOType.ZO || BEDAE_ITEM == ConstSOType.ZP || BEDAE_ITEM == ConstSOType.ZS)
                        {
                            #region Collect Data
                            var VBELN_ITEM = dataRowItem["VBELN"].ToString();
                            var POSNR = dataRowItem["POSNR"].ToString();
                            string MATNR = string.Empty;
                            if (!string.IsNullOrEmpty(dataRowItem["MATNR"].ToString()))
                            {
                                MATNR = Int32.Parse(dataRowItem["MATNR"].ToString()).ToString();
                            }
                            var WERKS = dataRowItem["WERKS"].ToString();

                            //Sản phẩm: khi đồng bộ từ SAP về nếu không có (MES) phải kéo bằng được
                            var companyId = _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefault();
                            var productInDb = _context.ProductModel.Where(p => p.ERPProductCode == MATNR && p.CompanyId == companyId).FirstOrDefault();
                            if (productInDb == null && !string.IsNullOrEmpty(MATNR))
                            {
                                syncMaterial(WERKS, MATNR);
                            }

                            var SOBKZ = dataRowItem["SOBKZ"].ToString();
                            var PS_PSP_PNR = dataRowItem["PS_PSP_PNR"].ToString();
                            var PS_PSP_PNR_OUTPUT_ITEM = dataRowItem["PS_PSP_PNR_OUTPUT"].ToString();
                            decimal? UMVKZ = null;
                            if (!string.IsNullOrEmpty(dataRowItem["UMVKZ"].ToString()))
                            {
                                UMVKZ = decimal.Parse(dataRowItem["UMVKZ"].ToString());
                            }
                            decimal? UMVKN = null;
                            if (!string.IsNullOrEmpty(dataRowItem["UMVKN"].ToString()))
                            {
                                UMVKN = decimal.Parse(dataRowItem["UMVKN"].ToString());
                            }
                            var MEINS = dataRowItem["MEINS"].ToString();
                            var ABGRU = dataRowItem["ABGRU"].ToString();
                            var GBSTA = dataRowItem["GBSTA"].ToString();
                            DateTime? ERDAT_ITEM = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ERDAT"].ToString()) && dataRowItem["ERDAT"].ToString() != "00000000")
                            {
                                string date = dataRowItem["ERDAT"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                ERDAT_ITEM = DateTime.Parse(date);
                            }
                            var ERNAM_ITEM = dataRowItem["ERNAM"].ToString();

                            TimeSpan? ERZET_ITEM = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ERZET"].ToString()))
                            {
                                ERZET_ITEM = TimeSpan.Parse(dataRowItem["ERZET"].ToString());
                            }
                            var ZZTERM_ITEM = dataRowItem["ZZTERM"].ToString();
                            var ZZTERM_DES_ITEM = dataRowItem["ZZTERM_DES"].ToString();
                            //=========================================================
                            var UEBTO = dataRowItem["UEBTO"].ToString();
                            var UNTTO = dataRowItem["UNTTO"].ToString();
                            decimal? KWMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["KWMENG"].ToString()))
                            {
                                KWMENG = decimal.Parse(dataRowItem["KWMENG"].ToString());
                            }
                            decimal? LSMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["LSMENG"].ToString()))
                            {
                                LSMENG = decimal.Parse(dataRowItem["LSMENG"].ToString());
                            }
                            decimal? KBMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["KBMENG"].ToString()))
                            {
                                KBMENG = decimal.Parse(dataRowItem["KBMENG"].ToString());
                            }
                            decimal? KLMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["KLMENG"].ToString()))
                            {
                                KLMENG = decimal.Parse(dataRowItem["KLMENG"].ToString());
                            }
                            var UPMAT = dataRowItem["UPMAT"].ToString();
                            if (!string.IsNullOrEmpty(UPMAT))
                            {
                                UPMAT = UPMAT.TrimStart(new Char[] { '0' });
                            }
                            var ZPYCSXDT = dataRowItem["ZPYCSXDT"].ToString();
                            var ZFLAG = dataRowItem["ZFLAG"].ToString();
                            DateTime? ZFDAT = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ZFDAT"].ToString()) && dataRowItem["ZFDAT"].ToString() != "00000000")
                            {
                                string date = dataRowItem["ZFDAT"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                ZFDAT = DateTime.Parse(date);
                            }
                            TimeSpan? ZFTM = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ZFTM"].ToString()))
                            {
                                ZFTM = TimeSpan.Parse(dataRowItem["ZFTM"].ToString());
                            }
                            var ZZSKU = dataRowItem["ZZSKU"].ToString();
                            var ZPTG = dataRowItem["ZPTG"].ToString();
                            var VRKME = dataRowItem["VRKME"].ToString();
                            #endregion

                            //1. Nếu có tồn tại thì cập nhật(trường hợp trên SAP trả về bị lặp data)
                            var so80InDb = _context.SaleOrderItem80Model.Where(p => p.VBELN == VBELN && p.POSNR == POSNR && p.WERKS == WERKS).FirstOrDefault();
                            if (so80InDb != null)
                            {
                                #region UpdateItem
                                so80InDb.POSNR = POSNR;
                                so80InDb.POSNR_MES = POSNR.TrimStart(new Char[] { '0' });
                                so80InDb.VBELN = VBELN_ITEM;
                                so80InDb.ABGRU = ABGRU;
                                so80InDb.BEDAE = BEDAE_ITEM;
                                so80InDb.ERDAT = ERDAT_ITEM;
                                so80InDb.ERNAM = ERNAM_ITEM;
                                so80InDb.ERZET = ERZET_ITEM;
                                so80InDb.GBSTA = GBSTA;
                                so80InDb.MATNR = MATNR;
                                so80InDb.MEINS = MEINS;
                                so80InDb.PS_PSP_PNR = PS_PSP_PNR;
                                so80InDb.PS_PSP_PNR_OUTPUT = PS_PSP_PNR_OUTPUT_ITEM;
                                so80InDb.SOBKZ = SOBKZ;
                                so80InDb.UMVKN = UMVKN;
                                so80InDb.UMVKZ = UMVKZ;
                                so80InDb.WERKS = WERKS;
                                so80InDb.ZZTERM = ZZTERM_ITEM;
                                so80InDb.ZZTERM_DES = ZZTERM_DES_ITEM;
                                so80InDb.UEBTO = UEBTO;
                                so80InDb.UNTTO = UNTTO;
                                so80InDb.KWMENG = KWMENG;
                                so80InDb.LSMENG = LSMENG;
                                so80InDb.KBMENG = KBMENG;
                                so80InDb.KLMENG = KLMENG;
                                so80InDb.UPMAT = UPMAT;
                                so80InDb.ZPYCSXDT = ZPYCSXDT;
                                so80InDb.ZFLAG = ZFLAG;
                                so80InDb.ZFDAT = ZFDAT;
                                so80InDb.ZFTM = ZFTM;
                                so80InDb.ZZSKU = ZZSKU;
                                so80InDb.ZPTG = ZPTG;
                                so80InDb.VRKME = VRKME;
                                so80InDb.LastEditTime = DateTime.Now;
                                _context.Entry(so80InDb).State = EntityState.Modified;
                                #endregion UpdateItem
                            }
                            //2. Chưa có thì thêm mới
                            else
                            {

                                #region InsertItem
                                var SOItem = new SaleOrderItem80Model
                                {
                                    SOItemId = Guid.NewGuid(),
                                    POSNR = POSNR,
                                    POSNR_MES = POSNR.TrimStart(new Char[] { '0' }),
                                    VBELN = VBELN_ITEM,
                                    ABGRU = ABGRU,
                                    BEDAE = BEDAE_ITEM,
                                    ERDAT = ERDAT_ITEM,
                                    ERNAM = ERNAM_ITEM,
                                    ERZET = ERZET_ITEM,
                                    GBSTA = GBSTA,
                                    MATNR = MATNR,
                                    MEINS = MEINS,
                                    PS_PSP_PNR = PS_PSP_PNR,
                                    PS_PSP_PNR_OUTPUT = PS_PSP_PNR_OUTPUT_ITEM,
                                    SOBKZ = SOBKZ,
                                    UMVKN = UMVKN,
                                    UMVKZ = UMVKZ,
                                    WERKS = WERKS,
                                    ZZTERM = ZZTERM_ITEM,
                                    ZZTERM_DES = ZZTERM_DES_ITEM,
                                    UEBTO = UEBTO,
                                    UNTTO = UNTTO,
                                    KWMENG = KWMENG,
                                    LSMENG = LSMENG,
                                    KBMENG = KBMENG,
                                    KLMENG = KLMENG,
                                    UPMAT = UPMAT,
                                    ZPYCSXDT = ZPYCSXDT,
                                    ZFLAG = ZFLAG,
                                    ZFDAT = ZFDAT,
                                    ZFTM = ZFTM,
                                    ZZSKU = ZZSKU,
                                    ZPTG = ZPTG,
                                    VRKME = VRKME,
                                    CreateTime = DateTime.Now
                                };
                                _context.Entry(SOItem).State = EntityState.Added;

                                #endregion InsertItem
                            }
                        }
                        _context.SaveChanges();
                    }
                    #endregion

                    #region ScheduleLine
                    foreach (DataRow dataRowSL in LT_SCHEDULE_LINE.Rows)
                    {
                        #region Collect Data
                        string VBELN_ITEM = dataRowSL["VBELN"].ToString();
                        string POSNR = dataRowSL["POSNR"].ToString();
                        string ETENR = dataRowSL["ETENR"].ToString();
                        string LFREL = dataRowSL["LFREL"].ToString();
                        DateTime? EDATU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["EDATU"].ToString()) && dataRowSL["EDATU"].ToString() != "00000000")
                        {
                            string date = dataRowSL["EDATU"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            EDATU = DateTime.Parse(date);
                        }
                        decimal? WMENG = null;
                        if (!string.IsNullOrEmpty(dataRowSL["WMENG"].ToString()))
                        {
                            WMENG = decimal.Parse(dataRowSL["WMENG"].ToString());
                        }

                        decimal? BMENG = null;
                        if (!string.IsNullOrEmpty(dataRowSL["BMENG"].ToString()))
                        {
                            BMENG = decimal.Parse(dataRowSL["BMENG"].ToString());
                        }

                        string VRKME = dataRowSL["VRKME"].ToString();

                        decimal? LMENG = null;
                        if (!string.IsNullOrEmpty(dataRowSL["LMENG"].ToString()))
                        {
                            LMENG = decimal.Parse(dataRowSL["LMENG"].ToString());
                        }

                        string MEINS = dataRowSL["MEINS"].ToString();
                        string LIFSP = dataRowSL["LIFSP"].ToString();

                        decimal? DLVQTY_BU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["DLVQTY_BU"].ToString()))
                        {
                            DLVQTY_BU = decimal.Parse(dataRowSL["DLVQTY_BU"].ToString());
                        }

                        decimal? DLVQTY_SU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["DLVQTY_SU"].ToString()))
                        {
                            DLVQTY_SU = decimal.Parse(dataRowSL["DLVQTY_SU"].ToString());
                        }

                        decimal? OCDQTY_BU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["OCDQTY_BU"].ToString()))
                        {
                            OCDQTY_BU = decimal.Parse(dataRowSL["OCDQTY_BU"].ToString());
                        }

                        decimal? OCDQTY_SU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["OCDQTY_SU"].ToString()))
                        {
                            OCDQTY_SU = decimal.Parse(dataRowSL["OCDQTY_SU"].ToString());
                        }

                        decimal? ORDQTY_BU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["ORDQTY_BU"].ToString()))
                        {
                            ORDQTY_BU = decimal.Parse(dataRowSL["ORDQTY_BU"].ToString());
                        }

                        decimal? ORDQTY_SU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["ORDQTY_SU"].ToString()))
                        {
                            ORDQTY_SU = decimal.Parse(dataRowSL["ORDQTY_SU"].ToString());
                        }

                        DateTime? CREA_DLVDATE = null;
                        if (!string.IsNullOrEmpty(dataRowSL["CREA_DLVDATE"].ToString()) && dataRowSL["CREA_DLVDATE"].ToString() != "00000000")
                        {
                            string date = dataRowSL["CREA_DLVDATE"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            CREA_DLVDATE = DateTime.Parse(date);
                        }

                        DateTime? REQ_DLVDATE = null;
                        if (!string.IsNullOrEmpty(dataRowSL["REQ_DLVDATE"].ToString()) && dataRowSL["REQ_DLVDATE"].ToString() != "00000000")
                        {
                            string date = dataRowSL["REQ_DLVDATE"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            REQ_DLVDATE = DateTime.Parse(date);
                        }
                        #endregion

                        #region InsertItem
                        var soSL80 = new SO80ScheduleLineModel
                        {
                            SO80ScheduleLineId = Guid.NewGuid(),
                            VBELN = VBELN_ITEM,
                            POSNR = POSNR,
                            ETENR = ETENR,
                            LFREL = LFREL,
                            EDATU = EDATU,
                            WMENG = WMENG,
                            BMENG = BMENG,
                            VRKME = VRKME,
                            LMENG = LMENG,
                            MEINS = MEINS,
                            LIFSP = LIFSP,
                            DLVQTY_BU = DLVQTY_BU,
                            DLVQTY_SU = DLVQTY_SU,
                            OCDQTY_BU = OCDQTY_BU,
                            OCDQTY_SU = OCDQTY_SU,
                            ORDQTY_BU = ORDQTY_BU,
                            ORDQTY_SU = ORDQTY_SU,
                            CREA_DLVDATE = CREA_DLVDATE,
                            REQ_DLVDATE = REQ_DLVDATE,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(soSL80).State = EntityState.Added;

                        #endregion InsertItem
                    }

                    #endregion

                    #region Text
                    //Header
                    if (IT_TEXT_HEADER != null && IT_TEXT_HEADER.Rows.Count > 0)
                    {
                        foreach (DataRow dataRowText in IT_TEXT_HEADER.Rows)
                        {
                            #region Collect Data
                            string SO = dataRowText["SO"].ToString();
                            string OBJECT = dataRowText["OBJECT"].ToString();
                            string TEXT_ID = dataRowText["TEXT_ID"].ToString();
                            string ID_NAME = dataRowText["ID_NAME"].ToString();
                            string LONGTEXT = dataRowText["LONGTEXT"].ToString();
                            #endregion

                            #region InsertItem
                            var soText80 = new SOTextHeader80Model
                            {
                                SOTextHeader80Id = Guid.NewGuid(),
                                SO = SO,
                                OBJECT = OBJECT,
                                TEXT_ID = TEXT_ID,
                                ID_NAME = ID_NAME,
                                LONGTEXT = LONGTEXT,
                                CreateTime = DateTime.Now
                            };
                            _context.Entry(soText80).State = EntityState.Added;
                            #endregion InsertItem
                        }
                    }
                    //Item
                    if (IT_TEXT_ITEM != null && IT_TEXT_ITEM.Rows.Count > 0)
                    {
                        foreach (DataRow dataRowText in IT_TEXT_ITEM.Rows)
                        {
                            #region Collect Data
                            string SO = dataRowText["SO"].ToString();
                            string SO_LINE = dataRowText["SO_LINE"].ToString();
                            string OBJECT = dataRowText["OBJECT"].ToString();
                            string TEXT_ID = dataRowText["TEXT_ID"].ToString();
                            string ID_NAME = dataRowText["ID_NAME"].ToString();
                            string LONGTEXT = dataRowText["LONGTEXT"].ToString();
                            #endregion

                            #region InsertItem
                            var soText80 = new SOTextItem80Model
                            {
                                SOTextItem80Id = Guid.NewGuid(),
                                SO = SO,
                                SO_LINE = SO_LINE,
                                OBJECT = OBJECT,
                                TEXT_ID = TEXT_ID,
                                ID_NAME = ID_NAME,
                                LONGTEXT = LONGTEXT,
                                CreateTime = DateTime.Now
                            };
                            _context.Entry(soText80).State = EntityState.Added;
                            #endregion InsertItem
                        }
                    }
                    #endregion

                    _context.SaveChanges();

                    //Xác nhận insert vào DB thành công cho SAP
                    var result = ConfirmInsert(VBELN);
                    if (result.IsSuccess == false)
                    {
                        _loggerRepository.Logging(string.Format("I_INSERT: E_ERROR (VBELN: {0})"
                                                                , VBELN), "ZMES_SALE_ORDER");
                    }
                }

                //The Transaction will be completed    
                //scope.Complete();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                if (ex.InnerException != null)
                {
                    error = ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        error = ex.InnerException.InnerException.Message;
                    }
                }
                //scope.Dispose();
            }
            //}
            return error;
        }

        private void syncMaterial(string CompanyCode, string MaterialCode)
        {
            // string error = string.Empty;
            var materialParam = new ParamRequestSyncSapModel
            {
                CompanyCode = CompanyCode,
                MATNR = MaterialCode,
            };
            //Thực hiện đồng bộ SAP
            //Khởi tạo context
            var _context = new EntityDataContext();
            //Khởi tạo repository
            var _materialMasterRepository = new MaterialMasterRepository(_context);
            var message = _materialMasterRepository.GetMaterialMaster(materialParam);
        }

        private void syncBOMSALE(string CompanyCode, string MaterialCode)
        {
            var paramRequest = new ParamRequestSyncSapModel
            {
                CompanyCode = CompanyCode,
                MATNR = MaterialCode,
            };
            var _BOMContext = new EntityDataContext();
            var _bomReposiroty = new BOMRepository(_BOMContext);
            var message = _bomReposiroty.SyncBOMSALE(paramRequest);
        }

        private void syncRouting(string WERKS, string MATNR)
        {
            EntityDataContext dataContext = new EntityDataContext();
            ParamRequestSyncSapModel paramRequest = new ParamRequestSyncSapModel
            {
                MATNR = MATNR,
                CompanyCode = WERKS
            };
            //Routing
            RoutingRepository _routingRepository = new RoutingRepository(dataContext);
            var message = _routingRepository.SyncRouting(paramRequest);
        }

        private string InsertSaleOrder100(DataTable IT_HEADER, DataTable IT_ITEM, DataTable LT_SCHEDULE_LINE, DataTable IT_TEXT_HEADER, DataTable IT_TEXT_ITEM)
        {
            string error = string.Empty;

            //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            //{
            try
            {
                foreach (DataRow dataRow in IT_HEADER.Rows)
                {
                    #region SOHeader

                    #region Collect Data
                    //var BEDAE = dataRow["BEDAE"].ToString();
                    var VBELN = dataRow["VBELN"].ToString();
                    DateTime? AUDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["AUDAT"].ToString()) && dataRow["AUDAT"].ToString() != "00000000")
                    {
                        string date = dataRow["AUDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        AUDAT = DateTime.Parse(date);
                    }

                    DateTime? ERDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["ERDAT"].ToString()) && dataRow["ERDAT"].ToString() != "00000000")
                    {
                        string date = dataRow["ERDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        ERDAT = DateTime.Parse(date);
                    }
                    TimeSpan? ERZET = null;
                    if (!string.IsNullOrEmpty(dataRow["ERZET"].ToString()))
                    {
                        ERZET = TimeSpan.Parse(dataRow["ERZET"].ToString());
                    }

                    var ERNAM = dataRow["ERNAM"].ToString();
                    var AUART = dataRow["AUART"].ToString();
                    var VKORG = dataRow["VKORG"].ToString();
                    var VTWEG = dataRow["VTWEG"].ToString();
                    var SPART = dataRow["SPART"].ToString();
                    var BSTNK = dataRow["BSTNK"].ToString();
                    var KUNNR = dataRow["KUNNR"].ToString();
                    var PSPSPPNR = dataRow["PS_PSP_PNR"].ToString();
                    var PS_PSP_PNR_OUTPUT = dataRow["PS_PSP_PNR_OUTPUT"].ToString();
                    DateTime? VDATU = null;
                    if (!string.IsNullOrEmpty(dataRow["VDATU"].ToString()) && dataRow["VDATU"].ToString() != "00000000")
                    {
                        string date = dataRow["VDATU"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        VDATU = DateTime.Parse(date);
                    }
                    var LFGSK = dataRow["LFGSK"].ToString();
                    var ZZTERM = dataRow["ZZTERM"].ToString();
                    var ZZTERM_DES = dataRow["ZZTERM_DES"].ToString();
                    var SORTL = dataRow["SORTL"].ToString();
                    #endregion

                    #region Insert
                    var saleHeaderNew = new SaleOrderHeader100Model
                    {
                        SO100HeaderId = Guid.NewGuid(),
                        AUART = AUART,
                        //BEDAE = BEDAE,
                        AUDAT = AUDAT,
                        BSTNK = BSTNK,
                        ERDAT = ERDAT,
                        ERNAM = ERNAM,
                        ERZET = ERZET,
                        KUNNR = KUNNR,
                        LFGSK = LFGSK,
                        PS_PSP_PNR = PSPSPPNR,
                        PS_PSP_PNR_OUTPUT = PS_PSP_PNR_OUTPUT,
                        SPART = SPART,
                        VBELN = VBELN,
                        VDATU = VDATU,
                        VKORG = VKORG,
                        VTWEG = VTWEG,
                        ZZTERM = ZZTERM,
                        ZZTERM_DES = ZZTERM_DES,
                        SORTL = SORTL,
                        CreateTime = DateTime.Now
                    };
                    _context.Entry(saleHeaderNew).State = EntityState.Added;
                    #endregion

                    #endregion

                    #region SOItem
                    foreach (DataRow dataRowItem in IT_ITEM.Rows)
                    {
                        var BEDAE_ITEM = dataRowItem["BEDAE"].ToString();
                        if (string.IsNullOrEmpty(BEDAE_ITEM) || BEDAE_ITEM == ConstSOType.ZO || BEDAE_ITEM == ConstSOType.ZP || BEDAE_ITEM == ConstSOType.ZS)
                        {
                            #region Collect Data

                            var VBELN_ITEM = dataRowItem["VBELN"].ToString();
                            var POSNR = dataRowItem["POSNR"].ToString();
                            string MATNR = string.Empty;
                            if (!string.IsNullOrEmpty(dataRowItem["MATNR"].ToString()))
                            {
                                MATNR = Int32.Parse(dataRowItem["MATNR"].ToString()).ToString();
                            }
                            var WERKS = dataRowItem["WERKS"].ToString();

                            //Sản phẩm: khi đồng bộ từ SAP về nếu không có (MES) phải kéo bằng được
                            var companyId = _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefault();
                            var productInDb = _context.ProductModel.Where(p => p.ERPProductCode == MATNR && p.CompanyId == companyId).FirstOrDefault();
                            if (productInDb == null && !string.IsNullOrEmpty(MATNR))
                            {
                                syncMaterial(WERKS, MATNR);
                            }
                            //Sync routing
                            //syncRouting(WERKS, MATNR);
                            //Sync bom sale
                            //Nếu ProductModel.MaterialType = "Z80" => Thì kéo ZMES_FM_BOM_SALE
                            if (productInDb != null)
                            {
                                if (productInDb.MTART == "Z80")
                                {
                                    syncBOMSALE(WERKS, MATNR);
                                }
                            }

                            var SOBKZ = dataRowItem["SOBKZ"].ToString();
                            var PS_PSP_PNR = dataRowItem["PS_PSP_PNR"].ToString();
                            var PS_PSP_PNR_OUTPUT_ITEM = dataRowItem["PS_PSP_PNR_OUTPUT"].ToString();
                            decimal? UMVKZ = null;
                            if (!string.IsNullOrEmpty(dataRowItem["UMVKZ"].ToString()))
                            {
                                UMVKZ = decimal.Parse(dataRowItem["UMVKZ"].ToString());
                            }
                            decimal? UMVKN = null;
                            if (!string.IsNullOrEmpty(dataRowItem["UMVKN"].ToString()))
                            {
                                UMVKN = decimal.Parse(dataRowItem["UMVKN"].ToString());
                            }
                            var MEINS = dataRowItem["MEINS"].ToString();
                            var ABGRU = dataRowItem["ABGRU"].ToString();
                            var GBSTA = dataRowItem["GBSTA"].ToString();
                            DateTime? ERDAT_ITEM = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ERDAT"].ToString()) && dataRowItem["ERDAT"].ToString() != "00000000")
                            {
                                string date = dataRowItem["ERDAT"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                ERDAT_ITEM = DateTime.Parse(date);
                            }
                            var ERNAM_ITEM = dataRowItem["ERNAM"].ToString();

                            TimeSpan? ERZET_ITEM = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ERZET"].ToString()))
                            {
                                ERZET_ITEM = TimeSpan.Parse(dataRowItem["ERZET"].ToString());
                            }
                            var ZZTERM_ITEM = dataRowItem["ZZTERM"].ToString();
                            var ZZTERM_DES_ITEM = dataRowItem["ZZTERM_DES"].ToString();
                            //=========================================================
                            var UEBTO = dataRowItem["UEBTO"].ToString();
                            var UNTTO = dataRowItem["UNTTO"].ToString();
                            decimal? KWMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["KWMENG"].ToString()))
                            {
                                KWMENG = decimal.Parse(dataRowItem["KWMENG"].ToString());
                            }
                            decimal? LSMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["LSMENG"].ToString()))
                            {
                                LSMENG = decimal.Parse(dataRowItem["LSMENG"].ToString());
                            }
                            decimal? KBMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["KBMENG"].ToString()))
                            {
                                KBMENG = decimal.Parse(dataRowItem["KBMENG"].ToString());
                            }
                            decimal? KLMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["KLMENG"].ToString()))
                            {
                                KLMENG = decimal.Parse(dataRowItem["KLMENG"].ToString());
                            }
                            var UPMAT = dataRowItem["UPMAT"].ToString();
                            if (!string.IsNullOrEmpty(UPMAT))
                            {
                                UPMAT = UPMAT.TrimStart(new Char[] { '0' });
                            }
                            var ZPYCSXDT = dataRowItem["ZPYCSXDT"].ToString();
                            var ZFLAG = dataRowItem["ZFLAG"].ToString();
                            DateTime? ZFDAT = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ZFDAT"].ToString()) && dataRowItem["ZFDAT"].ToString() != "00000000")
                            {
                                string date = dataRowItem["ZFDAT"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                ZFDAT = DateTime.Parse(date);
                            }
                            TimeSpan? ZFTM = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ZFTM"].ToString()))
                            {
                                ZFTM = TimeSpan.Parse(dataRowItem["ZFTM"].ToString());
                            }
                            var ZZSKU = dataRowItem["ZZSKU"].ToString();
                            var ZPTG = dataRowItem["ZPTG"].ToString();
                            var VRKME = dataRowItem["VRKME"].ToString();
                            #endregion
                            //1. Nếu có tồn tại thì cập nhật(trường hợp trên SAP trả về bị lặp data)
                            var so100InDb = _context.SaleOrderItem100Model.Where(p => p.VBELN == VBELN && p.POSNR == POSNR && p.WERKS == WERKS).FirstOrDefault();
                            if (so100InDb != null)
                            {
                                #region UpdateItem
                                so100InDb.POSNR = POSNR;
                                so100InDb.POSNR_MES = POSNR.TrimStart(new Char[] { '0' });
                                so100InDb.VBELN = VBELN_ITEM;
                                so100InDb.ABGRU = ABGRU;
                                so100InDb.BEDAE = BEDAE_ITEM;
                                so100InDb.ERDAT = ERDAT_ITEM;
                                so100InDb.ERNAM = ERNAM_ITEM;
                                so100InDb.ERZET = ERZET_ITEM;
                                so100InDb.GBSTA = GBSTA;
                                so100InDb.MATNR = MATNR;
                                so100InDb.MEINS = MEINS;
                                so100InDb.PS_PSP_PNR = PS_PSP_PNR;
                                so100InDb.PS_PSP_PNR_OUTPUT = PS_PSP_PNR_OUTPUT_ITEM;
                                so100InDb.SOBKZ = SOBKZ;
                                so100InDb.UMVKN = UMVKN;
                                so100InDb.UMVKZ = UMVKZ;
                                so100InDb.WERKS = WERKS;
                                so100InDb.ZZTERM = ZZTERM_ITEM;
                                so100InDb.ZZTERM_DES = ZZTERM_DES_ITEM;
                                so100InDb.UEBTO = UEBTO;
                                so100InDb.UNTTO = UNTTO;
                                so100InDb.KWMENG = KWMENG;
                                so100InDb.LSMENG = LSMENG;
                                so100InDb.KBMENG = KBMENG;
                                so100InDb.KLMENG = KLMENG;
                                so100InDb.UPMAT = UPMAT;
                                so100InDb.ZPYCSXDT = ZPYCSXDT;
                                so100InDb.ZFLAG = ZFLAG;
                                so100InDb.ZFDAT = ZFDAT;
                                so100InDb.ZFTM = ZFTM;
                                so100InDb.ZZSKU = ZZSKU;
                                so100InDb.ZPTG = ZPTG;
                                so100InDb.isDeleted = null;
                                so100InDb.DeletedTime = null;
                                so100InDb.VRKME = VRKME;
                                so100InDb.LastEditTime = DateTime.Now;
                                _context.Entry(so100InDb).State = EntityState.Modified;
                                #endregion UpdateItem
                            }
                            //2. Chưa có thì thêm mới
                            else
                            {
                                #region InsertItem
                                var SOItem = new SaleOrderItem100Model
                                {
                                    SO100ItemId = Guid.NewGuid(),
                                    POSNR = POSNR,
                                    POSNR_MES = POSNR.TrimStart(new Char[] { '0' }),
                                    VBELN = VBELN_ITEM,
                                    ABGRU = ABGRU,
                                    BEDAE = BEDAE_ITEM,
                                    ERDAT = ERDAT_ITEM,
                                    ERNAM = ERNAM_ITEM,
                                    ERZET = ERZET_ITEM,
                                    GBSTA = GBSTA,
                                    MATNR = MATNR,
                                    MEINS = MEINS,
                                    PS_PSP_PNR = PS_PSP_PNR,
                                    PS_PSP_PNR_OUTPUT = PS_PSP_PNR_OUTPUT_ITEM,
                                    SOBKZ = SOBKZ,
                                    UMVKN = UMVKN,
                                    UMVKZ = UMVKZ,
                                    WERKS = WERKS,
                                    ZZTERM = ZZTERM_ITEM,
                                    ZZTERM_DES = ZZTERM_DES_ITEM,
                                    UEBTO = UEBTO,
                                    UNTTO = UNTTO,
                                    KWMENG = KWMENG,
                                    LSMENG = LSMENG,
                                    KBMENG = KBMENG,
                                    KLMENG = KLMENG,
                                    UPMAT = UPMAT,
                                    ZPYCSXDT = ZPYCSXDT,
                                    ZFLAG = ZFLAG,
                                    ZFDAT = ZFDAT,
                                    ZFTM = ZFTM,
                                    ZZSKU = ZZSKU,
                                    ZPTG = ZPTG,
                                    VRKME = VRKME,
                                    CreateTime = DateTime.Now
                                };
                                _context.Entry(SOItem).State = EntityState.Added;

                                #endregion InsertItem
                            }
                        }
                        _context.SaveChanges();
                    }
                    #endregion

                    #region ScheduleLine
                    foreach (DataRow dataRowSL in LT_SCHEDULE_LINE.Rows)
                    {
                        #region Collect Data
                        string VBELN_ITEM = dataRowSL["VBELN"].ToString();
                        string POSNR = dataRowSL["POSNR"].ToString();
                        string ETENR = dataRowSL["ETENR"].ToString();
                        string LFREL = dataRowSL["LFREL"].ToString();
                        DateTime? EDATU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["EDATU"].ToString()) && dataRowSL["EDATU"].ToString() != "00000000")
                        {
                            string date = dataRowSL["EDATU"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            EDATU = DateTime.Parse(date);
                        }
                        decimal? WMENG = null;
                        if (!string.IsNullOrEmpty(dataRowSL["WMENG"].ToString()))
                        {
                            WMENG = decimal.Parse(dataRowSL["WMENG"].ToString());
                        }

                        decimal? BMENG = null;
                        if (!string.IsNullOrEmpty(dataRowSL["BMENG"].ToString()))
                        {
                            BMENG = decimal.Parse(dataRowSL["BMENG"].ToString());
                        }

                        string VRKME = dataRowSL["VRKME"].ToString();

                        decimal? LMENG = null;
                        if (!string.IsNullOrEmpty(dataRowSL["LMENG"].ToString()))
                        {
                            LMENG = decimal.Parse(dataRowSL["LMENG"].ToString());
                        }

                        string MEINS = dataRowSL["MEINS"].ToString();
                        string LIFSP = dataRowSL["LIFSP"].ToString();

                        decimal? DLVQTY_BU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["DLVQTY_BU"].ToString()))
                        {
                            DLVQTY_BU = decimal.Parse(dataRowSL["DLVQTY_BU"].ToString());
                        }

                        decimal? DLVQTY_SU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["DLVQTY_SU"].ToString()))
                        {
                            DLVQTY_SU = decimal.Parse(dataRowSL["DLVQTY_SU"].ToString());
                        }

                        decimal? OCDQTY_BU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["OCDQTY_BU"].ToString()))
                        {
                            OCDQTY_BU = decimal.Parse(dataRowSL["OCDQTY_BU"].ToString());
                        }

                        decimal? OCDQTY_SU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["OCDQTY_SU"].ToString()))
                        {
                            OCDQTY_SU = decimal.Parse(dataRowSL["OCDQTY_SU"].ToString());
                        }

                        decimal? ORDQTY_BU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["ORDQTY_BU"].ToString()))
                        {
                            ORDQTY_BU = decimal.Parse(dataRowSL["ORDQTY_BU"].ToString());
                        }

                        decimal? ORDQTY_SU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["ORDQTY_SU"].ToString()))
                        {
                            ORDQTY_SU = decimal.Parse(dataRowSL["ORDQTY_SU"].ToString());
                        }

                        DateTime? CREA_DLVDATE = null;
                        if (!string.IsNullOrEmpty(dataRowSL["CREA_DLVDATE"].ToString()) && dataRowSL["CREA_DLVDATE"].ToString() != "00000000")
                        {
                            string date = dataRowSL["CREA_DLVDATE"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            CREA_DLVDATE = DateTime.Parse(date);
                        }

                        DateTime? REQ_DLVDATE = null;
                        if (!string.IsNullOrEmpty(dataRowSL["REQ_DLVDATE"].ToString()) && dataRowSL["REQ_DLVDATE"].ToString() != "00000000")
                        {
                            string date = dataRowSL["REQ_DLVDATE"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            REQ_DLVDATE = DateTime.Parse(date);
                        }
                        #endregion

                        #region InsertItem
                        var soItem100 = new SO100ScheduleLineModel
                        {
                            SO100ScheduleLineId = Guid.NewGuid(),
                            VBELN = VBELN_ITEM,
                            POSNR = POSNR,
                            ETENR = ETENR,
                            LFREL = LFREL,
                            EDATU = EDATU,
                            WMENG = WMENG,
                            BMENG = BMENG,
                            VRKME = VRKME,
                            LMENG = LMENG,
                            MEINS = MEINS,
                            LIFSP = LIFSP,
                            DLVQTY_BU = DLVQTY_BU,
                            DLVQTY_SU = DLVQTY_SU,
                            OCDQTY_BU = OCDQTY_BU,
                            OCDQTY_SU = OCDQTY_SU,
                            ORDQTY_BU = ORDQTY_BU,
                            ORDQTY_SU = ORDQTY_SU,
                            CREA_DLVDATE = CREA_DLVDATE,
                            REQ_DLVDATE = REQ_DLVDATE,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(soItem100).State = EntityState.Added;

                        #endregion InsertItem
                    }

                    #endregion

                    #region Text
                    //Header
                    if (IT_TEXT_HEADER != null && IT_TEXT_HEADER.Rows.Count > 0)
                    {
                        foreach (DataRow dataRowText in IT_TEXT_HEADER.Rows)
                        {
                            #region Collect Data
                            string SO = dataRowText["SO"].ToString();
                            string OBJECT = dataRowText["OBJECT"].ToString();
                            string TEXT_ID = dataRowText["TEXT_ID"].ToString();
                            string ID_NAME = dataRowText["ID_NAME"].ToString();
                            string LONGTEXT = dataRowText["LONGTEXT"].ToString();
                            #endregion

                            #region InsertItem
                            var soText100 = new SOTextHeader100Model
                            {
                                SOTextHeader100Id = Guid.NewGuid(),
                                SO = SO,
                                OBJECT = OBJECT,
                                TEXT_ID = TEXT_ID,
                                ID_NAME = ID_NAME,
                                LONGTEXT = LONGTEXT,
                                CreateTime = DateTime.Now
                            };
                            _context.Entry(soText100).State = EntityState.Added;
                            #endregion InsertItem
                        }
                    }
                    //Item
                    if (IT_TEXT_ITEM != null && IT_TEXT_ITEM.Rows.Count > 0)
                    {
                        foreach (DataRow dataRowText in IT_TEXT_ITEM.Rows)
                        {
                            #region Collect Data
                            string SO = dataRowText["SO"].ToString();
                            string SO_LINE = dataRowText["SO_LINE"].ToString();
                            string OBJECT = dataRowText["OBJECT"].ToString();
                            string TEXT_ID = dataRowText["TEXT_ID"].ToString();
                            string ID_NAME = dataRowText["ID_NAME"].ToString();
                            string LONGTEXT = dataRowText["LONGTEXT"].ToString();
                            #endregion

                            #region InsertItem
                            var soText100 = new SOTextItem100Model
                            {
                                SOTextItem100Id = Guid.NewGuid(),
                                SO = SO,
                                SO_LINE = SO_LINE,
                                OBJECT = OBJECT,
                                TEXT_ID = TEXT_ID,
                                ID_NAME = ID_NAME,
                                LONGTEXT = LONGTEXT,
                                CreateTime = DateTime.Now
                            };
                            _context.Entry(soText100).State = EntityState.Added;
                            #endregion InsertItem
                        }
                    }
                    #endregion

                    _context.SaveChanges();

                    //Xác nhận insert vào DB thành công cho SAP
                    var result = ConfirmInsert(VBELN);
                    if (result.IsSuccess == false)
                    {
                        _loggerRepository.Logging(string.Format("I_INSERT: E_ERROR (VBELN: {0})"
                                                                 , VBELN), "ZMES_SALE_ORDER");
                    }
                }

                //The Transaction will be completed    
                //scope.Complete();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                if (ex.InnerException != null)
                {
                    error = ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        error = ex.InnerException.InnerException.Message;
                    }
                }
                //scope.Dispose();
            }
            //}
            return error;
        }

        private string UpdateSaleOrder100(DataTable IT_HEADER, DataTable IT_ITEM, DataTable LT_SCHEDULE_LINE, DataTable IT_TEXT_HEADER, DataTable IT_TEXT_ITEM)
        {
            string error = string.Empty;

            //using (TransactionScope scope = new TransactionScope())
            //{
            try
            {
                foreach (DataRow dataRow in IT_HEADER.Rows)
                {
                    #region SOHeader

                    #region Collect Data
                    //var BEDAE = dataRow["BEDAE"].ToString();
                    var VBELN = dataRow["VBELN"].ToString();
                    DateTime? AUDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["AUDAT"].ToString()) && dataRow["AUDAT"].ToString() != "00000000")
                    {
                        string date = dataRow["AUDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        AUDAT = DateTime.Parse(date);
                    }

                    DateTime? ERDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["ERDAT"].ToString()) && dataRow["ERDAT"].ToString() != "00000000")
                    {
                        string date = dataRow["ERDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        ERDAT = DateTime.Parse(date);
                    }
                    TimeSpan? ERZET = null;
                    if (!string.IsNullOrEmpty(dataRow["ERZET"].ToString()))
                    {
                        ERZET = TimeSpan.Parse(dataRow["ERZET"].ToString());
                    }

                    var ERNAM = dataRow["ERNAM"].ToString();
                    var AUART = dataRow["AUART"].ToString();
                    var VKORG = dataRow["VKORG"].ToString();
                    var VTWEG = dataRow["VTWEG"].ToString();
                    var SPART = dataRow["SPART"].ToString();
                    var BSTNK = dataRow["BSTNK"].ToString();
                    var KUNNR = dataRow["KUNNR"].ToString();
                    var PSPSPPNR = dataRow["PS_PSP_PNR"].ToString();
                    var PS_PSP_PNR_OUTPUT = dataRow["PS_PSP_PNR_OUTPUT"].ToString();
                    DateTime? VDATU = null;
                    if (!string.IsNullOrEmpty(dataRow["VDATU"].ToString()) && dataRow["VDATU"].ToString() != "00000000")
                    {
                        string date = dataRow["VDATU"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        VDATU = DateTime.Parse(date);
                    }
                    var LFGSK = dataRow["LFGSK"].ToString();
                    var ZZTERM = dataRow["ZZTERM"].ToString();
                    var ZZTERM_DES = dataRow["ZZTERM_DES"].ToString();
                    var SORTL = dataRow["SORTL"].ToString();
                    #endregion

                    var saleHeaderDb = _context.SaleOrderHeader100Model.Where(p => p.VBELN == VBELN).FirstOrDefault();
                    if (saleHeaderDb != null)
                    {
                        #region Update
                        saleHeaderDb.AUART = AUART;
                        saleHeaderDb.AUDAT = AUDAT;
                        saleHeaderDb.BSTNK = BSTNK;
                        saleHeaderDb.ERDAT = ERDAT;
                        saleHeaderDb.ERNAM = ERNAM;
                        saleHeaderDb.ERZET = ERZET;
                        saleHeaderDb.KUNNR = KUNNR;
                        saleHeaderDb.LFGSK = LFGSK;
                        saleHeaderDb.PS_PSP_PNR = PSPSPPNR;
                        saleHeaderDb.PS_PSP_PNR_OUTPUT = PS_PSP_PNR_OUTPUT;
                        saleHeaderDb.SPART = SPART;
                        saleHeaderDb.VBELN = VBELN;
                        saleHeaderDb.VDATU = VDATU;
                        saleHeaderDb.VKORG = VKORG;
                        saleHeaderDb.VTWEG = VTWEG;
                        saleHeaderDb.ZZTERM = ZZTERM;
                        saleHeaderDb.ZZTERM_DES = ZZTERM_DES;
                        saleHeaderDb.SORTL = SORTL;
                        saleHeaderDb.LastEditTime = DateTime.Now;
                        _context.Entry(saleHeaderDb).State = EntityState.Modified;
                        #endregion
                    }
                    //Cập nhật field thông tin KH cho SO80
                    //TODO: sau khi cập nhật xong thì xóa code này vì SO80 chỉ thêm mới
                    //var saleHeaderDb80 = _context.SaleOrderHeader80Model.Where(p => p.VBELN == VBELN).FirstOrDefault();
                    //if (saleHeaderDb80 != null)
                    //{
                    //    #region Update
                    //    saleHeaderDb80.SORTL = SORTL;
                    //    _context.Entry(saleHeaderDb80).State = EntityState.Modified;
                    //    #endregion
                    //}
                    #endregion

                    #region SOItem
                    List<SaleOrderItem100Model> listSOFromSAP = new List<SaleOrderItem100Model>();
                    foreach (DataRow dataRowItem in IT_ITEM.Rows)
                    {
                        listSOFromSAP.Add(new SaleOrderItem100Model
                        {
                            //SO NUMBER
                            VBELN = dataRowItem["VBELN"].ToString(),
                            //SO LINE NUMBER
                            POSNR = dataRowItem["POSNR"].ToString(),
                        });
                        //TYPE
                        var BEDAE_ITEM = dataRowItem["BEDAE"].ToString();
                        if (string.IsNullOrEmpty(BEDAE_ITEM) || BEDAE_ITEM == ConstSOType.ZO || BEDAE_ITEM == ConstSOType.ZP || BEDAE_ITEM == ConstSOType.ZS)
                        {
                            #region Collect Data
                            //SO NUMBER
                            var VBELN_ITEM = dataRowItem["VBELN"].ToString();
                            //SO LINE NUMBER
                            var POSNR = dataRowItem["POSNR"].ToString();
                            //MATERIAL NUMBER
                            string MATNR = string.Empty;
                            if (!string.IsNullOrEmpty(dataRowItem["MATNR"].ToString()))
                            {
                                MATNR = Int32.Parse(dataRowItem["MATNR"].ToString()).ToString();
                            }
                            var WERKS = dataRowItem["WERKS"].ToString();

                            //Sản phẩm: khi đồng bộ từ SAP về nếu không có (MES) phải kéo bằng được
                            var companyId = _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefault();
                            var productInDb = _context.ProductModel.Where(p => p.ERPProductCode == MATNR && p.CompanyId == companyId).FirstOrDefault();
                            if (productInDb == null && !string.IsNullOrEmpty(MATNR))
                            {
                                syncMaterial(WERKS, MATNR);
                            }
                            //Sync routing
                            //syncRouting(WERKS, MATNR);
                            //Sync bom sale
                            //Nếu ProductModel.MaterialType = "Z80" => Thì kéo ZMES_FM_BOM_SALE
                            if (productInDb != null)
                            {
                                if (productInDb.MTART == "Z80")
                                {
                                    syncBOMSALE(WERKS, MATNR);
                                }
                            }

                            var SOBKZ = dataRowItem["SOBKZ"].ToString();
                            var PS_PSP_PNR = dataRowItem["PS_PSP_PNR"].ToString();
                            var PS_PSP_PNR_OUTPUT_ITEM = dataRowItem["PS_PSP_PNR_OUTPUT"].ToString();
                            decimal? UMVKZ = null;
                            if (!string.IsNullOrEmpty(dataRowItem["UMVKZ"].ToString()))
                            {
                                UMVKZ = decimal.Parse(dataRowItem["UMVKZ"].ToString());
                            }
                            decimal? UMVKN = null;
                            if (!string.IsNullOrEmpty(dataRowItem["UMVKN"].ToString()))
                            {
                                UMVKN = decimal.Parse(dataRowItem["UMVKN"].ToString());
                            }
                            var MEINS = dataRowItem["MEINS"].ToString();
                            var ABGRU = dataRowItem["ABGRU"].ToString();
                            var GBSTA = dataRowItem["GBSTA"].ToString();
                            DateTime? ERDAT_ITEM = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ERDAT"].ToString()) && dataRowItem["ERDAT"].ToString() != "00000000")
                            {
                                string date = dataRowItem["ERDAT"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                ERDAT_ITEM = DateTime.Parse(date);
                            }
                            var ERNAM_ITEM = dataRowItem["ERNAM"].ToString();

                            TimeSpan? ERZET_ITEM = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ERZET"].ToString()))
                            {
                                ERZET_ITEM = TimeSpan.Parse(dataRowItem["ERZET"].ToString());
                            }
                            var ZZTERM_ITEM = dataRowItem["ZZTERM"].ToString();
                            var ZZTERM_DES_ITEM = dataRowItem["ZZTERM_DES"].ToString();
                            //=========================================================
                            var UEBTO = dataRowItem["UEBTO"].ToString();
                            var UNTTO = dataRowItem["UNTTO"].ToString();
                            decimal? KWMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["KWMENG"].ToString()))
                            {
                                KWMENG = decimal.Parse(dataRowItem["KWMENG"].ToString());
                            }
                            decimal? LSMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["LSMENG"].ToString()))
                            {
                                LSMENG = decimal.Parse(dataRowItem["LSMENG"].ToString());
                            }
                            decimal? KBMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["KBMENG"].ToString()))
                            {
                                KBMENG = decimal.Parse(dataRowItem["KBMENG"].ToString());
                            }
                            decimal? KLMENG = null;
                            if (!string.IsNullOrEmpty(dataRowItem["KLMENG"].ToString()))
                            {
                                KLMENG = decimal.Parse(dataRowItem["KLMENG"].ToString());
                            }
                            var UPMAT = dataRowItem["UPMAT"].ToString();
                            if (!string.IsNullOrEmpty(UPMAT))
                            {
                                UPMAT = UPMAT.TrimStart(new Char[] { '0' });
                            }
                            var ZPYCSXDT = dataRowItem["ZPYCSXDT"].ToString();
                            var ZFLAG = dataRowItem["ZFLAG"].ToString();
                            DateTime? ZFDAT = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ZFDAT"].ToString()) && dataRowItem["ZFDAT"].ToString() != "00000000")
                            {
                                string date = dataRowItem["ZFDAT"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                ZFDAT = DateTime.Parse(date);
                            }
                            TimeSpan? ZFTM = null;
                            if (!string.IsNullOrEmpty(dataRowItem["ZFTM"].ToString()))
                            {
                                ZFTM = TimeSpan.Parse(dataRowItem["ZFTM"].ToString());
                            }
                            var ZZSKU = dataRowItem["ZZSKU"].ToString();
                            var ZPTG = dataRowItem["ZPTG"].ToString();
                            var VRKME = dataRowItem["VRKME"].ToString();
                            #endregion

                            #region Update
                            //1. Nếu có tồn tại thì cập nhật 
                            var so100InDb = _context.SaleOrderItem100Model.Where(p => p.VBELN == VBELN && p.POSNR == POSNR && p.WERKS == WERKS).FirstOrDefault();
                            if (so100InDb != null)
                            {
                                #region UpdateItem
                                so100InDb.POSNR = POSNR;
                                so100InDb.POSNR_MES = POSNR.TrimStart(new Char[] { '0' });
                                so100InDb.VBELN = VBELN_ITEM;
                                so100InDb.ABGRU = ABGRU;
                                so100InDb.BEDAE = BEDAE_ITEM;
                                so100InDb.ERDAT = ERDAT_ITEM;
                                so100InDb.ERNAM = ERNAM_ITEM;
                                so100InDb.ERZET = ERZET_ITEM;
                                so100InDb.GBSTA = GBSTA;
                                so100InDb.MATNR = MATNR;
                                so100InDb.MEINS = MEINS;
                                so100InDb.PS_PSP_PNR = PS_PSP_PNR;
                                so100InDb.PS_PSP_PNR_OUTPUT = PS_PSP_PNR_OUTPUT_ITEM;
                                so100InDb.SOBKZ = SOBKZ;
                                so100InDb.UMVKN = UMVKN;
                                so100InDb.UMVKZ = UMVKZ;
                                so100InDb.WERKS = WERKS;
                                so100InDb.ZZTERM = ZZTERM_ITEM;
                                so100InDb.ZZTERM_DES = ZZTERM_DES_ITEM;
                                so100InDb.UEBTO = UEBTO;
                                so100InDb.UNTTO = UNTTO;
                                so100InDb.KWMENG = KWMENG;
                                so100InDb.LSMENG = LSMENG;
                                so100InDb.KBMENG = KBMENG;
                                so100InDb.KLMENG = KLMENG;
                                so100InDb.UPMAT = UPMAT;
                                so100InDb.ZPYCSXDT = ZPYCSXDT;
                                so100InDb.ZFLAG = ZFLAG;
                                so100InDb.ZFDAT = ZFDAT;
                                so100InDb.ZFTM = ZFTM;
                                so100InDb.ZZSKU = ZZSKU;
                                so100InDb.ZPTG = ZPTG;
                                so100InDb.isDeleted = null;
                                so100InDb.DeletedTime = null;
                                so100InDb.VRKME = VRKME;
                                so100InDb.LastEditTime = DateTime.Now;
                                _context.Entry(so100InDb).State = EntityState.Modified;
                                #endregion UpdateItem
                            }
                            //2. Chưa có thì thêm mới
                            else
                            {
                                #region InsertItem
                                var SOItem = new SaleOrderItem100Model
                                {
                                    SO100ItemId = Guid.NewGuid(),
                                    POSNR = POSNR,
                                    POSNR_MES = POSNR.TrimStart(new Char[] { '0' }),
                                    VBELN = VBELN_ITEM,
                                    ABGRU = ABGRU,
                                    BEDAE = BEDAE_ITEM,
                                    ERDAT = ERDAT_ITEM,
                                    ERNAM = ERNAM_ITEM,
                                    ERZET = ERZET_ITEM,
                                    GBSTA = GBSTA,
                                    MATNR = MATNR,
                                    MEINS = MEINS,
                                    PS_PSP_PNR = PS_PSP_PNR,
                                    PS_PSP_PNR_OUTPUT = PS_PSP_PNR_OUTPUT_ITEM,
                                    SOBKZ = SOBKZ,
                                    UMVKN = UMVKN,
                                    UMVKZ = UMVKZ,
                                    WERKS = WERKS,
                                    ZZTERM = ZZTERM_ITEM,
                                    ZZTERM_DES = ZZTERM_DES_ITEM,
                                    UEBTO = UEBTO,
                                    UNTTO = UNTTO,
                                    KWMENG = KWMENG,
                                    LSMENG = LSMENG,
                                    KBMENG = KBMENG,
                                    KLMENG = KLMENG,
                                    UPMAT = UPMAT,
                                    ZPYCSXDT = ZPYCSXDT,
                                    ZFLAG = ZFLAG,
                                    ZFDAT = ZFDAT,
                                    ZFTM = ZFTM,
                                    ZZSKU = ZZSKU,
                                    ZPTG = ZPTG,
                                    VRKME = VRKME,
                                    CreateTime = DateTime.Now
                                };
                                _context.Entry(SOItem).State = EntityState.Added;

                                #endregion InsertItem
                            }

                            //thêm những line BEDAE_ITEM = NULL cho SO Item 80 nếu chưa có
                            if (string.IsNullOrEmpty(BEDAE_ITEM))
                            {
                                var so80InDb = _context.SaleOrderItem80Model.Where(p => p.VBELN == VBELN && p.POSNR == POSNR && p.WERKS == WERKS).FirstOrDefault();
                                if (so80InDb == null)
                                {
                                    var SOItem = new SaleOrderItem80Model
                                    {
                                        SOItemId = Guid.NewGuid(),
                                        POSNR = POSNR,
                                        POSNR_MES = POSNR.TrimStart(new Char[] { '0' }),
                                        VBELN = VBELN_ITEM,
                                        ABGRU = ABGRU,
                                        BEDAE = BEDAE_ITEM,
                                        ERDAT = ERDAT_ITEM,
                                        ERNAM = ERNAM_ITEM,
                                        ERZET = ERZET_ITEM,
                                        GBSTA = GBSTA,
                                        MATNR = MATNR,
                                        MEINS = MEINS,
                                        PS_PSP_PNR = PS_PSP_PNR,
                                        PS_PSP_PNR_OUTPUT = PS_PSP_PNR_OUTPUT_ITEM,
                                        SOBKZ = SOBKZ,
                                        UMVKN = UMVKN,
                                        UMVKZ = UMVKZ,
                                        WERKS = WERKS,
                                        ZZTERM = ZZTERM_ITEM,
                                        ZZTERM_DES = ZZTERM_DES_ITEM,
                                        UEBTO = UEBTO,
                                        UNTTO = UNTTO,
                                        KWMENG = KWMENG,
                                        LSMENG = LSMENG,
                                        KBMENG = KBMENG,
                                        KLMENG = KLMENG,
                                        CreateTime = DateTime.Now
                                    };
                                    _context.Entry(SOItem).State = EntityState.Added;
                                }
                            }
                            #endregion
                        }
                        _context.SaveChanges();
                    }

                    //3. Nếu tồn tại trên MES mà trên SAP không có => trên SAP đã xóa SO LINE này => xóa trên MES => set isDeleted = true
                    if (listSOFromSAP != null && listSOFromSAP.Count > 0)
                    {
                        var existsList = (from a in _context.SaleOrderItem100Model
                                          where a.VBELN == VBELN
                                          select new { a.SO100ItemId, a.VBELN, a.POSNR }
                                               ).ToList();
                        var deleteSOItemLst = existsList.Where(t2 => !listSOFromSAP.Any(t1 => t1.VBELN == t2.VBELN && t1.POSNR == t2.POSNR)).ToList();
                        if (deleteSOItemLst != null && deleteSOItemLst.Count > 0)
                        {
                            foreach (var item in deleteSOItemLst)
                            {
                                var deleteItem = _context.SaleOrderItem100Model.Where(p => p.SO100ItemId == item.SO100ItemId).FirstOrDefault();
                                if (deleteItem != null)
                                {
                                    deleteItem.isDeleted = true;
                                    deleteItem.DeletedTime = DateTime.Now;
                                    _context.Entry(deleteItem).State = EntityState.Modified;
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                    #endregion

                    #region ScheduleLine
                    foreach (DataRow dataRowSL in LT_SCHEDULE_LINE.Rows)
                    {
                        #region Collect Data
                        string VBELN_ITEM = dataRowSL["VBELN"].ToString();
                        string POSNR = dataRowSL["POSNR"].ToString();
                        string ETENR = dataRowSL["ETENR"].ToString();
                        string LFREL = dataRowSL["LFREL"].ToString();
                        DateTime? EDATU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["EDATU"].ToString()) && dataRowSL["EDATU"].ToString() != "00000000")
                        {
                            string date = dataRowSL["EDATU"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            EDATU = DateTime.Parse(date);
                        }
                        decimal? WMENG = null;
                        if (!string.IsNullOrEmpty(dataRowSL["WMENG"].ToString()))
                        {
                            WMENG = decimal.Parse(dataRowSL["WMENG"].ToString());
                        }

                        decimal? BMENG = null;
                        if (!string.IsNullOrEmpty(dataRowSL["BMENG"].ToString()))
                        {
                            BMENG = decimal.Parse(dataRowSL["BMENG"].ToString());
                        }

                        string VRKME = dataRowSL["VRKME"].ToString();

                        decimal? LMENG = null;
                        if (!string.IsNullOrEmpty(dataRowSL["LMENG"].ToString()))
                        {
                            LMENG = decimal.Parse(dataRowSL["LMENG"].ToString());
                        }

                        string MEINS = dataRowSL["MEINS"].ToString();
                        string LIFSP = dataRowSL["LIFSP"].ToString();

                        decimal? DLVQTY_BU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["DLVQTY_BU"].ToString()))
                        {
                            DLVQTY_BU = decimal.Parse(dataRowSL["DLVQTY_BU"].ToString());
                        }

                        decimal? DLVQTY_SU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["DLVQTY_SU"].ToString()))
                        {
                            DLVQTY_SU = decimal.Parse(dataRowSL["DLVQTY_SU"].ToString());
                        }

                        decimal? OCDQTY_BU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["OCDQTY_BU"].ToString()))
                        {
                            OCDQTY_BU = decimal.Parse(dataRowSL["OCDQTY_BU"].ToString());
                        }

                        decimal? OCDQTY_SU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["OCDQTY_SU"].ToString()))
                        {
                            OCDQTY_SU = decimal.Parse(dataRowSL["OCDQTY_SU"].ToString());
                        }

                        decimal? ORDQTY_BU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["ORDQTY_BU"].ToString()))
                        {
                            ORDQTY_BU = decimal.Parse(dataRowSL["ORDQTY_BU"].ToString());
                        }

                        decimal? ORDQTY_SU = null;
                        if (!string.IsNullOrEmpty(dataRowSL["ORDQTY_SU"].ToString()))
                        {
                            ORDQTY_SU = decimal.Parse(dataRowSL["ORDQTY_SU"].ToString());
                        }

                        DateTime? CREA_DLVDATE = null;
                        if (!string.IsNullOrEmpty(dataRowSL["CREA_DLVDATE"].ToString()) && dataRowSL["CREA_DLVDATE"].ToString() != "00000000")
                        {
                            string date = dataRowSL["CREA_DLVDATE"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            CREA_DLVDATE = DateTime.Parse(date);
                        }

                        DateTime? REQ_DLVDATE = null;
                        if (!string.IsNullOrEmpty(dataRowSL["REQ_DLVDATE"].ToString()) && dataRowSL["REQ_DLVDATE"].ToString() != "00000000")
                        {
                            string date = dataRowSL["REQ_DLVDATE"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            REQ_DLVDATE = DateTime.Parse(date);
                        }
                        #endregion

                        var so100InDb = _context.SO100ScheduleLineModel.Where(p => p.VBELN == VBELN && p.POSNR == POSNR && p.ETENR == ETENR && p.EDATU == EDATU).FirstOrDefault();
                        if (so100InDb == null)
                        {
                            #region InsertItem
                            var soItem100 = new SO100ScheduleLineModel
                            {
                                SO100ScheduleLineId = Guid.NewGuid(),
                                VBELN = VBELN_ITEM,
                                POSNR = POSNR,
                                ETENR = ETENR,
                                LFREL = LFREL,
                                EDATU = EDATU,
                                WMENG = WMENG,
                                BMENG = BMENG,
                                VRKME = VRKME,
                                LMENG = LMENG,
                                MEINS = MEINS,
                                LIFSP = LIFSP,
                                DLVQTY_BU = DLVQTY_BU,
                                DLVQTY_SU = DLVQTY_SU,
                                OCDQTY_BU = OCDQTY_BU,
                                OCDQTY_SU = OCDQTY_SU,
                                ORDQTY_BU = ORDQTY_BU,
                                ORDQTY_SU = ORDQTY_SU,
                                CREA_DLVDATE = CREA_DLVDATE,
                                REQ_DLVDATE = REQ_DLVDATE,
                                CreateTime = DateTime.Now
                            };
                            _context.Entry(soItem100).State = EntityState.Added;

                            #endregion InsertItem
                        }
                        else
                        {
                            #region UpdateItem
                            so100InDb.VBELN = VBELN_ITEM;
                            so100InDb.POSNR = POSNR;
                            so100InDb.ETENR = ETENR;
                            so100InDb.LFREL = LFREL;
                            so100InDb.EDATU = EDATU;
                            so100InDb.WMENG = WMENG;
                            so100InDb.BMENG = BMENG;
                            so100InDb.VRKME = VRKME;
                            so100InDb.LMENG = LMENG;
                            so100InDb.MEINS = MEINS;
                            so100InDb.LIFSP = LIFSP;
                            so100InDb.DLVQTY_BU = DLVQTY_BU;
                            so100InDb.DLVQTY_SU = DLVQTY_SU;
                            so100InDb.OCDQTY_BU = OCDQTY_BU;
                            so100InDb.OCDQTY_SU = OCDQTY_SU;
                            so100InDb.ORDQTY_BU = ORDQTY_BU;
                            so100InDb.ORDQTY_SU = ORDQTY_SU;
                            so100InDb.CREA_DLVDATE = CREA_DLVDATE;
                            so100InDb.REQ_DLVDATE = REQ_DLVDATE;
                            so100InDb.LastEditTime = DateTime.Now;
                            _context.Entry(so100InDb).State = EntityState.Modified;
                            #endregion UpdateItem
                        }
                    }

                    #endregion

                    #region Text
                    //Header
                    if (IT_TEXT_HEADER != null && IT_TEXT_HEADER.Rows.Count > 0)
                    {
                        foreach (DataRow dataRowText in IT_TEXT_HEADER.Rows)
                        {
                            #region Collect Data
                            string SO = dataRowText["SO"].ToString();
                            string OBJECT = dataRowText["OBJECT"].ToString();
                            string TEXT_ID = dataRowText["TEXT_ID"].ToString();
                            string ID_NAME = dataRowText["ID_NAME"].ToString();
                            string LONGTEXT = dataRowText["LONGTEXT"].ToString();
                            #endregion

                            var so100InDb = _context.SOTextHeader100Model.Where(p => p.SO == SO && p.TEXT_ID == TEXT_ID).FirstOrDefault();
                            //Nếu chưa có thì thêm mới, có rồi thì cập nhật
                            if (so100InDb == null)
                            {
                                #region InsertItem
                                var soText100 = new SOTextHeader100Model
                                {
                                    SOTextHeader100Id = Guid.NewGuid(),
                                    SO = SO,
                                    OBJECT = OBJECT,
                                    TEXT_ID = TEXT_ID,
                                    ID_NAME = ID_NAME,
                                    LONGTEXT = LONGTEXT,
                                    CreateTime = DateTime.Now
                                };
                                _context.Entry(soText100).State = EntityState.Added;
                                #endregion InsertItem
                            }
                            else
                            {
                                #region UpdateItem
                                so100InDb.ID_NAME = ID_NAME;
                                so100InDb.LONGTEXT = LONGTEXT;
                                so100InDb.LastEditTime = DateTime.Now;
                                _context.Entry(so100InDb).State = EntityState.Modified;
                                #endregion UpdateItem
                            }
                        }
                    }
                    //Item
                    if (IT_TEXT_ITEM != null && IT_TEXT_ITEM.Rows.Count > 0)
                    {
                        foreach (DataRow dataRowText in IT_TEXT_ITEM.Rows)
                        {
                            #region Collect Data
                            string SO = dataRowText["SO"].ToString();
                            string SO_LINE = dataRowText["SO_LINE"].ToString();
                            string OBJECT = dataRowText["OBJECT"].ToString();
                            string TEXT_ID = dataRowText["TEXT_ID"].ToString();
                            string ID_NAME = dataRowText["ID_NAME"].ToString();
                            string LONGTEXT = dataRowText["LONGTEXT"].ToString();
                            #endregion

                            var so100InDb = _context.SOTextItem100Model.Where(p => p.SO == SO && p.SO_LINE == p.SO_LINE && p.TEXT_ID == TEXT_ID).FirstOrDefault();
                            //Nếu chưa có thì thêm mới, có rồi thì cập nhật
                            if (so100InDb == null)
                            {
                                #region InsertItem
                                var soText100 = new SOTextItem100Model
                                {
                                    SOTextItem100Id = Guid.NewGuid(),
                                    SO = SO,
                                    SO_LINE = SO_LINE,
                                    OBJECT = OBJECT,
                                    TEXT_ID = TEXT_ID,
                                    ID_NAME = ID_NAME,
                                    LONGTEXT = LONGTEXT,
                                    CreateTime = DateTime.Now
                                };
                                _context.Entry(soText100).State = EntityState.Added;
                                #endregion InsertItem
                            }
                            else
                            {
                                #region UpdateItem
                                so100InDb.ID_NAME = ID_NAME;
                                so100InDb.LONGTEXT = LONGTEXT;
                                so100InDb.LastEditTime = DateTime.Now;
                                _context.Entry(so100InDb).State = EntityState.Modified;
                                #endregion UpdateItem
                            }
                        }
                    }
                    #endregion

                    _context.SaveChanges();

                    //Xác nhận insert vào DB thành công cho SAP
                    var result = ConfirmInsert(VBELN);
                    if (result.IsSuccess == false)
                    {
                        _loggerRepository.Logging(string.Format("I_INSERT: E_ERROR (VBELN: {0})"
                                                                , VBELN), "ZMES_SALE_ORDER");
                    }
                }

                //The Transaction will be completed    
                //scope.Complete();
            }
            catch (Exception ex)
            {
                error = ex.Message;
                if (ex.InnerException != null)
                {
                    error = ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        error = ex.InnerException.InnerException.Message;
                    }
                }
                //scope.Dispose();
            }
            //}
            return error;
        }

        private async Task<string> InsertUpdateSheduleLine80(DataTable dataTable)
        {
            string error = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    #region Collect Data
                    string VBELN = dataRow["VBELN"].ToString();
                    string POSNR = dataRow["POSNR"].ToString();
                    string ETENR = dataRow["ETENR"].ToString();
                    string LFREL = dataRow["LFREL"].ToString();
                    DateTime? EDATU = null;
                    if (!string.IsNullOrEmpty(dataRow["EDATU"].ToString()) && dataRow["EDATU"].ToString() != "00000000")
                    {
                        string date = dataRow["EDATU"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        EDATU = DateTime.Parse(date);
                    }
                    decimal? WMENG = null;
                    if (!string.IsNullOrEmpty(dataRow["WMENG"].ToString()))
                    {
                        WMENG = decimal.Parse(dataRow["WMENG"].ToString());
                    }

                    decimal? BMENG = null;
                    if (!string.IsNullOrEmpty(dataRow["BMENG"].ToString()))
                    {
                        BMENG = decimal.Parse(dataRow["BMENG"].ToString());
                    }

                    string VRKME = dataRow["VRKME"].ToString();

                    decimal? LMENG = null;
                    if (!string.IsNullOrEmpty(dataRow["LMENG"].ToString()))
                    {
                        LMENG = decimal.Parse(dataRow["LMENG"].ToString());
                    }

                    string MEINS = dataRow["MEINS"].ToString();
                    string LIFSP = dataRow["LIFSP"].ToString();

                    decimal? DLVQTY_BU = null;
                    if (!string.IsNullOrEmpty(dataRow["DLVQTY_BU"].ToString()))
                    {
                        DLVQTY_BU = decimal.Parse(dataRow["DLVQTY_BU"].ToString());
                    }

                    decimal? DLVQTY_SU = null;
                    if (!string.IsNullOrEmpty(dataRow["DLVQTY_SU"].ToString()))
                    {
                        DLVQTY_SU = decimal.Parse(dataRow["DLVQTY_SU"].ToString());
                    }

                    decimal? OCDQTY_BU = null;
                    if (!string.IsNullOrEmpty(dataRow["OCDQTY_BU"].ToString()))
                    {
                        OCDQTY_BU = decimal.Parse(dataRow["OCDQTY_BU"].ToString());
                    }

                    decimal? OCDQTY_SU = null;
                    if (!string.IsNullOrEmpty(dataRow["OCDQTY_SU"].ToString()))
                    {
                        OCDQTY_SU = decimal.Parse(dataRow["OCDQTY_SU"].ToString());
                    }

                    decimal? ORDQTY_BU = null;
                    if (!string.IsNullOrEmpty(dataRow["ORDQTY_BU"].ToString()))
                    {
                        ORDQTY_BU = decimal.Parse(dataRow["ORDQTY_BU"].ToString());
                    }

                    decimal? ORDQTY_SU = null;
                    if (!string.IsNullOrEmpty(dataRow["ORDQTY_SU"].ToString()))
                    {
                        ORDQTY_SU = decimal.Parse(dataRow["ORDQTY_SU"].ToString());
                    }

                    DateTime? CREA_DLVDATE = null;
                    if (!string.IsNullOrEmpty(dataRow["CREA_DLVDATE"].ToString()) && dataRow["CREA_DLVDATE"].ToString() != "00000000")
                    {
                        string date = dataRow["CREA_DLVDATE"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        CREA_DLVDATE = DateTime.Parse(date);
                    }

                    DateTime? REQ_DLVDATE = null;
                    if (!string.IsNullOrEmpty(dataRow["REQ_DLVDATE"].ToString()) && dataRow["REQ_DLVDATE"].ToString() != "00000000")
                    {
                        string date = dataRow["REQ_DLVDATE"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        REQ_DLVDATE = DateTime.Parse(date);
                    }
                    #endregion

                    //var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefaultAsync();
                    var so80InDb = await _context.SO80ScheduleLineModel.Where(p => p.VBELN == VBELN && p.POSNR == POSNR && p.ETENR == ETENR && p.EDATU == EDATU).FirstOrDefaultAsync();
                    //Nếu chưa có product thì thêm mới: Có rồi thì update
                    if (so80InDb == null)
                    {
                        #region InsertItem
                        var soItem100 = new SO80ScheduleLineModel
                        {
                            SO80ScheduleLineId = Guid.NewGuid(),
                            VBELN = VBELN,
                            POSNR = POSNR,
                            ETENR = ETENR,
                            LFREL = LFREL,
                            EDATU = EDATU,
                            WMENG = WMENG,
                            BMENG = BMENG,
                            VRKME = VRKME,
                            LMENG = LMENG,
                            MEINS = MEINS,
                            LIFSP = LIFSP,
                            DLVQTY_BU = DLVQTY_BU,
                            DLVQTY_SU = DLVQTY_SU,
                            OCDQTY_BU = OCDQTY_BU,
                            OCDQTY_SU = OCDQTY_SU,
                            ORDQTY_BU = ORDQTY_BU,
                            ORDQTY_SU = ORDQTY_SU,
                            CREA_DLVDATE = CREA_DLVDATE,
                            REQ_DLVDATE = REQ_DLVDATE,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(soItem100).State = EntityState.Added;

                        #endregion InsertItem
                    }
                    else
                    {
                        #region UpdateItem
                        so80InDb.VBELN = VBELN;
                        so80InDb.POSNR = POSNR;
                        so80InDb.ETENR = ETENR;
                        so80InDb.LFREL = LFREL;
                        so80InDb.EDATU = EDATU;
                        so80InDb.WMENG = WMENG;
                        so80InDb.BMENG = BMENG;
                        so80InDb.VRKME = VRKME;
                        so80InDb.LMENG = LMENG;
                        so80InDb.MEINS = MEINS;
                        so80InDb.LIFSP = LIFSP;
                        so80InDb.DLVQTY_BU = DLVQTY_BU;
                        so80InDb.DLVQTY_SU = DLVQTY_SU;
                        so80InDb.OCDQTY_BU = OCDQTY_BU;
                        so80InDb.OCDQTY_SU = OCDQTY_SU;
                        so80InDb.ORDQTY_BU = ORDQTY_BU;
                        so80InDb.ORDQTY_SU = ORDQTY_SU;
                        so80InDb.CREA_DLVDATE = CREA_DLVDATE;
                        so80InDb.REQ_DLVDATE = REQ_DLVDATE;
                        so80InDb.LastEditTime = DateTime.Now;
                        _context.Entry(so80InDb).State = EntityState.Modified;
                        #endregion UpdateItem
                    }
                    // 2. CALL DetectChanges before SaveChanges
                    _context.ChangeTracker.DetectChanges();

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            error = ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }
            return error;
        }

        private async Task<string> InsertUpdateItem(DataTable dataTable)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    var BEDAE = dataRow["BEDAE"].ToString();
                    //Nếu là đơn 80
                    if (BEDAE == "ZNP")
                    {
                        #region Collect Data
                        var VBELN = dataRow["VBELN"].ToString();
                        var POSNR = dataRow["POSNR"].ToString();
                        string MATNR = string.Empty;
                        if (!string.IsNullOrEmpty(dataRow["MATNR"].ToString()))
                        {
                            MATNR = Int32.Parse(dataRow["MATNR"].ToString()).ToString();
                        }
                        var WERKS = dataRow["WERKS"].ToString();
                        int? ETENR = null;
                        if (dataTable.Columns.Contains("ETENR") && !string.IsNullOrEmpty(dataRow["ETENR"].ToString()))
                        {
                            ETENR = Int32.Parse(dataRow["ETENR"].ToString());
                        }
                        //var ARKTX = dataRow["ARKTX"].ToString();
                        DateTime? EDATU = null;
                        if (dataTable.Columns.Contains("EDATU") && !string.IsNullOrEmpty(dataRow["EDATU"].ToString()) && dataRow["EDATU"].ToString() != "00000000")
                        {
                            string date = dataRow["EDATU"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            EDATU = DateTime.Parse(date);
                        }
                        decimal? WMENG = null;
                        if (dataTable.Columns.Contains("WMENG") && !string.IsNullOrEmpty(dataRow["WMENG"].ToString()))
                        {
                            WMENG = decimal.Parse(dataRow["WMENG"].ToString());
                        }

                        string VRKME = string.Empty;
                        //var VRKME = dataRow["VRKME"].ToString();
                        decimal? LMENG = null;
                        if (dataTable.Columns.Contains("LMENG") && !string.IsNullOrEmpty(dataRow["LMENG"].ToString()))
                        {
                            LMENG = decimal.Parse(dataRow["LMENG"].ToString());
                        }
                        decimal? DLVQTY_BU = null;
                        if (dataTable.Columns.Contains("DLVQTY_BU") && !string.IsNullOrEmpty(dataRow["DLVQTY_BU"].ToString()))
                        {
                            DLVQTY_BU = decimal.Parse(dataRow["DLVQTY_BU"].ToString());
                        }
                        decimal? ORDQTY_BU = null;
                        if (dataTable.Columns.Contains("ORDQTY_BU") && !string.IsNullOrEmpty(dataRow["ORDQTY_BU"].ToString()))
                        {
                            ORDQTY_BU = decimal.Parse(dataRow["ORDQTY_BU"].ToString());
                        }
                        var SOBKZ = dataRow["SOBKZ"].ToString();
                        var PS_PSP_PNR = dataRow["PS_PSP_PNR"].ToString();
                        decimal? UMVKZ = null;
                        if (!string.IsNullOrEmpty(dataRow["UMVKZ"].ToString()))
                        {
                            UMVKZ = decimal.Parse(dataRow["UMVKZ"].ToString());
                        }
                        decimal? UMVKN = null;
                        if (!string.IsNullOrEmpty(dataRow["UMVKN"].ToString()))
                        {
                            UMVKN = decimal.Parse(dataRow["UMVKN"].ToString());
                        }
                        var MEINS = dataRow["MEINS"].ToString();
                        var ABGRU = dataRow["ABGRU"].ToString();
                        var GBSTA = dataRow["GBSTA"].ToString();
                        DateTime? ERDAT = null;
                        if (!string.IsNullOrEmpty(dataRow["ERDAT"].ToString()) && dataRow["ERDAT"].ToString() != "00000000")
                        {
                            string date = dataRow["ERDAT"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            ERDAT = DateTime.Parse(date);
                        }
                        var ERNAM = dataRow["ERNAM"].ToString();

                        TimeSpan? ERZET = null;
                        if (!string.IsNullOrEmpty(dataRow["ERZET"].ToString()))
                        {
                            ERZET = TimeSpan.Parse(dataRow["ERZET"].ToString());
                        }
                        var ZZTERM = dataRow["ZZTERM"].ToString();
                        var ZZTERM_DES = dataRow["ZZTERM_DES"].ToString();
                        #endregion

                        //var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefaultAsync();
                        var saleOrderDetailDb = await _context.SaleOrderItem80Model.Where(p => p.VBELN == VBELN && p.POSNR == POSNR).FirstOrDefaultAsync();
                        //Nếu chưa có product thì thêm mới: Có rồi thì update
                        if (saleOrderDetailDb == null)
                        {
                            #region InsertItem
                            var bomDetail = new SaleOrderItem80Model
                            {
                                SOItemId = Guid.NewGuid(),
                                POSNR = POSNR,
                                VBELN = VBELN,
                                ABGRU = ABGRU,
                                //ARKTX = ARKTX,
                                BEDAE = BEDAE,
                                DLVQTY_BU = DLVQTY_BU,
                                EDATU = EDATU,
                                ERDAT = ERDAT,
                                ERNAM = ERNAM,
                                ERZET = ERZET,
                                ETENR = ETENR,
                                GBSTA = GBSTA,
                                LMENG = LMENG,
                                MATNR = MATNR,
                                MEINS = MEINS,
                                ORDQTY_BU = ORDQTY_BU,
                                PS_PSP_PNR = PS_PSP_PNR,
                                SOBKZ = SOBKZ,
                                UMVKN = UMVKN,
                                UMVKZ = UMVKZ,
                                VRKME = VRKME,
                                WERKS = WERKS,
                                WMENG = WMENG,
                                ZZTERM = ZZTERM,
                                ZZTERM_DES = ZZTERM_DES,
                                CreateTime = DateTime.Now
                            };
                            _context.Entry(bomDetail).State = EntityState.Added;

                            #endregion InsertItem
                        }
                        else
                        {
                            #region UpdateItem
                            saleOrderDetailDb.VBELN = VBELN;
                            saleOrderDetailDb.POSNR = POSNR;
                            saleOrderDetailDb.VBELN = VBELN;
                            saleOrderDetailDb.ABGRU = ABGRU;
                            //saleOrderDetailDb.ARKTX = ARKTX;
                            saleOrderDetailDb.BEDAE = BEDAE;
                            saleOrderDetailDb.DLVQTY_BU = DLVQTY_BU;
                            saleOrderDetailDb.EDATU = EDATU;
                            saleOrderDetailDb.ERDAT = ERDAT;
                            saleOrderDetailDb.ERNAM = ERNAM;
                            saleOrderDetailDb.ERZET = ERZET;
                            saleOrderDetailDb.ETENR = ETENR;
                            saleOrderDetailDb.GBSTA = GBSTA;
                            saleOrderDetailDb.LMENG = LMENG;
                            saleOrderDetailDb.MATNR = MATNR;
                            saleOrderDetailDb.MEINS = MEINS;
                            saleOrderDetailDb.ORDQTY_BU = ORDQTY_BU;
                            saleOrderDetailDb.PS_PSP_PNR = PS_PSP_PNR;
                            saleOrderDetailDb.SOBKZ = SOBKZ;
                            saleOrderDetailDb.UMVKN = UMVKN;
                            saleOrderDetailDb.UMVKZ = UMVKZ;
                            saleOrderDetailDb.VRKME = VRKME;
                            saleOrderDetailDb.WERKS = WERKS;
                            saleOrderDetailDb.WMENG = WMENG;
                            saleOrderDetailDb.ZZTERM = ZZTERM;
                            saleOrderDetailDb.ZZTERM_DES = ZZTERM_DES;
                            _context.Entry(saleOrderDetailDb).State = EntityState.Modified;
                            #endregion UpdateItem
                            //updateTotal++;
                        }
                        _context.ChangeTracker.DetectChanges();
                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            error = ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }
            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMARM:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
            return error;
        }

        private async Task<Tuple<string, string>> InsertUpdateHeader(DataTable dataTable)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            string message = string.Empty;
            int insertTotal = 0, updateTotal = 0;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    var BEDAE = dataRow["BEDAE"].ToString();
                    //Nếu là đơn 80
                    if (BEDAE == "ZNP")
                    {
                        #region Collect Data
                        var VBELN = dataRow["VBELN"].ToString();
                        DateTime? AUDAT = null;
                        if (!string.IsNullOrEmpty(dataRow["AUDAT"].ToString()) && dataRow["AUDAT"].ToString() != "00000000")
                        {
                            string date = dataRow["AUDAT"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            AUDAT = DateTime.Parse(date);
                        }

                        DateTime? ERDAT = null;
                        if (!string.IsNullOrEmpty(dataRow["ERDAT"].ToString()) && dataRow["ERDAT"].ToString() != "00000000")
                        {
                            string date = dataRow["ERDAT"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            ERDAT = DateTime.Parse(date);
                        }
                        TimeSpan? ERZET = null;
                        if (!string.IsNullOrEmpty(dataRow["ERZET"].ToString()))
                        {
                            ERZET = TimeSpan.Parse(dataRow["ERZET"].ToString());
                        }

                        var ERNAM = dataRow["ERNAM"].ToString();
                        var AUART = dataRow["AUART"].ToString();
                        var VKORG = dataRow["VKORG"].ToString();
                        var VTWEG = dataRow["VTWEG"].ToString();
                        var SPART = dataRow["SPART"].ToString();
                        var BSTNK = dataRow["BSTNK"].ToString();
                        var KUNNR = dataRow["KUNNR"].ToString();
                        var PSPSPPNR = dataRow["PS_PSP_PNR"].ToString();
                        DateTime? VDATU = null;
                        if (!string.IsNullOrEmpty(dataRow["VDATU"].ToString()) && dataRow["VDATU"].ToString() != "00000000")
                        {
                            string date = dataRow["VDATU"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            VDATU = DateTime.Parse(date);
                        }
                        var LFGSK = dataRow["LFGSK"].ToString();
                        var ZZTERM = dataRow["ZZTERM"].ToString();
                        var ZZTERM_DES = dataRow["ZZTERM_DES"].ToString();
                        #endregion

                        var saleHeaderDb = await _context.SaleOrderHeader80Model.Where(p => p.VBELN == VBELN).FirstOrDefaultAsync();
                        //Nếu chưa có thì thêm mới : Có rồi thì update
                        if (saleHeaderDb == null)
                        {
                            #region Insert
                            var saleHeaderNew = new SaleOrderHeader80Model
                            {
                                SOHeaderId = Guid.NewGuid(),
                                AUART = AUART,
                                BEDAE = BEDAE,
                                AUDAT = AUDAT,
                                BSTNK = BSTNK,
                                ERDAT = ERDAT,
                                ERNAM = ERNAM,
                                ERZET = ERZET,
                                KUNNR = KUNNR,
                                LFGSK = LFGSK,
                                PS_PSP_PNR = PSPSPPNR,
                                SPART = SPART,
                                VBELN = VBELN,
                                VDATU = VDATU,
                                VKORG = VKORG,
                                VTWEG = VTWEG,
                                ZZTERM = ZZTERM,
                                ZZTERM_DES = ZZTERM_DES,
                                CreateTime = DateTime.Now
                            };
                            _context.Entry(saleHeaderNew).State = EntityState.Added;
                            #endregion
                            insertTotal++;
                        }
                        else
                        {
                            #region Update
                            saleHeaderDb.AUART = AUART;
                            saleHeaderDb.AUDAT = AUDAT;
                            saleHeaderDb.BSTNK = BSTNK;
                            saleHeaderDb.ERDAT = ERDAT;
                            saleHeaderDb.ERNAM = ERNAM;
                            saleHeaderDb.ERZET = ERZET;
                            saleHeaderDb.KUNNR = KUNNR;
                            saleHeaderDb.LFGSK = LFGSK;
                            saleHeaderDb.PS_PSP_PNR = PSPSPPNR;
                            saleHeaderDb.SPART = SPART;
                            saleHeaderDb.VBELN = VBELN;
                            saleHeaderDb.VDATU = VDATU;
                            saleHeaderDb.VKORG = VKORG;
                            saleHeaderDb.VTWEG = VTWEG;
                            _context.Entry(saleHeaderDb).State = EntityState.Modified;
                            #endregion
                        }
                        _context.ChangeTracker.DetectChanges();
                        await _context.SaveChangesAsync();
                        //Xác nhận insert vào DB thành công cho SAP
                        ConfirmInsert(VBELN, BEDAE);
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            error = ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }
            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMaterial:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
            message += $"Insert: {insertTotal}, Update: {updateTotal}. Total: {dataTable.Rows.Count}";

            return new Tuple<string, string>(message, error);

        }
        #endregion
        public string ResetSaleOrder()
        {
            string result = string.Empty;
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_RESET_MASTERDATA);

                //function.SetValue("I_ACTION", "SALE_ORDER");

                //function.Invoke(destination);

                //result = function.GetString("E_RECORD");
            }
            catch (Exception ex)
            {
                result = ex.Message;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        result = ex.InnerException.InnerException.Message;
                    }
                }
            }
            var newLog = new MesSyncLogModel
            {
                LogTime = DateTime.Now,
                LogType = "Infomation",
                Description = "Reset: " + result
            };
            _context.Entry(newLog).State = EntityState.Added;
            _context.SaveChanges();
            return result;
        }

        #region SaleOrder 100
        private async Task<string> InsertUpdateSheduleLine100(DataTable dataTable)
        {
            string error = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    #region Collect Data
                    string VBELN = dataRow["VBELN"].ToString();
                    string POSNR = dataRow["POSNR"].ToString();
                    string ETENR = dataRow["ETENR"].ToString();
                    string LFREL = dataRow["LFREL"].ToString();
                    DateTime? EDATU = null;
                    if (!string.IsNullOrEmpty(dataRow["EDATU"].ToString()) && dataRow["EDATU"].ToString() != "00000000")
                    {
                        string date = dataRow["EDATU"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        EDATU = DateTime.Parse(date);
                    }
                    decimal? WMENG = null;
                    if (!string.IsNullOrEmpty(dataRow["WMENG"].ToString()))
                    {
                        WMENG = decimal.Parse(dataRow["WMENG"].ToString());
                    }

                    decimal? BMENG = null;
                    if (!string.IsNullOrEmpty(dataRow["BMENG"].ToString()))
                    {
                        BMENG = decimal.Parse(dataRow["BMENG"].ToString());
                    }

                    string VRKME = dataRow["VRKME"].ToString();

                    decimal? LMENG = null;
                    if (!string.IsNullOrEmpty(dataRow["LMENG"].ToString()))
                    {
                        LMENG = decimal.Parse(dataRow["LMENG"].ToString());
                    }

                    string MEINS = dataRow["MEINS"].ToString();
                    string LIFSP = dataRow["LIFSP"].ToString();

                    decimal? DLVQTY_BU = null;
                    if (!string.IsNullOrEmpty(dataRow["DLVQTY_BU"].ToString()))
                    {
                        DLVQTY_BU = decimal.Parse(dataRow["DLVQTY_BU"].ToString());
                    }

                    decimal? DLVQTY_SU = null;
                    if (!string.IsNullOrEmpty(dataRow["DLVQTY_SU"].ToString()))
                    {
                        DLVQTY_SU = decimal.Parse(dataRow["DLVQTY_SU"].ToString());
                    }

                    decimal? OCDQTY_BU = null;
                    if (!string.IsNullOrEmpty(dataRow["OCDQTY_BU"].ToString()))
                    {
                        OCDQTY_BU = decimal.Parse(dataRow["OCDQTY_BU"].ToString());
                    }

                    decimal? OCDQTY_SU = null;
                    if (!string.IsNullOrEmpty(dataRow["OCDQTY_SU"].ToString()))
                    {
                        OCDQTY_SU = decimal.Parse(dataRow["OCDQTY_SU"].ToString());
                    }

                    decimal? ORDQTY_BU = null;
                    if (!string.IsNullOrEmpty(dataRow["ORDQTY_BU"].ToString()))
                    {
                        ORDQTY_BU = decimal.Parse(dataRow["ORDQTY_BU"].ToString());
                    }

                    decimal? ORDQTY_SU = null;
                    if (!string.IsNullOrEmpty(dataRow["ORDQTY_SU"].ToString()))
                    {
                        ORDQTY_SU = decimal.Parse(dataRow["ORDQTY_SU"].ToString());
                    }

                    DateTime? CREA_DLVDATE = null;
                    if (!string.IsNullOrEmpty(dataRow["CREA_DLVDATE"].ToString()) && dataRow["CREA_DLVDATE"].ToString() != "00000000")
                    {
                        string date = dataRow["CREA_DLVDATE"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        CREA_DLVDATE = DateTime.Parse(date);
                    }

                    DateTime? REQ_DLVDATE = null;
                    if (!string.IsNullOrEmpty(dataRow["REQ_DLVDATE"].ToString()) && dataRow["REQ_DLVDATE"].ToString() != "00000000")
                    {
                        string date = dataRow["REQ_DLVDATE"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        REQ_DLVDATE = DateTime.Parse(date);
                    }
                    #endregion

                    //var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefaultAsync();
                    var so100InDb = await _context.SO100ScheduleLineModel.Where(p => p.VBELN == VBELN && p.POSNR == POSNR && p.ETENR == ETENR && p.EDATU == EDATU).FirstOrDefaultAsync();
                    //Nếu chưa có product thì thêm mới: Có rồi thì update
                    if (so100InDb == null)
                    {
                        #region InsertItem
                        var soItem100 = new SO100ScheduleLineModel
                        {
                            SO100ScheduleLineId = Guid.NewGuid(),
                            VBELN = VBELN,
                            POSNR = POSNR,
                            ETENR = ETENR,
                            LFREL = LFREL,
                            EDATU = EDATU,
                            WMENG = WMENG,
                            BMENG = BMENG,
                            VRKME = VRKME,
                            LMENG = LMENG,
                            MEINS = MEINS,
                            LIFSP = LIFSP,
                            DLVQTY_BU = DLVQTY_BU,
                            DLVQTY_SU = DLVQTY_SU,
                            OCDQTY_BU = OCDQTY_BU,
                            OCDQTY_SU = OCDQTY_SU,
                            ORDQTY_BU = ORDQTY_BU,
                            ORDQTY_SU = ORDQTY_SU,
                            CREA_DLVDATE = CREA_DLVDATE,
                            REQ_DLVDATE = REQ_DLVDATE,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(soItem100).State = EntityState.Added;

                        #endregion InsertItem
                    }
                    else
                    {
                        #region UpdateItem
                        so100InDb.VBELN = VBELN;
                        so100InDb.POSNR = POSNR;
                        so100InDb.ETENR = ETENR;
                        so100InDb.LFREL = LFREL;
                        so100InDb.EDATU = EDATU;
                        so100InDb.WMENG = WMENG;
                        so100InDb.BMENG = BMENG;
                        so100InDb.VRKME = VRKME;
                        so100InDb.LMENG = LMENG;
                        so100InDb.MEINS = MEINS;
                        so100InDb.LIFSP = LIFSP;
                        so100InDb.DLVQTY_BU = DLVQTY_BU;
                        so100InDb.DLVQTY_SU = DLVQTY_SU;
                        so100InDb.OCDQTY_BU = OCDQTY_BU;
                        so100InDb.OCDQTY_SU = OCDQTY_SU;
                        so100InDb.ORDQTY_BU = ORDQTY_BU;
                        so100InDb.ORDQTY_SU = ORDQTY_SU;
                        so100InDb.CREA_DLVDATE = CREA_DLVDATE;
                        so100InDb.REQ_DLVDATE = REQ_DLVDATE;
                        so100InDb.LastEditTime = DateTime.Now;
                        _context.Entry(so100InDb).State = EntityState.Modified;
                        #endregion UpdateItem
                    }
                    // 2. CALL DetectChanges before SaveChanges
                    _context.ChangeTracker.DetectChanges();

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            error = ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }
            return error;
        }

        private async Task<string> InsertUpdateItem100(DataTable dataTable, bool isGetRefer)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    var BEDAE = dataRow["BEDAE"].ToString();
                    //Nếu không phải là đơn 80 thì mới add
                    if (BEDAE != "ZNP")
                    {
                        #region Collect Data
                        var VBELN = dataRow["VBELN"].ToString();
                        var POSNR = dataRow["POSNR"].ToString();
                        string MATNR = string.Empty;
                        if (!string.IsNullOrEmpty(dataRow["MATNR"].ToString()))
                        {
                            MATNR = Int32.Parse(dataRow["MATNR"].ToString()).ToString();
                        }
                        var WERKS = dataRow["WERKS"].ToString();
                        int? ETENR = null;
                        if (dataTable.Columns.Contains("ETENR") && !string.IsNullOrEmpty(dataRow["ETENR"].ToString()))
                        {
                            ETENR = Int32.Parse(dataRow["ETENR"].ToString());
                        }
                        //var ARKTX = dataRow["ARKTX"].ToString();
                        DateTime? EDATU = null;
                        if (dataTable.Columns.Contains("EDATU") && !string.IsNullOrEmpty(dataRow["EDATU"].ToString()) && dataRow["EDATU"].ToString() != "00000000")
                        {
                            string date = dataRow["EDATU"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            EDATU = DateTime.Parse(date);
                        }
                        decimal? WMENG = null;
                        if (dataTable.Columns.Contains("WMENG") && !string.IsNullOrEmpty(dataRow["WMENG"].ToString()))
                        {
                            WMENG = decimal.Parse(dataRow["WMENG"].ToString());
                        }
                        string VRKME = string.Empty;
                        //var VRKME = dataRow["VRKME"].ToString();
                        decimal? LMENG = null;
                        if (dataTable.Columns.Contains("LMENG") && !string.IsNullOrEmpty(dataRow["LMENG"].ToString()))
                        {
                            LMENG = decimal.Parse(dataRow["LMENG"].ToString());
                        }
                        decimal? DLVQTY_BU = null;
                        if (dataTable.Columns.Contains("DLVQTY_BU") && !string.IsNullOrEmpty(dataRow["DLVQTY_BU"].ToString()))
                        {
                            DLVQTY_BU = decimal.Parse(dataRow["DLVQTY_BU"].ToString());
                        }
                        decimal? ORDQTY_BU = null;
                        if (dataTable.Columns.Contains("ORDQTY_BU") && !string.IsNullOrEmpty(dataRow["ORDQTY_BU"].ToString()))
                        {
                            ORDQTY_BU = decimal.Parse(dataRow["ORDQTY_BU"].ToString());
                        }
                        var SOBKZ = dataRow["SOBKZ"].ToString();
                        var PS_PSP_PNR = dataRow["PS_PSP_PNR"].ToString();
                        decimal? UMVKZ = null;
                        if (!string.IsNullOrEmpty(dataRow["UMVKZ"].ToString()))
                        {
                            UMVKZ = decimal.Parse(dataRow["UMVKZ"].ToString());
                        }
                        decimal? UMVKN = null;
                        if (!string.IsNullOrEmpty(dataRow["UMVKN"].ToString()))
                        {
                            UMVKN = decimal.Parse(dataRow["UMVKN"].ToString());
                        }
                        var MEINS = dataRow["MEINS"].ToString();
                        var ABGRU = dataRow["ABGRU"].ToString();
                        var GBSTA = dataRow["GBSTA"].ToString();
                        DateTime? ERDAT = null;
                        if (!string.IsNullOrEmpty(dataRow["ERDAT"].ToString()) && dataRow["ERDAT"].ToString() != "00000000")
                        {
                            string date = dataRow["ERDAT"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            ERDAT = DateTime.Parse(date);
                        }
                        var ERNAM = dataRow["ERNAM"].ToString();

                        TimeSpan? ERZET = null;
                        if (!string.IsNullOrEmpty(dataRow["ERZET"].ToString()))
                        {
                            ERZET = TimeSpan.Parse(dataRow["ERZET"].ToString());
                        }
                        var ZZTERM = dataRow["ZZTERM"].ToString();
                        var ZZTERM_DES = dataRow["ZZTERM_DES"].ToString();
                        #endregion

                        if (isGetRefer)
                        {
                            //Sync Material
                            EntityDataContext dataContext = new EntityDataContext();
                            ParamRequestSyncSapModel paramRequest = new ParamRequestSyncSapModel
                            {
                                MATNR = MATNR,
                                CompanyCode = WERKS
                            };
                            //Material
                            MaterialMasterRepository _materialRepository = new MaterialMasterRepository(dataContext);
                            await _materialRepository.GetMaterialMaster(paramRequest);
                            //Product Version
                            ProductVerRepository _productVerRepository = new ProductVerRepository(dataContext);
                            await _productVerRepository.SyncProductVersion(paramRequest);
                            //BOM
                            BOMRepository _BOMRepository = new BOMRepository(dataContext);
                            await _BOMRepository.SyncBOM(paramRequest);
                            //Routing
                            RoutingRepository _routingRepository = new RoutingRepository(dataContext);
                            await _routingRepository.SyncRouting(paramRequest);
                            //Production Order
                            ProductOrderRepository _productOrderRepository = new ProductOrderRepository(dataContext);
                            await _productOrderRepository.SyncProductionOrder(new ParamRequestSyncSapModel { VBELN = VBELN });
                        }
                        //var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefaultAsync();
                        var so100InDb = await _context.SaleOrderItem100Model.Where(p => p.VBELN == VBELN && p.POSNR == POSNR && p.WERKS == WERKS && p.ETENR == ETENR).FirstOrDefaultAsync();
                        //Nếu chưa có product thì thêm mới: Có rồi thì update
                        if (so100InDb == null)
                        {
                            #region InsertItem
                            var soItem100 = new SaleOrderItem100Model
                            {
                                SO100ItemId = Guid.NewGuid(),
                                POSNR = POSNR,
                                VBELN = VBELN,
                                ABGRU = ABGRU,
                                //ARKTX = ARKTX,
                                BEDAE = BEDAE,
                                DLVQTY_BU = DLVQTY_BU,
                                EDATU = EDATU,
                                ERDAT = ERDAT,
                                ERNAM = ERNAM,
                                ERZET = ERZET,
                                ETENR = ETENR,
                                GBSTA = GBSTA,
                                LMENG = LMENG,
                                MATNR = MATNR,
                                MEINS = MEINS,
                                ORDQTY_BU = ORDQTY_BU,
                                PS_PSP_PNR = PS_PSP_PNR,
                                SOBKZ = SOBKZ,
                                UMVKN = UMVKN,
                                UMVKZ = UMVKZ,
                                VRKME = VRKME,
                                WERKS = WERKS,
                                WMENG = WMENG,
                                ZZTERM = ZZTERM,
                                ZZTERM_DES = ZZTERM_DES,
                                CreateTime = DateTime.Now
                            };
                            _context.Entry(soItem100).State = EntityState.Added;

                            #endregion InsertItem
                        }
                        else
                        {
                            #region UpdateItem
                            so100InDb.VBELN = VBELN;
                            so100InDb.POSNR = POSNR;
                            so100InDb.VBELN = VBELN;
                            so100InDb.ABGRU = ABGRU;
                            so100InDb.BEDAE = BEDAE;
                            so100InDb.DLVQTY_BU = DLVQTY_BU;
                            so100InDb.EDATU = EDATU;
                            so100InDb.ERDAT = ERDAT;
                            so100InDb.ERNAM = ERNAM;
                            so100InDb.ERZET = ERZET;
                            so100InDb.ETENR = ETENR;
                            so100InDb.GBSTA = GBSTA;
                            so100InDb.LMENG = LMENG;
                            so100InDb.MATNR = MATNR;
                            so100InDb.MEINS = MEINS;
                            so100InDb.ORDQTY_BU = ORDQTY_BU;
                            so100InDb.PS_PSP_PNR = PS_PSP_PNR;
                            so100InDb.SOBKZ = SOBKZ;
                            so100InDb.UMVKN = UMVKN;
                            so100InDb.UMVKZ = UMVKZ;
                            so100InDb.VRKME = VRKME;
                            so100InDb.WERKS = WERKS;
                            so100InDb.WMENG = WMENG;
                            so100InDb.ZZTERM = ZZTERM;
                            so100InDb.ZZTERM_DES = ZZTERM_DES;
                            so100InDb.LastEditTime = DateTime.Now;
                            _context.Entry(so100InDb).State = EntityState.Modified;
                            #endregion UpdateItem
                        }
                        // 2. CALL DetectChanges before SaveChanges
                        _context.ChangeTracker.DetectChanges();

                        await _context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            error = ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }
            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMARM:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
            return error;
        }

        private async Task<Tuple<string, string>> InsertUpdateHeader100(DataTable dataTable)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            string message = string.Empty;
            int insertTotal = 0, updateTotal = 0;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {

                    var BEDAE = dataRow["BEDAE"].ToString();
                    if (BEDAE != "ZNP")
                    {
                        #region Collect Data
                        var VBELN = dataRow["VBELN"].ToString();
                        DateTime? AUDAT = null;
                        if (!string.IsNullOrEmpty(dataRow["AUDAT"].ToString()) && dataRow["AUDAT"].ToString() != "00000000")
                        {
                            string date = dataRow["AUDAT"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            AUDAT = DateTime.Parse(date);
                        }

                        DateTime? ERDAT = null;
                        if (!string.IsNullOrEmpty(dataRow["ERDAT"].ToString()) && dataRow["ERDAT"].ToString() != "00000000")
                        {
                            string date = dataRow["ERDAT"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            ERDAT = DateTime.Parse(date);
                        }
                        TimeSpan? ERZET = null;
                        if (!string.IsNullOrEmpty(dataRow["ERZET"].ToString()))
                        {
                            ERZET = TimeSpan.Parse(dataRow["ERZET"].ToString());
                        }

                        var ERNAM = dataRow["ERNAM"].ToString();
                        var AUART = dataRow["AUART"].ToString();
                        var VKORG = dataRow["VKORG"].ToString();
                        var VTWEG = dataRow["VTWEG"].ToString();
                        var SPART = dataRow["SPART"].ToString();
                        var BSTNK = dataRow["BSTNK"].ToString();
                        var KUNNR = dataRow["KUNNR"].ToString();
                        var PSPSPPNR = dataRow["PS_PSP_PNR"].ToString();
                        DateTime? VDATU = null;
                        if (!string.IsNullOrEmpty(dataRow["VDATU"].ToString()) && dataRow["VDATU"].ToString() != "00000000")
                        {
                            string date = dataRow["VDATU"].ToString();
                            date = date.Insert(4, "-");
                            date = date.Insert(7, "-");
                            VDATU = DateTime.Parse(date);
                        }
                        var LFGSK = dataRow["LFGSK"].ToString();
                        var ZZTERM = dataRow["ZZTERM"].ToString();
                        var ZZTERM_DES = dataRow["ZZTERM_DES"].ToString();
                        #endregion

                        var saleHeader100Db = await _context.SaleOrderHeader100Model.Where(p => p.VBELN == VBELN).FirstOrDefaultAsync();
                        //Nếu chưa có thì thêm mới : Có rồi thì update
                        if (saleHeader100Db == null)
                        {
                            #region Insert
                            var saleHeader100New = new SaleOrderHeader100Model
                            {
                                SO100HeaderId = Guid.NewGuid(),
                                AUART = AUART,
                                BEDAE = BEDAE,
                                AUDAT = AUDAT,
                                BSTNK = BSTNK,
                                ERDAT = ERDAT,
                                ERNAM = ERNAM,
                                ERZET = ERZET,
                                KUNNR = KUNNR,
                                LFGSK = LFGSK,
                                PS_PSP_PNR = PSPSPPNR,
                                SPART = SPART,
                                VBELN = VBELN,
                                VDATU = VDATU,
                                VKORG = VKORG,
                                VTWEG = VTWEG,
                                ZZTERM = ZZTERM,
                                ZZTERM_DES = ZZTERM_DES,
                                CreateTime = DateTime.Now
                            };
                            _context.Entry(saleHeader100New).State = EntityState.Added;
                            #endregion
                            insertTotal++;
                        }
                        else
                        {
                            #region Update
                            saleHeader100Db.AUART = AUART;
                            saleHeader100Db.BEDAE = BEDAE;
                            saleHeader100Db.AUDAT = AUDAT;
                            saleHeader100Db.BSTNK = BSTNK;
                            saleHeader100Db.ERDAT = ERDAT;
                            saleHeader100Db.ERNAM = ERNAM;
                            saleHeader100Db.ERZET = ERZET;
                            saleHeader100Db.KUNNR = KUNNR;
                            saleHeader100Db.LFGSK = LFGSK;
                            saleHeader100Db.PS_PSP_PNR = PSPSPPNR;
                            saleHeader100Db.SPART = SPART;
                            saleHeader100Db.VBELN = VBELN;
                            saleHeader100Db.VDATU = VDATU;
                            saleHeader100Db.VKORG = VKORG;
                            saleHeader100Db.VTWEG = VTWEG;
                            saleHeader100Db.ZZTERM = ZZTERM;
                            saleHeader100Db.ZZTERM_DES = ZZTERM_DES;
                            saleHeader100Db.LastEditTime = DateTime.Now;
                            _context.Entry(saleHeader100Db).State = EntityState.Modified;
                            #endregion
                            updateTotal++;
                        }
                        _context.ChangeTracker.DetectChanges();
                        await _context.SaveChangesAsync();
                        //Xác nhận insert vào DB thành công cho SAP
                        ConfirmInsert(VBELN, BEDAE);
                    }

                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            error = ex.InnerException.InnerException.Message;
                        }
                    }
                }
            }
            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMaterial:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
            message += $"Insert: {insertTotal}, Update: {updateTotal}. Total: {dataTable.Rows.Count}";

            return new Tuple<string, string>(message, error);

        }
        #endregion

        #region SOTEXT_PR
        public async Task<string> SyncSOTEXT_PR(ParamRequestSyncSapModel paramRequest)
        {
            var error = string.Empty;
            var message = string.Empty;
            var dataTables = GetSOTEXT_PR(paramRequest, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return error;
            }

            if (dataTables != null && dataTables.Count > 0)
            {
                try
                {
                    var IT_DATA_SO_PR = dataTables[0];

                    if (IT_DATA_SO_PR != null && IT_DATA_SO_PR.Rows.Count > 0)
                    {
                        foreach (DataRow SO_PR in IT_DATA_SO_PR.Rows)
                        {
                            #region Collect Data
                            var WERKS = SO_PR["WERKS"].ToString();
                            var BANFN = SO_PR["BANFN"].ToString();
                            var BNFPO = SO_PR["BNFPO"].ToString();
                            var ZMES_VBELN = SO_PR["ZMES_VBELN"].ToString();
                            var MATNR = SO_PR["MATNR"].ToString();
                            if (!string.IsNullOrEmpty(MATNR))
                            {
                                MATNR = MATNR.TrimStart(new Char[] { '0' });
                            }
                            DateTime? BADAT = null;
                            if (!string.IsNullOrEmpty(SO_PR["BADAT"].ToString()) && SO_PR["BADAT"].ToString() != "00000000")
                            {
                                string date = SO_PR["BADAT"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                BADAT = DateTime.Parse(date);
                            }
                            var ZCHECK = SO_PR["ZCHECK"].ToString();
                            var ZMES_LSXLON = SO_PR["ZMES_LSXLON"].ToString();
                            #endregion

                            var SO_PRDB = await _context.SOTEXT_PR_Model.Where(p => p.WERKS == WERKS && p.BANFN == BANFN && p.BNFPO == BNFPO && p.ZMES_VBELN == ZMES_VBELN && p.MATNR == MATNR).FirstOrDefaultAsync();
                            //Chưa có thì thêm mới
                            if (SO_PRDB == null)
                            {
                                #region Insert
                                var newSO_PR = new SOTEXT_PR_Model();
                                newSO_PR.WERKS = WERKS;
                                newSO_PR.BANFN = BANFN;
                                newSO_PR.BNFPO = BNFPO;
                                newSO_PR.ZMES_VBELN = ZMES_VBELN;
                                newSO_PR.MATNR = MATNR;
                                newSO_PR.BADAT = BADAT;
                                newSO_PR.ZMES_LSXLON = ZMES_LSXLON;
                                newSO_PR.ZCHECK = ZCHECK;
                                //SO nào != 10 ký tự => SOInvalid = true
                                if (!string.IsNullOrEmpty(ZMES_VBELN) && ZMES_VBELN.Length != 10)
                                {
                                    newSO_PR.SOInvalid = true;
                                }
                                newSO_PR.CreateTime = DateTime.Now;

                                _context.Entry(newSO_PR).State = EntityState.Added;
                                #endregion
                            }
                            //Có rồi thì cập nhật
                            else
                            {
                                #region Update
                                SO_PRDB.BADAT = BADAT;
                                SO_PRDB.ZMES_LSXLON = ZMES_LSXLON;
                                SO_PRDB.ZCHECK = ZCHECK;
                                //SO nào != 10 ký tự => SOInvalid = true
                                if (!string.IsNullOrEmpty(ZMES_VBELN))
                                {
                                    if (ZMES_VBELN.Length != 10)
                                    {
                                        SO_PRDB.SOInvalid = true;
                                    }
                                    else
                                    {
                                        SO_PRDB.SOInvalid = false;
                                    }
                                }
                                SO_PRDB.LastEditTime = DateTime.Now;
                                _context.Entry(SO_PRDB).State = EntityState.Modified;
                                #endregion
                            }
                        }
                        _context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.InnerException != null)
                        {
                            error = ex.InnerException.InnerException.Message;
                        }
                    }
                    return error;
                }
            }
            else
            {
                error = "No data returned from SAP";
                return error;
            }
            return message;
        }
        private List<DataTable> GetSOTEXT_PR(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_SOTEXT_PR);

                //function.SetValue("I_WERKS", paramRequest.CompanyCode);
                //function.SetValue("I_BANFN", paramRequest.IBANFN);


                //if (!string.IsNullOrEmpty(paramRequest.FromDate))
                //{
                //    DateTime fromDate = DateTime.ParseExact(paramRequest.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //    function.SetValue("I_FROM_DATE", fromDate);
                //}

                //if (!string.IsNullOrEmpty(paramRequest.ToDate))
                //{
                //    DateTime toDate = DateTime.ParseExact(paramRequest.ToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //    function.SetValue("I_TO_DATE", toDate);
                //}

                //function.Invoke(destination);

                //var table1 = function.GetTable("IT_DATA_SO_PR").ToDataTable("IT_DATA_SO_PR");
                //dataTables.Add(table1);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException != null)
                    {
                        errorMessage = ex.InnerException.InnerException.Message;
                    }
                }
            }
            return dataTables;

        }
        #endregion
    }
}
