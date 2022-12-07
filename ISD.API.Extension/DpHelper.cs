using ISD.API.EntityModels.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace ISD.API.Extensions
{
    public class DbHelper
    {
        EntityDataContext _context;
        public DbHelper(EntityDataContext dataContext)
        {
            _context = dataContext;
        }

        #region Execute Non Query  
        public int ExecuteNonQuery(string commandText, SqlParameter[] parameters)
        {
            var conn = _context.Database.GetDbConnection();
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = CommandType.StoredProcedure;
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            var result = command.ExecuteNonQuery();
            conn.Close();
            return result;
        }
        #endregion

        #region Execute Scalar  
        public string ExecuteScalar(string commandText, SqlParameter[] parameters)
        {
            var conn = _context.Database.GetDbConnection();
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = CommandType.StoredProcedure;
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            var result = command.ExecuteScalar();
            conn.Close();
            return result.ToString();
        }
        #endregion

        #region Execute Reader  
        public SqlDataReader ExecuteReader(string commandText, SqlParameter[] parameters)
        {
            var conn = _context.Database.GetDbConnection();
            conn.Open();
            var command = conn.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = CommandType.StoredProcedure;
            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            var result = command.ExecuteReader();
            conn.Close();
            return result as SqlDataReader;
        }
        #endregion

        #region DataSet  
        public DataSet ExecuteDataset(string commandText, SqlParameter[] parameters)
        {
            DataSet resultSet = new DataSet();
            using (var conn = _context.Database.GetDbConnection())
            {
                conn.Open();
                var command = conn.CreateCommand() as SqlCommand;
                command.CommandText = commandText;
                command.CommandType = CommandType.StoredProcedure;
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(resultSet);
            }
            return resultSet;
        }
        #endregion

        #region Create Parameter  
        public SqlParameter CreateParameter(string name, object value, SqlDbType dbType, ParameterDirection direction = ParameterDirection.Input, int size = 0)
        {
            return new SqlParameter()
            {
                ParameterName = name,
                SqlDbType = dbType,
                Value = value,
                Direction = direction,
                Size = size
            };
        }
        #endregion
    }
}
