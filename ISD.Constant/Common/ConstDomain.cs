using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace ISD.Constant
{
    public class ConstDomain
    {
        //Test
        //public const string Domain = "http://192.168.0.105:3000";
        //Thật
        public static string Domain = WebConfigurationManager.AppSettings["DomainUrl"].ToString();
        public static string DomainImageCustomerPromotion = Domain + "/Upload/CustomerPromotion/thum";
        public static string DomainImageCustomerGift = Domain + "/Upload/Gift/thum";
        public static string DomainImageParentCategory = Domain + "/Upload/Brand/thum";
        public static string DomainImageCategory = Domain + "/Upload/Category/thum";
        public static string DomainImageProduct = Domain + "/Upload/Color/thum";
        public static string DomainBanner = Domain + "/Upload/Banner/thum";
        public static string DomainImageAccessoryCategory = Domain + "/Upload/AccessoryCategory/thum";
        public static string DomainImageStore = Domain + "/Upload/Store/Image/thum";
        public static string DomainLogoStore = Domain + "/Upload/Store/Logo/thum";
        public static string DomainImageAccessory = Domain + "/Upload/Accessory/thum";
        public static string DomainPeriodicallyChecking = Domain + "/Upload/PeriodicallyChecking/thum";
        public static string DomainPeriodicallyCheckingAPI = Domain + "/Upload/PeriodicallyChecking/";
        public static string DomainImageDefaultProduct = Domain + "/Upload/Color/thum";
        //Icon
        public static string DomainIcon = Domain + "/Upload/MaterialGroup/thum";
        //Hình loại xe
        public static string DomainProductHierarchy = Domain + "/Upload/ProductHierarchy/thum";
        //Hình sản phẩm (Material)
        public static string DomainMaterial = Domain + "/Upload/Material/thum";
        //Hình mô tả sản phẩm (MaterialDescription)
        public static string DomainMaterialDescription = Domain + "/Upload/MaterialDescription/thum";
        //noimage
        public static string NoImageUrl = Domain + "/Upload/noimage.jpg?v=1";
        public const string NoImage = "/Upload/noimage.jpg";

        public const string UrgentServiceUrl = "/Service/ServiceOrder/Edit/";
        public const string UrgentService_AccessorySaleOrderUrl = "/Sale/AccessorySaleOrder/Edit/";

        //domain API
        public static string DomainAPI = WebConfigurationManager.AppSettings["APIDomainUrl"].ToString();

        //token, key 
        public const string tokenConst = "3AF796E3AC1FE5DE75CE1BDE51726";
        public const string keyConst = "798573da-b933-4537-897d-21d469aef59f";

        public const string tokenConst_New = "68D1B83BE7FB496E83E94F44FEFF8";
        public const string keyConst_New = "120a4adc-557b-404d-8339-e4b368ae8fbe";

        //domain SMS API VMG
        public static string DomainSMSAPI = "";

        public static bool isSentSMS = false;

        public static string DocumentDomain = "";

    }
}
