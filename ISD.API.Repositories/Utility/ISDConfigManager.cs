using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Repositories
{
    public class ISDConfigManager: IISDConfigManager
    {
        private readonly IConfiguration _configuration;
        public ISDConfigManager()
        {
            this._configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();
        }
        public string GetConnectionString(string connectionName)
        {
            return this._configuration.GetConnectionString(connectionName);
        }
        public string Username
        {
            get
            {
                return this._configuration["SAP:Account:API:Username"];
            }
        }

        public string Password
        {
            get
            {
                return this._configuration["SAP:Account:API:Password"];
            }
        }
    }
}
