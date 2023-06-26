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
        public Guid? MenuId { get; set; }
        public int? OrderIndex { get; set; }
        //Choose all functions in page
        public bool? isChooseAll { get; set; }
        public int? DomainConfig { get; set; }
    }
}
