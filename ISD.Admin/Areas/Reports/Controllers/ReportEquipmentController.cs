using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Web.Mvc;
using ISD.Constant;
using ISD.Core;
using ISD.ViewModels;
using ISD.ViewModels.API;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Reports.Controllers
{
    public class ReportEquipmentController : BaseController
    {
        // GET: ReportEquipment
        public ActionResult Index(string Id = null, string StepCode = null)
        {
            ViewBag.Id = Id;
            ViewBag.StepCode = StepCode;
            return View();
        }

        public ActionResult DocumentViewerPartial(string Id = null, string StepCode = null)
        {
            EquipmentXtraReport report = new EquipmentXtraReport();
            report = CreateDataReport(Id, StepCode);
            ViewData["Equipment_Report"] = report;
            ViewBag.Id = Id;
            ViewBag.StepCode = StepCode;
            return PartialView("_DocumentViewerPartial");
        }
        public ActionResult DocumentViewerPartialExport(string Id = null, string StepCode = null)
        {
            EquipmentXtraReport catalogueReport = CreateDataReport(Id, StepCode);
            return DocumentViewerExtension.ExportTo(catalogueReport, Request);
        }

        #region Lấy dữ liệu từ API
        //Tạo report
        public EquipmentXtraReport CreateDataReport(string Id = null, string StepCode = null)
        {
            DataSet ds = new DataSet();
            DataTable listdata = new DataTable();
            DataTable listdata2 = new DataTable();
            listdata2.Columns.Add("equipment");
            listdata2.Columns.Add("equipmentLabel");
            // Tạo http Client
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                // tạo request Authenticate
                var loginViewModel = new
                {
                    userName = CurrentUser.UserName,
                    password = ConfigurationManager.AppSettings["DEFAUTL_PASSWORD"]
                };
                var url = WebConfigurationManager.AppSettings["APIDomainUrl"] + "api/v1/Permission/Auth/Authenticate";
                var authRequest = client.PostAsync(url, loginViewModel, new JsonMediaTypeFormatter()).Result;
                //var authRequest = client.PostAsync("api/v1/Permission/Auth/Authenticate", loginViewModel, new JsonMediaTypeFormatter()).Result;

                var authResult = JsonConvert.DeserializeObject<AuthenticateResultViewModel>(authRequest.Content.ReadAsStringAsync().Result);

                if (authResult.Data.token != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + authResult.Data.token);
                    // Tạo Params
                    var param = string.Format("?EquipmentId={0}&StepCode={1}&Plant={2}", Id, StepCode, CurrentUser.SaleOrg);
                    // Gọi request
                    var requestApiUrl = WebConfigurationManager.AppSettings["APIDomainUrl"] + "api/v1/MasterData/Equipment/save-barcode";
                    //HttpResponseMessage request = client.GetAsync(requestApiUrl + param).Result;
                    var rquestmesseage = new HttpRequestMessage(HttpMethod.Get, requestApiUrl + param);
                    var request = client.SendAsync(new HttpRequestMessage(HttpMethod.Get,requestApiUrl + param));

                    var response = request.Result;
                    // Tạo dataset
                    if (response.IsSuccessStatusCode)
                    {
                        var data = JsonConvert.DeserializeObject<ApiResponse>(response.Content.ReadAsStringAsync().Result);

                        if (data.IsSuccess == true)
                        {
                            bool flag = true;
                            listdata = JsonConvert.DeserializeObject<DataTable>(data.Data.ToString());
                            dynamic temp = JsonConvert.DeserializeObject(data.Data.ToString());
                            foreach (var item in temp[0].equipments)
                            {
                                if (flag == true)
                                {
                                    listdata2.Rows.Add(item.equipmentCode + " | " + item.equipmentName, "Máy/chuyền:");
                                    flag = false;
                                }
                                else
                                {
                                    listdata2.Rows[0][0] += ",  " + item.equipmentCode + " | " + item.equipmentName;
                                }
                            }
                            //temp[0].equipments[0].equipmentCode


                        }
                    }
                    //listdata.Rows[0][6] = ConstDomain.Domain + listdata.Rows[0][6];
                    listdata.TableName = "Equipment";
                    listdata2.TableName = "EquipmentDetail";
           
                    ds.Tables.Add(listdata);
                    ds.Tables.Add(listdata2);
                    //handle QR Code URL
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        //var adminDomain = "F:/Working/ITP_IMES/SourceCode/ISD.Admin/";
                        var adminDomain = ConstDomain.Domain;
                        foreach (DataRow item in ds.Tables[0].Rows)
                        {
                            item["qrCode"] = string.Format("{0}{1}", adminDomain, item["qrCode"]?.ToString());
                        }
                    }
                }
            }

            //dataset ds = getdata(model);
            //bước 2: tạo report
            EquipmentXtraReport report = new EquipmentXtraReport();
            ////Bước 3: Gán data cho report
            report.DataSource = ds;
            report.DataMember = null;

            //Bước 4: Set các thông số khác cho report
            return report;

        }
        #endregion
    }
}