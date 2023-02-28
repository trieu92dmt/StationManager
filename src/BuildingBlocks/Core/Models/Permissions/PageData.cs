using System.ComponentModel.DataAnnotations;

namespace Core.Models.Permissions
{
    public class PageData
    {
        public Guid PageId { get; set; }
        [StringLength(100)]
        public string PageName { get; set; }
        [StringLength(300)]
        public string PageUrl { get; set; }
        public Guid? MenuId { get; set; }
        public int? OrderIndex { get; set; }
        [StringLength(100)]
        public string Icon { get; set; }
        public bool? Visiable { get; set; }
        public bool? isSystem { get; set; }
        public bool Actived { get; set; }
        [StringLength(100)]
        public string Parameter { get; set; }
        public int? DomainConfig { get; set; }
    }
}
