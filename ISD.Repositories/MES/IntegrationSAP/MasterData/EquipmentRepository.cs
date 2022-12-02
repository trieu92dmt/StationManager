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
    public class EquipmentRepository
    {
        EntityDataContext _context;
        //SAPRepository _sap;
        LoggerRepository _loggerRepository;
        public EquipmentRepository(EntityDataContext entityDataContext)
        {
            _context = entityDataContext;
            //_sap = new SAPRepository();
            _loggerRepository = new LoggerRepository();
        }

        private List<DataTable> GetEquipmentData(ParamRequestSyncSapModel paramRequest, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                ////var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_EQUIPMENT);

                ////function.SetValue("I_WERKS", paramRequest.CompanyCode);
                //function.SetValue("I_UPDATE", paramRequest.IUpdate);
                //function.SetValue("I_NEW", paramRequest.INew);
                //function.SetValue("I_RECORD", paramRequest.IRecord);

                //function.Invoke(destination);

                //var table1 = function.GetTable("IT_EQ_GROUP").ToDataTable("EQ_GROUP");
                //dataTables.Add(table1);
                //var table2 = function.GetTable("IT_EQ").ToDataTable("IT_EQ");
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

        private void ConfirmInsert(string EQUNR, string BUKRS)
        {
        //    var destination = _sap.GetRfcWithConfig();
        //    //Định nghĩa hàm cần gọi
        //    var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_EQUIPMENT);

        //    function.SetValue("I_EQUNR", EQUNR);
        //    function.SetValue("I_BUKRS", BUKRS);
        //    function.SetValue("I_INSERT", 'X');//Tham số xác nhận insert thành công

        //    function.Invoke(destination);

        }

        public async Task<string> SyncEquipment(ParamRequestSyncSapModel paramRequest)
        {
            var error = string.Empty;
            var message = string.Empty;
            var dataTables = GetEquipmentData(paramRequest, out error);
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


                    var taskEquipment = await InsertUpdateEquipment(IT_ITEM);

                    if (!string.IsNullOrEmpty(taskEquipment))
                    {
                        return taskEquipment;
                    }

                    var taskEQGroup = await InsertUpdateGroup(IT_HEADER);

                    if (!string.IsNullOrEmpty(taskEQGroup.Item2))
                    {
                        return taskEQGroup.Item2;
                    }
                    message = taskEQGroup.Item1;
                    //stopwatch.Stop();
                    //TimeSpan timeSpan = stopwatch.Elapsed;
                    //string elapsedTime = String.Format("Sync Equipment:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

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
        private async Task<string> InsertUpdateEquipment(DataTable dataTable)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {

                    #region Collect Data
                    var EQART = dataRow["EQART"].ToString();
                    var EARTX = dataRow["EARTX"].ToString();

                    #endregion

                    //var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefaultAsync();
                    // var bomDetailDb = await _context.EquipmentModel.Where(p => p.EQART == MANDT).FirstOrDefaultAsync();
                    //Nếu chưa có product thì thêm mới: Có rồi thì update
                    //if (bomDetailDb == null)
                    //{
                    #region InsertItem
                    var equipmentNew = new EquipmentModel
                    {
                        EquipmentId = Guid.NewGuid(),
                        EARTX = EARTX,
                        EQART = EQART,
                        CreateTime = DateTime.Now
                    };
                    _context.Entry(equipmentNew).State = EntityState.Added;

                    #endregion InsertItem
                    /*}
                    else
                    {
                        #region UpdateItem
                        bomDetailDb.LastEditTime = DateTime.Now;
                        _context.Entry(bomDetailDb).State = EntityState.Modified;
                        #endregion UpdateItem
                    }*/
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

        private async Task<Tuple<string, string>> InsertUpdateGroup(DataTable dataTable)
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
                    var EQUNR = dataRow["EQUNR"].ToString();
                    var EQART = dataRow["EQART"].ToString();
                    var SHTXT = dataRow["SHTXT"].ToString();
                    var SWERK = dataRow["SWERK"].ToString();
                    var STORT = dataRow["STORT"].ToString();
                    var ARBPL = dataRow["ARBPL"].ToString();
                    var BUKRS = dataRow["BUKRS"].ToString();
                    var ZSTATUS = dataRow["ZSTATUS"].ToString();
                    DateTime? DATBI = null;
                    if (!string.IsNullOrEmpty(dataRow["DATBI"].ToString()))
                    {
                        string date = dataRow["DATBI"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        DATBI = DateTime.Parse(date);
                    }
                    #endregion

                    //var bomHeaderDb = await _context.EquipmentGroupModel.Where(p => p.EQUNR == MATNR).FirstOrDefaultAsync();
                    //Nếu chưa có thì thêm mới : Có rồi thì update
                    //if (bomHeaderDb == null)
                    //{
                    #region Insert
                    var equipmentGroupNew = new EquipmentGroupModel
                    {
                        EquipmentGroupId = Guid.NewGuid(),
                        ARBPL = ARBPL,
                        DATBI = DATBI,
                        EQART = EQART,
                        EQUNR = EQUNR,
                        SHTXT = SHTXT,
                        STATU = ZSTATUS,
                        STORT = STORT,
                        SWERK = SWERK,
                        CreateTime = DateTime.Now
                    };
                    _context.Entry(equipmentGroupNew).State = EntityState.Added;
                    #endregion
                    insertTotal++;
                    /* }
                     else
                     {
                         #region Update
                         _context.Entry(bomHeaderDb).State = EntityState.Modified;
                         #endregion
                         updateTotal++;
                     }*/
                    await _context.SaveChangesAsync();
                    //Xác nhận insert vào DB thành công cho SAP
                    ConfirmInsert(EQUNR, BUKRS);
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
        public string ResetEquipment()
        {
            string result = string.Empty;
            try
            {
            //    var destination = _sap.GetRfcWithConfig();
            //    //Định nghĩa hàm cần gọi
            //    var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_RESET_MASTERDATA);

            //    function.SetValue("I_ACTION", "EQUIPMENT");

            //    function.Invoke(destination);

            //    result = function.GetString("E_RECORD");
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
