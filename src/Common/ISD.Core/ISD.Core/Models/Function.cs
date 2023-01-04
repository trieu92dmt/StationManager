namespace ISD.Core.Models
{
    public class Function
    {
        public Guid PageId { get; set; }
        public Guid MobileScreenId { get; set; }
        public string FunctionId { get; set; }
        public string FunctionName { get; set; }
        public int? OrderIndex { get; set; }
        public bool Selected { get; set; }
    }
}
