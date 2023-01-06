namespace MES.Application.Commands.MES
{
    public class GetNKMHCommand
    {
        public string Plant { get; set; }
        public string PurchasingOrgFrom { get; set; }
        public string PurchasingOrgTo { get; set; }

        public string PurchasingGroupFrom { get; set; }
        public string PurchasingGroupTo { get; set; }

        public string VendorFrom { get; set; }
        public string VendorTo { get; set; }
        public string POType { get; set; }
        public string PurchaseOrderFrom { get; set; }
        public string PurchaseOrderTo { get; set; }
        public string MaterialFrom { get; set; }
        public string MaterialTo { get; set; }

        public DateTime? DocumentDateFrom { get; set; }
        public DateTime? DocumentDateTo { get; set; }
    }
}
