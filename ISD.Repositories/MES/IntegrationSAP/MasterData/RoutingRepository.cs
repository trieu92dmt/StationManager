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
    public class RoutingRepository
    {
        EntityDataContext _context;
        //SAPRepository _sap;
        LoggerRepository _loggerRepository;
        public RoutingRepository(EntityDataContext entityDataContext)
        {
            _context = entityDataContext;
            //_sap = new SAPRepository();
            _loggerRepository = new LoggerRepository();
        }
        private List<DataTable> GetRoutingData(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_ROUTING);

                //function.SetValue("I_WERKS", paramRequest.CompanyCode);
                ////function.SetValue("I_UPDATE", paramRequest.IUpdate);
                //function.SetValue("I_MATNR", paramRequest.MATNR.PadLeft(18, '0'));
                ////function.SetValue("I_NEW", paramRequest.INew);
                ////function.SetValue("I_RECORD", "0");

                //function.Invoke(destination);

                //var inventor = function.GetTable("IT_DATA_RT_INVENTOR").ToDataTable("ROUTING_INVENTOR");
                //dataTables.Add(inventor);
                //var routing = function.GetTable("IT_DATA_RT_SAP").ToDataTable("ROUTING_SAP");
                //dataTables.Add(routing);
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

        private List<DataTable> GetRoutingMD(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_ROUTING);

                //function.SetValue("I_WERKS", paramRequest.CompanyCode);
                //if (!string.IsNullOrEmpty(paramRequest.FromDate))
                //{
                //    DateTime fromDate = DateTime.ParseExact(paramRequest.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //    function.SetValue("I_FROM_DATE", fromDate);
                //}

                //function.Invoke(destination);

                //var MDISD = function.GetTable("IT_DATA_RT_MDISD").ToDataTable("ROUTING_INVENTOR");
                //dataTables.Add(MDISD);
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

        private void ConfirmInsert(string CompanyCode, string ProductCode)
        {
            //var destination = _sap.GetRfcWithConfig();
            ////Định nghĩa hàm cần gọi
            //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_TB_ROUTING);

            //function.SetValue("I_WERKS", CompanyCode);
            //function.SetValue("I_MATNR", ProductCode);
            //function.SetValue("I_INSERT", 'X');//Tham số xác nhận insert thành công

            //function.Invoke(destination);

        }

        public async Task<string> SyncRouting(ParamRequestSyncSapModel paramRequest)
        {
            var error = string.Empty;
            var message = string.Empty;
            //Lấy danh sách những mã cần đồng bộ về
            var tempDataTables = GetRoutingMD(paramRequest, out error);

            if (tempDataTables != null && tempDataTables.Count > 0)
            {
                var TAB_MDISD = tempDataTables[0];
                //Foreach các mã cần đồng bộ call lại để update
                foreach (DataRow dataRow in TAB_MDISD.Rows)
                {
                    ParamRequestSyncSapModel newParamRequest = new ParamRequestSyncSapModel()
                    {
                        CompanyCode = dataRow["WERKS"].ToString(),
                        MATNR = dataRow["MATNR"].ToString()
                    };

                    //Xóa dữ liệu cũ => update đè dữ liệu mới lên luôn
                    var existsRouting = _context.RoutingInventorModel.Where(p => p.WERKS == newParamRequest.CompanyCode && p.MATNR == newParamRequest.MATNR).ToList();
                    if (existsRouting != null && existsRouting.Count > 0)
                    {
                        _context.RoutingInventorModel.RemoveRange(existsRouting);
                        _context.SaveChanges();
                    }

                    //Check SP đã có trên MES chưa, nếu chưa có thì phải kéo về trước
                    var companyId = _context.CompanyModel.Where(p => p.CompanyCode == newParamRequest.CompanyCode).Select(p => p.CompanyId).FirstOrDefault();
                    var productInDb = _context.ProductModel.Where(p => p.ERPProductCode == newParamRequest.MATNR && p.CompanyId == companyId).FirstOrDefault();
                    if (productInDb == null)
                    {
                        syncMaterial(newParamRequest.CompanyCode, newParamRequest.MATNR);
                    }
                    var dataTables = GetRoutingData(newParamRequest, out error);
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
                            var TAB_Inventory = dataTables[0];
                            var TAB_Routing = dataTables[1];


                            var taskRouting = await InsertUpdateRoutingSap(TAB_Routing);

                            if (!string.IsNullOrEmpty(taskRouting))
                            {
                                return taskRouting;
                            }

                            var taskInventory = await InsertUpdateInventory(TAB_Inventory);

                            if (!string.IsNullOrEmpty(taskInventory.Item2))
                            {
                                return taskInventory.Item2;
                            }
                            message = taskInventory.Item1;
                            //stopwatch.Stop();
                            //TimeSpan timeSpan = stopwatch.Elapsed;
                            //string elapsedTime = String.Format("Sync Routing:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

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
                }
            }
            return message;
        }

        public async Task<string> SyncRoutingByPlantAndMaterialNumber(ParamRequestSyncSapModel paramRequest)
        {
            var error = string.Empty;
            var message = string.Empty;
            //Xóa dữ liệu cũ => update đè dữ liệu mới lên luôn
            var existsRouting = _context.RoutingInventorModel.Where(p => p.WERKS == paramRequest.CompanyCode && p.MATNR == paramRequest.MATNR).ToList();
            if (existsRouting != null && existsRouting.Count > 0)
            {
                _context.RoutingInventorModel.RemoveRange(existsRouting);
                _context.SaveChanges();
            }
            //Check SP đã có trên MES chưa, nếu chưa có thì phải kéo về trước
            var companyId = _context.CompanyModel.Where(p => p.CompanyCode == paramRequest.CompanyCode).Select(p => p.CompanyId).FirstOrDefault();
            var productInDb = _context.ProductModel.Where(p => p.ERPProductCode == paramRequest.MATNR && p.CompanyId == companyId).FirstOrDefault();
            if (productInDb == null)
            {
                syncMaterial(paramRequest.CompanyCode, paramRequest.MATNR);
            }
            var dataTables = GetRoutingData(paramRequest, out error);
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
                    var TAB_Inventory = dataTables[0];
                    var TAB_Routing = dataTables[1];


                    var taskRouting = await InsertUpdateRoutingSap(TAB_Routing);

                    if (!string.IsNullOrEmpty(taskRouting))
                    {
                        return taskRouting;
                    }

                    var taskInventory = await InsertUpdateInventory(TAB_Inventory);

                    if (!string.IsNullOrEmpty(taskInventory.Item2))
                    {
                        return taskInventory.Item2;
                    }
                    message = taskInventory.Item1;
                    //stopwatch.Stop();
                    //TimeSpan timeSpan = stopwatch.Elapsed;
                    //string elapsedTime = String.Format("Sync Routing:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

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


        private async Task<string> InsertUpdateRoutingSap(DataTable dataTable)
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
                    var WERKS = dataRow["WERKS"].ToString();
                    var MANDT = dataRow["MANDT"].ToString();
                    var ARBPL = dataRow["ARBPL"].ToString();
                    var PLNTY = dataRow["PLNTY"].ToString();
                    var PLNNR = dataRow["PLNNR"].ToString();
                    var PLNAL = dataRow["PLNAL"].ToString();
                    var ZKRIZ = dataRow["ZKRIZ"].ToString();
                    var ZAEHL = dataRow["ZAEHL"].ToString();
                    var DATUV = dataRow["DATUV"].ToString();
                    var LOEKZ = dataRow["LOEKZ"].ToString();
                    var SUMNR = dataRow["SUMNR"].ToString();
                    var VORNR = dataRow["VORNR"].ToString();
                    // var KTSCH = dataRow["KTSCH"].ToString();
                    var LTXA1 = dataRow["LTXA1"].ToString();
                    var MEINH = dataRow["MEINH"].ToString();
                    var UMREN = dataRow["UMREN"].ToString();
                    var UMREZ = dataRow["UMREZ"].ToString();
                    var BMSCH = dataRow["BMSCH"].ToString();
                    //var LAR01 = dataRow["LAR01"].ToString();
                    var VGE01 = dataRow["VGE01"].ToString();
                    var VGW01 = dataRow["VGW01"].ToString();
                    // var LAR02 = dataRow["LAR02"].ToString();
                    var VGE02 = dataRow["VGE02"].ToString();
                    var VGW02 = dataRow["VGW02"].ToString();
                    // var LAR03 = dataRow["LAR03"].ToString();
                    var VGE03 = dataRow["VGE03"].ToString();
                    var VGW03 = dataRow["VGW03"].ToString();
                    // var LAR04 = dataRow["LAR04"].ToString();
                    var VGE04 = dataRow["VGE04"].ToString();
                    var VGW04 = dataRow["VGW04"].ToString();
                    //var LAR05 = dataRow["LAR05"].ToString();
                    var VGE05 = dataRow["VGE05"].ToString();
                    var VGW05 = dataRow["VGW05"].ToString();
                    //var LAR06 = dataRow["LAR06"].ToString();
                    var VGE06 = dataRow["VGE06"].ToString();
                    var VGW06 = dataRow["VGW06"].ToString();
                    var ZERMA = dataRow["ZERMA"].ToString();

                    #endregion
                    var routingInDb = await _context.RoutingSapModel.Where(p => p.MATNR == MATNR && p.WERKS == WERKS && p.PLNNR == PLNNR && p.PLNAL == PLNAL && p.VORNR == VORNR).FirstOrDefaultAsync();
                    //Nếu chưa có product thì thêm mới: Có rồi thì update
                    if (routingInDb == null)
                    {
                        #region InsertRouting
                        var routingSapNew = new RoutingSapModel
                        {
                            RoutingSapId = Guid.NewGuid(),
                            BMSCH = BMSCH,
                            DATUV = DATUV,
                            // KTSCH = KTSCH,
                            //LAR01 = LAR01,
                            //LAR02 = LAR02,
                            //LAR03 = LAR03,
                            //LAR04 = LAR04,
                            //LAR05 = LAR05,
                            //LAR06 = LAR06,
                            LOEKZ = LOEKZ,
                            LTXA1 = LTXA1,
                            MATNR = MATNR,
                            MEINH = MEINH,
                            PLNAL = PLNAL,
                            PLNNR = PLNNR,
                            PLNTY = PLNTY,
                            SUMNR = SUMNR,
                            UMREN = UMREN,
                            UMREZ = UMREZ,
                            VGE01 = VGE01,
                            VGE02 = VGE02,
                            VGE03 = VGE03,
                            VGE04 = VGE04,
                            VGE05 = VGE05,
                            VGE06 = VGE06,
                            VGW01 = VGW01,
                            VGW02 = VGW02,
                            VGW03 = VGW03,
                            VGW04 = VGW04,
                            VGW05 = VGW05,
                            VGW06 = VGW06,
                            VORNR = VORNR,
                            WERKS = WERKS,
                            ZAEHL = ZAEHL,
                            ZERMA = ZERMA,
                            ZKRIZ = ZKRIZ,
                            ARBPL = ARBPL,
                            MANDT = MANDT,
                            CreateTime = DateTime.Now

                        };
                        _context.Entry(routingSapNew).State = EntityState.Added;

                        #endregion InsertRouting
                    }
                    else
                    {
                        #region UpdateRouting
                        routingInDb.BMSCH = BMSCH;
                        routingInDb.DATUV = DATUV;
                        routingInDb.LOEKZ = LOEKZ;
                        routingInDb.LTXA1 = LTXA1;
                        routingInDb.MATNR = MATNR;
                        routingInDb.MEINH = MEINH;
                        routingInDb.PLNAL = PLNAL;
                        routingInDb.PLNNR = PLNNR;
                        routingInDb.PLNTY = PLNTY;
                        routingInDb.SUMNR = SUMNR;
                        routingInDb.UMREN = UMREN;
                        routingInDb.UMREZ = UMREZ;
                        routingInDb.VGE01 = VGE01;
                        routingInDb.VGE02 = VGE02;
                        routingInDb.VGE03 = VGE03;
                        routingInDb.VGE04 = VGE04;
                        routingInDb.VGE05 = VGE05;
                        routingInDb.VGE06 = VGE06;
                        routingInDb.VGW01 = VGW01;
                        routingInDb.VGW02 = VGW02;
                        routingInDb.VGW03 = VGW03;
                        routingInDb.VGW04 = VGW04;
                        routingInDb.VGW05 = VGW05;
                        routingInDb.VGW06 = VGW06;
                        routingInDb.VORNR = VORNR;
                        routingInDb.WERKS = WERKS;
                        routingInDb.ZAEHL = ZAEHL;
                        routingInDb.ZERMA = ZERMA;
                        routingInDb.ZKRIZ = ZKRIZ;
                        routingInDb.ARBPL = ARBPL;
                        routingInDb.MANDT = MANDT;
                        routingInDb.LastEditTime = DateTime.Now;
                        _context.Entry(routingInDb).State = EntityState.Modified;
                        #endregion UpdateRouting
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
            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMARM:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
            return error;
        }

        private async Task<Tuple<string, string>> InsertUpdateInventory(DataTable dataTable)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            string message = string.Empty;
            int insertTotal = 0, updateTotal = 0;
            string CompanyCode = string.Empty;
            string ERPProductCode = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                CompanyCode = dataRow["WERKS"].ToString();
                if (!string.IsNullOrEmpty(dataRow["MATNR"].ToString()))
                {
                    int? MATNRValue = null;
                    var MATNR_STRING = dataRow["MATNR"].ToString();
                    MATNRValue = ParseInt(MATNRValue, MATNR_STRING);
                    if (MATNRValue == null || MATNRValue == 0)
                    {
                        ERPProductCode = MATNR_STRING;
                    }
                    else
                    {
                        ERPProductCode = MATNRValue.ToString();
                    }
                }

                try
                {
                    #region Collect Data
                    var MANDT = dataRow["MANDT"].ToString();
                    var VERSO = dataRow["VERSO"].ToString();
                    var ARBPL = dataRow["ARBPL"].ToString();
                    var ARBPL_SUB = dataRow["ARBPL_SUB"].ToString();
                    var WERKS = dataRow["WERKS"].ToString();
                    string MATNR = ERPProductCode;
                    var ITMNO = dataRow["ITMNO"].ToString();
                    var KTEXT = dataRow["KTEXT"].ToString();
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
                    var BMEIN = dataRow["BMEIN"].ToString();

                    decimal? BMSCH = null;
                    var BMSCH_STRING = dataRow["BMSCH"].ToString();
                    BMSCH = ParseDecimal(BMSCH, BMSCH_STRING);

                    var PLNNR = dataRow["PLNNR"].ToString();
                    var LTXA1 = dataRow["LTXA1"].ToString();
                    var MEINS = dataRow["MEINS"].ToString();

                    decimal? MENGE = null;
                    var MENGE_STRING = dataRow["MENGE"].ToString();
                    MENGE = ParseDecimal(MENGE, MENGE_STRING);

                    decimal? VGW01 = null;
                    var VGW01_STRING = dataRow["VGW01"].ToString();
                    VGW01 = ParseDecimal(VGW01, VGW01_STRING);

                    decimal? VGW02 = null;
                    var VGW02_STRING = dataRow["VGW02"].ToString();
                    VGW02 = ParseDecimal(VGW02, VGW02_STRING);

                    decimal? VGW03 = null;
                    var VGW03_STRING = dataRow["VGW03"].ToString();
                    VGW03 = ParseDecimal(VGW03, VGW03_STRING);

                    decimal? VGW04 = null;
                    var VGW04_STRING = dataRow["VGW04"].ToString();
                    VGW04 = ParseDecimal(VGW04, VGW04_STRING);

                    decimal? VGW05 = null;
                    var VGW05_STRING = dataRow["VGW05"].ToString();
                    VGW05 = ParseDecimal(VGW05, VGW05_STRING);

                    decimal? VGW06 = null;
                    var VGW06_STRING = dataRow["VGW06"].ToString();
                    VGW06 = ParseDecimal(VGW06, VGW06_STRING);

                    var ACTON = dataRow["ACTON"].ToString();
                    DateTime? ANDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["ANDAT"].ToString()) && dataRow["ANDAT"].ToString() != "00000000")
                    {
                        string date = dataRow["ANDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        ANDAT = DateTime.Parse(date);
                    }
                    var AENAM = dataRow["AENAM"].ToString();
                    DateTime? AEDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["AEDAT"].ToString()) && dataRow["AEDAT"].ToString() != "00000000")
                    {
                        string date = dataRow["AEDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        AEDAT = DateTime.Parse(date);
                    }
                    #endregion
                    var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefaultAsync();
                    //NVL
                    var matInDb = await _context.ProductModel.Where(p => p.ERPProductCode == IDNRK_MES && p.CompanyId == companyId).FirstOrDefaultAsync();
                    if (matInDb == null && !string.IsNullOrEmpty(IDNRK_MES))
                    {
                        syncMaterial(WERKS, IDNRK_MES);
                    }
                    var routingInventorDb = await _context.RoutingInventorModel.Where(p => p.VERSO == VERSO && p.WERKS == WERKS && p.MATNR == MATNR && p.ARBPL == ARBPL && p.ARBPL_SUB == ARBPL_SUB && p.ITMNO == ITMNO).FirstOrDefaultAsync();
                    //Nếu chưa có thì thêm mới : Có rồi thì update
                    if (routingInventorDb == null)
                    {
                        #region Insert
                        var routingInventorNew = new RoutingInventorModel
                        {
                            RoutingInventorId = Guid.NewGuid(),
                            MANDT = MANDT,
                            ARBPL = ARBPL,
                            ACTON = ACTON,
                            AEDAT = AEDAT,
                            AENAM = AENAM,
                            ANDAT = ANDAT,
                            ARBPL_SUB = ARBPL_SUB,
                            BMEIN = BMEIN,
                            BMSCH = BMSCH,
                            IDNRK = IDNRK,
                            IDNRK_MES = IDNRK_MES,
                            ITMNO = ITMNO,
                            KTEXT = KTEXT,
                            LTXA1 = LTXA1,
                            MAKTX = MAKTX,
                            MATNR = MATNR,
                            MEINS = MEINS,
                            MENGE = MENGE,
                            PLNNR = PLNNR,
                            VERSO = VERSO,
                            VGW01 = VGW01,
                            VGW02 = VGW02,
                            VGW03 = VGW03,
                            VGW04 = VGW04,
                            VGW05 = VGW05,
                            VGW06 = VGW06,
                            WERKS = WERKS,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(routingInventorNew).State = EntityState.Added;
                        #endregion
                        insertTotal++;
                    }
                    else
                    {
                        #region Update

                        routingInventorDb.MANDT = MANDT;
                        routingInventorDb.ARBPL = ARBPL;
                        routingInventorDb.ACTON = ACTON;
                        routingInventorDb.AEDAT = AEDAT;
                        routingInventorDb.AENAM = AENAM;
                        routingInventorDb.ANDAT = ANDAT;
                        routingInventorDb.ARBPL_SUB = ARBPL_SUB;
                        routingInventorDb.BMEIN = BMEIN;
                        routingInventorDb.BMSCH = BMSCH;
                        routingInventorDb.IDNRK = IDNRK;
                        routingInventorDb.IDNRK_MES = IDNRK_MES;
                        routingInventorDb.ITMNO = ITMNO;
                        routingInventorDb.KTEXT = KTEXT;
                        routingInventorDb.LTXA1 = LTXA1;
                        routingInventorDb.MAKTX = MAKTX;
                        routingInventorDb.MATNR = MATNR;
                        routingInventorDb.MEINS = MEINS;
                        routingInventorDb.MENGE = MENGE;
                        routingInventorDb.PLNNR = PLNNR;
                        routingInventorDb.VERSO = VERSO;
                        routingInventorDb.VGW01 = VGW01;
                        routingInventorDb.VGW02 = VGW02;
                        routingInventorDb.VGW03 = VGW03;
                        routingInventorDb.VGW04 = VGW04;
                        routingInventorDb.VGW05 = VGW05;
                        routingInventorDb.VGW06 = VGW06;
                        routingInventorDb.WERKS = WERKS;
                        routingInventorDb.LastEditTime = DateTime.Now;
                        _context.Entry(routingInventorDb).State = EntityState.Modified;
                        #endregion
                        updateTotal++;
                    }
                    await _context.SaveChangesAsync();
                    //Xác nhận insert vào DB thành công cho SAP
                    // ConfirmInsert(EQUNR, BUKRS);
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

            //if (dataTable != null && dataTable.Rows.Count > 0)
            //{
            //    //insert thêm dòng cho routing để ghi nhận sản lượng cho sản phẩm
            //    var existsRecord = await _context.RoutingInventorModel.Where(p => p.WERKS == CompanyCode && p.MATNR == ERPProductCode && p.ITMNO == "-").FirstOrDefaultAsync();
            //    if (existsRecord == null)
            //    {
            //        var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == CompanyCode).Select(p => p.CompanyId).FirstOrDefaultAsync();
            //        var productName = await _context.ProductModel.Where(p => p.ERPProductCode == ERPProductCode && p.CompanyId == companyId).Select(p => p.ProductName).FirstOrDefaultAsync();
            //        var routingInventorNew = new RoutingInventorModel
            //        {
            //            RoutingInventorId = Guid.NewGuid(),
            //            ITMNO = "-",
            //            KTEXT = productName,
            //            MATNR = ERPProductCode,
            //            WERKS = CompanyCode,
            //            CreateTime = DateTime.Now
            //        };
            //        _context.Entry(routingInventorNew).State = EntityState.Added;
            //        await _context.SaveChangesAsync();
            //    }
            //}

            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMaterial:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
            message += $"Insert: {insertTotal}, Update: {updateTotal}. Total: {dataTable.Rows.Count}";

            return new Tuple<string, string>(message, error);

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

        private static int? ParseInt(int? MENGE, string MENGE_STRING)
        {
            if (!string.IsNullOrEmpty(MENGE_STRING))
            {
                int ret;
                if (int.TryParse(MENGE_STRING, out ret))
                {
                    MENGE = ret;
                }
            }
            return MENGE;
        }

        public string ResetRouting()
        {
            string result = string.Empty;
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_RESET_MASTERDATA);

                //function.SetValue("I_ACTION", "Routing");

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
