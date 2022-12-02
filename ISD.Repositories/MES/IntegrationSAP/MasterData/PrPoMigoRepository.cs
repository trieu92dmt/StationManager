using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.MES.IntegrationSAP.MasterData
{
    public class PrPoMigoRepository
    {
        EntityDataContext _context;
        //SAPRepository _sap;
        LoggerRepository _loggerRepository;
        public PrPoMigoRepository(EntityDataContext entityDataContext)
        {
            _context = entityDataContext;
            //_sap = new SAPRepository();
            _loggerRepository = new LoggerRepository();
        }

        private List<DataTable> GetPrPoMigoData(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_PRPO_MIGO);

                //function.SetValue("I_VBELN", paramRequest.VBELN);
                //function.SetValue("I_WERKS", paramRequest.CompanyCode);

                //function.Invoke(destination);

                //var datatable1 = function.GetTable("IT_PR").ToDataTable("IT_PR");
                //dataTables.Add(datatable1);
                //var datatable2 = function.GetTable("IT_PO").ToDataTable("IT_PO");
                //dataTables.Add(datatable2);
                //var datatable3 = function.GetTable("IT_MIGO").ToDataTable("IT_MIGO");
                //dataTables.Add(datatable3);
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

        private List<DataTable> GetPOData(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_PRPO_RETURN);

                //function.SetValue("I_VBELN", paramRequest.VBELN);
                //function.SetValue("I_WERKS", paramRequest.CompanyCode);

                //function.Invoke(destination);

                //var datatable1 = function.GetTable("IT_PO").ToDataTable("IT_PO");
                //dataTables.Add(datatable1);
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

        private List<DataTable> GetMigoData(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_MIGO);

                //function.SetValue("I_VBELN", paramRequest.VBELN);
                //function.SetValue("I_WERKS", paramRequest.CompanyCode);

                //function.Invoke(destination);

                //var datatable1 = function.GetTable("IT_MIGO").ToDataTable("IT_MIGO");
                //dataTables.Add(datatable1);
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

        private void ConfirmInsert(string AUFNR, string KDAUF)
        {

            //var destination = _sap.GetRfcWithConfig();
            ////Định nghĩa hàm cần gọi
            //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_PRPO_MIGO);

            //function.SetValue("I_AUFNR", AUFNR);
            //function.SetValue("I_KDAUF", KDAUF);
            //function.SetValue("I_INSERT", 'X');//Tham số xác nhận insert thành công

            //function.Invoke(destination);

        }

        public string ResertProductionOrder()
        {
            string result = string.Empty;
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_RESET_MASTERDATA);

                //function.SetValue("I_ACTION", "PRPO_MIGO");

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
        public async Task<string> SyncPrPoMigo(ParamRequestSyncSapModel materialParam)
        {
            var error = string.Empty;
            var message = string.Empty;

            #region PR PO: Kéo đám mua hàng, chuyển, xuất tiêu hao,....
            var dataTables = GetPrPoMigoData(materialParam, out error);
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
                    var PR = dataTables[0];
                    var PO = dataTables[1];
                    //var MIGO = dataTables[2];


                    var taskPR = await InsertUpdateTabPR(PR, materialParam.CompanyCode, materialParam.VBELN);

                    if (!string.IsNullOrEmpty(taskPR))
                    {
                        return taskPR;
                    }
                    var taskPO = await InsertUpdateTabPO(PO, materialParam.CompanyCode, materialParam.VBELN);

                    if (!string.IsNullOrEmpty(taskPO))
                    {
                        return taskPO;
                    }

                    //var taskProOrder = await InsertUpdateTabMigo(MIGO);

                    //if (!string.IsNullOrEmpty(taskProOrder.Item2))
                    //{
                    //    return taskProOrder.Item2;
                    //}
                    //message = taskProOrder.Item1;

                    //stopwatch.Stop();
                    //TimeSpan timeSpan = stopwatch.Elapsed;
                    //string elapsedTime = String.Format("Sync ProOrder:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

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
                //return error;
            }
            #endregion

            #region PO: ZMES_FM_PRPO_RETURN >> Kéo đám trả hàng
            var dataTablesPO = GetPOData(materialParam, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return error;
            }

            if (dataTablesPO != null && dataTablesPO.Count > 0)
            {
                try
                {
                    var PO = dataTablesPO[0];

                    var taskPO = await InsertUpdateTabPO(PO, materialParam.CompanyCode, materialParam.VBELN);

                    if (!string.IsNullOrEmpty(taskPO))
                    {
                        return taskPO;
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
            #endregion

            #region MIGO
            var dataTablesMIGO = GetMigoData(materialParam, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return error;
            }

            if (dataTablesMIGO != null && dataTablesMIGO.Count > 0)
            {
                try
                {
                    var MIGO = dataTablesMIGO[0];

                    var taskMIGO = await InsertUpdateTabMigo(MIGO, materialParam.CompanyCode, materialParam.VBELN);

                    if (!string.IsNullOrEmpty(taskMIGO.Item2))
                    {
                        return taskMIGO.Item2;
                    }
                    message = taskMIGO.Item1;
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
            #endregion

            return message;
        }

        private async Task<Tuple<string, string>> InsertUpdateTabMigo(DataTable dataTable, string WERKS_PARAM, string VBELN_PARAM)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            string message = string.Empty;
            int insertTotal = 0, updateTotal = 0;
            List<MigoModel> listMIGOFromSAP = new List<MigoModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                listMIGOFromSAP.Add(new MigoModel
                {
                    WERKS = dataRow["WERKS"].ToString(),
                    VBELN = dataRow["VBELN"].ToString(),
                });
                try
                {
                    #region Collect Data
                    var MBLNR = dataRow["MBLNR"].ToString();
                    var MJAHR = dataRow["MJAHR"].ToString();
                    var ZEILE = dataRow["ZEILE"].ToString();
                    var VBELN = dataRow["VBELN"].ToString();
                    var BWART = dataRow["BWART"].ToString();
                    var XAUTO = dataRow["XAUTO"].ToString();
                    var MATNR = dataRow["MATNR"].ToString();
                    if (!string.IsNullOrEmpty(MATNR))
                    {
                        MATNR = MATNR.TrimStart(new Char[] { '0' });
                    }
                    var WERKS = dataRow["WERKS"].ToString();
                    var LGORT = dataRow["LGORT"].ToString();
                    var CHARG = dataRow["CHARG"].ToString();
                    var INSMK = dataRow["INSMK"].ToString();
                    var SOBKZ = dataRow["SOBKZ"].ToString();
                    var SHKZG = dataRow["SHKZG"].ToString();
                    decimal? MENGE = null;
                    if (!string.IsNullOrEmpty(dataRow["MENGE"].ToString()))
                    {
                        MENGE = decimal.Parse(dataRow["MENGE"].ToString());
                    }
                    var MEINS = dataRow["MEINS"].ToString();
                    decimal? ERFMG = null;
                    if (!string.IsNullOrEmpty(dataRow["ERFMG"].ToString()))
                    {
                        ERFMG = decimal.Parse(dataRow["ERFMG"].ToString());
                    }
                    var ERFME = dataRow["ERFME"].ToString();
                    var EBELN = dataRow["EBELN"].ToString();
                    var EBELP = dataRow["EBELP"].ToString();
                    var RSNUM = dataRow["RSNUM"].ToString();
                    var RSPOS = dataRow["RSPOS"].ToString();
                    var AUFNR = dataRow["AUFNR"].ToString();
                    #endregion

                    //Tìm MType theo SP 
                    string MType = null;
                    var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefaultAsync();
                    var material = await _context.ProductModel.Where(p => p.ERPProductCode == MATNR && p.CompanyId == companyId).FirstOrDefaultAsync();
                    if (material != null)
                    {
                        MType = material.MTART;
                    }
                    else
                    {
                        var newMat = await syncMaterial(companyId, WERKS, MATNR);
                        if (newMat != null)
                        {
                            MType = newMat.MTART;
                        }
                    }

                    //Nếu chưa có thì thêm mới
                    var poInDB = await _context.MigoModel.Where(p => p.MBLNR == MBLNR && p.MJAHR == MJAHR && p.ZEILE == ZEILE).FirstOrDefaultAsync();
                    if (poInDB == null)
                    {
                        #region Insert
                        var migo = new MigoModel
                        {
                            MigoId = Guid.NewGuid(),
                            WERKS = WERKS,
                            AUFNR = AUFNR,
                            BWART = BWART,
                            CHARG = CHARG,
                            EBELN = EBELN,
                            EBELP = EBELP,
                            ERFME = ERFME,
                            ERFMG = ERFMG,
                            INSMK = INSMK,
                            LGORT = LGORT,
                            MATNR = MATNR,
                            MBLNR = MBLNR,
                            MEINS = MEINS,
                            MENGE = MENGE,
                            MJAHR = MJAHR,
                            RSNUM = RSNUM,
                            RSPOS = RSPOS,
                            SHKZG = SHKZG,
                            SOBKZ = SOBKZ,
                            XAUTO = XAUTO,
                            ZEILE = ZEILE,
                            VBELN = VBELN,
                            MType = MType,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(migo).State = EntityState.Added;
                        #endregion
                        insertTotal++;
                    }
                    //Nếu có thì cập nhật
                    else
                    {
                        //#region Update
                        //poInDB.WERKS = WERKS;
                        //poInDB.AUFNR = AUFNR;
                        //poInDB.BWART = BWART;
                        //poInDB.CHARG = CHARG;
                        //poInDB.EBELN = EBELN;
                        //poInDB.EBELP = EBELP;
                        //poInDB.ERFME = ERFME;
                        //poInDB.ERFMG = ERFMG;
                        //poInDB.INSMK = INSMK;
                        //poInDB.LGORT = LGORT;
                        //poInDB.MATNR = MATNR;
                        //poInDB.MBLNR = MBLNR;
                        //poInDB.MEINS = MEINS;
                        //poInDB.MENGE = MENGE;
                        //poInDB.MJAHR = MJAHR;
                        //poInDB.RSNUM = RSNUM;
                        //poInDB.RSPOS = RSPOS;
                        //poInDB.SHKZG = SHKZG;
                        //poInDB.SOBKZ = SOBKZ;
                        //poInDB.XAUTO = XAUTO;
                        //poInDB.ZEILE = ZEILE;
                        //poInDB.VBELN = VBELN;
                        //poInDB.LastEditTime = DateTime.Now;
                        //_context.Entry(poInDB).State = EntityState.Modified;
                        //#endregion
                        //updateTotal++;
                    }
                    await _context.SaveChangesAsync();
                    //_loggerRepository.Logging(string.Format("SYNC SUCCESSFULLY (MBLNR: {0}, MJAHR: {1}, ZEILE: {2})"
                    //                                                  , MBLNR
                    //                                                  , MJAHR
                    //                                                  , ZEILE), "SYNC MIGO");
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

            //3. Nếu tồn tại trên MES mà trên SAP không có => trên SAP đã xóa SO LINE này => xóa trên MES => set isDeleted = true
            //if (listMIGOFromSAP != null && listMIGOFromSAP.Count > 0)
            //{
            //    var existsList = (from a in _context.MigoModel
            //                      where a.WERKS == WERKS_PARAM && a.VBELN == VBELN_PARAM
            //                      select new { a.MigoId, a.VBELN, a.WERKS }
            //                           ).ToList();
            //    var deleteItemLst = existsList.Where(t2 => !listMIGOFromSAP.Any(t1 => t1.VBELN == t2.VBELN && t1.WERKS == t2.WERKS)).ToList();
            //    if (deleteItemLst != null && deleteItemLst.Count > 0)
            //    {
            //        foreach (var item in deleteItemLst)
            //        {
            //            var deleteItem = _context.MigoModel.Where(p => p.MigoId == item.MigoId).FirstOrDefault();
            //            if (deleteItem != null)
            //            {
            //                deleteItem.isDeleted = true;
            //                deleteItem.DeletedTime = DateTime.Now;
            //                _context.Entry(deleteItem).State = EntityState.Modified;
            //                _context.SaveChanges();
            //            }
            //        }
            //    }
            //}
            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMARM:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
            message += $"Insert: {insertTotal}, Update: {updateTotal}. Total: {dataTable.Rows.Count}";

            return new Tuple<string, string>(message, error);
        }

        private async Task<string> InsertUpdateTabPO(DataTable dataTable, string WERKS_PARAM, string VBELN_PARAM)
        {
            string error = string.Empty;
            List<PurchaseOrderModel> listPOFromSAP = new List<PurchaseOrderModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                listPOFromSAP.Add(new PurchaseOrderModel
                {
                    WERKS = dataRow["WERKS"].ToString(),
                    VBELN = dataRow["VBELN"].ToString(),
                });
                try
                {
                    #region Collect Data
                    var EBELN = dataRow["EBELN"].ToString();
                    if (!string.IsNullOrEmpty(EBELN))
                    {
                        var paramRequest = new ParamRequestSyncSapModel
                        {
                            IEBELN = EBELN,
                            CompanyCode = dataRow["WERKS"].ToString(),
                        };
                        syncPOTEXT_SO_PR(paramRequest);
                    }
                    var EBELP = dataRow["EBELP"].ToString();
                    var VBELN = dataRow["VBELN"].ToString();
                    var BUKRS = dataRow["BUKRS"].ToString();
                    var BSART = dataRow["BSART"].ToString();
                    var LOEKZ = dataRow["LOEKZ"].ToString();
                    var LIFNR = dataRow["LIFNR"].ToString();
                    var ZTERM = dataRow["ZTERM"].ToString();
                    var EKORG = dataRow["EKORG"].ToString();
                    var EKGRP = dataRow["EKGRP"].ToString();
                    var WAERS = dataRow["WAERS"].ToString();
                    var WKURS = dataRow["WKURS"].ToString();
                    DateTime? BEDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["BEDAT"].ToString()) && dataRow["BEDAT"].ToString() != "00000000")
                    {
                        string date = dataRow["BEDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        BEDAT = DateTime.Parse(date);
                    }
                    var RESWK = dataRow["RESWK"].ToString();
                    var WERKS = dataRow["WERKS"].ToString();
                    var LGORT = dataRow["LGORT"].ToString();
                    var MATNR = dataRow["MATNR"].ToString();
                    if (!string.IsNullOrEmpty(MATNR))
                    {
                        MATNR = MATNR.TrimStart(new Char[] { '0' });
                    }
                    var MTART = dataRow["MTART"].ToString();
                    var MATKL = dataRow["MATKL"].ToString();

                    decimal? MENGE = null;
                    var MENGE_STRING = dataRow["MENGE"].ToString();
                    MENGE = ParseDecimal(MENGE, MENGE_STRING);

                    var MEINS = dataRow["MEINS"].ToString();
                    var LMEIN = dataRow["LMEIN"].ToString();

                    decimal? UMREZ = null;
                    var UMREZ_STRING = dataRow["UMREZ"].ToString();
                    UMREZ = ParseDecimal(UMREZ, UMREZ_STRING);

                    decimal? UMREN = null;
                    var UMREN_STRING = dataRow["UMREN"].ToString();
                    UMREN = ParseDecimal(UMREN, UMREN_STRING);

                    var ELIKZ = dataRow["ELIKZ"].ToString();
                    var PSTYP = dataRow["PSTYP"].ToString();
                    var KNTTP = dataRow["KNTTP"].ToString();
                    var RETPO = dataRow["RETPO"].ToString();
                    var BANFN = dataRow["BANFN"].ToString();
                    var BNFPO = dataRow["BNFPO"].ToString();

                    decimal? GR_QTY = null;
                    var GR_QTY_STRING = dataRow["GR_QTY"].ToString();
                    GR_QTY = ParseDecimal(GR_QTY, GR_QTY_STRING);

                    DateTime? EINDT = null;
                    if (!string.IsNullOrEmpty(dataRow["EINDT"].ToString()) && dataRow["EINDT"].ToString() != "00000000")
                    {
                        string date = dataRow["EINDT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        EINDT = DateTime.Parse(date);
                    }

                    //(( EKPO~MENGE * EKPO~UMREZ ) / EKPO~UMREN ) AS MENGE_UOM
                    decimal? MENGE_UOM = MENGE;
                    if (MENGE.HasValue && UMREZ.HasValue && UMREN.HasValue && UMREN > 0)
                    {
                        MENGE_UOM = MENGE * UMREZ / UMREN;
                    }

                    //(( EKBE~GR_QTY * EKPO~UMREZ ) / EKPO~UMREN ) AS GR_QTY_UOM
                    decimal? GR_QTY_UOM = GR_QTY;
                    if (GR_QTY.HasValue && UMREZ.HasValue && UMREN.HasValue && UMREN > 0)
                    {
                        GR_QTY_UOM = GR_QTY * UMREZ / UMREN;
                    }
                    #endregion

                    //Nếu chưa có thì thêm mới
                    var poInDB = await _context.PurchaseOrderModel.Where(p => p.VBELN == VBELN && p.EBELN == EBELN && p.EBELP == EBELP && p.BUKRS == BUKRS && p.BSART == BSART).FirstOrDefaultAsync();
                    if (poInDB == null)
                    {
                        #region Insert
                        var purchaseOrderNew = new PurchaseOrderModel
                        {
                            PurchaseOrderId = Guid.NewGuid(),
                            LGORT = LGORT,
                            LOEKZ = LOEKZ,
                            LIFNR = LIFNR,
                            EBELN = EBELN,
                            MATNR = MATNR,
                            MTART = MTART,
                            MATKL = MATKL,
                            MEINS = MEINS,
                            LMEIN = LMEIN,
                            MENGE = MENGE,
                            PSTYP = PSTYP,
                            BANFN = BANFN,
                            BEDAT = BEDAT,
                            BNFPO = BNFPO,
                            BSART = BSART,
                            BUKRS = BUKRS,
                            EBELP = EBELP,
                            EKGRP = EKGRP,
                            EKORG = EKORG,
                            ELIKZ = ELIKZ,
                            KNTTP = KNTTP,
                            RESWK = RESWK,
                            RETPO = RETPO,
                            UMREN = UMREN,
                            UMREZ = UMREZ,
                            VBELN = VBELN,
                            WAERS = WAERS,
                            WERKS = WERKS,
                            WKURS = WKURS,
                            ZTERM = ZTERM,
                            GR_QTY = GR_QTY,
                            EINDT = EINDT,
                            MENGE_UOM = MENGE_UOM,
                            GR_QTY_UOM = GR_QTY_UOM,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(purchaseOrderNew).State = EntityState.Added;

                        #endregion Insert
                    }
                    //Nếu có rồi thì cập nhật
                    else
                    {
                        poInDB.LGORT = LGORT;
                        poInDB.LOEKZ = LOEKZ;
                        poInDB.LIFNR = LIFNR;
                        poInDB.EBELN = EBELN;
                        poInDB.MATNR = MATNR;
                        poInDB.MTART = MTART;
                        poInDB.MATKL = MATKL;
                        poInDB.MEINS = MEINS;
                        poInDB.LMEIN = LMEIN;
                        poInDB.MENGE = MENGE;
                        poInDB.PSTYP = PSTYP;
                        poInDB.BANFN = BANFN;
                        poInDB.BEDAT = BEDAT;
                        poInDB.BNFPO = BNFPO;
                        poInDB.BSART = BSART;
                        poInDB.BUKRS = BUKRS;
                        poInDB.EBELP = EBELP;
                        poInDB.EKGRP = EKGRP;
                        poInDB.EKORG = EKORG;
                        poInDB.ELIKZ = ELIKZ;
                        poInDB.KNTTP = KNTTP;
                        poInDB.RESWK = RESWK;
                        poInDB.RETPO = RETPO;
                        poInDB.UMREN = UMREN;
                        poInDB.UMREZ = UMREZ;
                        poInDB.VBELN = VBELN;
                        poInDB.WAERS = WAERS;
                        poInDB.WERKS = WERKS;
                        poInDB.WKURS = WKURS;
                        poInDB.ZTERM = ZTERM;
                        poInDB.GR_QTY = GR_QTY;
                        poInDB.EINDT = EINDT;
                        poInDB.MENGE_UOM = MENGE_UOM;
                        poInDB.GR_QTY_UOM = GR_QTY_UOM;
                        poInDB.LastEditTime = DateTime.Now;
                        _context.Entry(poInDB).State = EntityState.Modified;
                    }
                    await _context.SaveChangesAsync();
                    //_loggerRepository.Logging(string.Format("SYNC SUCCESSFULLY (VBELN: {0}, EBELN: {1}, EBELP: {2})"
                    //                                                   , VBELN
                    //                                                   , EBELN
                    //                                                   , EBELP), "SYNC PO");
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

            //3. Nếu tồn tại trên MES mà trên SAP không có => trên SAP đã xóa SO LINE này => xóa trên MES => set isDeleted = true
            //if (listPOFromSAP != null && listPOFromSAP.Count > 0)
            //{
            //    var deleteLst = (from a in _context.PurchaseOrderModel
            //                     where !listPOFromSAP.Any(k => k.VBELN == a.VBELN && k.EBELN == a.EBELN && k.EBELP == a.EBELP && k.BUKRS == a.BUKRS && k.BSART == a.BSART)
            //                     select a.PurchaseOrderId
            //                     ).ToList();
            //    if (deleteLst != null && deleteLst.Count > 0)
            //    {
            //        foreach (var item in deleteLst)
            //        {
            //            var deleteItem = _context.PurchaseOrderModel.Where(p => p.PurchaseOrderId == item).FirstOrDefault();
            //            if (deleteItem != null)
            //            {
            //                deleteItem.isDeleted = true;
            //                deleteItem.DeletedTime = DateTime.Now;
            //                _context.Entry(deleteItem).State = EntityState.Modified;
            //                _context.SaveChanges();
            //            }
            //        }
            //    }
            //}

            //3. Nếu tồn tại trên MES mà trên SAP không có => trên SAP đã xóa SO LINE này => xóa trên MES => set isDeleted = true
            if (listPOFromSAP != null && listPOFromSAP.Count > 0)
            {
                var existsList = (from a in _context.PurchaseOrderModel
                                  where a.WERKS == WERKS_PARAM && a.VBELN == VBELN_PARAM
                                  select new { a.PurchaseOrderId, a.VBELN, a.WERKS }
                                       ).ToList();
                var deleteItemLst = existsList.Where(t2 => !listPOFromSAP.Any(t1 => t1.VBELN == t2.VBELN && t1.WERKS == t2.WERKS)).ToList();
                if (deleteItemLst != null && deleteItemLst.Count > 0)
                {
                    foreach (var item in deleteItemLst)
                    {
                        var deleteItem = _context.PurchaseOrderModel.Where(p => p.PurchaseOrderId == item.PurchaseOrderId).FirstOrDefault();
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

            return error;

        }
        private async Task<string> InsertUpdateTabPR(DataTable dataTable, string WERKS_PARAM, string VBELN_PARAM)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            List<PurchaseRequisitionModel> listPRFromSAP = new List<PurchaseRequisitionModel>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                listPRFromSAP.Add(new PurchaseRequisitionModel
                {
                    WERKS = dataRow["WERKS"].ToString(),
                    VBELN = dataRow["VBELN"].ToString(),
                });
                try
                {
                    #region Collect Data
                    var BANFN = dataRow["BANFN"].ToString();
                    var BNFPO = dataRow["BNFPO"].ToString();
                    var BSART = dataRow["BSART"].ToString();
                    var LOEKZ = dataRow["LOEKZ"].ToString();
                    var FRGKZ = dataRow["FRGKZ"].ToString();
                    var EKGRP = dataRow["EKGRP"].ToString();
                    var TXZ01 = dataRow["TXZ01"].ToString();
                    var MATNR = dataRow["MATNR"].ToString();
                    if (!string.IsNullOrEmpty(MATNR))
                    {
                        MATNR = MATNR.TrimStart(new Char[] { '0' });
                    }
                    var WERKS = dataRow["WERKS"].ToString();
                    var LGORT = dataRow["LGORT"].ToString();
                    var BEDNR = dataRow["BEDNR"].ToString();
                    var MATKL = dataRow["MATKL"].ToString();
                    decimal? MENGE = null;
                    var MENGE_STRING = dataRow["MENGE"].ToString();
                    MENGE = ParseDecimal(MENGE, MENGE_STRING);
                    //decimal? MEINS = null;
                    //if (!string.IsNullOrEmpty(dataRow["MEINS"].ToString()))
                    //{
                    //    MEINS = decimal.Parse(dataRow["MEINS"].ToString());
                    //}
                    string MEINS = dataRow["MEINS"].ToString();
                    DateTime? LFDAT = null;
                    if (!string.IsNullOrEmpty(dataRow["LFDAT"].ToString()) && dataRow["LFDAT"].ToString() != "00000000")
                    {
                        string date = dataRow["LFDAT"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        LFDAT = DateTime.Parse(date);
                    }
                    var PSTYP = dataRow["PSTYP"].ToString();
                    var KNTTP = dataRow["KNTTP"].ToString();
                    var EKORG = dataRow["EKORG"].ToString();
                    var VBELN = dataRow["VBELN"].ToString();
                    var AFNAM = dataRow["AFNAM"].ToString();

                    decimal? BSMNG_SUM_PO = null;
                    var BSMNG_SUM_PO_STRING = dataRow["BSMNG_SUM_PO"].ToString();
                    BSMNG_SUM_PO = ParseDecimal(BSMNG_SUM_PO, BSMNG_SUM_PO_STRING);

                    decimal? GR_QTY = null;
                    var GR_QTY_STRING = dataRow["GR_QTY"].ToString();
                    GR_QTY = ParseDecimal(GR_QTY, GR_QTY_STRING);
                    #endregion

                    //Tìm MType theo SP 
                    string MType = null;
                    var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefaultAsync();
                    var material = await _context.ProductModel.Where(p => p.ERPProductCode == MATNR && p.CompanyId == companyId).FirstOrDefaultAsync();
                    if (material != null)
                    {
                        MType = material.MTART;
                    }
                    else
                    {
                        var newMat = await syncMaterial(companyId, WERKS, MATNR);
                        if (newMat != null)
                        {
                            MType = newMat.MTART;
                        }
                    }

                    //Nếu chưa có thì thêm mới
                    var prInDB = await _context.PurchaseRequisitionModel.Where(p => p.BANFN == BANFN && p.BNFPO == BNFPO && p.BSART == BSART).FirstOrDefaultAsync();
                    if (prInDB == null)
                    {
                        #region Insert
                        var purchaseNew = new PurchaseRequisitionModel
                        {
                            PurchaseRequisitionId = Guid.NewGuid(),
                            MATNR = MATNR,
                            BANFN = BANFN,
                            BEDNR = BEDNR,
                            BNFPO = BNFPO,
                            BSART = BSART,
                            EKGRP = EKGRP,
                            EKORG = EKORG,
                            FRGKZ = FRGKZ,
                            KNTTP = KNTTP,
                            LFDAT = LFDAT,
                            LGORT = LGORT,
                            LOEKZ = LOEKZ,
                            MATKL = MATKL,
                            MEINS = MEINS,
                            MENGE = MENGE,
                            PSTYP = PSTYP,
                            TXZ01 = TXZ01,
                            VBELN = VBELN,
                            AFNAM = AFNAM,
                            WERKS = WERKS,
                            BSMNG_SUM_PO = BSMNG_SUM_PO,
                            GR_QTY = GR_QTY,
                            MType = MType,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(purchaseNew).State = EntityState.Added;
                        #endregion
                    }
                    //Nếu đã có thì cập nhật
                    else
                    {
                        prInDB.MATNR = MATNR;
                        prInDB.BANFN = BANFN;
                        prInDB.BEDNR = BEDNR;
                        prInDB.BNFPO = BNFPO;
                        prInDB.BSART = BSART;
                        prInDB.EKGRP = EKGRP;
                        prInDB.EKORG = EKORG;
                        prInDB.FRGKZ = FRGKZ;
                        prInDB.KNTTP = KNTTP;
                        prInDB.LFDAT = LFDAT;
                        prInDB.LGORT = LGORT;
                        prInDB.LOEKZ = LOEKZ;
                        prInDB.MATKL = MATKL;
                        prInDB.MEINS = MEINS;
                        prInDB.MENGE = MENGE;
                        prInDB.PSTYP = PSTYP;
                        prInDB.TXZ01 = TXZ01;
                        prInDB.VBELN = VBELN;
                        prInDB.VBELN = VBELN;
                        prInDB.AFNAM = AFNAM;
                        prInDB.BSMNG_SUM_PO = BSMNG_SUM_PO;
                        prInDB.GR_QTY = GR_QTY;
                        prInDB.MType = MType;
                        prInDB.LastEditTime = DateTime.Now;
                        _context.Entry(prInDB).State = EntityState.Modified;
                    }

                    await _context.SaveChangesAsync();
                    //_loggerRepository.Logging(string.Format("SYNC SUCCESSFULLY (BANFN: {0}, BNFPO: {1})"
                    //                                                    , BANFN
                    //                                                    , BNFPO), "SYNC PR");
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

            //3. Nếu tồn tại trên MES mà trên SAP không có => trên SAP đã xóa SO LINE này => xóa trên MES => set isDeleted = true
            if (listPRFromSAP != null && listPRFromSAP.Count > 0)
            {
                var existsList = (from a in _context.PurchaseRequisitionModel
                                  where a.WERKS == WERKS_PARAM && a.VBELN == VBELN_PARAM
                                  select new { a.PurchaseRequisitionId, a.VBELN, a.WERKS }
                                       ).ToList();
                var deleteItemLst = existsList.Where(t2 => !listPRFromSAP.Any(t1 => t1.VBELN == t2.VBELN && t1.WERKS == t2.WERKS)).ToList();
                if (deleteItemLst != null && deleteItemLst.Count > 0)
                {
                    foreach (var item in deleteItemLst)
                    {
                        var deleteItem = _context.PurchaseRequisitionModel.Where(p => p.PurchaseRequisitionId == item.PurchaseRequisitionId).FirstOrDefault();
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

            //stopwatch.Stop();
            //TimeSpan timeSpan = stopwatch.Elapsed;
            //string elapsedTime = String.Format("InsertUpdateMARM:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //_loggerRepository.Logging(elapsedTime, "INFO");
            return error;
        }

        private List<DataTable> GetPOTEXT_SO_PR(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_PO_POTEXTPRSO);

                //function.SetValue("I_EBELN", paramRequest.IEBELN);
                //function.SetValue("I_WERKS", paramRequest.CompanyCode);
                //function.SetValue("I_BSART", "ZPO6");

                //function.Invoke(destination);

                //var datatable1 = function.GetTable("IT_DATA_PO_POTEXT").ToDataTable("IT_DATA_PO_POTEXT");
                //dataTables.Add(datatable1);
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

        public string syncPOTEXT_SO_PR(ParamRequestSyncSapModel paramRequest)
        {
            string error = string.Empty;
            var dataTables = GetPOTEXT_SO_PR(paramRequest, out error);
            if (dataTables != null && dataTables.Count > 0)
            {
                try
                {
                    var IT_DATA_PO_POTEXT = dataTables[0];

                    if (IT_DATA_PO_POTEXT != null && IT_DATA_PO_POTEXT.Rows.Count > 0)
                    {
                        foreach (DataRow PO_POTEXT in IT_DATA_PO_POTEXT.Rows)
                        {
                            #region Collect Data
                            var BSART = PO_POTEXT["BSART"].ToString();
                            var EBELN = PO_POTEXT["EBELN"].ToString();
                            var EBELP = PO_POTEXT["EBELP"].ToString();
                            var ZMES_ZPOREF = PO_POTEXT["ZMES_ZPOREF"].ToString();
                            var BANFN = PO_POTEXT["BANFN"].ToString();
                            var BNFPO = PO_POTEXT["BNFPO"].ToString();
                            var ZMES_VBELN = PO_POTEXT["ZMES_VBELN"].ToString();
                            var LOEKZ = PO_POTEXT["LOEKZ"].ToString();
                            var MATNR = PO_POTEXT["MATNR"].ToString();
                            if (!string.IsNullOrEmpty(MATNR))
                            {
                                MATNR = MATNR.TrimStart(new Char[] { '0' });
                            }
                            var WERKS = PO_POTEXT["WERKS"].ToString();
                            var BUKRS = PO_POTEXT["BUKRS"].ToString();
                            var TXZ01 = PO_POTEXT["TXZ01"].ToString();

                            decimal? MENGE = null;
                            var MENGE_STRING = PO_POTEXT["MENGE"].ToString();
                            MENGE = ParseDecimal(MENGE, MENGE_STRING);

                            var MEINS = PO_POTEXT["MEINS"].ToString();
                            var BPRME = PO_POTEXT["BPRME"].ToString();

                            decimal? UMREZ = null;
                            var UMREZ_STRING = PO_POTEXT["UMREZ"].ToString();
                            UMREZ = ParseDecimal(UMREZ, UMREZ_STRING);

                            decimal? UMREN = null;
                            var UMREN_STRING = PO_POTEXT["UMREN"].ToString();
                            UMREN = ParseDecimal(UMREN, UMREN_STRING);

                            DateTime? BEDAT = null;
                            if (!string.IsNullOrEmpty(PO_POTEXT["BEDAT"].ToString()) && PO_POTEXT["BEDAT"].ToString() != "00000000")
                            {
                                string date = PO_POTEXT["BEDAT"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                BEDAT = DateTime.Parse(date);
                            }

                            DateTime? AEDAT = null;
                            if (!string.IsNullOrEmpty(PO_POTEXT["AEDAT"].ToString()) && PO_POTEXT["AEDAT"].ToString() != "00000000")
                            {
                                string date = PO_POTEXT["AEDAT"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                AEDAT = DateTime.Parse(date);
                            }
                            var ZCHECK = PO_POTEXT["ZCHECK"].ToString();
                            var ERNAM = PO_POTEXT["ERNAM"].ToString();
                            var ZMES_LSXLON = PO_POTEXT["ZMES_LSXLON"].ToString();
                            var ZMES_IDLSX = PO_POTEXT["ZMES_IDLSX"].ToString();
                            var ZMES_POCN = PO_POTEXT["ZMES_POCN"].ToString();
                            #endregion

                            var POTEXT_DB = _context.POTEXT_PR_SO_Model.Where(p => p.BSART == BSART && p.EBELN == EBELN && p.EBELP == EBELP && p.ZMES_ZPOREF == ZMES_ZPOREF).FirstOrDefault();
                            //Chưa có thì thêm mới
                            if (POTEXT_DB == null)
                            {
                                #region Insert
                                var newPOTEXT = new POTEXT_PR_SO_Model();
                                newPOTEXT.POTEXTId = Guid.NewGuid();
                                newPOTEXT.BSART = BSART;
                                newPOTEXT.EBELN = EBELN;
                                newPOTEXT.EBELP = EBELP;
                                newPOTEXT.ZMES_ZPOREF = ZMES_ZPOREF;
                                newPOTEXT.BANFN = BANFN;
                                newPOTEXT.BNFPO = BNFPO;
                                newPOTEXT.ZMES_VBELN = ZMES_VBELN;
                                newPOTEXT.LOEKZ = LOEKZ;
                                newPOTEXT.MATNR = MATNR;
                                newPOTEXT.WERKS = WERKS;
                                newPOTEXT.BUKRS = BUKRS;
                                newPOTEXT.TXZ01 = TXZ01;
                                newPOTEXT.MENGE = MENGE;
                                newPOTEXT.MEINS = MEINS;
                                newPOTEXT.BPRME = BPRME;
                                newPOTEXT.UMREZ = UMREZ;
                                newPOTEXT.UMREN = UMREN;
                                newPOTEXT.BEDAT = BEDAT;
                                newPOTEXT.AEDAT = AEDAT;
                                newPOTEXT.ZCHECK = ZCHECK;
                                newPOTEXT.ERNAM = ERNAM;
                                newPOTEXT.ZMES_LSXLON = ZMES_LSXLON;
                                newPOTEXT.ZMES_IDLSX = ZMES_IDLSX;
                                newPOTEXT.ZMES_POCN = ZMES_POCN;
                               
                                //SO nào != 10 ký tự => SOInvalid = true
                                if (!string.IsNullOrEmpty(ZMES_VBELN) && ZMES_VBELN.Length != 10)
                                {
                                    newPOTEXT.SOInvalid = true;
                                }
                                newPOTEXT.CreateTime = DateTime.Now;

                                _context.Entry(newPOTEXT).State = EntityState.Added;
                                #endregion
                            }
                            //Có rồi thì cập nhật
                            else
                            {
                                #region Update
                                POTEXT_DB.BANFN = BANFN;
                                POTEXT_DB.BNFPO = BNFPO;
                                POTEXT_DB.ZMES_VBELN = ZMES_VBELN;
                                POTEXT_DB.LOEKZ = LOEKZ;
                                POTEXT_DB.MATNR = MATNR;
                                POTEXT_DB.WERKS = WERKS;
                                POTEXT_DB.BUKRS = BUKRS;
                                POTEXT_DB.TXZ01 = TXZ01;
                                POTEXT_DB.MENGE = MENGE;
                                POTEXT_DB.MEINS = MEINS;
                                POTEXT_DB.BPRME = BPRME;
                                POTEXT_DB.UMREZ = UMREZ;
                                POTEXT_DB.UMREN = UMREN;
                                POTEXT_DB.BEDAT = BEDAT;
                                POTEXT_DB.AEDAT = AEDAT;
                                POTEXT_DB.ZCHECK = ZCHECK;
                                POTEXT_DB.ERNAM = ERNAM;
                                POTEXT_DB.ZMES_LSXLON = ZMES_LSXLON;
                                POTEXT_DB.ZMES_IDLSX = ZMES_IDLSX;
                                POTEXT_DB.ZMES_POCN = ZMES_POCN;
                                //SO nào != 10 ký tự => SOInvalid = true
                                if (!string.IsNullOrEmpty(ZMES_VBELN))
                                {
                                    if (ZMES_VBELN.Length != 10)
                                    {
                                        POTEXT_DB.SOInvalid = true;
                                    }
                                    else
                                    {
                                        POTEXT_DB.SOInvalid = false;
                                    }
                                }
                                POTEXT_DB.LastEditTime = DateTime.Now;
                                _context.Entry(POTEXT_DB).State = EntityState.Modified;
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
                }
            }
            if (!string.IsNullOrEmpty(error))
            {
                _loggerRepository.Logging("Error: " + error, "GetPOTEXT_SO_PR");
            }
            return error;
        }

        //đồng bộ sản phẩm khi chưa tồn tại trên MES
        private async Task<ProductModel> syncMaterial(Guid CompanyId, string CompanyCode, string MaterialCode)
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
            var message = await _materialMasterRepository.GetMaterialMaster(materialParam);

            var proTotal = _context.ProductModel.Count();
            var marmTotal = _context.MarmModel.Count();
            _loggerRepository.Logging(message, "INFO");
            _loggerRepository.Logging($"Sync completed: Pro {proTotal}, Marn {marmTotal}", "INFO");

            var productInDB = await _context.ProductModel.Where(p => p.ProductCode == MaterialCode && p.CompanyId == CompanyId).FirstOrDefaultAsync();
            return productInDB;
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

    }
}
