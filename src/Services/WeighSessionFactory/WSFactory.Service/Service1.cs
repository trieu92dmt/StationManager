using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace WSFactory.Service
{
    [RunInstaller(true)]
    public partial class Service1 : ServiceBase
    {
        private Timer backupTimer;
        private string connectionStringDataCollection;
        private string connectionStringRiceFactoryDatabase;
        public Service1()
        {
            InitializeComponent();
            connectionStringRiceFactoryDatabase = "Data Source=192.168.100.233;Initial Catalog=RiceFactoryDatabase;User ID=isd;Password=pm123@abcd";
            connectionStringDataCollection = "Data Source=192.168.100.233;Initial Catalog=DataCollection;User ID=isd;Password=pm123@abcd";
        }

        protected override void OnStart(string[] args)
        {
            backupTimer = new Timer(60000); // 1 min interval
            backupTimer.Elapsed += new ElapsedEventHandler(OnBackupTimerElapsed);
            backupTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            backupTimer.Enabled = false;
            backupTimer.Dispose();
        }

        private void OnBackupTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                using (SqlConnection connectionA = new SqlConnection(connectionStringRiceFactoryDatabase))
                using (SqlConnection connectionB = new SqlConnection(connectionStringDataCollection))
                {
                    // Open connections to both databases
                    connectionA.Open();
                    connectionB.Open();

                    // Create a command to select all data from table A
                    string selectCommandText = "SELECT * FROM Tb_Session";
                    SqlCommand selectCommand = new SqlCommand(selectCommandText, connectionA);

                    // Execute the select command and read the data
                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        // Create a command to insert data into table B
                        //string insertCommandText = "INSERT INTO WeighSessionFactoryModel " +
                        //    "VALUES (@WeighSessionCode, @ScaleCode, @DateKey, @OrderIndex, @TotalNumberOfWeigh, @TotalWeight, @StartTime, @EndTime" +
                        //    "@CreateTime, @SessionCheck)";

                        string insertCommandText = "INSERT INTO dbo.MacBoard @id, @MacAddress, @boardid";

                        SqlCommand insertCommand = new SqlCommand(insertCommandText, connectionB);

                        // Add parameters to the insert command
                        insertCommand.Parameters.Add("@id", SqlDbType.Int);
                        insertCommand.Parameters.Add("@MacAddress", SqlDbType.NVarChar);
                        insertCommand.Parameters.Add("@boardid", SqlDbType.Int);
   
                        // Iterate over the data and insert into table B
                        while (reader.Read())
                        {
                            insertCommand.Parameters["@id"].Value = 9999;
                            insertCommand.Parameters["@MacAddress"].Value = "IOT";
                            insertCommand.Parameters["@boardid"].Value = 9999;
                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    // Close connections to both databases
                    connectionA.Close();
                    connectionB.Close();

                    string pathLog = "C:\\log-fail-success.txt";
                    using (StreamWriter writer = new StreamWriter(pathLog, true))
                    {
                        writer.WriteLine($"WSF.Service is called success on {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}.");
                        writer.Close();
                    }
                }
            }
            catch (System.Exception ex)
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
}
