using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ISD.API.Constant
{
    public static class DomainConfig 
    {
        private static readonly IConfigConstantManager configuration = new ConfigManager();

        public static string Domain = configuration.DomainUrl;
        public static string APIDomain = configuration.APIDomainUrl;

        public static string DomainImageMobile = Domain + "/Upload/Mobile/";


    }
}
