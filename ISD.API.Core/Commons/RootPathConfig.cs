using System.IO;

namespace ISD.API.Core.Commons
{
    public class RootPathConfig
    {
        private static readonly string Dirpath = Directory.GetCurrentDirectory();

        public class TemplatePath
        {
            public static readonly string ReportTemplate = Dirpath + @"/Templates/";
        }
    }
}
