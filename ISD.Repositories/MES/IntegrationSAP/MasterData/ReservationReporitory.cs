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
    public class ReservationReporitory
    {
        EntityDataContext _context;
        //SAPRepository _sap;
        LoggerRepository _loggerRepository;
        public ReservationReporitory(EntityDataContext entityDataContext)
        {
            _context = entityDataContext;
            //_sap = new SAPRepository();
            _loggerRepository = new LoggerRepository();
        }
        private DataTable GetReservationData(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            DataTable dataTable = new DataTable();
            try
            {
                //    var destination = _sap.GetRfcWithConfig();
                //    //Định nghĩa hàm cần gọi
                //    var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_RESERVATION);

                //    function.SetValue("I_UPDATE", paramRequest.IUpdate);
                //    function.SetValue("I_NEW", paramRequest.INew);
                //    function.SetValue("I_RECORD", paramRequest.IRecord);

                //    function.Invoke(destination);

                //    dataTable = function.GetTable("IT_DATA").ToDataTable("RESERVATION");
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
            return dataTable;

        }

        private OutputFromSAPViewModel ConfirmInsert(string RSNUM, string RSPOS)
        {
            var result = new OutputFromSAPViewModel();
            //var destination = _sap.GetRfcWithConfig();
            //Định nghĩa hàm cần gọi
            //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_RESERVATION);

            //function.SetValue("I_RSNUM", RSNUM);
            //function.SetValue("I_RSPOS", RSPOS);
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

        public string ResetReservation()
        {
            string result = string.Empty;
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_RESET_MASTERDATA);

                //function.SetValue("I_ACTION", "RESERVATION");

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

        public async Task<string> SyncReservation(ParamRequestSyncSapModel paramRequest)
        {
            var error = string.Empty;
            var message = string.Empty;
            var dataTable = GetReservationData(paramRequest, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return error;
            }

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                try
                {
                    //Stopwatch stopwatch = new Stopwatch();
                    //stopwatch.Start();
                    int insertTotal = 0, updateTotal = 0;
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        try
                        {
                            #region Collect Data
                            var RSNUM = dataRow["RSNUM"].ToString();
                            var RSPOS = dataRow["RSPOS"].ToString();
                            var BDART = dataRow["BDART"].ToString();
                            var RSSTA = dataRow["RSSTA"].ToString();
                            var XLOEK = dataRow["XLOEK"].ToString();
                            var XWAOK = dataRow["XWAOK"].ToString();
                            var KZEAR = dataRow["KZEAR"].ToString();
                            string MATNR = string.Empty;
                            if (!string.IsNullOrEmpty(dataRow["MATNR"].ToString()))
                            {
                                MATNR = Int32.Parse(dataRow["MATNR"].ToString()).ToString();
                            }
                            var WERKS = dataRow["WERKS"].ToString();
                            var LGORT = dataRow["LGORT"].ToString();
                            var CHARG = dataRow["CHARG"].ToString();
                            var SOBKZ = dataRow["SOBKZ"].ToString();
                            DateTime? BDTER = null;
                            if (!string.IsNullOrEmpty(dataRow["BDTER"].ToString()) && dataRow["BDTER"].ToString() != "00000000")
                            {
                                string date = dataRow["BDTER"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                BDTER = DateTime.Parse(date);
                            }
                            decimal? BDMNG = null;
                            if (!string.IsNullOrEmpty(dataRow["BDMNG"].ToString()))
                            {
                                BDMNG = decimal.Parse(dataRow["BDMNG"].ToString());
                            }
                            var MEINS = dataRow["MEINS"].ToString();
                            var SHKZG = dataRow["SHKZG"].ToString();
                            decimal? ERFMG = null;
                            if (!string.IsNullOrEmpty(dataRow["ERFMG"].ToString()))
                            {
                                ERFMG = decimal.Parse(dataRow["ERFMG"].ToString());
                            }
                            var ERFME = dataRow["ERFME"].ToString();
                            var PLNUM = dataRow["PLNUM"].ToString();
                            var BANFN = dataRow["BANFN"].ToString();
                            var BNFPO = dataRow["BNFPO"].ToString();
                            var AUFNR = dataRow["AUFNR"].ToString();
                            var KDAUF = dataRow["KDAUF"].ToString();
                            var KDPOS = dataRow["KDPOS"].ToString();
                            var KDEIN = dataRow["KDEIN"].ToString();
                            var BWART = dataRow["BWART"].ToString();
                            var SAKNR = dataRow["SAKNR"].ToString();
                            var UMWRK = dataRow["UMWRK"].ToString();
                            var UMLGO = dataRow["UMLGO"].ToString();
                            var POSTP = dataRow["POSTP"].ToString();

                            #endregion

                            var reservationInDb = await _context.ReservationModel.Where(p => p.RSNUM == RSNUM && p.RSPOS == RSPOS && p.BDART == BDART).FirstOrDefaultAsync();
                            // Nếu chưa có thì thêm mới
                            if (reservationInDb == null)
                            {
                                #region Insert
                                var reservationNew = new ReservationModel
                                {
                                    ReservationId = Guid.NewGuid(),
                                    RSNUM = RSNUM,
                                    AUFNR = AUFNR,
                                    BANFN = BANFN,
                                    BDART = BDART,
                                    BDMNG = BDMNG,
                                    BDTER = BDTER,
                                    BNFPO = BNFPO,
                                    BWART = BWART,
                                    CHARG = CHARG,
                                    ERFME = ERFME,
                                    ERFMG = ERFMG,
                                    KDAUF = KDAUF,
                                    KDEIN = KDEIN,
                                    KDPOS = KDPOS,
                                    KZEAR = KZEAR,
                                    LGORT = LGORT,
                                    MATNR = MATNR,
                                    MEINS = MEINS,
                                    PLNUM = PLNUM,
                                    POSTP = POSTP,
                                    RSPOS = RSPOS,
                                    RSSTA = RSSTA,
                                    SAKNR = SAKNR,
                                    SHKZG = SHKZG,
                                    SOBKZ = SOBKZ,
                                    UMLGO = UMLGO,
                                    UMWRK = UMWRK,
                                    WERKS = WERKS,
                                    XLOEK = XLOEK,
                                    XWAOK = XWAOK,
                                    CreateTime = DateTime.Now
                                };
                                _context.Entry(reservationNew).State = EntityState.Added;

                                #endregion Insert
                                insertTotal++;
                            }
                            //Chưa có thì cập nhật
                            else
                            {
                                //#region Update
                                ////reservationInDb.LastEditTime = DateTime.Now;
                                //_context.Entry(reservationInDb).State = EntityState.Modified;
                                //#endregion Update
                                //updateTotal++;
                            }
                            await _context.SaveChangesAsync();
                            //Xác nhận insert vào DB thành công cho SAP
                            var result = ConfirmInsert(RSNUM, RSPOS);
                            if (result.IsSuccess == false)
                            {
                                _loggerRepository.Logging(result.Message, string.Format("ERROR AT ZMES_FM_RESERVATION (RSNUM: {0}, RSPOS: {1})"
                                                                        , RSNUM
                                                                        , RSPOS));
                            }
                            else
                            {
                                _loggerRepository.Logging(string.Format("SYNC SUCCESSFULLY (RSNUM: {0}, RSPOS: {1})"
                                                                        , RSNUM
                                                                        , RSPOS), "SYNC RESERVATION");
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
                    message += $"Insert: {insertTotal}, Update: {updateTotal}. Total: {dataTable.Rows.Count}";

                    //stopwatch.Stop();
                    //TimeSpan timeSpan = stopwatch.Elapsed;
                    //string elapsedTime = String.Format("Sync Reservation:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

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


    }
}
