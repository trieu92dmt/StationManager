namespace Shared.Identity.Permissions
{
    public class MobileScreenResponse
    {
        public Guid MobileScreenId { get; set; }

        public string ScreenName { get; set; }

        public string ScreenCode { get; set; }

        public Guid MenuId { get; set; }

        public string IconType { get; set; }
        public string IconName { get; set; }

        public Nullable<int> OrderIndex { get; set; }

        public List<FunctionResponse> Functions { get; set; } = new List<FunctionResponse>();
    }
}
