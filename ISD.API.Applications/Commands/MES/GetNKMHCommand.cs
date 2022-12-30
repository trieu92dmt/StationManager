using ISD.API.Core.SeedWork;

namespace ISD.API.Applications.Commands.MES
{
    public class GetNKMHCommand
    {
        public string Plant { get; set; }
        public int? PurchasingOrgFrom { get; set; }
        public int? PurchasingOrgTo { get; set; }

        public int? PurchasingGroupFrom { get; set; }
        public int? PurchasingGroupTo { get; set; }

        public int? VendorFrom { get; set; }
        public int? VendorTo { get; set; }
        public string POType { get; set; }
        public int? PurchaseOrderFrom { get; set; }
        public int? PurchaseOrderTo { get; set; }
        public int? MaterialFrom { get; set; }
        public int? MaterialTo { get; set; }

        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }

    }
}
