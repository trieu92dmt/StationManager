using System.Data;
using System.Data.SqlClient;
using System.ServiceProcess;
using System.Timers;

namespace WSF.Service
{
    public partial class WeighSessionService : ServiceBase
    {
        private Timer backupTimer;
        private string connectionStringDataCollection;
        private string connectionStringRiceFactoryDatabase;

        public WeighSessionService()
        {
            connectionStringRiceFactoryDatabase = "Data Source=192.168.100.233;Initial Catalog=RiceFactoryDatabase;User ID=isd;Password=pm123@abcd;TrustServerCertificate=true";
            connectionStringDataCollection = "Data Source=192.168.100.233;Initial Catalog=DataCollection;User ID=isd;Password=pm123@abcd;TrustServerCertificate=true";
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
            using (SqlConnection connectionA = new SqlConnection(connectionStringRiceFactoryDatabase))
            using (SqlConnection connectionB = new SqlConnection(connectionStringDataCollection))
            {
                // Open connections to both databases
                connectionA.Open();
                connectionB.Open();

                // Create a command to select all data from table A
                string selectCommandText = "SELECT * FROM WeighSessionModel";
                SqlCommand selectCommand = new SqlCommand(selectCommandText, connectionA);

                // Execute the select command and read the data
                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    // Create a command to insert data into table B
                    string insertCommandText = "INSERT INTO WeighSessionFactoryModel VALUES (@WeighSessionCode, @ScaleCode, @DateKey)";
                    SqlCommand insertCommand = new SqlCommand(insertCommandText, connectionB);

                    // Add parameters to the insert command
                    insertCommand.Parameters.Add("@WeighSessionCode", SqlDbType.NVarChar);
                    insertCommand.Parameters.Add("@ScaleCode", SqlDbType.NVarChar);
                    insertCommand.Parameters.Add("@DateKey", SqlDbType.NVarChar);

                    // Iterate over the data and insert into table B
                    while (reader.Read())
                    {
                        insertCommand.Parameters["@WeighSessionCode"].Value = "TEST";
                        insertCommand.Parameters["@ScaleCode"].Value = "TEST";
                        insertCommand.Parameters["@DateKey"].Value = "TEST";
                        insertCommand.ExecuteNonQuery();
                    }
                }

                // Close connections to both databases
                connectionA.Close();
                connectionB.Close();
            }
        }
    }
}
