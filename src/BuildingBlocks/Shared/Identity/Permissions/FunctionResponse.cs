namespace Shared.Identity.Permissions
{
    public class FunctionResponse
    {
        public Guid PageId { get; set; }
        public Guid MobileScreenId { get; set; }
        public string FunctionId { get; set; }
        public string FunctionName { get; set; }
        public int? OrderIndex { get; set; }
        public bool Selected { get; set; }
    }
}
