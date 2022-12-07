using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISD.API.Constant
{
    public class ConfigManager: IConfigConstantManager
    {
        private readonly IConfiguration _configuration;
        public ConfigManager()
        {
            this._configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();
        }
       
        public string DomainUrl
        {
            get
            {
                return this._configuration["DomainUrl"];
            }
        }

        public string APIDomainUrl
        {
            get
            {
                return this._configuration["APIDomainUrl"];
            }
        }
        public string DocumentDomain
        {
            get
            {
                return this._configuration["DocumentDomain"];
            }
        }
        public string DocumentDomainUpload
        {
            get
            {
                return this._configuration["DocumentDomainUpload"];
            }
        }
    }
}
