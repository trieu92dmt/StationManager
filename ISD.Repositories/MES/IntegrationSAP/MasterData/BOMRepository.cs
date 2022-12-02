using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.MES.IntegrationSAP.MasterData
{
    public class BOMRepository
    {
        EntityDataContext _context;
        //SAPRepository _sap;
        LoggerRepository _loggerRepository;
        public BOMRepository(EntityDataContext entityDataContext)
        {
            _context = entityDataContext;
            //_sap = new SAPRepository();
            _loggerRepository = new LoggerRepository();
        }

        private List<DataTable> GetBOMData(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_BOM);

                //function.SetValue("I_WERKS", paramRequest.CompanyCode);
                ////function.SetValue("I_MATNR", paramRequest.MATNR);
                //if (!string.IsNullOrEmpty(paramRequest.MATNR))
                //{
                //    function.SetValue("I_MATNR", paramRequest.MATNR.PadLeft(18, '0'));
                //}
                //function.SetValue("I_STLAN", paramRequest.STLAN);
                //function.SetValue("I_STLAL", paramRequest.STLAL);
                //function.SetValue("I_STLNR", paramRequest.STLNR);
                ////function.SetValue("I_UPDATE", paramRequest.IUpdate);
                ////function.SetValue("I_NEW", paramRequest.INew);
                //function.SetValue("I_RECORD", paramRequest.IRecord);
                //function.SetValue("I_STLTY", "M");

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
                //var table3 = function.GetTable("IT_INV_HD").ToDataTable("IT_INV_HD");
                //dataTables.Add(table3);
                //var table4 = function.GetTable("IT_INV_IT").ToDataTable("IT_INV_IT");
                //dataTables.Add(table4);

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

        private List<DataTable> GetBOMSALEData(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_BOM_SALE);

                //function.SetValue("I_WERKS", paramRequest.CompanyCode);
                ////function.SetValue("I_MATNR", paramRequest.MATNR);
                //if (!string.IsNullOrEmpty(paramRequest.MATNR))
                //{
                //    function.SetValue("I_MATNR", paramRequest.MATNR.PadLeft(18, '0'));
                //}
                //function.SetValue("I_STLAN", "5");
                //function.SetValue("I_STLTY", "M");

                //function.Invoke(destination);

                //var table1 = function.GetTable("IT_HEADER").ToDataTable("IT_HEADER");
                //dataTables.Add(table1);
                //var table2 = function.GetTable("IT_DATA_BOM_SALE").ToDataTable("IT_ITEM");
                //dataTables.Add(table2);
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

        private OutputFromSAPViewModel ConfirmInsert(string CompanyCode, string MATNR, string STLAN = null)
        {
            var result = new OutputFromSAPViewModel();
            //var destination = _sap.GetRfcWithConfig();
            //Định nghĩa hàm cần gọi
            //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_BOM);
            ////if (MATNR.StartsWith("8"))
            ////{
            ////    function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_BOM_SALE);
            ////}
            ////000000000510000036
            //function.SetValue("I_WERKS", CompanyCode);
            ////Mã đầu 8 là BOM_SALE => usage = 5 
            ////Còn lại là BOM => usage = 1
            //if (!string.IsNullOrEmpty(STLAN))
            //{
            //    function.SetValue("I_STLAN", STLAN);
            //}
            //else
            //{
            //    if (MATNR.StartsWith("8"))
            //    {
            //        function.SetValue("I_STLAN", "5");
            //    }
            //    else
            //    {
            //        function.SetValue("I_STLAN", "1");
            //    }
            //}
            //function.SetValue("I_MATNR", MATNR.ToString().PadLeft(18, '0'));
            ////function.SetValue("I_STLTY", "M");
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

        public async Task<string> SyncBOM(ParamRequestSyncSapModel paramRequest)
        {
            var error = string.Empty;
            var message = string.Empty;
            var dataTables = GetBOMData(paramRequest, out error);
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
                    var IT_INV_HD = dataTables[2];
                    var IT_INV_IT = dataTables[3];

                    if (IT_HEADER == null || IT_HEADER.Rows.Count == 0)
                    {
                        error = "No data returned from SAP";
                        return error;
                    }

                    var taskInventorItem = await InsertUpdateInventorItem(IT_INV_IT);

                    if (!string.IsNullOrEmpty(taskInventorItem))
                    {
                        return taskInventorItem;
                    }

                    var taskInventorHeader = await InsertUpdateInventorHeader(IT_INV_HD);

                    if (!string.IsNullOrEmpty(taskInventorHeader))
                    {
                        return taskInventorHeader;
                    }

                    var taskItem = await InsertUpdateItem(IT_ITEM);

                    if (!string.IsNullOrEmpty(taskItem))
                    {
                        return taskItem;
                    }

                    var taskHeader = await InsertUpdateHeader(IT_HEADER, paramRequest.CompanyCode, paramRequest.MATNR);

                    if (!string.IsNullOrEmpty(taskHeader.Item2))
                    {
                        return taskHeader.Item2;
                    }
                    message = taskHeader.Item1;
                    //stopwatch.Stop();
                    //TimeSpan timeSpan = stopwatch.Elapsed;
                    //string elapsedTime = String.Format("SyncBOM:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

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

        public async Task<string> SyncBOMSALE(ParamRequestSyncSapModel paramRequest)
        {
            var error = string.Empty;
            var message = string.Empty;
            var dataTables = GetBOMSALEData(paramRequest, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return error;
            }

            if (dataTables != null && dataTables.Count > 0)
            {
                try
                {
                    var IT_HEADER = dataTables[0];
                    var IT_ITEM = dataTables[1];

                    var taskItem = await InsertUpdateItem(IT_ITEM);

                    if (!string.IsNullOrEmpty(taskItem))
                    {
                        return taskItem;
                    }

                    var taskHeader = await InsertUpdateHeader(IT_HEADER, paramRequest.CompanyCode, paramRequest.MATNR);

                    if (!string.IsNullOrEmpty(taskHeader.Item2))
                    {
                        return taskHeader.Item2;
                    }
                    message = taskHeader.Item1;
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

        private async Task<string> InsertUpdateInventorItem(DataTable dataTable)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {

                    #region Collect Data
                    string MATNR = string.Empty;
                    if (!string.IsNullOrEmpty(dataRow["MATNR"].ToString()))
                    {
                        MATNR = Int32.Parse(dataRow["MATNR"].ToString()).ToString();
                    }
                    var VERSO = dataRow["VERSO"].ToString();
                    var PART_ID = dataRow["PART_ID"].ToString();
                    var STLAN = dataRow["STLAN"].ToString();
                    var STLAL = dataRow["STLAL"].ToString();
                    var IDNRK = dataRow["IDNRK"].ToString();
                    string IDNRK_MES = null;
                    if (!string.IsNullOrEmpty(IDNRK))
                    {
                        try
                        {
                            IDNRK_MES = Int32.Parse(IDNRK).ToString();
                        }
                        catch (Exception)
                        {

                        }
                    }
                    //Quantity
                    decimal? MENGE = null;
                    var MENGE_STRING = dataRow["MENGE"].ToString();
                    MENGE = ParseDecimal(MENGE, MENGE_STRING);

                    var MEINS = dataRow["MEINS"].ToString();

                    //Component Scrap
                    decimal? AUSCH = null;
                    var AUSCH_STRING = dataRow["AUSCH"].ToString();
                    AUSCH = ParseDecimal(AUSCH, AUSCH_STRING);

                    var POT11 = dataRow["POT11"].ToString();
                    var POT12 = dataRow["POT12"].ToString();
                    //tách quy cách thành 3 cột:
                    //1. Replace "." => ""
                    //2. Replace phẩy "," => chấm "."
                    decimal? P1 = null;
                    decimal? P2 = null;
                    decimal? P3 = null;
                    if (!string.IsNullOrEmpty(POT12))
                    {
                        try
                        {
                            var stringToSplit = POT12.ToLower().Replace(".", "").Replace(",", ".");
                            var stringArray = stringToSplit.Split('x');
                            if (stringArray.Length == 3)
                            {
                                decimal number1;
                                decimal number2;
                                decimal number3;

                                bool successP1 = decimal.TryParse(stringArray[0], out number1);
                                bool successP2 = decimal.TryParse(stringArray[1], out number2);
                                bool successP3 = decimal.TryParse(stringArray[2], out number3);
                                //Nếu dữ liệu tách ra đều là số thì mới lưu
                                if (successP1 && successP2 && successP3)
                                {
                                    P1 = number1;
                                    P2 = number2;
                                    P3 = number3;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    var POT21 = dataRow["POT21"].ToString();
                    var POT22 = dataRow["POT22"].ToString();
                    var SANKA = dataRow["SANKA"].ToString();
                    var AENNR = dataRow["AENNR"].ToString();
                    var ACTION = dataRow["ACTION"].ToString();
                    var ANNAM = dataRow["ANNAM"].ToString();
                    var POSNR = dataRow["POSNR"].ToString();
                    #endregion

                    var bomInvItemDb = await _context.BOM_Item_InventorModel.Where(p => p.MATNR == MATNR && p.VERSO == VERSO && p.PART_ID == PART_ID
                                                                                        && p.STLAN == STLAN && p.STLAL == STLAL && p.IDNRK == IDNRK
                                                                                    ).FirstOrDefaultAsync();
                    //Nếu chưa có product thì thêm mới: Có rồi thì update
                    if (bomInvItemDb == null)
                    {
                        #region InsertItem
                        var bomDetail = new BOM_Item_InventorModel
                        {
                            BOMItemInventorId = Guid.NewGuid(),
                            MATNR = MATNR,
                            VERSO = VERSO,
                            PART_ID = PART_ID,
                            STLAN = STLAN,
                            STLAL = STLAL,
                            IDNRK = IDNRK,
                            IDNRK_MES = IDNRK_MES,
                            MENGE = MENGE,
                            MEINS = MEINS,
                            AUSCH = AUSCH,
                            POT11 = POT11,
                            POT12 = POT12,
                            P1 = P1,
                            P2 = P2,
                            P3 = P3,
                            POT21 = POT21,
                            POT22 = POT22,
                            SANKA = SANKA,
                            AENNR = AENNR,
                            ACTION = ACTION,
                            ANNAM = ANNAM,
                            POSNR = POSNR,
                            CreateTime = DateTime.Now,

                        };
                        _context.Entry(bomDetail).State = EntityState.Added;

                        #endregion InsertItem
                    }
                    else
                    {
                        #region UpdateItem
                        bomInvItemDb.MATNR = MATNR;
                        bomInvItemDb.VERSO = VERSO;
                        bomInvItemDb.PART_ID = PART_ID;
                        bomInvItemDb.STLAN = STLAN;
                        bomInvItemDb.STLAL = STLAL;
                        bomInvItemDb.IDNRK = IDNRK;
                        bomInvItemDb.IDNRK_MES = IDNRK_MES;
                        bomInvItemDb.MENGE = MENGE;
                        bomInvItemDb.MEINS = MEINS;
                        bomInvItemDb.AUSCH = AUSCH;
                        bomInvItemDb.POT11 = POT11;
                        bomInvItemDb.POT12 = POT12;
                        bomInvItemDb.P1 = P1;
                        bomInvItemDb.P2 = P2;
                        bomInvItemDb.P3 = P3;
                        bomInvItemDb.POT21 = POT21;
                        bomInvItemDb.POT22 = POT22;
                        bomInvItemDb.SANKA = SANKA;
                        bomInvItemDb.AENNR = AENNR;
                        bomInvItemDb.ACTION = ACTION;
                        bomInvItemDb.ANNAM = ANNAM;
                        bomInvItemDb.POSNR = POSNR;
                        bomInvItemDb.LastEditTime = DateTime.Now;
                        _context.Entry(bomInvItemDb).State = EntityState.Modified;
                        #endregion UpdateItem
                    }
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
            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMARM:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
            return error;
        }

        private async Task<string> InsertUpdateInventorHeader(DataTable dataTable)
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
                    #region Collect Data
                    string MATNR = string.Empty;
                    if (!string.IsNullOrEmpty(dataRow["MATNR"].ToString()))
                    {
                        MATNR = Int32.Parse(dataRow["MATNR"].ToString()).ToString();
                    }
                    var WERKS = dataRow["WERKS"].ToString();
                    var VERSO = dataRow["VERSO"].ToString();
                    var STLAN = dataRow["STLAN"].ToString();
                    var STLAL = dataRow["STLAL"].ToString();

                    decimal? BMENG = null;
                    var BMENG_STRING = dataRow["BMENG"].ToString();
                    BMENG = ParseDecimal(BMENG, BMENG_STRING);

                    var BMEIN = dataRow["BMEIN"].ToString();
                    var PTURE = dataRow["PTURE"].ToString();
                    var DRAWG = dataRow["DRAWG"].ToString();
                    var NOTES = dataRow["NOTES"].ToString();
                    var STATE = dataRow["STATE"].ToString();
                    var ANNAM = dataRow["ANNAM"].ToString();
                    DateTime? ANDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["ANDAT"].ToString()) && dataRow["ANDAT"].ToString() != "00000000")
                    {
                        string date = dataRow["ANDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        ANDAT = DateTime.Parse(date);
                    }
                    var AENAM = dataRow["AENAM"].ToString();
                    #endregion

                    var bomInventorHeaderDb = await _context.BOM_Header_InventorModel.Where(p => p.MATNR == MATNR && p.WERKS == WERKS && p.VERSO == VERSO).FirstOrDefaultAsync();
                    //Nếu chưa có thì thêm mới : Có rồi thì update
                    if (bomInventorHeaderDb == null)
                    {
                        #region Insert
                        var bomHeaderNew = new BOM_Header_InventorModel
                        {
                            BOMHeaderInventorId = Guid.NewGuid(),
                            MATNR = MATNR,
                            WERKS = WERKS,
                            VERSO = VERSO,
                            STLAN = STLAN,
                            STLAL = STLAL,
                            BMENG = BMENG,
                            BMEIN = BMEIN,
                            PTURE = PTURE,
                            DRAWG = DRAWG,
                            NOTES = NOTES,
                            STATE = STATE,
                            ANNAM = ANNAM,
                            ANDAT = ANDAT,
                            AENAM = AENAM,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(bomHeaderNew).State = EntityState.Added;
                        #endregion
                        insertTotal++;
                    }
                    else
                    {
                        #region Update
                        bomInventorHeaderDb.MATNR = MATNR;
                        bomInventorHeaderDb.WERKS = WERKS;
                        bomInventorHeaderDb.VERSO = VERSO;
                        bomInventorHeaderDb.STLAN = STLAN;
                        bomInventorHeaderDb.STLAL = STLAL;
                        bomInventorHeaderDb.BMENG = BMENG;
                        bomInventorHeaderDb.BMEIN = BMEIN;
                        bomInventorHeaderDb.PTURE = PTURE;
                        bomInventorHeaderDb.DRAWG = DRAWG;
                        bomInventorHeaderDb.NOTES = NOTES;
                        bomInventorHeaderDb.STATE = STATE;
                        bomInventorHeaderDb.ANNAM = ANNAM;
                        bomInventorHeaderDb.ANDAT = ANDAT;
                        bomInventorHeaderDb.AENAM = AENAM;
                        bomInventorHeaderDb.LastEditTime = DateTime.Now;
                        _context.Entry(bomInventorHeaderDb).State = EntityState.Modified;
                        #endregion
                        updateTotal++;
                    }
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
            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMaterial:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
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

                    #region Collect Data
                    var MANDT = dataRow["MANDT"].ToString();
                    var STLTY = dataRow["STLTY"].ToString();
                    var STLNR = dataRow["STLNR"].ToString();
                    var STLKN = dataRow["STLKN"].ToString();
                    var STPOZ = dataRow["STPOZ"].ToString();
                    DateTime? DATUV = null;
                    if (!string.IsNullOrEmpty(dataRow["DATUV"].ToString()))
                    {
                        string date = dataRow["DATUV"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        DATUV = DateTime.Parse(date);
                    }
                    var LKENZ = dataRow["LKENZ"].ToString();
                    DateTime? ANDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["ANDAT"].ToString()))
                    {
                        string date = dataRow["ANDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        ANDAT = DateTime.Parse(date);
                    }
                    var ANNAM = dataRow["ANNAM"].ToString();
                    if (!ANDAT.HasValue && string.IsNullOrEmpty(ANNAM))
                    {
                        error = "ANDAT and ANNAM are NULL";
                        return error;
                    }
                    var IDNRK = dataRow["IDNRK"].ToString();
                    string IDNRK_MES = null;
                    if (!string.IsNullOrEmpty(IDNRK))
                    {
                        try
                        {
                            IDNRK_MES = Int32.Parse(IDNRK).ToString();
                        }
                        catch (Exception)
                        {

                        }
                    }
                    var MAKTX = dataRow["MAKTX"].ToString();
                    var MTART = dataRow["MTART"].ToString();
                    var POSTP = dataRow["POSTP"].ToString();
                    var POSNR = dataRow["POSNR"].ToString();
                    string POSNR_MES = null;
                    if (!string.IsNullOrEmpty(POSNR))
                    {
                        POSNR_MES = POSNR.TrimStart(new Char[] { '0' });
                    }
                    var MEINS = dataRow["MEINS"].ToString();
                    //Quantity
                    decimal? MENGE = null;
                    var MENGE_STRING = dataRow["MENGE"].ToString();
                    MENGE = ParseDecimal(MENGE, MENGE_STRING);
                    //Component Scrap
                    decimal? AUSCH = null;
                    var AUSCH_STRING = dataRow["AUSCH"].ToString();
                    AUSCH = ParseDecimal(AUSCH, AUSCH_STRING);
                    //Operation Scrap
                    decimal? AVOAU = null;
                    var AVOAU_STRING = dataRow["AVOAU"].ToString();
                    AVOAU = ParseDecimal(AVOAU, AVOAU_STRING);

                    var POTX1 = dataRow["POTX1"].ToString();
                    //tách thành 5 cột: 2-30x60x500-Rip
                    //P1 = 2
                    //P2 = 30
                    //P3 = 60
                    //P4 = 500
                    //P5 = Rip
                    string P1 = null;
                    string P2 = null;
                    string P3 = null;
                    string P4 = null;
                    string P5 = null;
                    if (!string.IsNullOrEmpty(POTX1))
                    {
                        try
                        {
                            var arrayString = POTX1.Split('-');
                            if (arrayString.Length == 3)
                            {
                                P1 = arrayString[0];
                                if (!string.IsNullOrEmpty(arrayString[1]) && arrayString[1].Contains('x'))
                                {
                                    var arrayString2 = arrayString[1].Split('x');
                                    if (arrayString2.Length == 3)
                                    {
                                        P2 = arrayString2[0];
                                        P3 = arrayString2[1];
                                        P4 = arrayString2[2];
                                    }
                                }
                                P5 = arrayString[2];
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                    var POTX2 = dataRow["POTX2"].ToString();
                    var WERKS = dataRow["WERKS"].ToString();
                    var MATNR = dataRow["MATNR"].ToString();
                    if (!string.IsNullOrEmpty(MATNR))
                    {
                        MATNR = MATNR.TrimStart(new Char[] { '0' });
                    }
                    var AENNR = dataRow["AENNR"].ToString();
                    #endregion

                    var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefaultAsync();
                    //NVL
                    var matInDb = await _context.ProductModel.Where(p => p.ERPProductCode == IDNRK_MES && p.CompanyId == companyId).FirstOrDefaultAsync();
                    if (matInDb == null && !string.IsNullOrEmpty(IDNRK_MES))
                    {
                        syncMaterial(WERKS, IDNRK_MES);
                    }
                    var bomDetailDb = await _context.BOMDetailModel.Where(p => p.MATNR == MATNR && p.WERKS == WERKS && p.STLNR == STLNR && p.STLKN == STLKN && p.STPOZ == STPOZ).FirstOrDefaultAsync();
                    //Nếu chưa có product thì thêm mới: Có rồi thì update
                    if (bomDetailDb == null)
                    {
                        #region InsertItem
                        var bomDetail = new BOMDetailModel
                        {
                            BomDetailId = Guid.NewGuid(),
                            STLKN = STLKN,
                            ANDAT = ANDAT,
                            ANNAM = ANNAM,
                            AUSCH = AUSCH,
                            AVOAU = AVOAU,
                            DATUV = DATUV,
                            IDNRK = IDNRK,
                            IDNRK_MES = IDNRK_MES,
                            LKENZ = LKENZ,
                            MAKTX = MAKTX,
                            MEINS = MEINS,
                            MENGE = MENGE,
                            MTART = MTART,
                            POSNR = POSNR,
                            POSNR_MES = POSNR_MES,
                            POSTP = POSTP,
                            POTX1 = POTX1,
                            P1 = P1,
                            P2 = P2,
                            P3 = P3,
                            P4 = P4,
                            P5 = P5,
                            POTX2 = POTX2,
                            STLNR = STLNR,
                            STLTY = STLTY,
                            STPOZ = STPOZ,
                            WERKS = WERKS,
                            MATNR = MATNR,
                            AENNR = AENNR,
                            CreateTime = DateTime.Now,
                        };
                        _context.Entry(bomDetail).State = EntityState.Added;

                        #endregion InsertItem
                    }
                    else
                    {
                        #region UpdateItem
                        bomDetailDb.STLKN = STLKN;
                        bomDetailDb.ANDAT = ANDAT;
                        bomDetailDb.ANNAM = ANNAM;
                        bomDetailDb.AUSCH = AUSCH;
                        bomDetailDb.AVOAU = AVOAU;
                        bomDetailDb.DATUV = DATUV;
                        bomDetailDb.IDNRK = IDNRK;
                        bomDetailDb.IDNRK_MES = IDNRK_MES;
                        bomDetailDb.LKENZ = LKENZ;
                        bomDetailDb.MAKTX = MAKTX;
                        bomDetailDb.MEINS = MEINS;
                        bomDetailDb.MENGE = MENGE;
                        bomDetailDb.MTART = MTART;
                        bomDetailDb.POSNR = POSNR;
                        bomDetailDb.POSNR_MES = POSNR_MES;
                        bomDetailDb.POSTP = POSTP;
                        bomDetailDb.POTX1 = POTX1;
                        bomDetailDb.P1 = P1;
                        bomDetailDb.P2 = P2;
                        bomDetailDb.P3 = P3;
                        bomDetailDb.P4 = P4;
                        bomDetailDb.P5 = P5;
                        bomDetailDb.POTX2 = POTX2;
                        bomDetailDb.STLNR = STLNR;
                        bomDetailDb.STLTY = STLTY;
                        bomDetailDb.STPOZ = STPOZ;
                        bomDetailDb.WERKS = WERKS;
                        bomDetailDb.MATNR = MATNR;
                        bomDetailDb.AENNR = AENNR;
                        bomDetailDb.LastEditTime = DateTime.Now;
                        _context.Entry(bomDetailDb).State = EntityState.Modified;
                        #endregion UpdateItem
                    }
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
            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMARM:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
            return error;
        }

        private static decimal? ParseDecimal(decimal? MENGE, string MENGE_STRING)
        {
            if (!string.IsNullOrEmpty(MENGE_STRING))
            {
                decimal ret;
                if (decimal.TryParse(MENGE_STRING, out ret))
                {
                    MENGE = ret;
                }
            }
            return MENGE;
        }

        private async Task<Tuple<string, string>> InsertUpdateHeader(DataTable dataTable, string CompanyCode, string ProductCode)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            string message = string.Empty;
            int insertTotal = 0, updateTotal = 0;
            bool isSyncFromWeb = false;
            if (!string.IsNullOrEmpty(CompanyCode) && !string.IsNullOrEmpty(ProductCode))
            {
                isSyncFromWeb = true;
            }
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    #region Collect Data
                    //DateTime? MANDT = null;
                    //if (!string.IsNullOrEmpty(dataRow["MANDT"].ToString()) && dataRow["ERDAT"].ToString() != "00000000")
                    //{
                    //    MANDT = DateTime.Parse(dataRow["MANDT"].ToString());
                    //}
                    string MATNR = string.Empty;
                    if (!string.IsNullOrEmpty(dataRow["MATNR"].ToString()))
                    {
                        MATNR = Int32.Parse(dataRow["MATNR"].ToString()).ToString();
                    }
                    var STLAN = dataRow["STLAN"].ToString();
                    var MANDT = dataRow["MANDT"].ToString();
                    var STLAL = dataRow["STLAL"].ToString();
                    var STLNR = dataRow["STLNR"].ToString();
                    var WERKS = dataRow["WERKS"].ToString();
                    var MAKTX = dataRow["MAKTX"].ToString();

                    if (STLAN != "1" && STLAN != "5")
                    {
                        ConfirmInsert(WERKS, MATNR, STLAN);
                        message = string.Format("Skip MATNR = {0}, STLAN = {1}", MATNR, STLAN);
                        return new Tuple<string, string>(message, error);
                    }
                    else
                    {
                        if (MATNR.StartsWith("8") && STLAN == "1")
                        {
                            ConfirmInsert(WERKS, MATNR, STLAN);
                            message = string.Format("Skip MATNR = {0}, STLAN = {1}", MATNR, STLAN);
                            return new Tuple<string, string>(message, error);
                        }
                        else if (!MATNR.StartsWith("8") && STLAN == "5")
                        {
                            ConfirmInsert(WERKS, MATNR, STLAN);
                            message = string.Format("Skip MATNR = {0}, STLAN = {1}", MATNR, STLAN);
                            return new Tuple<string, string>(message, error);
                        }
                    }

                    decimal? LOSVN = null;
                    var LOSVN_STRING = dataRow["LOSVN"].ToString();
                    LOSVN = ParseDecimal(LOSVN, LOSVN_STRING);

                    decimal? LOSBS = null;
                    var LOSBS_STRING = dataRow["LOSBS"].ToString();
                    LOSBS = ParseDecimal(LOSBS, LOSBS_STRING);

                    if (string.IsNullOrEmpty(CompanyCode))
                    {
                        CompanyCode = WERKS;
                    }
                    if (string.IsNullOrEmpty(ProductCode))
                    {
                        ProductCode = MATNR;
                    }
                    decimal? BMENG = null;
                    var BMENG_STRING = dataRow["BMENG"].ToString();
                    BMENG = ParseDecimal(BMENG, BMENG_STRING);

                    var BMEIN = dataRow["BMEIN"].ToString();
                    #endregion

                    //Sản phẩm: khi đồng bộ từ SAP về nếu không có (MES) phải kéo bằng được
                    var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefaultAsync();
                    var productInDb = await _context.ProductModel.Where(p => p.ERPProductCode == MATNR && p.CompanyId == companyId).FirstOrDefaultAsync();
                    if (productInDb == null && !string.IsNullOrEmpty(MATNR))
                    {
                        syncMaterial(WERKS, MATNR);
                    }

                    var bomHeaderDb = await _context.BOMHeaderModel.Where(p => p.MATNR == MATNR && p.WERKS == WERKS && p.STLAN == STLAN && p.BMEIN == BMEIN).FirstOrDefaultAsync();
                    //Nếu chưa có thì thêm mới : Có rồi thì update
                    if (bomHeaderDb == null)
                    {
                        #region Insert
                        var bomHeaderNew = new BOMHeaderModel
                        {
                            BomHeaderId = Guid.NewGuid(),
                            MATNR = MATNR,
                            MANDT = MANDT,
                            // ANDAT = MANDT,
                            LOSBS = LOSBS,
                            LOSVN = LOSVN,
                            MAKTX = MAKTX,
                            STLAL = STLAL,
                            STLAN = STLAN,
                            STLNR = STLNR,
                            WERKS = WERKS,
                            BMENG = BMENG,
                            BMEIN = BMEIN,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(bomHeaderNew).State = EntityState.Added;
                        #endregion
                        insertTotal++;
                    }
                    else
                    {
                        #region Update
                        bomHeaderDb.MATNR = MATNR;
                        //bomHeaderDb.ANDAT = ;
                        bomHeaderDb.LOSBS = LOSBS;
                        bomHeaderDb.LOSVN = LOSVN;
                        bomHeaderDb.MAKTX = MAKTX;
                        bomHeaderDb.STLAL = STLAL;
                        bomHeaderDb.STLAN = STLAN;
                        bomHeaderDb.STLNR = STLNR;
                        bomHeaderDb.WERKS = WERKS;
                        bomHeaderDb.BMENG = BMENG;
                        bomHeaderDb.BMEIN = BMEIN;
                        bomHeaderDb.LastEditTime = DateTime.Now;
                        _context.Entry(bomHeaderDb).State = EntityState.Modified;
                        #endregion
                        updateTotal++;
                    }
                    await _context.SaveChangesAsync();
                    //Xác nhận insert vào DB thành công cho SAP
                    //ConfirmInsert(WERKS, MATNR);
                    
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

            if (isSyncFromWeb == false)
            {
                var result = ConfirmInsert(CompanyCode, ProductCode);
                if (result.IsSuccess == false)
                {
                    _loggerRepository.Logging(string.Format("I_INSERT: E_ERROR (WERKS: {0}, MATNR: {1})"
                                                            , CompanyCode
                                                            , ProductCode), "ZMES_FM_BOM");
                }
            }
            
            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMaterial:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
            message += $"Insert: {insertTotal}, Update: {updateTotal}. Total: {dataTable.Rows.Count}";

            return new Tuple<string, string>(message, error);

        }

        //đồng bộ sản phẩm khi chưa tồn tại trên MES
        private async void syncMaterial(string CompanyCode, string MaterialCode)
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
            await _materialMasterRepository.GetMaterialMaster(materialParam);
        }
        public string ResetBOM()
        {
            string result = string.Empty;
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_RESET_MASTERDATA);

                //function.SetValue("I_ACTION", "BOM");

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
    }
}
