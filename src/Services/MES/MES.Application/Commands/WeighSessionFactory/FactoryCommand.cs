using MediatR;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using static Core.Constants.MESConstants;

namespace MES.Application.Commands.WeighSessionFactory
{
    public class FactoryCommand : IRequest<bool>
    {
        
    }

    public class FactoryCommandHandler : IRequestHandler<FactoryCommand, bool>
    {
        private string connectionStringDataCollection;
        private string connectionStringRiceFactoryDatabase;
        public FactoryCommandHandler()
        {
            connectionStringRiceFactoryDatabase = "Data Source=192.168.181.50;Initial Catalog=RiceFactoryDatabase_2017;User ID=citek;Password=123456;TrustServerCertificate=true";
            connectionStringDataCollection = "Data Source=192.168.100.233;Initial Catalog=DataCollection;User ID=isd;Password=pm123@abcd;TrustServerCertificate=true";
        }
        public async Task<bool> Handle(FactoryCommand request, CancellationToken cancellationToken)
        {
            var dateNow = DateTime.Now;
            var dateKey = DateTime.Now.ToString("yyyyMMdd");

            DateTime startTime = new DateTime(2023, 3, 15);
            DateTime endTime = dateNow.Date.AddDays(1).AddSeconds(-1);

            try
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                var weighSessions = new List<WeighSessionRefactoryResponse>();

                // Lấy dữ liệu từ table trong cơ sở dữ liệu RiceFactoryDatabase
                DataTable ds = new DataTable();
                using (SqlConnection connection = new SqlConnection(connectionStringRiceFactoryDatabase))
                {
                    //GET danh sách số lô sử dụng trong ngày
                    //connection.Open();
                    var query = $"SELECT TOP (2) * FROM Tb_Session WHERE (StartTime >= '{startTime}') AND (StartTime<='{endTime}') ORDER BY EndTime DESC";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                    connection.Open();

                    adapter.Fill(ds);

                    var jsonStringTbSession = JsonConvert.SerializeObject(ds);
                    var sessionRespone = JsonConvert.DeserializeObject<List<TbSession>>(jsonStringTbSession, jsonSettings);

                    //Data cân
                    foreach (var session in sessionRespone)
                    {
                        //Data 150Kg Cân nguyên liệu đầu vào để sản xuất, Cân thành phẩm đầu ra số 1 và 2
                        DataTable dsLine1 = new DataTable();
                        string line_1Command = $"SELECT [Id], [LotNumber],[StartTime],[EndTime],[V_Material],[V_Product_1] FROM Line_1 WHERE LotNumber = '{session.LotNumber}'";
                        SqlDataAdapter adapterLine1 = new SqlDataAdapter(line_1Command, connection);
                        adapterLine1.Fill(dsLine1);
                        var jsonStringLine1 = JsonConvert.SerializeObject(dsLine1);

                        var line1Respone = JsonConvert.DeserializeObject<List<Line1And2Response>>(jsonStringLine1, jsonSettings).FirstOrDefault();

                        DataTable dsLine2 = new DataTable();
                        string line_2Command = $"SELECT [Id], [LotNumber],[StartTime],[EndTime],[V_Material],[V_Product_1] FROM Line_2 WHERE LotNumber = '{session.LotNumber}'";
                        SqlDataAdapter adapterLine2 = new SqlDataAdapter(line_2Command, connection);
                        adapterLine2.Fill(dsLine2);
                        var jsonStringLine2 = JsonConvert.SerializeObject(dsLine2);
                        var line2Respone = JsonConvert.DeserializeObject<List<Line1And2Response>>(jsonStringLine2, jsonSettings).FirstOrDefault();

                        //Data 150Kg Cân tấm thành phẩm đầu ra 1 và 2
                        DataTable dsLine3 = new DataTable();
                        string line_3Command = $"SELECT [Id], [LotNumber],[StartTime],[EndTime],[V_Product_1_1] FROM Line_3 WHERE LotNumber = '{session.LotNumber}'";
                        SqlDataAdapter adapterLine3 = new SqlDataAdapter(line_3Command, connection);
                        adapterLine3.Fill(dsLine3);
                        var jsonStringLine3 = JsonConvert.SerializeObject(dsLine3);
                        var line3Respone = JsonConvert.DeserializeObject<List<Line1And2Response>>(jsonStringLine3, jsonSettings).FirstOrDefault();

                        if (line1Respone.V_Product_1 == 0 && line1Respone.V_Material == 0)
                        {
                            //Data cân NVL đầu vào - đầu cân 2
                            weighSessions.Add(new WeighSessionRefactoryResponse
                                (line2Respone.LotNumber, ScaleProduction.NVL_Input2, Convert.ToDecimal(line2Respone.V_Material), line2Respone.StartTime, line2Respone.EndTime, 0));

                            //Data cân thành phẩm - đầu cân 2
                            weighSessions.Add(new WeighSessionRefactoryResponse
                                 (line2Respone.LotNumber, ScaleProduction.NVL_Output2, Convert.ToDecimal(line2Respone.V_Product_1), line2Respone.StartTime, line2Respone.EndTime, 0));

                            //Data cân thành phẩm - đầu cân 2
                            weighSessions.Add(new WeighSessionRefactoryResponse
                                 (line2Respone.LotNumber, ScaleProduction.TTP_Output2, Convert.ToDecimal(line2Respone.V_Product_1), line2Respone.StartTime, line2Respone.EndTime, 0));
                        }
                        else
                        {
                            //Data cân NVL đầu vào - đầu cân 1
                            weighSessions.Add(new WeighSessionRefactoryResponse
                                (line2Respone.LotNumber, ScaleProduction.NVL_Input1, Convert.ToDecimal(line1Respone.V_Material), line1Respone.StartTime, line1Respone.EndTime, 0));

                            //Data cân thành phẩm - đầu cân 1
                            weighSessions.Add(new WeighSessionRefactoryResponse
                                 (line2Respone.LotNumber, ScaleProduction.NVL_Output1, Convert.ToDecimal(line1Respone.V_Product_1), line1Respone.StartTime, line1Respone.EndTime, 0));

                            //Data cân thành phẩm - đầu cân 1
                            weighSessions.Add(new WeighSessionRefactoryResponse
                                 (line2Respone.LotNumber, ScaleProduction.TTP_Output1, Convert.ToDecimal(line1Respone.V_Product_1), line1Respone.StartTime, line1Respone.EndTime, 0));
                        }
                    }

                    connection.Close();

                }

                if (weighSessions.Any())
                {
                    using (SqlConnection connectionDataCollection = new SqlConnection(connectionStringDataCollection))
                    {
                        string queryInsert = "INSERT INTO WeighSessionModel ([WeighSessionCode], [ScaleCode], [DateKey], [OrderIndex], [TotalNumberOfWeigh], [TotalWeight], [StartTime]," +
                                                                    "[EndTime], [CreateTime], [SessionCheck]) " +
                                                                    "VALUES (@WeighSessionCode, @ScaleCode, @DateKey, @OrderIndex, @TotalNumberOfWeigh, @TotalWeight, @StartTime, " +
                                                                    "@EndTime, @CreateTime, @SessionCheck)";

                        SqlCommand command = new SqlCommand(queryInsert, connectionDataCollection);

                        connectionDataCollection.Open();

                        command.Parameters.Add("@WeighSessionCode", SqlDbType.VarChar);
                        command.Parameters.Add("@ScaleCode", SqlDbType.VarChar);
                        command.Parameters.Add("@DateKey", SqlDbType.VarChar);
                        command.Parameters.Add("@OrderIndex", SqlDbType.Int);
                        command.Parameters.Add("@TotalNumberOfWeigh", SqlDbType.Int);
                        command.Parameters.Add("@TotalWeight", SqlDbType.Decimal);
                        command.Parameters.Add("@StartTime", SqlDbType.DateTime);
                        command.Parameters.Add("@EndTime", SqlDbType.DateTime);
                        command.Parameters.Add("@CreateTime", SqlDbType.DateTime);
                        command.Parameters.Add("@SessionCheck", SqlDbType.Int);

                        //Lưu data weigh session
                        foreach (var item in weighSessions)
                        {
                            var weighSessionCode = $"{item.ScaleCode}-{dateKey}-{item.LotNumber}";

                            DataTable dbIndex = new DataTable();
                            var queryOrderIndex = $"SELECT [DateKey], [OrderIndex] FROM WeighSessionModel WHERE (StartTime >= '{startTime}') AND (StartTime<='{endTime}')" +
                                                  $"AND ScaleCode = '{item.ScaleCode}' ORDER BY OrderIndex DESC";
                            SqlDataAdapter adapterIndex = new SqlDataAdapter(queryOrderIndex, connectionDataCollection);
                            adapterIndex.Fill(dbIndex);

                            var jsonStringIndex = JsonConvert.SerializeObject(ds);
                            var indexRespone = JsonConvert.DeserializeObject<List<WeighSessionResponse>>(jsonStringIndex, jsonSettings).FirstOrDefault();

                            if (!indexRespone.OrderIndex.HasValue)
                            {
                                string sql = $"UPDATE WeighSessionModel SET SessionCheck = 1 WHERE ScaleCode = '{item.ScaleCode}'";
                                SqlCommand commandUpdate = new SqlCommand(sql, connectionDataCollection);
                                commandUpdate.ExecuteNonQuery();

                                command.Parameters["@WeighSessionCode"].Value = weighSessionCode;
                                command.Parameters["@ScaleCode"].Value = item.ScaleCode;
                                command.Parameters["@DateKey"].Value = dateKey;
                                command.Parameters["@OrderIndex"].Value = item.LotNumber;
                                command.Parameters["@TotalNumberOfWeigh"].Value = 1;
                                command.Parameters["@TotalWeight"].Value = item.TotalWeight;
                                command.Parameters["@StartTime"].Value = item.StartTime;
                                command.Parameters["@EndTime"].Value = item.EndTime;
                                command.Parameters["@CreateTime"].Value = dateNow;
                                command.Parameters["@SessionCheck"].Value = 0;
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                string sql = $"UPDATE WeighSessionModel SET TotalWeight = {item.TotalWeight} WHERE ScaleCode = '{item.ScaleCode}' AND" +
                                             $"WeighSessionCode = {weighSessionCode}";
                                SqlCommand commandUpdate = new SqlCommand(sql, connectionDataCollection);
                                commandUpdate.ExecuteNonQuery();
                            }

                            
                        }

                        connectionDataCollection.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                string pathLog = "C:\\log-fail-wsfservice.txt";
                using (StreamWriter writer = new StreamWriter(pathLog, true))
                {
                    writer.WriteLine($"WSF.Service is called fail on {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}. Error: {ex.Message} ");
                    writer.Close();
                }
                throw;
            }
        }
    }
    public class WeighSessionRefactoryResponse
    {
        public WeighSessionRefactoryResponse(int lotNumber, string scaleCode, decimal? totalWeight, DateTime? startTime, DateTime? endTime, int sessionCheck)
        {
            LotNumber = lotNumber;
            ScaleCode = scaleCode;
            TotalWeight = totalWeight;
            StartTime = startTime;
            EndTime = endTime;
            SessionCheck = sessionCheck;
        }
        public int LotNumber { get; set; }
        public string ScaleCode { get; set; }
        public decimal? TotalWeight { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int SessionCheck { get; set; }
    }

    public class TbSession
    {
        public int Id { get; set; }
        public int LotNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class Line1And2Response : TbSession
    {
        public float V_Material { get; set; }  //Data 150Kg Cân nguyên liệu đầu vào để sản xuất số 1
        public float V_Product_1 { get; set; }   //Data 150Kg Cân thành phẩm đầu ra 1
    }

    public class Line3Response : TbSession
    {
        public float V_Product_1_1 { get; set; }  //Data 150Kg Cân tấm thành phẩm đầu ra 1 và 2
    }

    public class WeighSessionResponse
    {
        public string DateKey { get; set; }
        public int? OrderIndex { get; set; }
    }
}
