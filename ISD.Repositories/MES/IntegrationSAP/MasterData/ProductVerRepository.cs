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
    public class ProductVerRepository
    {
        EntityDataContext _context;
        //SAPRepository _sap;
        LoggerRepository _loggerRepository;
        public ProductVerRepository(EntityDataContext entityDataContext)
        {
            _context = entityDataContext;
            //_sap = new SAPRepository();
            _loggerRepository = new LoggerRepository();
        }
        private DataTable GetProductVersionData(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            DataTable dataTable = new DataTable();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_PRODUCTION_VER);

                //function.SetValue("I_WERKS", paramRequest.CompanyCode);
                //function.SetValue("I_MATNR", paramRequest.MATNR);
                //function.SetValue("I_VERID", paramRequest.ProVer);
                //function.SetValue("I_UPDATE", paramRequest.IUpdate);
                //function.SetValue("I_NEW", paramRequest.INew);
                //function.SetValue("I_RECORD", paramRequest.IRecord);

                //function.Invoke(destination);

                //dataTable = function.GetTable("IT_DATA").ToDataTable("IT_DATA");
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

        private OutputFromSAPViewModel ConfirmInsert(string CompanyCode, string ProductCode)
        {
            var result = new OutputFromSAPViewModel();
            //var destination = _sap.GetRfcWithConfig();
            //Định nghĩa hàm cần gọi
            //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_PRODUCTION_VER);

            //function.SetValue("I_WERKS", CompanyCode);
            //function.SetValue("I_MATNR", ProductCode);
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

        public string ResertProductVersion()
        {
            string result = string.Empty;
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_RESET_MASTERDATA);

                //function.SetValue("I_ACTION", "PRODUCTION_VER");

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

        public async Task<string> SyncProductVersion(ParamRequestSyncSapModel paramRequest)
        {
            var error = string.Empty;
            var message = string.Empty;

            var dataTable = GetProductVersionData(paramRequest, out error);
            if (!string.IsNullOrEmpty(error))
            {
                return error;
            }
            //3. Cập nhật vào MES (thêm mới || cập nhật)
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
                            var MATNR = dataRow["MATNR"].ToString();//Mã Material
                            var WERKS = dataRow["WERKS"].ToString();//Mã Cty
                            var VERID = dataRow["VERID"].ToString();//Production version

                            DateTime? BDATU = null;//Valid to
                            if (!string.IsNullOrEmpty(dataRow["BDATU"].ToString()) && dataRow["BDATU"].ToString() != "00000000")
                            {
                                string date = dataRow["BDATU"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                BDATU = DateTime.Parse(date);
                            }
                            DateTime? ADATU = null;//Valid from
                            if (!string.IsNullOrEmpty(dataRow["ADATU"].ToString()) && dataRow["ADATU"].ToString() != "00000000")
                            {
                                string date = dataRow["ADATU"].ToString();
                                date = date.Insert(4, "-");
                                date = date.Insert(7, "-");
                                ADATU = DateTime.Parse(date);
                            }
                            var STLAL = dataRow["STLAL"].ToString();//Alternative
                            var PLNNR = dataRow["PLNNR"].ToString();//Group

                            decimal? BSTMI = null;//From Lot size = ;//Size-Dimension
                            if (!string.IsNullOrEmpty(dataRow["BSTMI"].ToString()))
                            {
                                BSTMI = decimal.Parse(dataRow["BSTMI"].ToString());
                            }
                            decimal? BSTMA = null;//To Lot size
                            if (!string.IsNullOrEmpty(dataRow["BSTMA"].ToString()))
                            {
                                BSTMA = decimal.Parse(dataRow["BSTMA"].ToString());
                            }

                            #endregion
                            var productInDb = await _context.ProductVersionModel.Where(p => p.MATNR == MATNR && p.WERKS == WERKS).FirstOrDefaultAsync();
                            //Nếu chưa có product thì thêm mới: Có rồi thì update
                            if (productInDb == null)
                            {
                                #region InsertProduct
                                var productNew = new ProductVersionModel
                                {
                                    ProductVersionId = Guid.NewGuid(),
                                    MATNR = MATNR,
                                    WERKS = WERKS,
                                    VERID = VERID,
                                    ADATU = ADATU,
                                    BDATU = BDATU,
                                    BSTMA = BSTMA,
                                    BSTMI = BSTMI,
                                    PLNNR = PLNNR,
                                    STLAL = STLAL,
                                    CreateTime = DateTime.Now
                                };
                                _context.Entry(productNew).State = EntityState.Added;

                                #endregion InsertProduct
                                insertTotal++;
                            }
                            else
                            {
                                #region UpdateProductver
                                productInDb.MATNR = MATNR;
                                productInDb.WERKS = WERKS;
                                productInDb.VERID = VERID;
                                productInDb.ADATU = ADATU;
                                productInDb.BDATU = BDATU;
                                productInDb.BSTMA = BSTMA;
                                productInDb.BSTMI = BSTMI;
                                productInDb.PLNNR = PLNNR;
                                productInDb.STLAL = STLAL;
                                productInDb.LastEditTime = DateTime.Now;
                                _context.Entry(productInDb).State = EntityState.Modified;
                                #endregion UpdateProductver
                                updateTotal++;
                            }
                            await _context.SaveChangesAsync();
                            //4. Thông báo ngược về SAP để xóa dữ liệu trong temp table 
                            var result = ConfirmInsert(WERKS, MATNR);
                            if (result.IsSuccess == false)
                            {
                                _loggerRepository.Logging(string.Format("I_INSERT: E_ERROR (MATNR: {0}, WERKS: {1}, VERID: {2})"
                                                                        , MATNR
                                                                        , WERKS
                                                                        , VERID), "ZMES_PRODUCTION_VER");
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
                    //message += $"Insert: {insertTotal}, Update: {updateTotal}. Total: {dataTable.Rows.Count}";

                    //stopwatch.Stop();
                    //TimeSpan timeSpan = stopwatch.Elapsed;
                    //string elapsedTime = String.Format("SyncProductVer:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

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
