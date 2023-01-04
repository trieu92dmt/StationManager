namespace ISD.Core.Models
{
    public class Menu
    {
        public Guid MenuId { get; set; }
        public string MenuName { get; set; }
        public Nullable<Guid> ModuleId { get; set; }
        public string Icon { get; set; }
        public int? OrderIndex { get; set; }
        public List<Page> Pages { get; set; }
        public List<MobileScreen> MobileScreens { get; set; }

    }
}
