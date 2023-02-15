namespace Core.Models
{
    public class Page
    {
        public Guid PageId { get; set; }

        public string PageName { get; set; }

        public string PageUrl { get; set; }
        public string Icon { get; set; }

        public string Parameter { get; set; }

        public System.Guid MenuId { get; set; }

        public int? OrderIndex { get; set; }

        public List<Function> Functions { get; set; } = new List<Function>();

        public bool? isChooseAll { get; set; }
    }
}
