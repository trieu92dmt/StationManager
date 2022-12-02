using ISD.Core;
using ISD.EntityModels;
using ISD.Extensions;
using ISD.Repositories.MES.IntegrationSAP.MasterData;
using ISD.Resources;
using ISD.ViewModels.MES;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MES.Controllers
{
    public class SyncDataFromSAPController : BaseController
    {
        // GET: SyncDataFromSAP
        #region Index
        [ISDAuthorizationAttribute]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Sync Routing
        public Task<JsonResult> SyncRouting(ParamRequestSyncRoutingModel viewModel)
        {
            return ExecuteContainerAsync(async () =>
            {
                var paramRequest = new ParamRequestSyncSapModel
                {
                    CompanyCode = viewModel.WERKS,
                    MATNR = viewModel.MATNR,
                };
                var _routingReposiroty = new RoutingRepository(_context);
                var message = await _routingReposiroty.SyncRoutingByPlantAndMaterialNumber(paramRequest);
                if (string.IsNullOrEmpty(message) || (message.Contains("Insert:") && message.Contains("Update:")))
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_SyncData_Success, "Routing") + (!string.IsNullOrEmpty(message) ? " (" + message + ")" : string.Empty),
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Đã xảy ra lỗi: " + message,
                    });
                }

            });
        }
        #endregion

        #region Sync BOM
        public Task<JsonResult> SyncBOM(ParamRequestSyncBOMModel viewModel)
        {
            return ExecuteContainerAsync(async () =>
            {
                var paramRequest = new ParamRequestSyncSapModel
                {
                    CompanyCode = viewModel.WERKS,
                    MATNR = viewModel.MATNR,
                };
                var _bomReposiroty = new BOMRepository(_context);
                var message = await _bomReposiroty.SyncBOM(paramRequest);
                if (string.IsNullOrEmpty(message) || (message.Contains("Insert:") && message.Contains("Update:")))
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_SyncData_Success, "BOM") + (!string.IsNullOrEmpty(message) ? " (" + message + ")" : string.Empty),
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Đã xảy ra lỗi: " + message,
                    });
                }

            });
        }
        #endregion

        #region Sync Material
        public Task<JsonResult> SyncMaterial(ParamRequestSyncMaterialModel viewModel)
        {
            return ExecuteContainerAsync(async () =>
            {
                var paramRequest = new ParamRequestSyncSapModel
                {
                    CompanyCode = viewModel.WERKS,
                    MATNR = viewModel.MATNR,
                };
                var _materialReposiroty = new MaterialMasterRepository(_context);
                var message = await _materialReposiroty.GetMaterialMaster(paramRequest);
                if (string.IsNullOrEmpty(message) || (message.Contains("Insert:") && message.Contains("Update:")))
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_SyncData_Success, "material") + (!string.IsNullOrEmpty(message) ? " (" + message + ")" : string.Empty),
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Đã xảy ra lỗi: " + message,
                    });
                }

            });
        }
        #endregion

        #region Sync PP Order
        public Task<JsonResult> SyncPPOrder(ParamRequestSyncPPOrderModel viewModel)
        {
            return ExecuteContainerAsync(async () =>
            {
                if (string.IsNullOrEmpty(viewModel.LSXDT) && string.IsNullOrEmpty(viewModel.PPOrder))
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Vui lòng nhập thông tin LSX ĐT hoặc LSX SAP",
                    });
                }
                //Nếu truyền LSX ĐT thì tìm trong MES và lấy danh sách LSX SAP + kết quả trả về => update tất cả LSX SAP liên quan
                if (!string.IsNullOrEmpty(viewModel.LSXDT))
                {
                    var paramRequest = new ParamRequestSyncSapModel
                    {
                        IZZLSX = viewModel.LSXDT,
                        CompanyCode = viewModel.WERKS,
                    };
                    var _ProductionOrderRepository = new ProductOrderRepository(_context);
                    var error = string.Empty;
                    var dataTables = _ProductionOrderRepository.GetProductionOrderData(paramRequest, out error);
                    if (!string.IsNullOrEmpty(error))
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.NotModified,
                            Success = false,
                            Data = error,
                        });
                    }
                    //Danh sách LSX SAP từ MES
                    var LSXSAPList = _context.TaskModel.Where(p => p.ParentTaskId != null && p.Property3 == viewModel.LSXDT).Select(p => p.Summary).ToList();
                    if (dataTables != null && dataTables.Count > 0)
                    {
                        foreach (DataRow dataRow in dataTables[0].Rows)
                        {
                            try
                            {
                                var AUFNR = dataRow["AUFNR"].ToString();
                                //Nếu chưa có trong danh sách từ MES thì mới add
                                if (!LSXSAPList.Contains(AUFNR))
                                {
                                    LSXSAPList.Add(AUFNR);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.NotModified,
                            Success = false,
                            Data = "Không tìm thấy LSXSAP nào",
                        });
                    }

                    if (LSXSAPList != null && LSXSAPList.Count > 0)
                    {
                        foreach (var LSXSAP in LSXSAPList)
                        {
                            paramRequest = new ParamRequestSyncSapModel
                            {
                                AUFNR = LSXSAP,
                                CompanyCode = viewModel.WERKS,
                            };
                            var message = await _ProductionOrderRepository.SyncProductionOrder(paramRequest);
                            if (string.IsNullOrEmpty(message) || (message.Contains("Insert:") && message.Contains("Update:")))
                            {
                              
                            }
                            else
                            {
                                return Json(new
                                {
                                    Code = System.Net.HttpStatusCode.NotModified,
                                    Success = false,
                                    Data = "Đã xảy ra lỗi: " + message,
                                });
                            }
                        }
                    }
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_SyncData_Success, "PP Order"),
                    });
                }
                //Nếu truyền LSX SAP thì chỉ cần gọi hàm đồng bộ bình thường
                else
                {
                    var paramRequest = new ParamRequestSyncSapModel
                    {
                        AUFNR = viewModel.PPOrder,
                        CompanyCode = viewModel.WERKS,
                    };
                    var _ProductionOrderRepository = new ProductOrderRepository(_context);
                    var message = await _ProductionOrderRepository.SyncProductionOrder(paramRequest);
                    if (string.IsNullOrEmpty(message) || (message.Contains("Insert:") && message.Contains("Update:")))
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.Created,
                            Success = true,
                            Data = string.Format(LanguageResource.Alert_SyncData_Success, "PP Order") + (!string.IsNullOrEmpty(message) ? " (" + message + ")" : string.Empty),
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Code = System.Net.HttpStatusCode.NotModified,
                            Success = false,
                            Data = "Đã xảy ra lỗi: " + message,
                        });
                    }
                }
            });
        }
        #endregion

        #region Sync PR PO
        public Task<JsonResult> SyncPRPO(ParamRequestSyncPRPOModel viewModel)
        {
            return ExecuteContainerAsync(async () =>
            {
                var paramRequest = new ParamRequestSyncSapModel
                {
                    CompanyCode = viewModel.WERKS,
                    VBELN = viewModel.VBELN,
                };
                PrPoMigoRepository _prPoMigoRepository = new PrPoMigoRepository(_context);
                var message = await _prPoMigoRepository.SyncPrPoMigo(paramRequest);
                if (string.IsNullOrEmpty(message) || (message.Contains("Insert:") && message.Contains("Update:")))
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_SyncData_Success, "PR PO MIGO") + (!string.IsNullOrEmpty(message) ? " (" + message + ")" : string.Empty),
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Đã xảy ra lỗi: " + message,
                    });
                }

            });
        }
        #endregion

        #region Sync SO
        public Task<JsonResult> SyncSO(ParamRequestSyncSOModel viewModel)
        {
            return ExecuteContainerAsync(async () =>
            {
                var paramRequest = new ParamRequestSyncSapModel
                {
                    CompanyCode = viewModel.WERKS,
                    VBELN = viewModel.VBELN,
                };
                var _soReposiroty = new SaleOrderRepository(_context);
                var message = await _soReposiroty.SyncSaleOrder(paramRequest);
                if (string.IsNullOrEmpty(message) || (message.Contains("Insert:") && message.Contains("Update:")))
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_SyncData_Success, "SO") + (!string.IsNullOrEmpty(message) ? " (" + message + ")" : string.Empty),
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Đã xảy ra lỗi: " + message,
                    });
                }

            });
        }
        #endregion

        #region Sync SOTEXT_PR
        public Task<JsonResult> SyncSOTEXT_PR(ParamRequestSyncSOTEXTPRModel viewModel)
        {
            return ExecuteContainerAsync(async () =>
            {
                var paramRequest = new ParamRequestSyncSapModel
                {
                    CompanyCode = viewModel.WERKS,
                    IBANFN = viewModel.BANFN,
                };
                var _soReposiroty = new SaleOrderRepository(_context);
                var message = await _soReposiroty.SyncSOTEXT_PR(paramRequest);
                if (string.IsNullOrEmpty(message) || (message.Contains("Insert:") && message.Contains("Update:")))
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_SyncData_Success, "SOTEXT_PR") + (!string.IsNullOrEmpty(message) ? " (" + message + ")" : string.Empty),
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Đã xảy ra lỗi: " + message,
                    });
                }

            });
        }
        #endregion

        #region Sync POTEXT_SO_PR
        public JsonResult SyncPOTEXT_SO_PR(ParamRequestSyncPOTEXT_SO_PRModel viewModel)
        {
            return ExecuteContainer(() =>
            {
                var paramRequest = new ParamRequestSyncSapModel
                {
                    CompanyCode = viewModel.WERKS,
                    IEBELN = viewModel.EBELN,
                };
                var _prpoReposiroty = new PrPoMigoRepository(_context);
                var message = _prpoReposiroty.syncPOTEXT_SO_PR(paramRequest);
                if (string.IsNullOrEmpty(message))
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.Created,
                        Success = true,
                        Data = string.Format(LanguageResource.Alert_SyncData_Success, "POTEXT_SO_PR"),
                    });
                }
                else
                {
                    return Json(new
                    {
                        Code = System.Net.HttpStatusCode.NotModified,
                        Success = false,
                        Data = "Đã xảy ra lỗi: " + message,
                    });
                }

            });
        }
        #endregion
    }
}