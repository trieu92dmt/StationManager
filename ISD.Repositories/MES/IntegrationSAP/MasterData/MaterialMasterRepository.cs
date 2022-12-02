using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using System.Diagnostics;
using System.Globalization;

namespace ISD.Repositories.MES.IntegrationSAP.MasterData
{
    public class MaterialMasterRepository
    {
        EntityDataContext _context;
        //SAPRepository _sap;
        LoggerRepository _loggerRepository;
        public MaterialMasterRepository(EntityDataContext db)
        {
            _context = db;
            _context.Database.CommandTimeout = 1800;
            //_sap = new SAPRepository();
            _loggerRepository = new LoggerRepository();
        }

        private List<DataTable> GetMaterialData(ParamRequestSyncSapModel materialParam, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_MATERIAL);

               

                //function.SetValue("I_WERKS", materialParam.CompanyCode);
                //function.SetValue("I_MATNR", materialParam.MATNR);
                ////function.SetValue("I_UPDATE", materialParam.IUpdate);
                ////function.SetValue("I_NEW", materialParam.INew);
                //function.SetValue("I_RECORD", materialParam.IRecord);
                //if(!string.IsNullOrEmpty(materialParam.FromDate))
                //{
                //    DateTime fromDate = DateTime.ParseExact(materialParam.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                //    function.SetValue("I_FROM_DATE", fromDate);
                //    function.SetValue("I_FROM_TIME", materialParam.FromTime);
                //    function.SetValue("I_TO_TIME", materialParam.ToTime);
                //}  

                //function.Invoke(destination);

                //var datatable1 = function.GetTable("IT_DATA_MARA").ToDataTable("IT_MARA");
                //dataTables.Add(datatable1);
                //var datatable2 = function.GetTable("IT_DATA_MARM").ToDataTable("IT_MARM");
                //dataTables.Add(datatable2);
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

