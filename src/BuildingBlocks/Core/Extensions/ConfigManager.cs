using Microsoft.Extensions.Configuration;

namespace Core.Extensions
{
    public interface IConfigConstantManager
    {
        string DomainUrl { get; }
        string APIDomainUrl { get; }
    }

    public class ConfigManager : IConfigConstantManager
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
        public string DomainUploadUrl
        {
            get
            {
                return this._configuration["DomainUploadUrl"];
            }
        }

        public string WeighSessionUrl
        {
            get
            {
                return this._configuration["WeighSessionUrl"];
            }
        }

        public string DomainMVC
        {
            get
            {
                return this._configuration["DomainMVC"];
            }
        }

        public string DomainFE
        {
            get
            {
                return this._configuration["DomainFE"];
            }
        }

        public string MailService
        {
            get
            {
                return this._configuration["MailService"];
            }
        }
    }

}
