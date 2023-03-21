namespace Shared.Identity.Permissions
{
    public class MenuResponse
    {
        public System.Guid MenuId { get; set; }
        public string MenuName { get; set; }
        public Nullable<System.Guid> ModuleId { get; set; }

        public string Icon { get; set; }
        public int? OrderIndex { get; set; }

        public List<PageResponse> PageViewModels { get; set; }
        //Excel
        public int RowIndex { get; set; }
        public string Error { get; set; }
        public bool isNullValueId { get; set; }

        //Choose all functions in pages in menu
        public bool? isChooseAll { get; set; }
    }
}