        private OutputFromSAPViewModel ConfirmInsert(string CompanyCode, string ProductCode)
        {
            var result = new OutputFromSAPViewModel();
            //var destination = _sap.GetRfcWithConfig();
            ////Định nghĩa hàm cần gọi
            //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_MATERIAL);

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

        public string ResertMaterial()
        {
            string result = string.Empty;
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_RESET_MASTERDATA);

                //function.SetValue("I_ACTION", "MATERIAL");

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
        public async Task<string> GetMaterialMaster(ParamRequestSyncSapModel materialParam)
        {
            var error = string.Empty;
            var message = string.Empty;
            var dataTables = GetMaterialData(materialParam, out error);
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
                    var IT_MARA = dataTables[0];
                    var IT_MARM = dataTables[1];

                    //Insert dữ liệu từ bảng MARM
                    var taskMarm = await InsertUpdateMARM(IT_MARM);

                    if (!string.IsNullOrEmpty(taskMarm))
                    {
                        return taskMarm;
                    }

                    //Insert dữ liệu từ bảng MARA
                    var taskCrUp = await InsertUpdateMaterial(IT_MARA);

                    if (!string.IsNullOrEmpty(taskCrUp.Item2))
                    {
                        return taskCrUp.Item2;
                    }
                    message = taskCrUp.Item1;
                    //stopwatch.Stop();
                    //TimeSpan timeSpan = stopwatch.Elapsed;
                    //string elapsedTime = String.Format("GetMaterialMaster:  {0:00}:{1:00}:{2:00}.{3:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);

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

        private async Task<string> InsertUpdateMARM(DataTable dataTable)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    #region Collect Data
                    var MATNR = dataRow["MATNR"].ToString();//Mã Material
                    var MEINH = dataRow["MEINH"].ToString();//Unit
                    int UMREZ = 0;
                    if (string.IsNullOrEmpty(dataRow["UMREZ"].ToString()))
                    {
                        UMREZ = Int32.Parse(dataRow["UMREZ"].ToString());//Numerator for Conversion to Base Units of Measure
                    }
                    int? UMREN = 0;
                    if (string.IsNullOrEmpty(dataRow["UMREN"].ToString()))
                    {
                        UMREN = Int32.Parse(dataRow["UMREN"].ToString());//Denominator for conversion to base units of measure
                    }
                    decimal? LAENG = 0;
                    if (string.IsNullOrEmpty(dataRow["LAENG"].ToString()))
                    {
                        LAENG = Decimal.Parse(dataRow["LAENG"].ToString());//Length
                    }
                    decimal? BREIT = 0;
                    if (string.IsNullOrEmpty(dataRow["BREIT"].ToString()))
                    {
                        BREIT = decimal.Parse(dataRow["BREIT"].ToString());//Width
                    }
                    decimal? HOEHE = 0;
                    if (string.IsNullOrEmpty(dataRow["HOEHE"].ToString()))
                    {
                        HOEHE = decimal.Parse(dataRow["HOEHE"].ToString());//Height
                    }
                    decimal? VOLUM = 0;
                    if (!string.IsNullOrEmpty(dataRow["VOLUM"].ToString()))
                    {
                        VOLUM = Decimal.Parse(dataRow["VOLUM"].ToString());//Volume
                    }
                    var VOLEH = dataRow["VOLEH"].ToString();//Volume Unit
                    decimal? BRGEW = 0;
                    if (string.IsNullOrEmpty(dataRow["BRGEW"].ToString()))
                    {
                        BRGEW = Decimal.Parse(dataRow["BRGEW"].ToString());//Gross weight
                    }
                    decimal? NTGEW = 0;
                    if (string.IsNullOrEmpty(dataRow["NTGEW"].ToString()))
                    {
                        NTGEW = Decimal.Parse(dataRow["NTGEW"].ToString());//Net weight
                    }
                    var GEWEI = dataRow["GEWEI"].ToString();//Weight unit
                    var MEABM = dataRow["MEABM"].ToString();//Unit of Dimension for Length/Width/Height
                    #endregion

                    var marmInDb = await _context.MarmModel.Where(p => p.MATNR == MATNR && p.MEINH == MEINH).FirstOrDefaultAsync();
                    //Nếu chưa có thì thêm mới : Có rồi thì update
                    if (marmInDb == null)
                    {
                        #region Insert
                        var marnNew = new MarmModel
                        {
                            MarnId = Guid.NewGuid(),
                            MATNR = MATNR,//Mã Material
                            MEINH = MEINH,//Unit
                            UMREZ = UMREZ,//Numerator for Conversion to Base Units of Measure
                            UMREN = UMREN,//Denominator for conversion to base units of measure
                            LAENG = LAENG,//Length
                            BREIT = BREIT,//Width
                            HOEHE = HOEHE,//Height
                            VOLUM = VOLUM,//Volume
                            VOLEH = VOLEH,//Volume Unit
                            BRGEW = BRGEW,//Gross weight
                            NTGEW = NTGEW,//Net weight
                            GEWEI = GEWEI,//Weight unit
                            MEABM = MEABM, //Unit of Dimension for Length/Width/Height
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(marnNew).State = EntityState.Added;
                        #endregion
                    }
                    else
                    {
                        #region Update
                        //Update Product Attribute
                        marmInDb.UMREZ = UMREZ; //Numerator for Conversion to Base Units of Measure
                        marmInDb.UMREN = UMREN; //Denominator for conversion to base units of measure
                        marmInDb.LAENG = LAENG; //Length
                        marmInDb.BREIT = BREIT; //Width
                        marmInDb.HOEHE = HOEHE; //Height
                        marmInDb.VOLUM = VOLUM; //Volume
                        marmInDb.VOLEH = VOLEH; //Volume Unit
                        marmInDb.BRGEW = BRGEW; //Gross weight
                        marmInDb.NTGEW = NTGEW; //Net weight
                        marmInDb.GEWEI = GEWEI; //Weight unit
                        marmInDb.MEABM = MEABM; //Unit of Dimension for Length/Width/Height
                        marmInDb.LastEditTime = DateTime.Now;
                        _context.Entry(marmInDb).State = EntityState.Modified;
                        #endregion
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

        private async Task<Tuple<string, string>> InsertUpdateMaterial(DataTable dataTable)
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
                    var MATNR = dataRow["MATNR"].ToString();//Mã Material
                    var WERKS = dataRow["WERKS"].ToString();//Mã Cty
                    var MAKTX = dataRow["MAKTX"].ToString();//Tên Material
                    var MEINS = dataRow["MEINS"].ToString();//Based unit of measure
                    decimal? VOLUM = 0;
                    if (!string.IsNullOrEmpty(dataRow["VOLUM"].ToString()))
                    {
                        VOLUM = Decimal.Parse(dataRow["VOLUM"].ToString());//Volume
                    }
                    var VOLEH = dataRow["VOLEH"].ToString();//Volume Unit
                    var GROES = dataRow["GROES"].ToString();//Size-Dimension
                    decimal? BRGEW = 0;
                    if (string.IsNullOrEmpty(dataRow["BRGEW"].ToString()))
                    {
                        BRGEW = Decimal.Parse(dataRow["BRGEW"].ToString());//Gross weight
                    }
                    decimal? NTGEW = 0;
                    if (string.IsNullOrEmpty(dataRow["NTGEW"].ToString()))
                    {
                        NTGEW = Decimal.Parse(dataRow["NTGEW"].ToString());//Net weight
                    }
                    var GEWEI = dataRow["GEWEI"].ToString();//Weight unit
                    var Z_SLCARTON = dataRow["Z_SLCARTON"].ToString();//Số lượng SP/ Thùng
                    var MTART = dataRow["MTART"].ToString();//Phân loại sản phẩm
                    //Hình ảnh
                    var IMAGE_URL = dataRow["IMAGE_URL"].ToString();
                    if (!string.IsNullOrEmpty(IMAGE_URL) && IMAGE_URL.Contains("http://"))
                    {
                        IMAGE_URL = IMAGE_URL.Replace("http://", "https://");
                    }
                    #endregion

                    var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == WERKS).Select(p => p.CompanyId).FirstOrDefaultAsync();
                    var productInDb = await _context.ProductModel.Where(p => p.ERPProductCode == MATNR && p.CompanyId == companyId).FirstOrDefaultAsync();
                    //Nếu chưa có product thì thêm mới: Có rồi thì update
                    if (productInDb == null)
                    {
                        #region InsertProduct
                        var productNew = new ProductModel
                        {
                            ProductId = Guid.NewGuid(),
                            ERPProductCode = MATNR,
                            ProductCode = MATNR,
                            CompanyId = companyId,
                            ProductName = MAKTX,
                            MEINS = MEINS,
                            VOLUM = VOLUM,
                            VOLEH = VOLEH,
                            GROES = GROES,
                            BRGEW = BRGEW,
                            NTGEW = NTGEW,
                            GEWEI = GEWEI,
                            SLCARTON = Z_SLCARTON,
                            MTART = MTART,
                            CreateTime = DateTime.Now,
                            Actived = true
                        };
                        _context.Entry(productNew).State = EntityState.Added;

                        #endregion InsertProduct
                        insertTotal++;
                    }
                    else
                    {
                        #region UpdateProduct
                        productInDb.ProductName = MAKTX;
                        productInDb.MEINS = MEINS;
                        productInDb.VOLUM = VOLUM;
                        productInDb.VOLEH = VOLEH;
                        productInDb.GROES = GROES;
                        productInDb.BRGEW = BRGEW;
                        productInDb.NTGEW = NTGEW;
                        productInDb.GEWEI = GEWEI;
                        productInDb.SLCARTON = Z_SLCARTON;
                        productInDb.MTART = MTART;
                        productInDb.LastEditTime = DateTime.Now;
                        _context.Entry(productInDb).State = EntityState.Modified;
                        #endregion UpdateProduct
                        updateTotal++;
                    }

                    //Lưu hình ảnh
                    if (!string.IsNullOrEmpty(IMAGE_URL))
                    {
                        var image = await _context.ProductImageModel.Where(p => p.ERPProductCode == MATNR).FirstOrDefaultAsync();
                        //Nếu chưa có thì thêm mới. Có rồi thì update
                        if (image == null)
                        {
                            var imageNew = new ProductImageModel
                            {
                                ERPProductCode = MATNR,
                                ImageUrl = IMAGE_URL,
                            };
                            _context.Entry(imageNew).State = EntityState.Added;
                        }
                        else
                        {
                            image.ImageUrl = IMAGE_URL;
                            _context.Entry(image).State = EntityState.Modified;
                        }
                    }

                    await _context.SaveChangesAsync();
                    //Xác nhận insert vào DB thành công cho SAP
                    //ConfirmInsert(WERKS, MATNR);
                    var result = ConfirmInsert(WERKS, MATNR);
                    if (result.IsSuccess == false)
                    {
                        _loggerRepository.Logging(string.Format("I_INSERT: E_ERROR (WERKS: {0}, MATNR: {1})"
                                                               , WERKS
                                                               , MATNR), "ZMES_MATERIAL");
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

        private List<DataTable> GetMaterialTypeGroupData(ParamRequestSyncSapModel materialParam, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                //Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_FM_MATLTYPE_GROUP);

                //function.SetValue("I_RECORD", materialParam.IRecord);

                //function.Invoke(destination);

                //var datatable1 = function.GetTable("IT_DATA_MATLTYPE").ToDataTable("MATLTYPE");
                //dataTables.Add(datatable1);
                //var datatable2 = function.GetTable("IT_DATA_MATLGROUP").ToDataTable("MATLGROUP");
                //dataTables.Add(datatable2);
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

        public async Task<string> GetMaterialTypeGroup(ParamRequestSyncSapModel materialParam)
        {
            var error = string.Empty;
            var message = string.Empty;
            var dataTables = GetMaterialTypeGroupData(materialParam, out error);
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
                    var IT_MATLTYPE = dataTables[0];
                    var IT_MATLGROUP = dataTables[1];

                    //Insert dữ liệu từ bảng Material Type
                    var taskType = await InsertUpdateMATLTYPE(IT_MATLTYPE);

                    if (!string.IsNullOrEmpty(taskType))
                    {
                        return taskType;
                    }

                    //Insert dữ liệu từ bảng Material Group
                    var taskGroup = await InsertUpdateMATLGROUP(IT_MATLGROUP);

                    if (!string.IsNullOrEmpty(taskGroup))
                    {
                        return taskGroup;
                    }
                    message = taskGroup;
                    //stopwatch.Stop();
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

        private async Task<string> InsertUpdateMATLTYPE(DataTable dataTable)
        {
            string error = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    #region Collect Data
                    var MANDT = dataRow["MANDT"].ToString();
                    var MTART = dataRow["MTART"].ToString();
                    var MTBEZ = dataRow["MTBEZ"].ToString();
                    #endregion

                    var matInDb = await _context.MaterialTypeModel.Where(p => p.MTART == MTART).FirstOrDefaultAsync();
                    //Nếu chưa có thì thêm mới : Có rồi thì update
                    if (matInDb == null)
                    {
                        #region Insert
                        var matNew = new MaterialTypeModel
                        {
                            MaterialTypeId = Guid.NewGuid(),
                            MANDT = MANDT,
                            MTART = MTART,
                            MTBEZ = MTBEZ,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(matNew).State = EntityState.Added;
                        #endregion
                    }
                    else
                    {
                        #region Update
                        matInDb.MTBEZ = MTBEZ;
                        matInDb.LastEditTime = DateTime.Now;
                        _context.Entry(matInDb).State = EntityState.Modified;
                        #endregion
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
            return error;
        }

        private async Task<string> InsertUpdateMATLGROUP(DataTable dataTable)
        {
            string error = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    #region Collect Data
                    var MANDT = dataRow["MANDT"].ToString();
                    var MATKL = dataRow["MATKL"].ToString();
                    var WGBEZ = dataRow["WGBEZ"].ToString();
                    var WGBEZ60 = dataRow["WGBEZ60"].ToString();
                    #endregion

                    if (!string.IsNullOrEmpty(MATKL))
                    {
                        var matInDb = await _context.MaterialGroupModel.Where(p => p.MATKL == MATKL).FirstOrDefaultAsync();
                        //Nếu chưa có thì thêm mới : Có rồi thì update
                        if (matInDb == null)
                        {
                            #region Insert
                            var matNew = new MaterialGroupModel
                            {
                                MaterialGroupId = Guid.NewGuid(),
                                MANDT = MANDT,
                                MATKL = MATKL,
                                WGBEZ = WGBEZ,
                                WGBEZ60 = WGBEZ60,
                                CreateTime = DateTime.Now
                            };
                            _context.Entry(matNew).State = EntityState.Added;
                            #endregion
                        }
                        else
                        {
                            #region Update
                            matInDb.WGBEZ = WGBEZ;
                            matInDb.WGBEZ60 = WGBEZ60;
                            matInDb.LastEditTime = DateTime.Now;
                            _context.Entry(matInDb).State = EntityState.Modified;
                            #endregion
                        }
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
            return error;
        }
    }
}
