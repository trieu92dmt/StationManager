namespace Shared.Identity.Permissions
{
    public class PageResponse
    {
        public System.Guid PageId { get; set; }
        public string PageName { get; set; }
        public string PageUrl { get; set; }
        public string Icon { get; set; }
        public string Parameter { get; set; }
        //public Nullable<System.Guid> ModuleId { get; set; }
        public System.Guid MenuId { get; set; }
        public int? OrderIndex { get; set; }
        //Choose all functions in page
        public bool? isChooseAll { get; set; }
        public int? DomainConfig { get; set; }
        public string DomainConfigUrl { get; set; }
        //public string DomainConfigUrl => DomainConfig.HasValue ? (DomainConfig.Value == 1 ? "https://192.168.180.5:5055" : "https://192.168.180.5:9099") : null;
        //public string DomainConfigUrl => DomainConfig.HasValue ? (DomainConfig.Value == 1 ? "https://tlg-mes.isdcorp.vn" : "https://tlg-mes-fe.isdcorp.vn") : null;
    }
}
