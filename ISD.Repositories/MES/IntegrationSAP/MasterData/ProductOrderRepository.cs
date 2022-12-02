using ISD.Constant;
using ISD.EntityModels;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.Repositories.MES.IntegrationSAP.MasterData
{
    public class ProductOrderRepository
    {
        EntityDataContext _context;
        //SAPRepository _sap;
        LoggerRepository _loggerRepository;
        public ProductOrderRepository(EntityDataContext entityDataContext)
        {
            _context = entityDataContext;
            _context.Database.CommandTimeout = 1800;
            //_sap = new SAPRepository();
            _loggerRepository = new LoggerRepository();
        }

        public List<DataTable> GetProductionOrderData(ParamRequestSyncSapModel materialParam, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<DataTable> dataTables = new List<DataTable>();
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                //Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_PRODUCTION_ORDER);

                //function.SetValue("I_WERKS", materialParam.CompanyCode);
                //function.SetValue("I_KDAUF", materialParam.VBELN);
                //function.SetValue("I_AUFNR", materialParam.AUFNR);
                ////function.SetValue("I_UPDATE", materialParam.IUpdate);
                ////function.SetValue("I_NEW", materialParam.INew);
                //function.SetValue("I_RECORD", materialParam.IRecord);
                //function.SetValue("I_ZZLSX", materialParam.IZZLSX);

                //function.Invoke(destination);

                //var datatable1 = function.GetTable("IT_PROD_ORDER").ToDataTable("PROD_ORDER");
                //dataTables.Add(datatable1);
                //var datatable2 = function.GetTable("IT_PROD_COMPO").ToDataTable("PROD_COMPO");
                //dataTables.Add(datatable2);
                //var datatable3 = function.GetTable("IT_PROD_OPER").ToDataTable("PROD_OPER");
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

        private OutputFromSAPViewModel ConfirmInsert(string AUFNR, string KDAUF, string WERKS)
        {
            var result = new OutputFromSAPViewModel();
            //var destination = _sap.GetRfcWithConfig();
            //Định nghĩa hàm cần gọi
            //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_PRODUCTION_ORDER);
            ////Mã lệnh sản xuất SAP
            //function.SetValue("I_AUFNR", AUFNR);
            //function.SetValue("I_WERKS", WERKS);
            ////Sale Order Number
            ////function.SetValue("I_KDAUF", KDAUF);
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

        public string ResertProductionOrder()
        {
            string result = string.Empty;
            try
            {
                //var destination = _sap.GetRfcWithConfig();
                ////Định nghĩa hàm cần gọi
                //var function = destination.Repository.CreateFunction(ConstantFunctionName.ZMES_RESET_MASTERDATA);

                //function.SetValue("I_ACTION", "PRODUCTION_ORDER");

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
        public async Task<string> SyncProductionOrder(ParamRequestSyncSapModel materialParam)
        {
            var error = string.Empty;
            var message = string.Empty;
            var dataTables = GetProductionOrderData(materialParam, out error);
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
                    var PROD_ORDER = dataTables[0];
                    var PROD_COMPONENT = dataTables[1];
                    var PROD_OPERATION = dataTables[2];

                    var taskOperation = await InsertUpdateOperation(PROD_OPERATION);

                    if (!string.IsNullOrEmpty(taskOperation))
                    {
                        return taskOperation;
                    }
                    var taskComponent = await InsertUpdateComponent(PROD_COMPONENT);

                    if (!string.IsNullOrEmpty(taskComponent))
                    {
                        return taskComponent;
                    }

                    var taskProOrder = await InsertUpdateOrder(PROD_ORDER, materialParam.CompanyCode);

                    if (!string.IsNullOrEmpty(taskProOrder.Item2))
                    {
                        return taskProOrder.Item2;
                    }
                    message = taskProOrder.Item1;

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
                return error;
            }
            return message;
        }

        private async Task<Tuple<string, string>> InsertUpdateOrder(DataTable dataTable, string WERKS)
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
                    var AUFNR = dataRow["AUFNR"].ToString();
                    //var paramRequest = new ParamRequestSyncSapModel
                    //{
                    //    AUFNR = AUFNR.TrimStart(new Char[] { '0' }),
                    //};
                    //await SyncProductionOrder(paramRequest);
                    DateTime? GLTRP = null;
                    if (!string.IsNullOrEmpty(dataRow["GLTRP"].ToString()) && dataRow["GLTRP"].ToString() != "00000000")
                    {
                        string date = dataRow["GLTRP"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        GLTRP = DateTime.Parse(date);
                    }
                    DateTime? GSTRP = null;
                    if (!string.IsNullOrEmpty(dataRow["GSTRP"].ToString()) && dataRow["GSTRP"].ToString() != "00000000")
                    {
                        string date = dataRow["GSTRP"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        GSTRP = DateTime.Parse(date);
                    }
                    DateTime? FTRMS = null;
                    if (!string.IsNullOrEmpty(dataRow["FTRMS"].ToString()) && dataRow["FTRMS"].ToString() != "00000000")
                    {
                        string date = dataRow["FTRMS"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        FTRMS = DateTime.Parse(date);
                    }
                    DateTime? GLTRS = null;
                    if (!string.IsNullOrEmpty(dataRow["GLTRS"].ToString()) && dataRow["GLTRS"].ToString() != "00000000")
                    {
                        string date = dataRow["GLTRS"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        GLTRS = DateTime.Parse(date);
                    }
                    DateTime? GSTRS = null;
                    if (!string.IsNullOrEmpty(dataRow["GSTRS"].ToString()) && dataRow["GSTRS"].ToString() != "00000000")
                    {
                        string date = dataRow["GSTRS"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        GSTRS = DateTime.Parse(date);
                    }
                    int? RSNUM = null;
                    if (!string.IsNullOrEmpty(dataRow["RSNUM"].ToString()))
                    {
                        RSNUM = Int32.Parse(dataRow["RSNUM"].ToString());
                    }
                    decimal? GASMG = null;
                    if (!string.IsNullOrEmpty(dataRow["GASMG"].ToString()))
                    {
                        GASMG = decimal.Parse(dataRow["GASMG"].ToString());
                    }
                    decimal? GAMNG = null;
                    if (!string.IsNullOrEmpty(dataRow["GAMNG"].ToString()))
                    {
                        GAMNG = decimal.Parse(dataRow["GAMNG"].ToString());
                    }
                    var KDAUF = dataRow["KDAUF"].ToString();
                    var KDPOS = dataRow["KDPOS"].ToString();
                    decimal? PSMNG = null;
                    if (!string.IsNullOrEmpty(dataRow["PSMNG"].ToString()))
                    {
                        PSMNG = decimal.Parse(dataRow["PSMNG"].ToString());
                    }
                    decimal? WEMNG = null;
                    if (!string.IsNullOrEmpty(dataRow["WEMNG"].ToString()))
                    {
                        WEMNG = decimal.Parse(dataRow["WEMNG"].ToString());
                    }
                    var AMEIN = dataRow["AMEIN"].ToString();
                    var MEINS = dataRow["MEINS"].ToString();
                    string MATNR = string.Empty;
                    if (!string.IsNullOrEmpty(dataRow["MATNR"].ToString()))
                    {
                        MATNR = Int32.Parse(dataRow["MATNR"].ToString()).ToString();
                    }
                    decimal? PAMNG = null;
                    if (!string.IsNullOrEmpty(dataRow["PAMNG"].ToString()))
                    {
                        PAMNG = decimal.Parse(dataRow["PAMNG"].ToString());
                    }
                    decimal? PGMNG = null;
                    if (!string.IsNullOrEmpty(dataRow["PGMNG"].ToString()))
                    {
                        PGMNG = decimal.Parse(dataRow["PGMNG"].ToString());
                    }
                    DateTime? LTRMI = null;
                    if (!string.IsNullOrEmpty(dataRow["LTRMI"].ToString()) && dataRow["LTRMI"].ToString() != "00000000")
                    {
                        string date = dataRow["LTRMI"].ToString();
                        date = date.Insert(4, "-");
                        date = date.Insert(7, "-");
                        LTRMI = DateTime.Parse(date);
                    }
                    decimal? UEBTO = null;
                    if (!string.IsNullOrEmpty(dataRow["UEBTO"].ToString()))
                    {
                        UEBTO = decimal.Parse(dataRow["UEBTO"].ToString());
                    }
                    var UEBTK = dataRow["UEBTK"].ToString();
                    decimal? UNTTO = null;
                    if (!string.IsNullOrEmpty(dataRow["UNTTO"].ToString()))
                    {
                        UNTTO = decimal.Parse(dataRow["UNTTO"].ToString());
                    }
                    var INSMK = dataRow["INSMK"].ToString();
                    var DWERK = dataRow["DWERK"].ToString();
                    var DAUAT = dataRow["DAUAT"].ToString();
                    var ZZSLANSUA = dataRow["ZZSLANSUA"].ToString();
                    var ZZGHICHU = dataRow["ZZGHICHU"].ToString();
                    var ZZLSX = dataRow["ZZLSX"].ToString();
                    var VERID = dataRow["VERID"].ToString();
                    var PROJN = dataRow["PROJN"].ToString();
                    var ZCLOSE = dataRow["ZCLOSE"].ToString();
                    var LOEKZ = dataRow["LOEKZ"].ToString();
                    var ZSTATUS = dataRow["ZSTATUS"].ToString();
                    #endregion

                    var ProOrderInDb = await _context.ProductionOrderModel.Where(p => p.AUFNR == AUFNR && p.DWERK == DWERK).FirstOrDefaultAsync();
                    var companyId = await _context.CompanyModel.Where(p => p.CompanyCode == DWERK).Select(p => p.CompanyId).FirstOrDefaultAsync();
                    //Sản phẩm: khi đồng bộ từ SAP về nếu không có (MES) phải kéo bằng được
                    var productInDb = await _context.ProductModel.Where(p => p.ERPProductCode == MATNR && p.CompanyId == companyId).FirstOrDefaultAsync();
                    if (productInDb == null && !string.IsNullOrEmpty(MATNR))
                    {
                        productInDb = await syncMaterial(companyId, DWERK, MATNR);
                    }
                    //Lệnh sản xuất con (SAP)
                    var LSXC = await _context.WorkFlowModel.Where(p => p.WorkFlowCode == ConstWorkFlow.LSXC).FirstOrDefaultAsync();
                    Guid taskStatusId = Guid.Empty;
                    if (LSXC != null)
                    {
                        taskStatusId = await _context.TaskStatusModel.Where(p => p.WorkFlowId == LSXC.WorkFlowId).OrderBy(p => p.OrderIndex).Select(p => p.TaskStatusId).FirstOrDefaultAsync();
                    }
                    //Đợt
                    var LSXD = await _context.WorkFlowModel.Where(p => p.WorkFlowCode == ConstWorkFlow.LSXD).FirstOrDefaultAsync();
                    Guid taskStatusId2 = Guid.Empty;
                    if (LSXD != null)
                    {
                        taskStatusId2 = await _context.TaskStatusModel.Where(p => p.WorkFlowId == LSXD.WorkFlowId).OrderBy(p => p.OrderIndex).Select(p => p.TaskStatusId).FirstOrDefaultAsync();
                    }
                    //SYSTEM
                    var SYSTEM = await _context.AccountModel.Where(p => p.UserName == "SYSTEM").Select(p => p.AccountId).FirstOrDefaultAsync();

                    string NguoiTheoDoi = ZZLSX;
                    string LSXDT = ZZLSX;
                    if (!string.IsNullOrEmpty(ZZLSX))
                    {
                        int index = ZZLSX.IndexOf("-");
                        if (index > 0)
                        {
                            NguoiTheoDoi = ZZLSX.Substring(0, index);
                            LSXDT = ZZLSX.Substring(index + 1);
                        }
                    }
                    //80: Nếu chưa có thì thêm mới, có rồi thì không làm gì hết
                    var ProOrderInDb80 = await _context.ProductionOrder80Model.Where(p => p.AUFNR == AUFNR && p.DWERK == DWERK).FirstOrDefaultAsync();
                    if (ProOrderInDb80 == null)
                    {
                        #region Insert
                        var ProOrderNew = new ProductionOrder80Model
                        {
                            ProductionOrderId = Guid.NewGuid(),
                            AMEIN = AMEIN,
                            AUFNR = AUFNR,
                            AUFNR_MES = AUFNR.TrimStart(new Char[] { '0' }),
                            DAUAT = DAUAT,
                            DWERK = DWERK,
                            FTRMS = FTRMS,
                            GAMNG = GAMNG,
                            GASMG = GASMG,
                            GLTRP = GLTRP,
                            GLTRS = GLTRS,
                            GSTRP = GSTRP,
                            GSTRS = GSTRS,
                            INSMK = INSMK,
                            KDAUF = KDAUF,
                            KDPOS = KDPOS,
                            KDPOS_MES = KDPOS.TrimStart(new Char[] { '0' }),
                            LTRMI = LTRMI,
                            MATNR = MATNR,
                            MEINS = MEINS,
                            PAMNG = PAMNG,
                            PGMNG = PGMNG,
                            PSMNG = PSMNG,
                            RSNUM = RSNUM,
                            UEBTK = UEBTK,
                            UEBTO = UEBTO,
                            UNTTO = UNTTO,
                            WEMNG = WEMNG,
                            ZZGHICHU = ZZGHICHU,
                            ZZLSX = ZZLSX,
                            ZZSLANSUA = ZZSLANSUA,
                            VERID = VERID,
                            PROJN = PROJN,
                            ZCLOSE = ZCLOSE,
                            LOEKZ = LOEKZ,
                            ZSTATUS = ZSTATUS,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(ProOrderNew).State = EntityState.Added;
                        #endregion
                    }
                    //100
                    //Nếu chưa có thì thêm mới : Có rồi thì update
                    //Nếu trường LSX ĐT không có dữ liệu thì check DB
                    //1. Nếu có => update
                    //2. Không có => không làm gì hết
                    if (string.IsNullOrEmpty(ZZLSX))
                    {
                        if (ProOrderInDb != null)
                        {
                            #region Update
                            ProOrderInDb.AUFNR_MES = AUFNR.TrimStart(new Char[] { '0' });
                            ProOrderInDb.AMEIN = AMEIN;
                            ProOrderInDb.DAUAT = DAUAT;
                            ProOrderInDb.FTRMS = FTRMS;
                            ProOrderInDb.GAMNG = GAMNG;
                            ProOrderInDb.GASMG = GASMG;
                            ProOrderInDb.GLTRP = GLTRP;
                            ProOrderInDb.GLTRS = GLTRS;
                            ProOrderInDb.GSTRP = GSTRP;
                            ProOrderInDb.GSTRS = GSTRS;
                            ProOrderInDb.INSMK = INSMK;
                            ProOrderInDb.KDAUF = KDAUF;
                            ProOrderInDb.KDPOS = KDPOS;
                            ProOrderInDb.KDPOS_MES = KDPOS.TrimStart(new Char[] { '0' });
                            ProOrderInDb.LTRMI = LTRMI;
                            ProOrderInDb.MATNR = MATNR;
                            ProOrderInDb.MEINS = MEINS;
                            ProOrderInDb.PAMNG = PAMNG;
                            ProOrderInDb.PGMNG = PGMNG;
                            ProOrderInDb.PSMNG = PSMNG;
                            ProOrderInDb.RSNUM = RSNUM;
                            ProOrderInDb.UEBTK = UEBTK;
                            ProOrderInDb.UEBTO = UEBTO;
                            ProOrderInDb.UNTTO = UNTTO;
                            ProOrderInDb.WEMNG = WEMNG;
                            ProOrderInDb.ZZGHICHU = ZZGHICHU;
                            ProOrderInDb.ZZLSX = ZZLSX;
                            ProOrderInDb.ZZSLANSUA = ZZSLANSUA;
                            ProOrderInDb.VERID = VERID;
                            ProOrderInDb.PROJN = PROJN;
                            ProOrderInDb.ZCLOSE = ZCLOSE;
                            ProOrderInDb.LOEKZ = LOEKZ;
                            ProOrderInDb.ZSTATUS = ZSTATUS;
                            ProOrderInDb.LastEditTime = DateTime.Now;
                            _context.Entry(ProOrderInDb).State = EntityState.Modified;
                            //Update task nếu có tồn tại
                            var ProductionOrderTaskInDB = await _context.TaskModel.Where(p => p.TaskId == ProOrderInDb.ProductionOrderId).FirstOrDefaultAsync();
                            if (ProductionOrderTaskInDB != null)
                            {
                                //Ngày bắt đầu
                                ProductionOrderTaskInDB.StartDate = GSTRS;
                                //Ngày dự kiến hoàn thành
                                ProductionOrderTaskInDB.EstimateEndDate = GLTRS;
                                //SO Number
                                ProductionOrderTaskInDB.Property1 = KDAUF.TrimStart(new Char[] { '0' });
                                //SO Line Number
                                ProductionOrderTaskInDB.Property2 = KDPOS.TrimStart(new Char[] { '0' });
                                //Số lượng
                                //ProductionOrderTaskInDB.Qty = Convert.ToInt32(PSMNG);
                                //LSX ĐT:
                                //Fulltext: TRANG-LSXDT	=> NTD:TRANG => Mã lệnh: LSXDT
                                //1.FullText
                                ProductionOrderTaskInDB.Property3 = ZZLSX;
                                //2.NTD
                                ProductionOrderTaskInDB.Property4 = NguoiTheoDoi;
                                //3. LSXDT
                                ProductionOrderTaskInDB.Property5 = LSXDT;
                                ProductionOrderTaskInDB.ProductId = productInDb != null ? productInDb.ProductId : Guid.Empty;
                                ProductionOrderTaskInDB.LastEditTime = DateTime.Now;
                                ProductionOrderTaskInDB.LastEditBy = SYSTEM;
                                ProductionOrderTaskInDB.isKhongDongBo = false;
                                //Nếu 1 trong 2 cột này đánh dấu x là coi như lệnh sx đó đóng
                                ProductionOrderTaskInDB.isDeleted = (ZCLOSE == "X" || LOEKZ == "X") ? true : false;

                                //Update số lượng
                                //Kiểm tra SL cũ vs SL mới: nếu khác update đổi đè dữ liệu DATA SAP 100 => 105 
                                var oldQty = ProductionOrderTaskInDB.Qty;
                                if (ProductionOrderTaskInDB.Qty != Convert.ToInt32(PSMNG))
                                {
                                    ProductionOrderTaskInDB.Qty = Convert.ToInt32(PSMNG);

                                    //Update LSX SAP đã tách theo đợt
                                    //Nếu có tách (> 1 task cùng số PP Order) & Check số lượng mới nếu != SUM của các đợt tách
                                    //Phát hiện không đồng bộ giữa LSX SAP vs LSX SAP đã tách: thì bật cờ isKhongDongBo = true, ngược lại set isKhongDongBo = false
                                    var LSXSAP = AUFNR.TrimStart(new Char[] { '0' });
                                    var SLKH = Convert.ToInt32(PSMNG);
                                    var LSXSAPList = _context.TaskModel.Where(p => p.Summary == LSXSAP).ToList();
                                    bool isKhongDongBo = false;
                                    if (LSXSAPList != null && LSXSAPList.Count > 1)
                                    {
                                        var LSXSAPDB = _context.TaskModel.Where(f => f.Summary == LSXSAP).ToList();
                                        LSXSAPDB.ForEach(a => a.Qty = SLKH);

                                        //SL ĐC
                                        var SLDC = _context.TaskModel.Where(p => p.Summary == LSXSAP).Sum(p => p.Number2);
                                        if (SLKH != SLDC)
                                        {
                                            isKhongDongBo = true;
                                        }
                                    }

                                    if (isKhongDongBo == true)
                                    {
                                        var LSXSAPDB = _context.TaskModel.Where(f => f.Summary == LSXSAP && f.WorkFlowId == LSXC.WorkFlowId).ToList();
                                        LSXSAPDB.ForEach(a => a.isKhongDongBo = true);
                                        _context.SaveChanges();
                                    }
                                }
                                //Số lượng điều chỉnh = với số lượng trên SAP(cũ) => update đè SLKH mới 
                                //Nếu khác nhau thì không update => do số lượng điều chỉnh đã được cập nhật ở thao tác chia đợt
                                if (oldQty == ProductionOrderTaskInDB.Number2)
                                {
                                    ProductionOrderTaskInDB.Number2 = ProductionOrderTaskInDB.Qty;
                                }

                                _context.Entry(ProductionOrderTaskInDB).State = EntityState.Modified;

                                //Update đợt sản xuất
                                var dotSX = _context.TaskModel.Where(p => p.TaskId == ProductionOrderTaskInDB.ParentTaskId).FirstOrDefault();
                                if (dotSX != null)
                                {
                                    //LSX ĐT:
                                    //Fulltext: TRANG-LSXDT	=> NTD:TRANG => Mã lệnh: LSXDT
                                    //1.FullText
                                    dotSX.Property3 = ZZLSX;
                                    //2.NTD
                                    dotSX.Property4 = NguoiTheoDoi;
                                    //3. LSXDT
                                    dotSX.Property5 = LSXDT;
                                }
                                _context.Entry(dotSX).State = EntityState.Modified;
                            }
                            //Nếu chưa có không làm gì hết
                            /*
                            else
                            {
                                
                                //Tìm trong bảng TaskModel đã tồn tại LSX ĐT chưa (field Property3)
                                //1. Chưa có thì tạo đợt + tạo LSX SAP
                                //2. Có rồi thì không tạo đợt mới, chỉ tạo LSX SAP và lấy đợt đã tồn tại
                                //=====================================================================
                                Guid DSXId = Guid.Empty;
                                var existsDSX = _context.TaskModel.Where(p => p.WorkFlowId == LSXD.WorkFlowId && p.Property3 == ZZLSX).FirstOrDefault();
                                if (existsDSX == null)
                                {
                                    //Tạo đợt cho lệnh sản xuất đại trà: mặc định là đợt 1
                                    var newProductionOrder = new TaskModel
                                    {
                                        TaskId = Guid.NewGuid(),
                                        //500001821 - Đợt 1
                                        //Summary = string.Format("{0} - {1}", AUFNR.TrimStart(new Char[] { '0' }), "Đợt 1"),
                                        //Summary = "Đợt 1",
                                        Summary = LSXDT + "-D1",
                                        //Số thứ tự đợt
                                        Number1 = 1,
                                        //LSX ĐT:
                                        //Fulltext: TRANG-LSXDT	=> NTD:TRANG => Mã lệnh: LSXDT
                                        //1.FullText
                                        Property3 = ZZLSX,
                                        //2.NTD
                                        Property4 = NguoiTheoDoi,
                                        //3. LSXDT
                                        Property5 = LSXDT,
                                        //Loại: lệnh sản xuất đại trà
                                        WorkFlowId = LSXD.WorkFlowId,
                                        //Trạng thái: mặc định lấy trạng thái đầu tiên
                                        TaskStatusId = taskStatusId2,
                                        PriorityCode = ConstPriotityCode.NORMAL,
                                        CompanyId = companyId,
                                        CreateTime = DateTime.Now,
                                        CreateBy = SYSTEM,
                                        Actived = true,
                                    };
                                    _context.Entry(newProductionOrder).State = EntityState.Added;

                                    DSXId = newProductionOrder.TaskId;
                                }
                                else
                                {
                                    DSXId = existsDSX.TaskId;
                                }

                                //Tạo lệnh sản xuất con (SAP)
                                var newProductionOrderSubtask = new TaskModel
                                {
                                    TaskId = ProOrderInDb.ProductionOrderId,
                                    ParentTaskId = DSXId,
                                    //Lệnh sản xuất SAP: 500001821
                                    Summary = AUFNR.TrimStart(new Char[] { '0' }),
                                    //Ngày bắt đầu: 16/08/2019
                                    StartDate = GSTRS,
                                    //Ngày bắt đầu điều chỉnh
                                    Date1 = GSTRS,
                                    //Ngày dự kiến hoàn thành: 16/08/2019
                                    EstimateEndDate = GLTRS,
                                    //Ngày kết thúc điều chỉnh
                                    ReceiveDate = GLTRS,
                                    //SO Number: 2200000140
                                    Property1 = KDAUF.TrimStart(new Char[] { '0' }),
                                    //SO Line Number: 10
                                    Property2 = KDPOS.TrimStart(new Char[] { '0' }),
                                    //Số lượng
                                    Qty = Convert.ToInt32(PSMNG),
                                    Number2 = Convert.ToInt32(PSMNG),
                                    //Đơn vị tính
                                    Unit = MEINS,
                                    //Sản phẩm
                                    ProductId = productInDb != null ? productInDb.ProductId : Guid.Empty,
                                    //Loại: lệnh sản xuất con (SAP)
                                    WorkFlowId = LSXC.WorkFlowId,
                                    //Trạng thái: mặc định lấy trạng thái đầu tiên
                                    TaskStatusId = taskStatusId,
                                    PriorityCode = ConstPriotityCode.NORMAL,
                                    //LSX ĐT:
                                    //Fulltext: TRANG-LSXDT	=> NTD:TRANG => Mã lệnh: LSXDT
                                    //1.FullText
                                    Property3 = ZZLSX,
                                    //2.NTD
                                    Property4 = NguoiTheoDoi,
                                    //3. LSXDT
                                    Property5 = LSXDT,

                                    CompanyId = companyId,
                                    CreateTime = DateTime.Now,
                                    CreateBy = SYSTEM,
                                    Actived = true,
                                    isKhongDongBo = false,
                                    isDeleted = (ZCLOSE == "X" || LOEKZ == "X") ? true : false
                                };
                                _context.Entry(newProductionOrderSubtask).State = EntityState.Added;
                                await _context.SaveChangesAsync();

                                //Cập nhật TaskCode cho subtask
                                if (newProductionOrderSubtask.ParentTaskId.HasValue)
                                {
                                    var parentTask = _context.TaskModel.Where(p => p.TaskId == newProductionOrderSubtask.ParentTaskId).FirstOrDefault();
                                    if (parentTask != null)
                                    {
                                        int index = 0;
                                        var lastSubtask = _context.TaskModel.Where(p => p.ParentTaskId == newProductionOrderSubtask.ParentTaskId).Count();
                                        index = lastSubtask + 1;
                                        string subtaskCode = string.Format("{0}-{1}", parentTask.TaskCode, index);
                                        newProductionOrderSubtask.SubtaskCode = subtaskCode;
                                    }
                                }
                            }
                            */
                            #endregion
                        }
                    }
                    else
                    {
                        if (ProOrderInDb == null)
                        {
                            #region Insert
                            var ProOrderNew = new ProductionOrderModel
                            {
                                ProductionOrderId = Guid.NewGuid(),
                                AMEIN = AMEIN,
                                AUFNR = AUFNR,
                                AUFNR_MES = AUFNR.TrimStart(new Char[] { '0' }),
                                DAUAT = DAUAT,
                                DWERK = DWERK,
                                FTRMS = FTRMS,
                                GAMNG = GAMNG,
                                GASMG = GASMG,
                                GLTRP = GLTRP,
                                GLTRS = GLTRS,
                                GSTRP = GSTRP,
                                GSTRS = GSTRS,
                                INSMK = INSMK,
                                KDAUF = KDAUF,
                                KDPOS = KDPOS,
                                KDPOS_MES = KDPOS.TrimStart(new Char[] { '0' }),
                                LTRMI = LTRMI,
                                MATNR = MATNR,
                                MEINS = MEINS,
                                PAMNG = PAMNG,
                                PGMNG = PGMNG,
                                PSMNG = PSMNG,
                                RSNUM = RSNUM,
                                UEBTK = UEBTK,
                                UEBTO = UEBTO,
                                UNTTO = UNTTO,
                                WEMNG = WEMNG,
                                ZZGHICHU = ZZGHICHU,
                                ZZLSX = ZZLSX,
                                ZZSLANSUA = ZZSLANSUA,
                                VERID = VERID,
                                PROJN = PROJN,
                                ZCLOSE = ZCLOSE,
                                LOEKZ = LOEKZ,
                                ZSTATUS = ZSTATUS,
                                CreateTime = DateTime.Now
                            };
                            _context.Entry(ProOrderNew).State = EntityState.Added;

                            //Nếu có SO Number với SO Line Number mới insert vào bảng Task
                            if (!string.IsNullOrEmpty(KDAUF) && !string.IsNullOrEmpty(KDPOS))
                            {
                                //Tìm trong bảng TaskModel đã tồn tại LSX ĐT chưa (field Property3)
                                //1. Chưa có thì tạo đợt + tạo LSX SAP
                                //2. Có rồi thì không tạo đợt mới, chỉ tạo LSX SAP và lấy đợt đã tồn tại
                                //=====================================================================
                                Guid DSXId = Guid.Empty;
                                var existsDSX = _context.TaskModel.Where(p => p.WorkFlowId == LSXD.WorkFlowId && p.Property3 == ZZLSX).FirstOrDefault();
                                if (existsDSX == null)
                                {
                                    //Tạo đợt cho lệnh sản xuất đại trà: mặc định là đợt 1
                                    var newProductionOrder = new TaskModel
                                    {
                                        TaskId = Guid.NewGuid(),
                                        //500001821 - Đợt 1
                                        //Summary = string.Format("{0} - {1}", AUFNR.TrimStart(new Char[] { '0' }), "Đợt 1"),
                                        //Summary = "Đợt 1",
                                        Summary = LSXDT + "-D1",
                                        //Số thứ tự đợt
                                        Number1 = 1,
                                        //LSX ĐT:
                                        //Fulltext: TRANG-LSXDT	=> NTD:TRANG => Mã lệnh: LSXDT
                                        //1.FullText
                                        Property3 = ZZLSX,
                                        //2.NTD
                                        Property4 = NguoiTheoDoi,
                                        //3. LSXDT
                                        Property5 = LSXDT,
                                        //Loại: lệnh sản xuất đại trà
                                        WorkFlowId = LSXD.WorkFlowId,
                                        //Trạng thái: mặc định lấy trạng thái đầu tiên
                                        TaskStatusId = taskStatusId2,
                                        PriorityCode = ConstPriotityCode.NORMAL,
                                        CompanyId = companyId,
                                        CreateTime = DateTime.Now,
                                        CreateBy = SYSTEM,
                                        Actived = true,
                                    };
                                    _context.Entry(newProductionOrder).State = EntityState.Added;

                                    DSXId = newProductionOrder.TaskId;
                                }
                                else
                                {
                                    DSXId = existsDSX.TaskId;
                                }

                                //Tạo lệnh sản xuất con (SAP)
                                var newProductionOrderSubtask = new TaskModel
                                {
                                    TaskId = ProOrderNew.ProductionOrderId,
                                    ParentTaskId = DSXId,
                                    //Lệnh sản xuất SAP: 500001821
                                    Summary = AUFNR.TrimStart(new Char[] { '0' }),
                                    //Ngày bắt đầu: 16/08/2019
                                    StartDate = GSTRS,
                                    //Ngày bắt đầu điều chỉnh
                                    Date1 = GSTRS,
                                    //Ngày dự kiến hoàn thành: 16/08/2019
                                    EstimateEndDate = GLTRS,
                                    //Ngày kết thúc điều chỉnh
                                    ReceiveDate = GLTRS,
                                    //SO Number: 2200000140
                                    Property1 = KDAUF.TrimStart(new Char[] { '0' }),
                                    //SO Line Number: 10
                                    Property2 = KDPOS.TrimStart(new Char[] { '0' }),
                                    //Số lượng
                                    Qty = Convert.ToInt32(PSMNG),
                                    Number2 = Convert.ToInt32(PSMNG),
                                    //Đơn vị tính
                                    Unit = MEINS,
                                    //Sản phẩm
                                    ProductId = productInDb != null ? productInDb.ProductId : Guid.Empty,
                                    //Loại: lệnh sản xuất con (SAP)
                                    WorkFlowId = LSXC.WorkFlowId,
                                    //Trạng thái: mặc định lấy trạng thái đầu tiên
                                    TaskStatusId = taskStatusId,
                                    PriorityCode = ConstPriotityCode.NORMAL,
                                    //LSX ĐT:
                                    //Fulltext: TRANG-LSXDT	=> NTD:TRANG => Mã lệnh: LSXDT
                                    //1.FullText
                                    Property3 = ZZLSX,
                                    //2.NTD
                                    Property4 = NguoiTheoDoi,
                                    //3. LSXDT
                                    Property5 = LSXDT,

                                    CompanyId = companyId,
                                    CreateTime = DateTime.Now,
                                    CreateBy = SYSTEM,
                                    Actived = true,
                                    isKhongDongBo = false,
                                    isDeleted = (ZCLOSE == "X" || LOEKZ == "X") ? true : false
                                };
                                _context.Entry(newProductionOrderSubtask).State = EntityState.Added;
                                await _context.SaveChangesAsync();

                                //Cập nhật TaskCode cho subtask
                                if (newProductionOrderSubtask.ParentTaskId.HasValue)
                                {
                                    var parentTask = _context.TaskModel.Where(p => p.TaskId == newProductionOrderSubtask.ParentTaskId).FirstOrDefault();
                                    if (parentTask != null)
                                    {
                                        int index = 0;
                                        var lastSubtask = _context.TaskModel.Where(p => p.ParentTaskId == newProductionOrderSubtask.ParentTaskId).Count();
                                        index = lastSubtask + 1;
                                        string subtaskCode = string.Format("{0}-{1}", parentTask.TaskCode, index);
                                        newProductionOrderSubtask.SubtaskCode = subtaskCode;
                                    }
                                }
                            }
                            #endregion
                            insertTotal++;
                        }
                        else
                        {
                            #region Update
                            ProOrderInDb.AUFNR_MES = AUFNR.TrimStart(new Char[] { '0' });
                            ProOrderInDb.AMEIN = AMEIN;
                            ProOrderInDb.DAUAT = DAUAT;
                            ProOrderInDb.FTRMS = FTRMS;
                            ProOrderInDb.GAMNG = GAMNG;
                            ProOrderInDb.GASMG = GASMG;
                            ProOrderInDb.GLTRP = GLTRP;
                            ProOrderInDb.GLTRS = GLTRS;
                            ProOrderInDb.GSTRP = GSTRP;
                            ProOrderInDb.GSTRS = GSTRS;
                            ProOrderInDb.INSMK = INSMK;
                            ProOrderInDb.KDAUF = KDAUF;
                            ProOrderInDb.KDPOS = KDPOS;
                            ProOrderInDb.KDPOS_MES = KDPOS.TrimStart(new Char[] { '0' });
                            ProOrderInDb.LTRMI = LTRMI;
                            ProOrderInDb.MATNR = MATNR;
                            ProOrderInDb.MEINS = MEINS;
                            ProOrderInDb.PAMNG = PAMNG;
                            ProOrderInDb.PGMNG = PGMNG;
                            ProOrderInDb.PSMNG = PSMNG;
                            ProOrderInDb.RSNUM = RSNUM;
                            ProOrderInDb.UEBTK = UEBTK;
                            ProOrderInDb.UEBTO = UEBTO;
                            ProOrderInDb.UNTTO = UNTTO;
                            ProOrderInDb.WEMNG = WEMNG;
                            ProOrderInDb.ZZGHICHU = ZZGHICHU;
                            ProOrderInDb.ZZLSX = ZZLSX;
                            ProOrderInDb.ZZSLANSUA = ZZSLANSUA;
                            ProOrderInDb.VERID = VERID;
                            ProOrderInDb.PROJN = PROJN;
                            ProOrderInDb.ZCLOSE = ZCLOSE;
                            ProOrderInDb.LOEKZ = LOEKZ;
                            ProOrderInDb.ZSTATUS = ZSTATUS;
                            ProOrderInDb.LastEditTime = DateTime.Now;
                            _context.Entry(ProOrderInDb).State = EntityState.Modified;

                            //Update task nếu có tồn tại
                            var ProductionOrderTaskInDB = await _context.TaskModel.Where(p => p.TaskId == ProOrderInDb.ProductionOrderId).FirstOrDefaultAsync();
                            if (ProductionOrderTaskInDB != null)
                            {
                                //Ngày bắt đầu
                                ProductionOrderTaskInDB.StartDate = GSTRS;
                                //Ngày dự kiến hoàn thành
                                ProductionOrderTaskInDB.EstimateEndDate = GLTRS;
                                //SO Number
                                ProductionOrderTaskInDB.Property1 = KDAUF;
                                //SO Line Number
                                ProductionOrderTaskInDB.Property2 = KDPOS;
                                //Số lượng
                                //ProductionOrderTaskInDB.Qty = Convert.ToInt32(PSMNG);
                                //LSX ĐT:
                                //Fulltext: TRANG-LSXDT	=> NTD:TRANG => Mã lệnh: LSXDT
                                //1.FullText
                                ProductionOrderTaskInDB.Property3 = ZZLSX;
                                //2.NTD
                                ProductionOrderTaskInDB.Property4 = NguoiTheoDoi;
                                //3. LSXDT
                                ProductionOrderTaskInDB.Property5 = LSXDT;
                                ProductionOrderTaskInDB.ProductId = productInDb != null ? productInDb.ProductId : Guid.Empty;
                                ProductionOrderTaskInDB.LastEditTime = DateTime.Now;
                                ProductionOrderTaskInDB.LastEditBy = SYSTEM;
                                //Nếu 1 trong 2 cột này đánh dấu x là coi như lệnh sx đó đóng
                                ProductionOrderTaskInDB.isDeleted = (ZCLOSE == "X" || LOEKZ == "X") ? true : false;
                                //Update số lượng
                                //Kiểm tra SL cũ vs SL mới: nếu khác update đổi đè dữ liệu DATA SAP 100 => 105 
                                var oldQty = ProductionOrderTaskInDB.Qty;

                                if (ProductionOrderTaskInDB.Qty != Convert.ToInt32(PSMNG))
                                {
                                    ProductionOrderTaskInDB.Qty = Convert.ToInt32(PSMNG);

                                    //Update LSX SAP đã tách theo đợt
                                    //Nếu có tách (> 1 task cùng số PP Order) & Check số lượng mới nếu != SUM của các đợt tách
                                    //Phát hiện không đồng bộ giữa LSX SAP vs LSX SAP đã tách: thì bật cờ isKhongDongBo = true, ngược lại set isKhongDongBo = false
                                    var LSXSAP = AUFNR.TrimStart(new Char[] { '0' });
                                    var SLKH = Convert.ToInt32(PSMNG);
                                    var LSXSAPList = _context.TaskModel.Where(p => p.Summary == LSXSAP).ToList();
                                    bool isKhongDongBo = false;
                                    if (LSXSAPList != null && LSXSAPList.Count > 1)
                                    {
                                        var LSXSAPDB = _context.TaskModel.Where(f => f.Summary == LSXSAP).ToList();
                                        LSXSAPDB.ForEach(a => a.Qty = SLKH);

                                        //SL ĐC
                                        var SLDC = _context.TaskModel.Where(p => p.Summary == LSXSAP).Sum(p => p.Number2);
                                        if (SLKH != SLDC)
                                        {
                                            isKhongDongBo = true;
                                        }
                                    }

                                    if (isKhongDongBo == true)
                                    {
                                        var LSXSAPDB = _context.TaskModel.Where(f => f.Summary == LSXSAP && f.WorkFlowId == LSXC.WorkFlowId).ToList();
                                        LSXSAPDB.ForEach(a => a.isKhongDongBo = true);
                                        _context.SaveChanges();
                                    }
                                }
                                //Số lượng điều chỉnh = với số lượng trên SAP(cũ) => update đè SLKH mới 
                                //Nếu khác nhau thì không update => do số lượng điều chỉnh đã được cập nhật ở thao tác chia đợt
                                if (oldQty == ProductionOrderTaskInDB.Number2)
                                {
                                    ProductionOrderTaskInDB.Number2 = ProductionOrderTaskInDB.Qty;
                                }


                                _context.Entry(ProductionOrderTaskInDB).State = EntityState.Modified;

                                //Update đợt sản xuất
                                var dotSX = _context.TaskModel.Where(p => p.TaskId == ProductionOrderTaskInDB.ParentTaskId).FirstOrDefault();
                                if (dotSX != null)
                                {
                                    //LSX ĐT:
                                    //Fulltext: TRANG-LSXDT	=> NTD:TRANG => Mã lệnh: LSXDT
                                    //1.FullText
                                    dotSX.Property3 = ZZLSX;
                                    //2.NTD
                                    dotSX.Property4 = NguoiTheoDoi;
                                    //3. LSXDT
                                    dotSX.Property5 = LSXDT;
                                }
                                _context.Entry(dotSX).State = EntityState.Modified;
                            }
                            //Nếu chưa có thì thêm mới:
                            //Trường hợp có dữ liệu trong bảng ProductionOrder nhưng chưa có dữ liệu trong Task => khi đồng bộ từ SAP bị thiếu SO Number và SO Line Number nên chưa lưu vào task
                            //=> Cập nhật trên SAP bổ sung 2 field SO Number và SO Line Number => lưu vào task: tạo LSX SAP và Đợt
                            else
                            {
                                //Tìm trong bảng TaskModel đã tồn tại LSX ĐT chưa (field Property3)
                                //1. Chưa có thì tạo đợt + tạo LSX SAP
                                //2. Có rồi thì không tạo đợt mới, chỉ tạo LSX SAP và lấy đợt đã tồn tại
                                //=====================================================================
                                Guid DSXId = Guid.Empty;
                                var existsDSX = _context.TaskModel.Where(p => p.WorkFlowId == LSXD.WorkFlowId && p.Property3 == ZZLSX).FirstOrDefault();
                                if (existsDSX == null)
                                {
                                    //Tạo đợt cho lệnh sản xuất đại trà: mặc định là đợt 1
                                    var newProductionOrder = new TaskModel
                                    {
                                        TaskId = Guid.NewGuid(),
                                        //500001821 - Đợt 1
                                        //Summary = string.Format("{0} - {1}", AUFNR.TrimStart(new Char[] { '0' }), "Đợt 1"),
                                        //Summary = "Đợt 1",
                                        Summary = LSXDT + "-D1",
                                        //Số thứ tự đợt
                                        Number1 = 1,
                                        //LSX ĐT:
                                        //Fulltext: TRANG-LSXDT	=> NTD:TRANG => Mã lệnh: LSXDT
                                        //1.FullText
                                        Property3 = ZZLSX,
                                        //2.NTD
                                        Property4 = NguoiTheoDoi,
                                        //3. LSXDT
                                        Property5 = LSXDT,
                                        //Loại: lệnh sản xuất đại trà
                                        WorkFlowId = LSXD.WorkFlowId,
                                        //Trạng thái: mặc định lấy trạng thái đầu tiên
                                        TaskStatusId = taskStatusId2,
                                        PriorityCode = ConstPriotityCode.NORMAL,
                                        CompanyId = companyId,
                                        CreateTime = DateTime.Now,
                                        CreateBy = SYSTEM,
                                        Actived = true,
                                    };
                                    _context.Entry(newProductionOrder).State = EntityState.Added;

                                    DSXId = newProductionOrder.TaskId;
                                }
                                else
                                {
                                    DSXId = existsDSX.TaskId;
                                }

                                //Tạo lệnh sản xuất con (SAP)
                                var newProductionOrderSubtask = new TaskModel
                                {
                                    TaskId = ProOrderInDb.ProductionOrderId,
                                    ParentTaskId = DSXId,
                                    //Lệnh sản xuất SAP: 500001821
                                    Summary = AUFNR.TrimStart(new Char[] { '0' }),
                                    //Ngày bắt đầu: 16/08/2019
                                    StartDate = GSTRS,
                                    //Ngày bắt đầu điều chỉnh
                                    Date1 = GSTRS,
                                    //Ngày dự kiến hoàn thành: 16/08/2019
                                    EstimateEndDate = GLTRS,
                                    //Ngày kết thúc điều chỉnh
                                    ReceiveDate = GLTRS,
                                    //SO Number: 2200000140
                                    Property1 = KDAUF.TrimStart(new Char[] { '0' }),
                                    //SO Line Number: 10
                                    Property2 = KDPOS.TrimStart(new Char[] { '0' }),
                                    //Số lượng
                                    Qty = Convert.ToInt32(PSMNG),
                                    Number2 = Convert.ToInt32(PSMNG),
                                    //Đơn vị tính
                                    Unit = MEINS,
                                    //Sản phẩm
                                    ProductId = productInDb != null ? productInDb.ProductId : Guid.Empty,
                                    //Loại: lệnh sản xuất con (SAP)
                                    WorkFlowId = LSXC.WorkFlowId,
                                    //Trạng thái: mặc định lấy trạng thái đầu tiên
                                    TaskStatusId = taskStatusId,
                                    PriorityCode = ConstPriotityCode.NORMAL,
                                    //LSX ĐT:
                                    //Fulltext: TRANG-LSXDT	=> NTD:TRANG => Mã lệnh: LSXDT
                                    //1.FullText
                                    Property3 = ZZLSX,
                                    //2.NTD
                                    Property4 = NguoiTheoDoi,
                                    //3. LSXDT
                                    Property5 = LSXDT,

                                    CompanyId = companyId,
                                    CreateTime = DateTime.Now,
                                    CreateBy = SYSTEM,
                                    Actived = true,
                                    isKhongDongBo = false,
                                    isDeleted = (ZCLOSE == "X" || LOEKZ == "X") ? true : false
                                };
                                _context.Entry(newProductionOrderSubtask).State = EntityState.Added;
                                await _context.SaveChangesAsync();

                                //Cập nhật TaskCode cho subtask
                                if (newProductionOrderSubtask.ParentTaskId.HasValue)
                                {
                                    var parentTask = _context.TaskModel.Where(p => p.TaskId == newProductionOrderSubtask.ParentTaskId).FirstOrDefault();
                                    if (parentTask != null)
                                    {
                                        int index = 0;
                                        var lastSubtask = _context.TaskModel.Where(p => p.ParentTaskId == newProductionOrderSubtask.ParentTaskId).Count();
                                        index = lastSubtask + 1;
                                        string subtaskCode = string.Format("{0}-{1}", parentTask.TaskCode, index);
                                        newProductionOrderSubtask.SubtaskCode = subtaskCode;
                                    }
                                }
                            }
                            #endregion
                            updateTotal++;
                        }
                    }
                    await _context.SaveChangesAsync();

                    //ConfirmInsert(AUFNR, KDAUF);

                    var result = ConfirmInsert(AUFNR, KDAUF, WERKS);
                    if (result.IsSuccess == false)
                    {
                        _loggerRepository.Logging(string.Format("I_INSERT: E_ERROR (AUFNR: {0}, WERKS: {1})"
                                                                , AUFNR
                                                                , WERKS), "ZMES_PRODUCTION_ORDER");
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
            message += $"Insert: {insertTotal}, Update: {updateTotal}. Total: {dataTable.Rows.Count}";

            return new Tuple<string, string>(message, error);
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

        private async Task<string> InsertUpdateComponent(DataTable dataTable)
        {
            string error = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    #region Collect Data
                    var AUFNR = dataRow["AUFNR"].ToString();
                    int? RSNUM = null;
                    if (!string.IsNullOrEmpty(dataRow["RSNUM"].ToString()))
                    {
                        RSNUM = Int32.Parse(dataRow["RSNUM"].ToString());
                    }
                    int? RSPOS = null;
                    if (!string.IsNullOrEmpty(dataRow["RSPOS"].ToString()))
                    {
                        RSPOS = Int32.Parse(dataRow["RSPOS"].ToString());
                    }
                    var XLOEK = dataRow["XLOEK"].ToString();
                    var XWAOK = dataRow["XWAOK"].ToString();
                    string MATNR = string.Empty;
                    if (!string.IsNullOrEmpty(dataRow["MATNR"].ToString()))
                    {
                        MATNR = Int32.Parse(dataRow["MATNR"].ToString()).ToString();
                    }
                    var WERKS = dataRow["WERKS"].ToString();
                    var LGORT = dataRow["LGORT"].ToString();
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
                    decimal? ERFMG = null;
                    if (!string.IsNullOrEmpty(dataRow["ERFMG"].ToString()))
                    {
                        ERFMG = decimal.Parse(dataRow["ERFMG"].ToString());
                    }
                    var ERFME = dataRow["ERFME"].ToString();
                    var KDAUF = dataRow["KDAUF"].ToString();
                    var KDPOS = dataRow["KDPOS"].ToString();
                    var SHKZG = dataRow["SHKZG"].ToString();
                    var VERID = dataRow["VERID"].ToString();
                    #endregion

                    //80: Nếu chưa có thì thêm mới, có rồi thì không làm gì hết
                    var componentInDb80 = await _context.ProductionComponent80Model.Where(p => p.AUFNR == AUFNR && p.RSPOS == RSPOS && p.WERKS == WERKS).FirstOrDefaultAsync();
                    //Nếu chưa có product thì thêm mới: Có rồi thì update
                    if (componentInDb80 == null)
                    {
                        #region Insert
                        var componentNew = new ProductionComponent80Model
                        {
                            ProductComponentId = Guid.NewGuid(),
                            WERKS = WERKS,
                            AUFNR = AUFNR,
                            BDMNG = BDMNG,
                            BDTER = BDTER,
                            ERFME = ERFME,
                            ERFMG = ERFMG,
                            KDAUF = KDAUF,
                            KDPOS = KDPOS,
                            LGORT = LGORT,
                            MATNR = MATNR,
                            MEINS = MEINS,
                            RSNUM = RSNUM,
                            RSPOS = RSPOS,
                            SHKZG = SHKZG,
                            SOBKZ = SOBKZ,
                            XLOEK = XLOEK,
                            XWAOK = XWAOK,
                            VERID = VERID,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(componentNew).State = EntityState.Added;

                        #endregion Insert
                    }

                    //100
                    var componentInDb = await _context.ProductionComponentModel.Where(p => p.AUFNR == AUFNR && p.RSPOS == RSPOS && p.WERKS == WERKS).FirstOrDefaultAsync();
                    //Nếu chưa có product thì thêm mới: Có rồi thì update
                    if (componentInDb == null)
                    {
                        #region Insert
                        var componentNew = new ProductionComponentModel
                        {
                            ProductComponentId = Guid.NewGuid(),
                            WERKS = WERKS,
                            AUFNR = AUFNR,
                            BDMNG = BDMNG,
                            BDTER = BDTER,
                            ERFME = ERFME,
                            ERFMG = ERFMG,
                            KDAUF = KDAUF,
                            KDPOS = KDPOS,
                            LGORT = LGORT,
                            MATNR = MATNR,
                            MEINS = MEINS,
                            RSNUM = RSNUM,
                            RSPOS = RSPOS,
                            SHKZG = SHKZG,
                            SOBKZ = SOBKZ,
                            XLOEK = XLOEK,
                            XWAOK = XWAOK,
                            VERID = VERID,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(componentNew).State = EntityState.Added;

                        #endregion Insert
                    }
                    else
                    {
                        #region Update
                        componentInDb.BDMNG = BDMNG;
                        componentInDb.BDTER = BDTER;
                        componentInDb.ERFME = ERFME;
                        componentInDb.ERFMG = ERFMG;
                        componentInDb.KDAUF = KDAUF;
                        componentInDb.KDPOS = KDPOS;
                        componentInDb.LGORT = LGORT;
                        componentInDb.MATNR = MATNR;
                        componentInDb.MEINS = MEINS;
                        componentInDb.RSNUM = RSNUM;
                        componentInDb.SHKZG = SHKZG;
                        componentInDb.SOBKZ = SOBKZ;
                        componentInDb.XLOEK = XLOEK;
                        componentInDb.XWAOK = XWAOK;
                        componentInDb.VERID = VERID;
                        componentInDb.LastEditTime = DateTime.Now;
                        _context.Entry(componentInDb).State = EntityState.Modified;
                        #endregion Update
                    }
                    await _context.SaveChangesAsync();
                    //Xác nhận insert vào DB thành công cho SAP
                    // ConfirmInsert(WERKS, MATNR);
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
        private async Task<string> InsertUpdateOperation(DataTable dataTable)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            string error = string.Empty;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                try
                {
                    #region Collect Data
                    var VORNR = dataRow["VORNR"].ToString();
                    var AUFNR = dataRow["AUFNR"].ToString();

                    int? AUFPL = null;
                    if (!string.IsNullOrEmpty(dataRow["AUFPL"].ToString()))
                    {
                        AUFPL = Int32.Parse(dataRow["AUFPL"].ToString());
                    }
                    int? APLZL = null;
                    if (!string.IsNullOrEmpty(dataRow["APLZL"].ToString()))
                    {
                        APLZL = Int32.Parse(dataRow["APLZL"].ToString());
                    }
                    var STEUS = dataRow["STEUS"].ToString();
                    var WERKS = dataRow["WERKS"].ToString();
                    var LTXA1 = dataRow["LTXA1"].ToString();
                    var LTXA2 = dataRow["LTXA2"].ToString();
                    var LAR01 = dataRow["LAR01"].ToString();
                    var LAR02 = dataRow["LAR02"].ToString();
                    var LAR03 = dataRow["LAR03"].ToString();
                    var LAR04 = dataRow["LAR04"].ToString();
                    var LAR05 = dataRow["LAR05"].ToString();
                    var LAR06 = dataRow["LAR06"].ToString();
                    var ZERMA = dataRow["ZERMA"].ToString();
                    var VGWTS = dataRow["VGWTS"].ToString();
                    int? RUECK = null;
                    if (!string.IsNullOrEmpty(dataRow["RUECK"].ToString()))
                    {
                        RUECK = Int32.Parse(dataRow["RUECK"].ToString());
                    }
                    var VERID = dataRow["VERID"].ToString();
                    #endregion

                    //80: Nếu chưa có thì thêm mới, có rồi thì không làm gì hết
                    var operationInDb80 = await _context.ProductionOperation80Model.Where(p => p.VORNR == VORNR && p.AUFPL == AUFPL && p.WERKS == WERKS).FirstOrDefaultAsync();
                    //Nếu chưa có thì thêm mới : Có rồi thì update
                    if (operationInDb80 == null)
                    {
                        #region Insert
                        var newProductionOperation = new ProductionOperation80Model
                        {
                            ProductOperationId = Guid.NewGuid(),
                            VORNR = VORNR,
                            APLZL = APLZL,
                            AUFPL = AUFPL,
                            LAR01 = LAR01,
                            LAR02 = LAR02,
                            LAR03 = LAR03,
                            LAR04 = LAR04,
                            LAR05 = LAR05,
                            LAR06 = LAR06,
                            LTXA1 = LTXA1,
                            LTXA2 = LTXA2,
                            RUECK = RUECK,
                            STEUS = STEUS,
                            VGWTS = VGWTS,
                            WERKS = WERKS,
                            ZERMA = ZERMA,
                            VERID = VERID,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(newProductionOperation).State = EntityState.Added;
                        #endregion
                    }

                    //100
                    var operationInDb = await _context.ProductionOperationModel.Where(p => p.VORNR == VORNR && p.AUFPL == AUFPL && p.WERKS == WERKS).FirstOrDefaultAsync();
                    //Nếu chưa có thì thêm mới : Có rồi thì update
                    if (operationInDb == null)
                    {
                        #region Insert
                        var newProductionOperation = new ProductionOperationModel
                        {
                            ProductOperationId = Guid.NewGuid(),
                            VORNR = VORNR,
                            APLZL = APLZL,
                            AUFPL = AUFPL,
                            LAR01 = LAR01,
                            LAR02 = LAR02,
                            LAR03 = LAR03,
                            LAR04 = LAR04,
                            LAR05 = LAR05,
                            LAR06 = LAR06,
                            LTXA1 = LTXA1,
                            LTXA2 = LTXA2,
                            RUECK = RUECK,
                            STEUS = STEUS,
                            VGWTS = VGWTS,
                            WERKS = WERKS,
                            ZERMA = ZERMA,
                            VERID = VERID,
                            CreateTime = DateTime.Now
                        };
                        _context.Entry(newProductionOperation).State = EntityState.Added;
                        #endregion
                    }
                    else
                    {
                        #region Update
                        //Update Product Attribute
                        operationInDb.APLZL = APLZL;
                        operationInDb.LAR01 = LAR01;
                        operationInDb.LAR02 = LAR02;
                        operationInDb.LAR03 = LAR03;
                        operationInDb.LAR04 = LAR04;
                        operationInDb.LAR05 = LAR05;
                        operationInDb.LAR06 = LAR06;
                        operationInDb.LTXA1 = LTXA1;
                        operationInDb.LTXA2 = LTXA2;
                        operationInDb.RUECK = RUECK;
                        operationInDb.STEUS = STEUS;
                        operationInDb.VGWTS = VGWTS;
                        operationInDb.ZERMA = ZERMA;
                        operationInDb.VERID = VERID;
                        operationInDb.LastEditTime = DateTime.Now;
                        _context.Entry(operationInDb).State = EntityState.Modified;
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
    }
}
