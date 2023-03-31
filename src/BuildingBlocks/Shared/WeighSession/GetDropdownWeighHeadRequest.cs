namespace Shared.WeighSession
{
    public class GetDropdownWeighHeadRequest
    {
        public string KeyWord { get; set; }
        public string Plant { get; set; }
        public List<string> ScaleCodes { get; set; } = new List<string>();
    }
}
